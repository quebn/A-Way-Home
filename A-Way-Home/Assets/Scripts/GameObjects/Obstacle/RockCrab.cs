using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class RockCrab : Obstacle, ITrap, ITremor, ICommand, IActionWaitProcess, ILightning, ISelectable
{
    // Should maybe eat Juvenile Plants
    [SerializeField] private Animator animator;
    [SerializeField] private int tileRange;

    private Dictionary<Vector2Int, Node> walkableGrid;
    private List<Node> path;
    private List<Vector3> targetPositions = new List<Vector3>();
    private Node currentTargetNode;
    private int targetIndex;
    private bool wasInteracted = false;
    private List<Node> gridNodes;

    public override bool isBurnable => !hasShell;
    public override bool isTrampleable => !hasShell;
    public override bool isFragile => !hasShell;
    public override bool isCorrosive => true;
    public override bool isMeltable => true;
    public override bool isWalkableByTerra => true;

    public bool hasShell => hitpoints == 2;
    private bool hasPath => path.Count > 0;

    private bool isWalking {
        get => animator.GetBool("isWalking");
        set => animator.SetBool("isWalking", value);
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetGridNodes();
        animator.SetBool("hasShell", hasShell);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
        Debug.Log($"{damage}");
        wasInteracted = true;
    }

    public void OnAftershock(Vector2 lightningOrigin)
    {
        if(!hasShell)
        {
            GoToNearestRock();
            if(isWalking && hasPath)
                return;
        }   
        // Move one node away from origin
        Vector2 currentPos = this.worldPos;
        Vector2 targetPos = currentPos + (currentPos - lightningOrigin);
        Node targetNode;
        targetNode = NodeGrid.NodeWorldPointPos(targetPos);
        if(targetNode.worldPosition == this.transform.position || !targetNode.IsWalkable())
            return;
        ForceDehighlight();
        isWalking = true;
        audioSources[3].Play();
        Node prevNode = nodes[0];
        ClearNodes();
        FireNode.ContinueFire(prevNode);
        StartCoroutine(StepToNode(targetNode));
    }

    private IEnumerator StepToNode(Node targetNode)
    {
        while(isWalking)
        {
            if(this.transform.position == targetNode.worldPosition)
            {
                OnStatusInteract(targetNode, IfFireImmune);
                if(targetNode.hasObstacle)
                    if(targetNode.GetObstacle().isTrampleable||(targetNode.GetObstacle().isFragile && hasShell))
                        Destroy(targetNode.GetObstacle());
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    public void OnTremor()
    {
        Damage(1);
        wasInteracted = true;
    }

    public bool OnCommand(List<Node> nodes)
    {
        if(nodes.Count == 0)
            return false;
        return GoToNode(nodes[0]);
    }

    private bool GoToNode(Node node)
    {
        targetPositions.Clear();
        Node targetNode = CheckNode(node);
        if(targetNode == null || !NodeGrid.Instance.grid.ContainsValue(targetNode))
            return false;
        targetPositions.Add(targetNode.worldPosition);
        Debug.Assert(targetPositions.Count == 1, "ERROR: Crab target positions more than 1.");
        TryGetPath(targetNode.currentType, targetNode.hasObstacle ? targetNode.GetObstacleType() : null);
        if(hasPath)
        {
            MoveLocation();
            wasInteracted = true;
            StartCoroutine(FollowPath());
        }
        // Debug.LogWarning(hasPath ? "Crab has Path": "Crab has no Path");
        return hasPath;
    }

    private Node CheckNode(Node node)
    {
        if(node.IsType(NodeType.Obstacle))
            if(node.IsObstacle(typeof(Plant)) || (node.IsObstacle(typeof(Rock)) && !hasShell))
                return node;
        if(node.IsType(NodeType.Walkable))
            return node;
        Debug.LogWarning($"RETURNING NULL node is {node.currentType.ToString()}");
        return null;
    }

    public void OnSelect(Tool tool)
    {
        if(tool != Tool.Command)
            return;
        gridNodes = new List<Node>();
        foreach(Node node in walkableGrid.Values)
            if(node.IsWalkable())//&& !node.IsObstacle(typeof(Spider)))
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
            node.HighlightObstacle(hasShell ? Node.colorRed : Node.colorPurple, Tool.Inspect);
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

    private void DehighlightNode(Node node)
    {
        if(!gridNodes.Contains(node))
            return;
        node.RevealNode();
        if(!node.hasObstacle)
            return;
        node.GetObstacle().Dehighlight();
    }


    protected override void OnHighlight(Tool tool)
    {
        base.OnHighlight(tool);
    }

    public void OnTrapTrigger(Character character)
    {
        character.TriggerDeath();
    }

    public void OnPlayerAction()
    {
        if(!hasShell && !isWalking && !wasInteracted)
            GoToNearestRock();
        if(wasInteracted)
            wasInteracted = false;
        if(isWalking)
            return;
        PlayerActions.FinishProcess(this);
    }

    private void GoToNearestRock()
    {
        targetPositions.Clear();
        targetPositions = NodeGrid.GetNodesPositions(typeof(Rock), walkableGrid);
        if(targetPositions.Count == 0)
            return;
        TryGetPath(NodeType.Obstacle, typeof(Rock));
        if(path.Count == 0)
            return;
        MoveLocation();
        StartCoroutine(FollowPathRock());
    }

    public override void Damage(int value = 1)
    {
        hitpoints -= value > hitpoints ? hitpoints : value;
        if(hitpoints == 1)
            audioSources[0].Play();
        animator.SetBool("hasShell", hasShell);
        nodes[0].currentType = hasShell ? NodeType.Obstacle : NodeType.Walkable;
        if(hitpoints == 0)
            Remove();
    }

    public override void Remove()
    {
        ForceDehighlight();
        hitpoints = 0;
        if(isWalking){
            isWalking = false;
            audioSources[3].Stop();
        }
        ForceDehighlight();
        ClearNodes();
        PlayerActions.FinishProcess(this);
        PlayerActions.FinishCommand(this);
        StartCoroutine(DeathAnimation());
    } 

    private IEnumerator DeathAnimation()
    {
        audioSources[1].Play();
        animator.Play("Death");
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorClipInfo(0).Length);
        this.gameObject.SetActive(false);
    }

    private void MoveLocation()
    {
        if(nodes.Count == 0)
            return;
        Debug.Assert(path != null && path.Count > 0);
        ForceDehighlight();
        isWalking = true;
        audioSources[3].Play();
        targetIndex = 0;
        currentTargetNode = path[targetIndex];
        Node prevNode = nodes[0];
        ClearNodes();
        FireNode.ContinueFire(prevNode);
    }

    private IEnumerator FollowPath()
    {
        while(isWalking)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                targetIndex++;
                OnStatusInteract(currentTargetNode, IfFireImmune);
                if(currentTargetNode.hasObstacle)
                {
                    if(currentTargetNode.GetObstacle().isTrampleable||(currentTargetNode.GetObstacle().isFragile && hasShell)){
                        Destroy(currentTargetNode.GetObstacle());
                    }
                    else
                    {
                        currentTargetNode.GetObstacle().Destroy(this);
                        yield break;
                    }
                }
                // 
                if (targetPositions.Contains(this.transform.position))
                {
                    Stop();
                    yield break;
                }
                currentTargetNode = path[targetIndex];
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FollowPathRock()
    {
        while(isWalking)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                targetIndex++;
                OnStatusInteract(currentTargetNode, IfFireImmune);
                if(currentTargetNode.hasObstacle && !currentTargetNode.IsObstacle(typeof(Rock)))
                {
                    if(currentTargetNode.GetObstacle().isTrampleable||(currentTargetNode.GetObstacle().isFragile && hasShell)){
                        Destroy(currentTargetNode.GetObstacle());
                    }
                    else
                    {
                        currentTargetNode.GetObstacle().Destroy(this);
                        yield break;
                    }
                }
                // 
                if (targetPositions.Contains(this.transform.position))
                {
                    Stop();
                    yield break;
                }
                currentTargetNode = path[targetIndex];
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private void IfFireImmune(Node node)
    {
        if(node.IsStatus(NodeStatus.Burning))
        {
            FireNode.PauseFire(node);
        }
    }

    private bool TryGetPath(NodeType nodeType, Type type)
    {
        if(targetPositions.Count < 1)
            return false;
        Debug.Assert(targetPositions.Count > 0, "ERROR: No Target!");
        path = type == null 
            ? Pathfinding.FindPath(this.worldPos, targetPositions, walkableGrid, nodeType)  
            : Pathfinding.FindPath(this.worldPos, targetPositions, walkableGrid, nodeType, type: type);
        return hasPath;
    }

    private void Stop()
    {
        isWalking = false;
        audioSources[3].Stop();
        Node node  = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(Rock)))
            RegenerateShell((Rock)node.GetObstacle());
        SetGridNodes();
        PlayerActions.FinishProcess(this);
        PlayerActions.FinishCommand(this);
    }

    private void SetGridNodes()
    {
        SetNodes(this.worldPos, hasShell ? NodeType.Obstacle : NodeType.Walkable, this);
        walkableGrid = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, tileRange);
    }

    private void RegenerateShell(Rock rock)
    {
        hitpoints = 2;
        audioSources[2].Play();
        Debug.Assert(hitpoints == 2, "ERROR: HP is not equals to 1");
        animator.SetBool("hasShell", hasShell);
        Destroy(rock);
    }

}
