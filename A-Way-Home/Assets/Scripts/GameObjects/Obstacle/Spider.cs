using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Obstacle, IOnPlayerAction, ILightning, ICommand
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject webPrefab;

    private Node currentTargetNode;
    private Node lastNode;
    private List<Node> walkableNodes;
    private bool canWeb => hitpoints > 1;

    private bool isMoving {
        get => animator.GetBool("isMoving"); 
        set => animator.SetBool("isMoving", value); 
    }
    protected override int hitpoints { 
        get => animator.GetInteger("hitpoints"); 
        set => animator.SetInteger("hitpoints", value); 
    }

    private void Update()
    {
        if(isMoving)
        {
            Step();
        }
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
        walkableNodes = NodeGrid.GetPathNeighborNodes(nodes[0], NodeGrid.Instance.grid);
        Debug.Assert(walkableNodes.Count > 0);
    }


    protected override void OnHighlight(Tool tool)
    {
        if(tool == Tool.Lightning || tool == Tool.Command)
            spriteRenderer.color = Color.red;
    }

    public void OnLightningHit()
    {
        Remove();
    }

    public void OnCommand()
    {
        hitpoints = canWeb? 1 : 2;
    }


    public void OnPerformAction()
    {
        if(canWeb)
            Move();
    }

    private void Move()
    {
        if(walkableNodes == null || walkableNodes.Count == 0 || hitpoints <= 0)
            return;
        SetCurrentTargetNode();
        if(currentTargetNode == null)
            return;
        Debug.LogWarning($"Targetnode pos => {currentTargetNode.worldPosition}");
        lastNode = nodes[0];
        ClearNodes();
        SpawnWeb();
        isMoving = true;
    }

    private void SetCurrentTargetNode()
    {
        if(walkableNodes.Count == 0)
            return;
        int randomIndex = UnityEngine.Random.Range(0, walkableNodes.Count);
        if(!walkableNodes[randomIndex].IsType(NodeType.Walkable) || walkableNodes[randomIndex].hasObstacle){
            walkableNodes.RemoveAt(randomIndex);
            currentTargetNode = null;
            SetCurrentTargetNode();
        }else
            currentTargetNode = walkableNodes[randomIndex];
    }

    private void Step()
    {
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            Stop();
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
    }

    private void Stop()
    {
        isMoving = false;
        currentTargetNode = null;
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        walkableNodes = NodeGrid.GetPathNeighborNodes(nodes[0], NodeGrid.Instance.grid);
    }

    private void SpawnWeb()
    {
        Web web = GameObject.Instantiate(webPrefab, lastNode.worldPosition, Quaternion.identity).GetComponent<Web>();
        web.AddAsSpawned($"{GameData.levelData.spawnCount += 1}");
    }

}
