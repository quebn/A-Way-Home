using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance {get; private set;}
    [SerializeField] private LayerMask unwalkableMask;
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
        if (Instance == null)   
            Instance = this;
        nodeDiameter = nodeRadius * 2;
        gridSizeInt.x = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeInt.y = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Dictionary<Vector2, Node>(gridSizeInt.x * gridSizeInt.y);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSizeInt.x; x++){
            for (int y = 0; y < gridSizeInt.y; y++){
                CreateNode(x, y, worldBottomLeft);
            }
        }
    }

    private void CreateNode(int x, int y, Vector3 worldBottomLeft)
    {
        NodeType nodeType = NodeType.Walkable;
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
        Collider2D collider2D = Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask);
        if (collider2D == null)
            nodeType = NodeType.Walkable;
        // else if (collider2D.gameObject.tag == "Obstacle")
        //     nodeType = NodeType.Obstacle;
        else if (collider2D.gameObject.tag == "Water")
            nodeType = NodeType.Water;
        else if (collider2D.gameObject.tag == "Terrain")
            nodeType = NodeType.Terrain;
        grid[new Vector2(x, y)] = new Node(nodeType, worldPoint, new Vector2Int(x, y), tilePrefab, tilePrefabParent, false);
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

    private void OnDrawGizmos()
    {
        float diameter = nodeRadius * 2;
        Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSize.x; x++){
            for (int y = 0; y < gridSize.y; y++){
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * diameter + nodeRadius) + Vector3.up * (y * diameter+ nodeRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(worldPoint, new Vector3(diameter, diameter, 0));
                UnityEditor.Handles.Label(worldPoint, $"{worldPoint.x}, {worldPoint.y}");
            }
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 0));
    }

}
