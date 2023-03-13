using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class RockCrab : Rock , ITrap
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
        SetGrid();
        if(!hasShell)
            Invoke("SetPath", .5f);
            // SetPath();
    }

    public override void OnInteract()
    {
        if(currentTool == Tool.Lightning)
            Remove();
        if(currentTool == Tool.Command && !hasShell) 
            MoveLocation();
        if(currentTool == Tool.Tremor)
        {
            SetPath();
            MoveLocation();
        }
    }

    public override void OnHighlight()
    {
        base.OnHighlight();
        if(currentTool == Tool.Command && !hasShell)
        {
            Node.ToggleNodes(path, Color.magenta, NodeGrid.nodesVisibility);
            spriteRenderer.color = Color.red;
        }
    }

    public override void OnDehighlight()
    {
        base.OnDehighlight();
        if(currentTool == Tool.Command)
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

    private void Remove()
    {
        if (currentTool != Tool.Lightning)
            return;
        if(hasShell)
            RemoveRock();
        else 
            TriggerDeath();
    }

    private void RemoveRock()
    {
        hitpoints -= 1;
        Debug.Assert(hitpoints == 1, "ERROR: HP is not equals to 1");
        SetNodes(this.worldPos, NodeType.Walkable, this);
        SetPath();
    }

    public void TriggerDeath()
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
                GroundSpike spike = (GroundSpike)currentTargetNode.GetObstacle();
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
        Debug.LogWarning($"hasShell: {hasShell} | hp: {hitpoints}");
        if(!hasShell) 
            TryGetCrabPath(typeof(Rock));
        if(!hasPath) 
            TryGetCrabPath(typeof(Plant));
        if(!hasPath)
            SetRandomPath();
        Debug.Assert(path.Count > 0, $"ERROR: Crab path is empty | Path count:{path.Count}");
        targetIndex = 0;
        currentTargetNode = path[0];
    }

    private void Eat(Plant plant)
    {
        plant.DamagePlant();
    }

    private bool TryGetCrabPath(Type type)
    {
        // Debug.Log($"Trying to get path with target node of {type.ToString()}");
        targetPositions = new List<Vector3>();
        targetPositions = NodeGrid.GetReachableWorldPos(type, travelRangeGrid);
        if(targetPositions.Count < 1)
            return false;
        Debug.Assert(targetPositions.Count > 0, "ERROR: No Target!");
        path = Pathfinding.FindPath(this.worldPos, targetPositions, travelRangeGrid, type: type);
        return path.Count > 0;
    }

    private void Stop()
    {
        isWalking = false;
        Node node  = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(Rock)))
            PickUpRock((Rock)node.GetObstacle());
        else if(node.IsObstacle(typeof(Plant)))
            Eat((Plant)node.GetObstacle());
        else if(node.IsObstacle(typeof(GroundSpike)))
            GroundSpikeInteract((GroundSpike)node.GetObstacle());
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

    private void GroundSpikeInteract(GroundSpike groundSpike)
    {
        Debug.Assert(hasShell, "ERROR: this should have shell!");
        groundSpike.TriggerDeath();
    }

    private void PickUpRock(Rock rock)
    {
        hitpoints += 1;
        Debug.Assert(hitpoints == 2, "ERROR: HP is not equals to 1");
        rock.ClearRock();
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
