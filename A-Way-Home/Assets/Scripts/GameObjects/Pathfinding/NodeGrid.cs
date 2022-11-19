using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance {get; private set;}
    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private LayerMask walkableMask;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float nodeRadius;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilePrefabParent;
    [HideInInspector] public Vector2Int gridSizeInt;
    [HideInInspector] public Dictionary<Vector2, Node> grid;
    private float nodeDiameter;

    public int  maxSize{
        get {return gridSizeInt.x * gridSizeInt.y ; }
    }

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeInt.x = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeInt.y = Mathf.RoundToInt(gridSize.y / nodeDiameter);
    }

    private void Start()
    {
        CreateGrid();
        if (Instance == null)   
            Instance = this;
    }

    private void CreateGrid()
    {
        NodeType nodeType = NodeType.Walkable;
        grid = new Dictionary<Vector2, Node>(gridSizeInt.x * gridSizeInt.y);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSizeInt.x; x++)
        {
            for (int y = 0; y < gridSizeInt.y; y++)
            {
                Vector2 gridCoord = new Vector2(x, y);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool hasPlatform = Physics2D.OverlapCircle(worldPoint, nodeRadius, walkableMask);
                Collider2D collider2D = Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
                if (collider2D == null)
                    nodeType = NodeType.Walkable;
                else if (collider2D.gameObject.tag == "Obstacle")
                    nodeType = NodeType.Obstacle;
                else if (collider2D.gameObject.tag == "Water")
                    nodeType = NodeType.Water;
                else if (collider2D.gameObject.tag == "Terrain")
                    nodeType = NodeType.Terrain;
                grid[gridCoord] = new Node(nodeType, worldPoint, new Vector2Int(x, y), tilePrefab, tilePrefabParent, hasPlatform);
                if (hasPlatform)
                    grid[gridCoord].currentType = NodeType.Walkable;
            }
        }
    }
    
    public void UpdateGrid()
    {
        foreach (KeyValuePair<Vector2, Node> pair in grid)
        {
            if (pair.Value.currentType == NodeType.Terrain || pair.Value.currentType == NodeType.Water)
                continue;
            Vector3 nodeWorldPos = pair.Value.worldPosition;
            Collider2D collider2D = Physics2D.OverlapCircle(nodeWorldPos, nodeRadius, unwalkableMask); 
            if (collider2D != null && collider2D.gameObject.tag == "Obstacle")
                pair.Value.currentType = NodeType.Obstacle;
            else    
                pair.Value.currentType = NodeType.Walkable;
        }
    }
    public static void ToggleGridTiles(bool toggle)
    {
        foreach(KeyValuePair<Vector2, Node> pair in Instance.grid)
        {
            pair.Value.tileObject.SetActive(toggle);
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
