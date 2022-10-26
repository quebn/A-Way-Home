using System.Collections.Generic;
using UnityEngine;
// Grid should be in Platform GameObject
public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance {get; private set;}
    public LayerMask unwakableMask;
    public LayerMask walkablePlatform;
    public Vector2 gridSize;
    public float nodeRadius;
    public GameObject revealedTile;
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
                Gizmos.color = (pair.Value.isWalkable)?Color.white : Color.red;
                if (path != null && path.Contains(pair.Value))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(pair.Value.worldPosition, new Vector3( nodeDiameter - .1f, nodeDiameter - .1f, 0));
            }
        }            
    }

    private void CreateGrid()
    {
        grid = new Dictionary<Vector2, Node>(gridSizeInt.x * gridSizeInt.y);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSizeInt.x; x++)
        {
            for (int y = 0; y < gridSizeInt.y; y++)
            {
                Vector2 gridCoord = new Vector2(x, y);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwakableMask));
                grid[gridCoord] = new Node(walkable, worldPoint, new Vector2Int(x, y));
            }
        }
    }
    // UpdateGrid() should be called every player actions that places an obstacle
    public void UpdateGrid()
    {
        foreach (KeyValuePair<Vector2, Node> pair in grid)
        {
            Vector3 nodeWorldPos = pair.Value.worldPosition;
            bool hasPlatform = Physics2D.OverlapCircle(nodeWorldPos, nodeRadius, walkablePlatform);
            if(hasPlatform)
                pair.Value.isWalkable = hasPlatform;
            else    
                pair.Value.isWalkable = !(Physics2D.OverlapCircle(nodeWorldPos, nodeRadius, unwakableMask));
        }
    }

    public Node NodeWorldPointPos(Vector3 worldpos)
    {
        float percentX = (worldpos.x + gridSize.x / 2) /  gridSize.x;
        float percentY = (worldpos.y + gridSize.y / 2) /  gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeInt.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeInt.y - 1) * percentY);

        return grid[new Vector2(x, y)];
    }

}
