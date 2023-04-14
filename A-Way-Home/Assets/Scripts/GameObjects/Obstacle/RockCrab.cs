using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class RockCrab : Rock , ITrap, ITremor, ICommand, IActionWaitProcess
{
    // Should maybe eat Juvenile Plants
    [SerializeField] private Animator animator;
    [SerializeField] private int tileRange;

    private Dictionary<Vector2Int, Node> travelRangeGrid;
    private List<Node> path;
    private List<Vector3> targetPositions;
    private Node currentTargetNode;
    private int targetIndex;


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


    private void Update()
    {
        if(isWalking)
            Step();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetGrid();
        animator.SetBool("hasShell", hasShell);
        if(!hasShell)
            Invoke("SetPath", .5f);
    }

    public override void OnLightningHit()
    {
        Damage(1);
    }

    public void OnTremor()
    {
        MoveLocation();
    }

    public void OnCommand()
    {
        if(!hasShell)
            MoveLocation();
    }
    
 
    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Tremor && tool != Tool.Inspect && tool != Tool.Command )
            return;
        if(tool == Tool.Command && !hasShell)
            Node.ToggleNodes(path, Color.magenta, NodeGrid.nodesVisibility);
        outline.SetActive(true);
    }

    protected override void OnDehighlight(Tool tool)
    {
        base.OnDehighlight(tool);
        Node.ToggleNodes(path, NodeGrid.nodesVisibility);
    }

    public void OnTrapTrigger(Character character)
    {
        if(isWalking || !hasShell)
            character.TriggerDeath();
    }

    public void OnPlayerAction()
    {
        SetPath();
        PlayerActions.FinishProcess(this);
    }

    public override void Damage(int value = 1)
    {
        hitpoints -= value;
        animator.SetBool("hasShell", hasShell);
        if(!hasShell)
            SetPath();
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
        if(path == null || path.Count == 0)
            return;
        ForceDehighlight();
        isWalking = true;
        ClearNodes();
    }

    private void Step()
    {
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            targetIndex++;
            if(currentTargetNode.IsObstacle(typeof(Plant)))
                Destroy(currentTargetNode.GetObstacle());
            else if(currentTargetNode.IsObstacle(typeof(PoisonMiasma)) || currentTargetNode.IsObstacle(typeof(FireField)) || (currentTargetNode.IsObstacle(typeof(GroundSpike)) && !hasShell))
            {
                isWalking = false;
                currentTargetNode.GetObstacle().Destroy(this);
                return;
            }
            if (targetPositions.Contains(this.transform.position)){
                Stop();
                return;
            }
            currentTargetNode = path[targetIndex];
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
    }


    private void SetGrid()
    {
        Debug.Assert(nodes.Count > 0, "ERROR: Node is empty");
        travelRangeGrid = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, tileRange);
        Debug.Assert(travelRangeGrid.Count > 0, "ERROR: Grid Travel Range is empty");
    }

    private void SetPath()
    {
        path = new List<Node>();
        Debug.Log($"Looking for Path");
        if(!hasShell)
        {
            bool success = TryGetPath(NodeType.Obstacle, typeof(Rock));
            if(!success)
                Debug.LogWarning($"Looking for Rock.... Failed");
        } 
        if(!hasPath) 
            TryGetPath(NodeType.Walkable, typeof(Plant));
        if(!hasPath)
            SetRandomPath();
        // Debug.Assert(path.Count > 0, $"ERROR: Crab path is empty | Path count:{path.Count}");
        targetIndex = 0;
        if(path.Count == 0)
            return;
        currentTargetNode = path[0];
    }

    private bool TryGetPath(NodeType nodeType, Type type)
    {
        // Debug.Log($"Trying to get path with target node of {type.ToString()}");
        targetPositions = new List<Vector3>();
        targetPositions = NodeGrid.GetNodesPositions(type, travelRangeGrid);
        if(targetPositions.Count < 1)
            return false;
        Debug.Assert(targetPositions.Count > 0, "ERROR: No Target!");
        path = Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid,nodeType, type: type);//should have obstacle
        return path.Count > 0;
    }

    private void Stop()
    {
        isWalking = false;
        Node node  = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(Rock)))
            RegenerateShell((Rock)node.GetObstacle());
        else if(node.IsObstacle(typeof(Plant)) || node.IsObstacle(typeof(GroundSpike)))
            Destroy(node.GetObstacle());
        SettleDown(node);
    }

    private void SettleDown(Node node)
    {
        // Maybe Depends on the shell bool and the type of object interacted  when settling down
        if(!hasShell)
            SetNodes(this.worldPos, NodeType.Walkable, this);
        else
            SetNodes(this.worldPos, NodeType.Obstacle, this);
        SetGrid();
        if(!hasShell)
            SetPath();
    }

    private void RegenerateShell(Rock rock)
    {
        hitpoints = 2;
        Debug.Assert(hitpoints == 2, "ERROR: HP is not equals to 1");
        Destroy(rock);
    }

    private void SetRandomPath()
    {
        if(!NodeGrid.IfNeigbhorsWalkable(nodes[0], travelRangeGrid))
            return;
        targetPositions = new List<Vector3>();
        targetPositions = Node.GetRandomWorldPos(travelRangeGrid, 1);
        path = Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid);
        if(!hasPath)
            SetRandomPath();
    }

}
