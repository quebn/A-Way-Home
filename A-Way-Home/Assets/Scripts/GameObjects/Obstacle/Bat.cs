using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle , IInteractable
{
    // Bat should 
    // - fly to random node when hit near a lightning
    // - killed by directly hit by lightning.
    [SerializeField] private Animator animator;

    private bool isMoving = false;
    private Dictionary<Vector2Int, Node> nodeGridRange;
    private List<Node> path;
    private List<Vector3> targetPositions;
    private Node currentTargetNode;
    private int targetIndex;
    private Node destionationNode;

    private bool destinationReached => this.transform.position == destionationNode.worldPosition;

    protected override int hitpoints { 
        get => animator.GetInteger("hitpoints"); 
        set => animator.SetInteger("hitpoints", value); 
    }
    protected bool isFlying {
        get => animator.GetBool("isFlying"); 
        set => animator.SetBool("isFlying", value); 
    }

    public void OnDehighlight()
    {
        if(currentTool == Tool.Lightning)
            spriteRenderer.color = Color.white;
    }

    public void OnHighlight()
    {
        if(currentTool == Tool.Lightning)
            spriteRenderer.color = Color.green;
    }

    public void OnInteract()
    {
        if(currentTool == Tool.Lightning)
        {
            ClearNodes();
            this.gameObject.SetActive(false);
        }

    }

    public void OnAfterShock()
    {
        if(currentTool == Tool.Lightning)
            Move();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodeGridRange();
    }

    private void Move()
    {
        Debug.Assert(path.Count > 0, "ERROE: Bat has no Path!");
        isMoving = true;
        ClearNodes();
        throw new System.NotImplementedException();
    }

    private void SetNodeGridRange()
    {
        // if(nodes != null || nodes.Count > 0 )
        //     ClearNodes();
        SetNodes(this.worldPos, NodeType.Walkable, this);
        nodeGridRange = NodeGrid.GetNeighorNodes(nodes[0], NodeGrid.Instance.grid, 5);
    }

    private void Step()
    {
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            targetIndex++;
            if(destinationReached)
            {
                isMoving = false;
                return;
            }
            currentTargetNode = path[targetIndex];
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
    }

    private void SetPath()
    {
        // destionationNode = 
        // Debug.Assert(destionationNode != null, "ERROR: Destination is null");
        // path = 
    }


    private void OnStop()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
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
