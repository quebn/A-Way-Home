using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle, ITrap, ILightning
{
    // Bat should 
    // - fly to random node when hit near a lightning
    // - killed by directly hit by lightning.
    [SerializeField] private Animator animator;
    [SerializeField] private int damage;

    private Dictionary<Vector2Int, Node> nodeGridRange;
    private Vector3 targetPosition;
    // private List<Node> path;
    // private Node currentTargetNode;
    // private int targetIndex;

    private bool destinationReached => targetPosition == (Vector3)worldPos;

    protected override int hitpoints { 
        get => animator.GetInteger("hitpoints"); 
        set => animator.SetInteger("hitpoints", value); 
    }
    protected bool isFlying {
        get => animator.GetBool("isFlying"); 
        set => animator.SetBool("isFlying", value); 
    }

    private void Update()
    {
        if(isFlying)
            Step();
    }

    public void OnLightningHit()
    {
        TriggerDeath();
    }


    public void OnAftershock()
    {
        Move();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodeGridRange();
        SetRandomPosition();
    }

    public void Move()
    {
        // Debug.Assert(path.Count > 0, "ERROE: Bat has no Path!");
        isFlying = true;
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
        // if(this.transform.position == currentTargetNode.worldPosition)
        // {
            // targetIndex++;
            // if(destinationReached)
            // {
                // OnStop();
                // return;
            // }
            // currentTargetNode = path[targetIndex];
        // }
        if(destinationReached)
        {
            OnStop();
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
    }

    private void OnStop()
    {
        isFlying = false;
        SetNodes(this.worldPos, NodeType.Walkable, this);
        SetNodeGridRange();
        SetRandomPosition();
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
        character.IncrementEnergy(damage);
        Move();
    }



    // private void HighlightNodes()
    // {
    //     Node.HideNodes(nodes);
    // }

    // private void GoToDestination()
    // {
    //     // Make the Bat wake up and go to its destination.
    //     animator.Play("Bat_Flying");
    //     batGameObject.transform.position = destination.transform.position;
    //     ClosePath();
    // }

    // private void ClosePath()
    // {
    //     // Make the node that the bat overlaps unwalkable
    //     this.gameObject.tag = "Interacted";
    //     SetNodesType(NodeType.Obstacle);
    // }
}
