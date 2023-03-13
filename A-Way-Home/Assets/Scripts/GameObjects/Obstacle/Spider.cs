using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Obstacle, IInteractable, IOnPlayerAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject webPrefab;
    [SerializeField] private int gridRange;

    private List<Node> path;
    private int currentTargetIndex;
    private Node currentTargetNode;
    private Node lastNode;
    private Dictionary<Vector2Int, Node> walkableGrid;


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

    protected override void Initialize()
    {
        base.Initialize();
        AddToOnPlayerActionList(this);
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        SetPath();
        Debug.Assert(path.Count > 0);
    }

    public void OnDehighlight()
    {
        throw new System.NotImplementedException();
    }

    public void OnHighlight()
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    public void OnPerformAction()
    {
        Move();
    }

    private void Move()
    {
        if(path.Count == 0)
            SetPath();
        if(path.Count > 0)
        {
            isMoving = true;
            currentTargetIndex = 0;
            currentTargetNode = path[0];
            lastNode = nodes[0];
            ClearNodes();
            // SpawnWeb();
        }
    }

    private void Step()
    {
        if(this.transform.position == currentTargetNode.worldPosition)
        {
            currentTargetIndex ++;
            // Debug.Assert(path.Count > currentTargetIndex, $"ERROR: Tried to access index {currentTargetIndex} with path of size {path.Count}");
            currentTargetNode = path[currentTargetIndex];
            isMoving = false;
            SetNodes(this.worldPos, NodeType.Obstacle, this);
            return;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
    }

    private void SetPath()
    {
        Debug.Assert(nodes.Count > 0, "ERROR: spider is not in node");
        walkableGrid = NodeGrid.GetNeighborNodes(this.nodes[0], NodeGrid.Instance.grid, gridRange); 
        List<Vector3> targets = Node.GetRandomWorldPos(walkableGrid, 3);
        path = Pathfinding.FindPath(this.nodes[0].worldPosition, targets);
    }

    private void SpawnWeb()
    {
        Web web = GameObject.Instantiate(webPrefab, lastNode.worldPosition, Quaternion.identity, this.transform).GetComponent<Web>();
        web.AddAsSpawned($"{GameData.levelData.spawnCount += 1}");
    }
}
