using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class RockCrab : Obstacle, ITrap, ITremor, ICommand, IActionWaitProcess, ILightning
{
    // Should maybe eat Juvenile Plants
    [SerializeField] private Animator animator;
    [SerializeField] private int tileRange;

    private Dictionary<Vector2Int, Node> travelRangeGrid;
    private List<Node> path;
    private List<Vector3> targetPositions = new List<Vector3>();
    private Node currentTargetNode;
    private int targetIndex;
    private bool wasInteracted = false; 

    public override bool isBurnable => true;
    public override bool isTrampleable => !hasShell;
    public override bool isFragile => !hasShell;
    public override bool isCorrosive => true;
    public override bool isMeltable => true;
    public bool hasShell => hitpoints == 2;
    private bool hasPath => path.Count > 0;

    private bool isWalking {
        get => animator.GetBool("isWalking");
        set => animator.SetBool("isWalking", value);
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodesAndGrid();
        animator.SetBool("hasShell", hasShell);
    }

    public void OnLightningHit()
    {
        Damage(1);
        wasInteracted = true;
    }

    public void OnAftershock(Vector2 lightningOrigin)
    {
        // Move one ode away from origin
        Vector2 currentPos = this.worldPos;
        Vector2 targetPos = currentPos + (currentPos - lightningOrigin);
        Node targetNode;
        // if(targe)
        targetNode = NodeGrid.NodeWorldPointPos(targetPos);
        if(targetNode.worldPosition == this.transform.position || !targetNode.IsWalkable())
            return;
        ForceDehighlight();
        isWalking = true;
        ClearNodes();
        StartCoroutine(StepToNode(targetNode));
    }

    private IEnumerator StepToNode(Node targetNode)
    {
        while(isWalking)
        {

            if(this.transform.position == targetNode.worldPosition)
            {
                Stop();
                PlayerActions.FinishProcess(this);
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
            MoveLocation();
        return hasPath;
    }

    private Node CheckNode(Node node)
    {
        if(node.IsType(NodeType.Obstacle))
            if(node.IsObstacle(typeof(Plant)) || (node.IsObstacle(typeof(Rock)) && !hasShell))
                return node;
        else if(node.IsType(NodeType.Walkable))
            return node;
        return null;
    }

    public override void OnSelect(Tool tool)
    {
        if(tool != Tool.Command)
            return;

    }
    
    protected override void OnHighlight(Tool tool)
    {
        base.OnHighlight(tool);
    }

    public void OnTrapTrigger(Character character)
    {
        if(isWalking || !hasShell)
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
        targetPositions = NodeGrid.GetNodesPositions(typeof(Rock), travelRangeGrid);
        if(targetPositions.Count == 0)
            return;
        TryGetPath(NodeType.Obstacle, typeof(Rock));
        if(path.Count == 0)
            return;
        MoveLocation();
    }

    public override void Damage(int value = 1)
    {
        hitpoints -= value;
        animator.SetBool("hasShell", hasShell);
        if(hitpoints == 0)
            Remove();
    }

    public override void Remove()
    {
        ForceDehighlight();
        ClearNodes();
        hitpoints = 0;
        StartCoroutine(DeathAnimation());
    } 

    private IEnumerator DeathAnimation()
    {
        animator.Play("Death");
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorClipInfo(0).Length);
        this.gameObject.SetActive(false);
    }

    private void MoveLocation()
    {
        Debug.Assert(path != null && path.Count > 0);
        ForceDehighlight();
        isWalking = true;
        targetIndex = 0;
        currentTargetNode = path[targetIndex];
        ClearNodes();
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        while(isWalking)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                targetIndex++;
                if(currentTargetNode.hasObstacle && !currentTargetNode.IsObstacle(typeof(GroundSpike)) && !currentTargetNode.IsObstacle(typeof(Rock)))
                    Destroy(currentTargetNode.GetObstacle());
                else if(currentTargetNode.IsObstacle(typeof(PoisonMiasma)) || currentTargetNode.IsObstacle(typeof(FireField)) || (currentTargetNode.IsObstacle(typeof(GroundSpike)) && !hasShell))
                {
                    isWalking = false;
                    PlayerActions.FinishProcess(this);
                    currentTargetNode.GetObstacle().Destroy(this);
                    yield break;
                }
                if (targetPositions.Contains(this.transform.position))
                {
                    Stop();
                    PlayerActions.FinishProcess(this);
                    yield break;
                }
                currentTargetNode = path[targetIndex];
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private bool TryGetPath(NodeType nodeType, Type type)
    {
        if(targetPositions.Count < 1)
            return false;
        Debug.Assert(targetPositions.Count > 0, "ERROR: No Target!");
        path = type == null 
            ? Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid, nodeType)  
            : Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid,nodeType, type: type);
        return hasPath;
    }

    private void Stop()
    {
        isWalking = false;
        Node node  = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(Rock)))
            RegenerateShell((Rock)node.GetObstacle());
        else if(node.IsObstacle(typeof(Plant)) || node.IsObstacle(typeof(GroundSpike)))
            Destroy(node.GetObstacle());
        SetNodesAndGrid();
    }

    private void SetNodesAndGrid()
    {
        SetNodes(this.worldPos, hasShell ? NodeType.Obstacle : NodeType.Walkable, this);
        travelRangeGrid = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, tileRange);
    }

    private void RegenerateShell(Rock rock)
    {
        hitpoints = 2;
        Debug.Assert(hitpoints == 2, "ERROR: HP is not equals to 1");
        animator.SetBool("hasShell", hasShell);
        Destroy(rock);
    }

    private void SetRandomPath()
    {
        if(!NodeGrid.IfNeigbhorsWalkable(nodes[0], travelRangeGrid))
            return;
        targetPositions.Clear();
        targetPositions = Node.GetRandomWorldPos(travelRangeGrid, 1);
        path = Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid);
        if(!hasPath)
            SetRandomPath();
    }

}
