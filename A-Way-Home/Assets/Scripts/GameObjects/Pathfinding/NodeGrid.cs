using System.Collections.Generic;
using UnityEngine;
// Grid should be in Platform GameObject
public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance {get; private set;}
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private LayerMask walkablePlatform;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float nodeRadius;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilePrefabParent;
    // publics
    [HideInInspector] public Vector2Int gridSizeInt;
    [HideInInspector] public Dictionary<Vector2, Node> grid;
    [HideInInspector] public List<Node> path;
    private float nodeDiameter;

    public int  maxSize{
        get {return gridSizeInt.x * gridSizeInt.y ; }
    }

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeInt.x = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeInt.y = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    private void Start()
    {
        if (Instance == null)   
            Instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 0));
        if (grid != null)
        {
            foreach (KeyValuePair<Vector2, Node> pair in grid)
            {
                if (pair.Value.type == NodeType.Water)
                    Gizmos.color = Color.blue;
                else if(pair.Value.type == NodeType.Terrain)
                    Gizmos.color = Color.black;
                else if (pair.Value.type == NodeType.Obstacle)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.white;
                if (path != null && path.Contains(pair.Value))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(pair.Value.worldPosition, new Vector3( nodeDiameter - .1f, nodeDiameter - .1f, 0));
            }
        }
    }

    private void CreateGrid()
    {
        NodeType nodeType;
        grid = new Dictionary<Vector2, Node>(gridSizeInt.x * gridSizeInt.y);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSizeInt.x; x++)
        {
            for (int y = 0; y < gridSizeInt.y; y++)
            {
                Vector2 gridCoord = new Vector2(x, y);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                if (Physics2D.OverlapCircle(worldPoint, nodeRadius, terrainMask))
                    nodeType = NodeType.Terrain;
                else if (Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask))
                    nodeType = NodeType.Obstacle;
                else if (Physics2D.OverlapCircle(worldPoint, nodeRadius, waterMask))
                    nodeType = NodeType.Water;
                else
                    nodeType = NodeType.Walkable;
                grid[gridCoord] = new Node(nodeType, worldPoint, new Vector2Int(x, y), tilePrefab, tilePrefabParent);
            }
        }
    }
    // UpdateGrid() should be called every player actions that places an obstacle
    public void UpdateGrid()
    {
        foreach (KeyValuePair<Vector2, Node> pair in grid)
        {
            if (pair.Value.type == NodeType.Terrain || pair.Value.type == NodeType.Water)
                continue;
            Vector3 nodeWorldPos = pair.Value.worldPosition;
            if (Physics2D.OverlapCircle(nodeWorldPos, nodeRadius, obstacleMask))
                pair.Value.type = NodeType.Obstacle;
            else    
                pair.Value.type = NodeType.Walkable;
        }
    }

    public static Node NodeWorldPointPos(Vector3 worldpos)
    {
        Vector2 gridSize = Instance.gridSize; 
        Vector2Int gridSizeInt = Instance.gridSizeInt;

        float percentX = (worldpos.x + gridSize.x * 0.5f) /  gridSize.x;
        float percentY = (worldpos.y + gridSize.y * 0.5f) /  gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        
        Vector2 index = new Vector2();
        index.x = Mathf.RoundToInt((gridSizeInt.x - 1) * percentX);
        index.y = Mathf.RoundToInt((gridSizeInt.y - 1) * percentY);

        return Instance.grid[index];
    }

}
