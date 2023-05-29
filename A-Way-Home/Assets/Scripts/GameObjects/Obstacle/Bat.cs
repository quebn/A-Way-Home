using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle, ITrap, ILightning, IActionWaitProcess, ISelectable, ICommand
{
    [SerializeField] private Animator animator;
    
    private bool isMoving = false;
    private Dictionary<Vector2Int, Node> nodeGridRange;
    private Vector3 targetPosition;
    private List<Node> gridNodes;
    private bool wasInteracted = false;
    private Vector2 direction = Vector2.left;

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;

    private bool destinationReached => targetPosition == (Vector3)worldPos;

    public void OnLightningHit(int damage)
    {
        ForceDehighlight();
        Damage(damage);
        audioSources[1].Play();
    }

    public void OnAftershock(Vector2 lightningOrigin)
    {
        ForceDehighlight();
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos + (this.worldPos - lightningOrigin));
        if(node.worldPosition == this.transform.position || node.IsType(NodeType.Terrain) || node.hasObstacle)
            return;
        targetPosition = node.worldPosition; 
        Move();
    }

    public void OnPlayerAction()
    {

        if(isMoving)
           return;
        if(!wasInteracted)
            MoveHorizontal();
        PlayerActions.FinishProcess(this);
    }

    private void MoveHorizontal()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos + direction);
        if(node.worldPosition == this.transform.position || node.IsType(NodeType.Terrain) || node.hasObstacle)
        {
            direction = direction == Vector2.left ? Vector3.right : Vector3.left;
            node = NodeGrid.NodeWorldPointPos(this.worldPos + direction);
            if(node.worldPosition == this.transform.position || node.IsType(NodeType.Terrain) || node.hasObstacle)
                return;
        }
        targetPosition = node.worldPosition; 
        Move();
    }

    public void OnSelect(Tool tool)
    {
        if(tool != Tool.Command)
            return;
        gridNodes = new List<Node>();
        foreach(Node node in nodeGridRange.Values)
            if(node.IsWalkable() || (node.hasObstacle && node.GetObstacle().isCorrosive))
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
            node.HighlightObstacle(Node.colorPurple, Tool.Inspect);
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

    public bool OnCommand(List<Node> nodes)
    {
        if(nodes.Count == 0)
            return false;
        return GoToTargetNode(nodes[0]);
    }

    public override void Remove()
    {
        ForceDehighlight();
        audioSources[0].Play();
        if(isMoving){
            isMoving = false;
            audioSources[2].Stop();
        }
        hitpoints = 0;
        ClearNodes(isRetained:true);
        PlayerActions.FinishCommand(this);
        PlayerActions.FinishProcess(this);
        this.gameObject.SetActive(false);
    }

    private bool GoToTargetNode(Node node)
    {
        if(!gridNodes.Contains(node))
            return false;
        targetPosition = node.worldPosition;
        isMoving = true;
        audioSources[2].Play();
        ClearNodes(isRetained:true);
        StartCoroutine(GoToTargetCommand());
        return true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodeGridRange();
    }

    public void Move()
    {
        isMoving = true;
        audioSources[2].Play();
        ClearNodes(isRetained:true);
        StartCoroutine(GoToTarget());
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

    private void SetNodeGridRange()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this ,  retainType:true);
        nodeGridRange = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, 5);
    }

    private IEnumerator GoToTarget()
    {
        while(isMoving)
        {
            if(destinationReached)
            {
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
            yield return null; 
        }
    }

    private IEnumerator GoToTargetCommand()
    {
        while(isMoving)
        {
            if(destinationReached)
            {
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
            yield return null; 
        }
    }


    private void Step()
    {
        if(destinationReached)
        {
            Stop();
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
    }

    private void Stop()
    {
        isMoving = false;
        audioSources[2].Stop();
        Node node = NodeGrid.NodeWorldPointPos(targetPosition);
        OnStatusInteract(node);
        if(!gameObject.activeSelf)
            return;
        if(node.hasObstacle && node.GetObstacle().isCorrosive)
            Destroy(node.GetObstacle());
        SetNodes(this.worldPos, NodeType.Walkable, this, retainType:true);
        SetNodeGridRange();
        PlayerActions.FinishProcess(this);
        PlayerActions.FinishCommand(this);
    }

    private void SetRandomPosition()
    {
        targetPosition = new Vector3();
        targetPosition = Node.GetRandomWorldPos(nodeGridRange, NodeType.Walkable, false);
    }

    private void TriggerDeath()
    {
        hitpoints = 0;
        ClearNodes(isRetained: true);
        this.gameObject.SetActive(false);
    }

    public void OnTrapTrigger(Character character)
    {
        character.TriggerDeath();
    }
}
