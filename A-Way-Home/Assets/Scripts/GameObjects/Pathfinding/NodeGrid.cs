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
        else if (collider2D.gameObject.tag == "Water")
            nodeType = NodeType.Water;
        else if (collider2D.gameObject.tag == "Terrain")
            nodeType = NodeType.Terrain;
        grid[new Vector2(x, y)] = new Node(nodeType, worldPoint, new Vector2Int(x, y), tilePrefab, tilePrefabParent, false);
    }

    public static List<Vector2> WorldPosByTileSize(Vector2 worldPos, int width, int height)
    {
        int size = width * height;
        List<Vector2> worldPositions = new List<Vector2>(size);
        Vector2 coord = new Vector2(SetToMid(worldPos.x, width), SetToMid(worldPos.y, height));
        Vector2 worldBottomLeft = coord - Vector2.right * width / 2 - Vector2.up * height / 2;
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * 1 + 0.5f) + Vector2.up * (y * 1 + 0.5f);
                worldPositions.Add(SetToMid(worldPoint));
            }
        }
        return worldPositions;
    }

    public static List<Node> NodesByTileSize(Vector2 worldPos, int width, int height)
    {
        List<Node> nodes = new List<Node>();
        List<Vector2> vector2s = WorldPosByTileSize(worldPos, width, height);
        for(int i = 0; i < vector2s.Count; i++)
            nodes.Add(NodeWorldPointPos(vector2s[i]));
        return nodes;
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

    [SerializeField] private bool enablePositionValues;
    private void OnDrawGizmos()
    {
        float diameter = nodeRadius * 2;
        Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;
        for (int x = 0; x < gridSize.x; x++){
            for (int y = 0; y < gridSize.y; y++){
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * diameter + nodeRadius) + Vector3.up * (y * diameter+ nodeRadius);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(worldPoint, new Vector3(diameter, diameter, 0));
                if (enablePositionValues)
                    UnityEditor.Handles.Label(worldPoint, $"{worldPoint.x}, {worldPoint.y}");
            }
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 0));
    }


    public static List<Node> GetNodesByRange(Node node, int tileRange)
    {
        List<Node> nodes = new List<Node>();
        Vector2Int check;
        for (int x = -tileRange; x <= tileRange; x++){
            for (int y = -tileRange; y <= tileRange; y++){
                if (x == 0 && y == 0)
                    continue;
                check = new Vector2Int(node.gridPos.x + x, node.gridPos.y + y);
                if (Instance.grid.ContainsKey(check) && Instance.grid[check].IsWalkable())
                    nodes.Add(Instance.grid[check]);
            }
        }
        return nodes;
    }

    public static List<Node> NodeWorldPointPos(List<Vector3> positions)
    {
        List<Node> nodes = new List<Node>();
        foreach (Vector3 pos in positions)
            nodes.Add(NodeWorldPointPos(pos));
        return nodes;
    }

    public static float SetToMid(float f, int size)
    {
        if (size % 2 == 0)
            return Mathf.RoundToInt(f);
        return SetToMid(f);

    }

    public static float SetToMid(float f)
    {
        if (f < 0)
            return  ((float)(int)f) - .5f;
        return ((float)(int)f) + .5f;
    }

    public static Vector3 SetToMid(Vector3 vector3)
    {
        return new Vector3(SetToMid(vector3.x), SetToMid(vector3.y), 0);
    }
}
