using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : Obstacle, ITrap, IInteractable, IOnPlayerAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private int damage; 
    [SerializeField] private int travelSpeed; 
    [SerializeField] private bool canPhase; 
    
    private List<Node> path;
    private Node currentTargetNode;
    private int currentTargetIndex;
    private int  xPositionDiff => (int)(currentTargetNode.worldPosition.x - this.worldPos.x);
    private int  yPositionDiff => (int)(currentTargetNode.worldPosition.y - this.worldPos.y);
    private bool canMove => currentTargetIndex < travelSpeed;
    private bool isMoving {get => animator.GetBool("isMoving"); set => animator.SetBool("isMoving", value); }
    protected override int hitpoints {get => animator.GetInteger("hitpoints"); set => animator.SetInteger("hitpoints", value); }

    private void Update()
    {
        if(isMoving)
            Step();
    }

    private void OnDestroy()
    {
        PlayerActions.onPlayerActions.Remove(this);
    }

    protected override void Initialize()
    {
        base.Initialize();
        AddToOnPlayerActionList(this);
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        TrySetPath();
    }

    public void OnTrapTrigger(Character character)
    {
        character.TriggerDeath();
    }

    private bool TrySetPath()
    {
        List<Vector3> targetPositions = new List<Vector3>();
        targetPositions.Add(Character.instance.currentPosition);
        path = !canPhase ? Pathfinding.FindPath(this.worldPos, targetPositions) : Pathfinding.FindPathPhased(this.worldPos, targetPositions, NodeGrid.Instance.grid);
        // Debug.LogWarning($"Skeleton Path is {path.Count}");
        return path.Count > 0;
    }

    private void Move()
    {
        if(TrySetPath())
        {
            ClearNodes();
            isMoving = true;
            currentTargetIndex = 0;
            currentTargetNode = path[0];
        }
    } 

    private void Step()
    {
        // if(currentTargetIndex > travelSpeed)
        //     return;
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            currentTargetIndex ++;
            if(Character.instance.isDead || !canMove)
            {
                SetNodes(this.worldPos, NodeType.Obstacle, this);
                isMoving = false;
                return;
            }
            Debug.Assert(path.Count > currentTargetIndex, $"ERROR: Tried to access index {currentTargetIndex} with path of size {path.Count}");
            currentTargetNode = path[currentTargetIndex];
        }
        UpdateAnimation();
        this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        // Debug.Log($"Pos: {xPositionDiff}, {yPositionDiff}");
        if(xPositionDiff > 0 && yPositionDiff == 0)
            animator.Play("Right");
        else if(xPositionDiff < 0 && yPositionDiff == 0)
            animator.Play("Left");
        else if(xPositionDiff == 0 && yPositionDiff > 0)
            animator.Play("Backward");
        else if(xPositionDiff == 0 && yPositionDiff < 0)
            animator.Play("Forward");
    }

    public void OnInteract()
    {
        if(currentTool == Tool.Lightning)
            Damage(1);
    }

    public void OnHighlight()
    {
        if(currentTool == Tool.Lightning)
            spriteRenderer.color = Color.green;
    }

    public void OnDehighlight()
    {
        if(currentTool == Tool.Lightning)
            spriteRenderer.color = Color.white;
    }

    public void Damage(int damage)
    {
        if(canPhase)
            return;
        hitpoints -= damage;
        Debug.Log($"hitpoints:{hitpoints}");
        if(hitpoints <= 0 )
            TriggerDeath();
    }

    public void TriggerDeath()
    {
        // Debug.Assert(false, "ERROR: UNIMPLEMENTED");
        this.gameObject.SetActive(false);
        ClearNodes();
    }

    public void OnPerformAction()
    {
        // Debug.Log($"Undead({this.gameObject.name}) moved to {currentTargetNode.worldPosition}");
        Move();
    }
}
