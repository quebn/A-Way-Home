using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle, ITrap, ILightning, IActionWaitProcess, ISelectable, ICommand
{
    [SerializeField] private Animator animator;
    [SerializeField] private int damage;
    [SerializeField] private GameObject poisonMiasma;
    
    private bool isMoving = false;
    private Dictionary<Vector2Int, Node> nodeGridRange;
    private Vector3 targetPosition;
    private List<Node> gridNodes;

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;

    private bool destinationReached => targetPosition == (Vector3)worldPos;

    public void OnLightningHit(int damage)
    {
        ForceDehighlight();
        Damage(damage);
        if(hitpoints > 0)
            Move();
    }

    public void OnAftershock(Vector2 lightningOrigin)
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

    private bool GoToTargetNode(Node node)
    {
        if(!gridNodes.Contains(node))
            return false;
        targetPosition = node.worldPosition;
        isMoving = true;
        ClearNodes();
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
        // Debug.Assert(path.Count > 0, "ERROE: Bat has no Path!");
        SetRandomPosition();
        isMoving = true;
        ClearNodes();
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
        SetNodes(this.worldPos, NodeType.Walkable, this);
        nodeGridRange = NodeGrid.GetNeighborNodes(nodes[0], NodeGrid.Instance.grid, 5);
    }

    private IEnumerator GoToTarget()
    {
        while(isMoving)
        {
            if(destinationReached)
            {
                OnStop();
                PlayerActions.FinishProcess(this);
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
                OnStop();
                PlayerActions.FinishCommand();
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
            OnStop();
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, 5f * Time.deltaTime);
    }

    private void OnStop()
    {
        isMoving = false;
        Node node = NodeGrid.NodeWorldPointPos(targetPosition);
        if(node.GetObstacle().isCorrosive)
            Destroy(node.GetObstacle());
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
