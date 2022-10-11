using System.Collections.Generic;
using UnityEngine;
// Grid should be in Platform GameObject
public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance {get; private set;}
    public LayerMask unwakableMask;
    public Vector2 gridSize;
    public float nodeRadius;
    public GameObject revealedTile;
    [HideInInspector] public Vector2Int gridSizeInt;
    [HideInInspector] public Node[,] grid;
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
    private void FixedUpdate()
    {
        UpdateGrid();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 0));
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.isWalkable)?Color.white : Color.red;
                if (path != null && path.Contains(node))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(node.worldPosition, new Vector3( nodeDiameter - .1f, nodeDiameter - .1f, 0));
            }
        }            
    }
    private void CreateGrid()
    {
        grid = new Node[gridSizeInt.x, gridSizeInt.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSizeInt.x; x++)
        {
            for (int y = 0; y < gridSizeInt.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwakableMask));
                grid[x, y] = new Node(walkable, worldPoint, new Vector2Int(x, y));
            }
        }
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < gridSizeInt.x; x++)
        {
            for (int y = 0; y < gridSizeInt.y; y++)
            {
                Vector3 NodeWorldPos = grid[x, y].worldPosition;
                grid[x, y].isWalkable = !(Physics2D.OverlapCircle(NodeWorldPos, nodeRadius, unwakableMask));
            }
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

        return grid[x, y];
    }

}
