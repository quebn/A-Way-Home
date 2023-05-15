using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Obstacle, IActionWaitProcess, ILightning, ITrap, ICommand, ISelectable
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private int tileRange;

    private int currentTargetIndex;
    private Node targetNode;
    private Node currentTargetNode;
    private Node lastNode;
    private List<Node> walkableNodes;
    private Dictionary<Vector2Int, Node> walkableGrid;
    private List<Node> gridNodes;
    private List<Node> path;
    private bool isCommanded = false;

    public override bool isBurnable => true;
    public override bool isTrampleable => true;
    public override bool isFragile => true;
    public override bool isCorrosive => true;
    public override bool isMeltable => true;

    private bool hasPath => path.Count > 0;

    private bool isMoving {
        get => animator.GetBool("isMoving"); 
        set => animator.SetBool("isMoving", value); 
    }


    protected override void Initialize()
    {
        base.Initialize();
        SetNodesGrids();
        Debug.Assert(walkableNodes.Count > 0);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public void OnPlayerAction()
    {
        if(isCommanded){
            PlayerActions.FinishProcess(this);
            return;
        }
        isCommanded = false;
        if(TryGetNode())
            StartCoroutine(FollowPath());
    }

    public override void Remove()
    {
        ForceDehighlight();
        if(isMoving)
            isMoving = false;
        hitpoints = 0;
        ClearNodes();
        PlayerActions.FinishProcess(this);
        PlayerActions.FinishCommand(this);
        StartCoroutine(DeathAnimation());
    }

    public bool OnCommand(List<Node> nodes)
    {
        if(nodes.Count == 0)
            return false;
        return GoToNode(nodes[0]);
    }

    public void OnSelect(Tool tool)
    {
        if(tool != Tool.Command)
            return;
        gridNodes = new List<Node>();
        foreach(Node node in walkableGrid.Values)
            if(node.IsWalkable())
                gridNodes.Add(node);
        for(int i = 0 ; i < gridNodes.Count; i++)
            gridNodes[i].RevealNode();
    }

    public List<Node> OnSelectedHover(Vector3 mouseWorldPos, List<Node> currentNodes)
    {
        Vector2 origin = NodeGrid.GetMiddle(mouseWorldPos);
        Node node = NodeGrid.NodeWorldPointPos(origin);
        Debug.Assert(!gridNodes.Contains(nodes[0]));
        if(node == currentNodes[0])
            return currentNodes;
        List<Node> nodeList = new List<Node>();
        DehighlightNode(currentNodes[0]);
        if(gridNodes.Contains(node))
            node.HighlightObstacle(Node.colorRed, Tool.Inspect);
        nodeList.Add(node);
        return nodeList;
    }


    public void OnDeselect()
    {
        Node.ToggleNodes(gridNodes, NodeGrid.nodesVisibility);
        if(nodes.Count > 0)
            nodes[0].Dehighlight();
    }

    public List<Node> IgnoredToggledNodes()
    {
        List<Node> list = new List<Node>(gridNodes);
        list.Add(nodes[0]);
        return list;
    }

    private bool GoToNode(Node node)
    {
        List<Vector3> targetPositions = new List<Vector3>();
        if(!node.IsWalkable() || !NodeGrid.Instance.grid.ContainsValue(node))
            return false;
        targetPositions.Add(node.worldPosition);
        Debug.Assert(targetPositions.Count == 1, "ERROR: Crab target positions more than 1.");
        if(TryGetPath(targetPositions))
        {
            ForceDehighlight();
            isMoving = true;
            lastNode = nodes[0];
            currentTargetIndex = 0;
            currentTargetNode = path[0];
            targetNode = node;
            ClearNodes();
            StartCoroutine(FollowPathCommand());
            isCommanded = true;
        }
        return hasPath;
    }

    private bool TryGetPath(List<Vector3> targetPositions)
    {
        path = Pathfinding.FindPath(this.worldPos, targetPositions, walkableGrid);
        return hasPath;
    }

    private IEnumerator FollowPathCommand()
    {
        while(isMoving)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                currentTargetIndex++;
                OnStatusInteract(currentTargetNode);
                SpawnWeb();
                if(currentTargetNode.hasObstacle)
                {
                    Obstacle obs = currentTargetNode.GetObstacle(); 
                    if(obs.isTrampleable)
                        Destroy(obs);
                    else{
                        obs.Destroy(this);
                        yield break;
                    }
                }
                lastNode = currentTargetNode;
                if(this.transform.position == targetNode.worldPosition)
                {
                    Stop();
                    yield break;
                }
                Debug.Assert(path.Count > currentTargetIndex, $"ERROR: Tried to access index {currentTargetIndex} with path of size {path.Count}");
                currentTargetNode = path[currentTargetIndex];
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private void DehighlightNode(Node node)
    {
        if(!gridNodes.Contains(node))
            return;
        node.RevealNode();
        if(!node.hasObstacle)
            return;
        node.GetObstacle().Dehighlight();
    }

    private IEnumerator DeathAnimation()
    {
        animator.Play("Death");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        this.gameObject.SetActive(false);
    }

    private IEnumerator FollowPath()
    {
        while(isMoving)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                if(currentTargetNode.hasObstacle)
                {
                    Obstacle obs = currentTargetNode.GetObstacle(); 
                    if(obs.isTrampleable)
                        Destroy(obs);
                    else{
                        obs.Destroy(this);
                        yield break;
                    }
                }
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private bool TryGetNode()
    {
        if(walkableNodes == null || walkableNodes.Count == 0 || hitpoints <= 0 || !NodeGrid.IfNeigbhorsWalkable(nodes[0]))
        {
            PlayerActions.FinishProcess(this);
            return false;
        }
        SetCurrentTargetNode();
        Debug.LogWarning($"Targetnode pos => {currentTargetNode.worldPosition}");
        lastNode = nodes[0];
        ClearNodes();
        SpawnWeb();
        isMoving = true;
        return true;
    }

    private void SetCurrentTargetNode()
    {
        currentTargetNode = null;
        int randomIndex = UnityEngine.Random.Range(0, walkableNodes.Count);
        if(walkableNodes[randomIndex].IsWalkable())
            currentTargetNode = walkableNodes[randomIndex];
        else
            SetCurrentTargetNode();
    }

    private void Stop()
    {
        isMoving = false;
        OnStatusInteract(currentTargetNode);
        currentTargetNode = null;
        SetNodesGrids();
        PlayerActions.FinishCommand(this);
        PlayerActions.FinishProcess(this);

    }

    private void SetNodesGrids()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
        walkableGrid = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, tileRange);
        walkableNodes = NodeGrid.GetPathNeighborNodes(nodes[0], walkableGrid);
    }

    private void SpawnWeb()
    {
        GameObject.Instantiate(
            webPrefab, 
            lastNode.worldPosition, 
            Quaternion.identity);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(-7);
        character.DamageAnimation();
        Remove();
    }
}
