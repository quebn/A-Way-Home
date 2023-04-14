using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle, ITrap, ILightning, IActionWaitProcess
{
    [SerializeField] private Animator animator;
    [SerializeField] private int damage;
    [SerializeField] private GameObject poisonMiasma;
    
    private bool isMoving = false;
    private Dictionary<Vector2Int, Node> nodeGridRange;
    private Vector3 targetPosition;

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;

    private bool destinationReached => targetPosition == (Vector3)worldPos;


    private void Update()
    {
        if(isMoving)
            Step();
    }

    public void OnLightningHit()
    {
        ForceDehighlight();
        Damage(1);
        if(hitpoints > 0)
            Move();
    }

    public void OnAftershock()
    {
        ForceDehighlight();
        Vector3 pos = nodes[0].worldPosition;
        Move();
        GameObject.Instantiate(poisonMiasma, pos, Quaternion.identity);
    }

    public void OnPlayerAction()
    {
        if(!isMoving)
            PlayerActions.FinishProcess(this);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Command)
            return;
        base.OnHighlight(tool);
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodeGridRange();
    }

    public void Move()
    {
        // Debug.Assert(path.Count > 0, "ERROE: Bat has no Path!");
        SetRandomPosition();
        isMoving = true;
        ClearNodes();
        // currentTargetNode = path[0];
        // targetIndex = 0;
    }

    private void SetNodeGridRange()
    {
        // if(nodes != null || nodes.Count > 0 )
        //     ClearNodes();
        SetNodes(this.worldPos, NodeType.Walkable, this);
        nodeGridRange = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, 5);
    }

    private void Step()
    {
        if(destinationReached)
        {
            OnStop();
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
    }

    private void OnStop()
    {
        isMoving = false;
        PlayerActions.FinishProcess(this);
        SetNodes(this.worldPos, NodeType.Walkable, this);
        SetNodeGridRange();
    }

    private void SetRandomPosition()
    {
        targetPosition = new Vector3();
        targetPosition = Node.GetRandomWorldPos(nodeGridRange, NodeType.Walkable, false);
        // path = Pathfinding.FindPath(this.worldPos, targetPositions, nodeGridRange);
        // if(path.Count == 0)
            // SetRandomPath();
    }

    private void TriggerDeath()
    {
        hitpoints = 0;
        ClearNodes();
        this.gameObject.SetActive(false);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(-damage);
        character.DamageAnimation();
        Move();
    }


}
