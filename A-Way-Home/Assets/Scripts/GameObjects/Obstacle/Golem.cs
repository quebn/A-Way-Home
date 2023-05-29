using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Golem : Obstacle, ILightning, ICommand, ITremor, IActionWaitProcess, ISelectable, ITrap
{
    [SerializeField] private Animator animator;
    [SerializeField] private Light2D light2D;
    private Vector3 direction = Vector3.up;
    private bool isMoving  = false;
    private bool isUp = true;
    private Node targetNode;


    private bool canMove => hitpoints  == 3;

    // public override bool isCorrosive => true;

    protected override void Initialize()
    {
        base.Initialize();
        light2D.enabled = NodeGrid.isCovered;
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public List<Node> IgnoredToggledNodes()
    {
        return nodes;
    }

    public bool OnCommand(List<Node> nodes)
    {
        hitpoints = canMove ? 2 : 3;
        return true;
    }

    public void OnTrapTrigger(Character character)
    {
        character.TriggerDeath();
    }

    public void OnDeselect()
    {
        PlayerActions.FinishCommand(this);
        return;
    }

    public void OnLightningHit(int damage)
    {
        Damage(1);
    }

    public void OnTremor()
    {
        Damage(2);
    }

    public void OnPlayerAction()
    {
        if(!canMove)
        {
            PlayerActions.FinishProcess(this);
            return;
        }
        SetTargetNode();
        if(targetNode != null && canMove)
            StartCoroutine(MoveTile());
        else
            PlayerActions.FinishProcess(this);
    }

    public void OnSelect(Tool tool)
    {
        return;
    }

    public List<Node> OnSelectedHover(Vector3 mouseWorldPos, List<Node> currentNodes)
    {
        return nodes;
    }

    private void SetTargetNode()
    {
        // Check if upper node is walkable if not check under.
        Node node = NodeGrid.NodeWorldPointPos(this.transform.position + direction);
        if(node == nodes[0] || !node.IsWalkable() || node.hasObstacle)
        {
            direction = direction == Vector3.down ? Vector3.up : Vector3.down;
            node = NodeGrid.NodeWorldPointPos(this.transform.position + direction);
            if(node == nodes[0] || !node.IsWalkable() || node.hasObstacle)
                node = null;
        }
        isUp = direction == Vector3.up;
        targetNode = node;

    }

    private IEnumerator MoveTile()
    {
        ForceDehighlight();
        isMoving = true;
        animator.SetBool("isMoving", isMoving);
        animator.Play(isUp ? "Golem_up" : "Golem_down");
        Node prevNode = nodes[0];
        ClearNodes();
        FireNode.ContinueFire(prevNode);
        while(isMoving)
        {
            if(this.transform.position == targetNode.worldPosition)
            {
                OnStatusInteract(targetNode, IfFireImmune);
                audioSources[0].Play();
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetNode.worldPosition, 5f * Time.deltaTime);
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

    private void Stop()
    {
        isMoving = false;
        animator.SetBool("isMoving", isMoving);
        if(targetNode.hasObstacle)
            Destroy(targetNode.GetObstacle());
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        PlayerActions.FinishCommand(this);
        PlayerActions.FinishProcess(this);
    }
}
