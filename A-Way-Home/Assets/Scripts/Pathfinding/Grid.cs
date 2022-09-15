using System.Collections.Generic;
using UnityEngine;
// Grid should be in Platform GameObject
public class Grid : MonoBehaviour
{
    public LayerMask UnwakableMask;
    public Vector2 GridSize;
    public float NodeRadius;
    
    private List<Node> _Path;
    private Node[,] _Grid;
    private float _NodeDiameter;
    private Vector2Int _GridSizeInt;

    public int  MaxSize{
        get {return _GridSizeInt.x * _GridSizeInt.y ; }
    }
    public Vector2Int GridSizeInt{
        get {return _GridSizeInt;}
    }
    public Node[,] NodeGrid{
        get { return _Grid; }
    }
    public List<Node> Path{
        get {return _Path;}
        set {_Path = value;}
    }
    private void Awake()
    {
        _NodeDiameter = NodeRadius * 2;
        _GridSizeInt.x = Mathf.RoundToInt(GridSize.x / _NodeDiameter);
        _GridSizeInt.y = Mathf.RoundToInt(GridSize.y / _NodeDiameter);
        CreateGrid();
    }
    private void FixedUpdate()
    {
        UpdateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x, GridSize.y, 0));
        if (_Grid != null)
        {
            foreach (Node node in _Grid)
            {
                Gizmos.color = (node.IsWalkable)?Color.white : Color.red;
                if (Path != null && Path.Contains(node))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(node.WorldPosition, new Vector3( _NodeDiameter - .1f, _NodeDiameter - .1f, 0));
            }
        }            
    }
    private void CreateGrid()
    {
        _Grid = new Node[GridSizeInt.x, GridSizeInt.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * GridSize.x / 2 - Vector3.up * GridSize.y / 2;
        for (int x = 0; x < GridSizeInt.x; x++)
        {
            for (int y = 0; y < GridSizeInt.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _NodeDiameter + NodeRadius) + Vector3.up * (y * _NodeDiameter + NodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, NodeRadius, UnwakableMask));
                _Grid[x, y] = new Node(walkable, worldPoint, new Vector2Int(x, y));
            }
        }
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < GridSizeInt.x; x++)
        {
            for (int y = 0; y < GridSizeInt.y; y++)
            {
                Vector3 NodeWorldPos = _Grid[x, y].WorldPosition;
                _Grid[x, y].IsWalkable = !(Physics2D.OverlapCircle(NodeWorldPos, NodeRadius, UnwakableMask));
            }
        }
    }

    public Node NodeWorldPointPos(Vector3 worldpos)
    {
        float percentX = (worldpos.x + GridSize.x / 2) /  GridSize.x;
        float percentY = (worldpos.y + GridSize.y / 2) /  GridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((GridSizeInt.x - 1) * percentX);
        int y = Mathf.RoundToInt((GridSizeInt.y - 1) * percentY);

        return _Grid[x, y];
    }

}
