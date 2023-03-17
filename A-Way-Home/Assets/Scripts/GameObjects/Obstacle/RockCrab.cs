using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class RockCrab : Rock , ITrap, ITremor, ICommand, IOnPlayerAction
{
    // Should maybe eat Juvenile Plants
    [SerializeField] private Animator animator;
    [SerializeField] private int tileRange;

    private Dictionary<Vector2Int, Node> travelRangeGrid;
    private List<Node> path;
    private List<Vector3> targetPositions;
    private Node currentTargetNode;
    private int targetIndex;

    private bool hasPath => path.Count > 0;
    public bool hasShell => hitpoints == 2;

    protected override int hitpoints {
        get => animator.GetInteger("hitpoints");
        set => animator.SetInteger("hitpoints", value);
    }

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
        AddToOnPlayerActionList(this);
        SetGrid();
        if(!hasShell)
            Invoke("SetPath", .5f);
            // SetPath();
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
        base.OnHighlight(tool);
        if(tool == Tool.Command && !hasShell)
        {
            Node.ToggleNodes(path, Color.magenta, NodeGrid.nodesVisibility);
            spriteRenderer.color = Color.red;
        }
    }

    protected override void OnDehighlight(Tool tool)
    {
        base.OnDehighlight(tool);
        if(tool == Tool.Command)
        {
            Node.ToggleNodes(path, NodeGrid.nodesVisibility);
            spriteRenderer.color = Color.white;
        }
    }

    public void OnTrapTrigger(Character character)
    {
        if(isWalking || !hasShell)
            character.TriggerDeath();
    }

    public void OnPerformAction()
    {
        SetPath();
    }

    public override void Damage(int value = 1)
    {
        hitpoints -= value;
        if(!hasShell)
            SetPath();
        else if(hitpoints == 0)
            Remove();
    }

    public override void Remove()
    {
        ClearNodes();
        hitpoints = 0;
        StartCoroutine(DeathAnimation());
    } 

    private IEnumerator DeathAnimation()
    {

        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorClipInfo(0).Length);
        this.gameObject.SetActive(false);
    }

    private void MoveLocation()
    {
        isWalking = true;
        ClearNodes();
    }

    private void Step()
    {
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            targetIndex++;
            if(currentTargetNode.IsObstacle(typeof(GroundSpike)) && !hasShell)
            {
                isWalking = false;
                GroundSpike spike = currentTargetNode.GetObstacle() as GroundSpike;
                StartCoroutine(spike.Kill(this));
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
            bool success = TryGetCrabPath(NodeType.Obstacle, typeof(Rock));
            if(!success)
                Debug.LogWarning($"Looking for Rock.... Failed");
        } 
        if(!hasPath) 
            TryGetCrabPath(NodeType.Walkable, typeof(Plant));
        if(!hasPath)
            SetRandomPath();
        Debug.Assert(path.Count > 0, $"ERROR: Crab path is empty | Path count:{path.Count}");
        targetIndex = 0;
        currentTargetNode = path[0];
    }

    private bool TryGetCrabPath(NodeType nodeType, Type type)
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
        else if(node.IsObstacle(typeof(Plant)))
            Destroy(node.GetObstacle());
        else if(node.IsObstacle(typeof(GroundSpike)))
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
        targetPositions = new List<Vector3>();
        targetPositions = Node.GetRandomWorldPos(travelRangeGrid, 4);
        path = Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid);
        if(!hasPath)
            SetRandomPath();
    }

}
