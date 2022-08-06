using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask UnwakableMask;
    public Vector2 GridSize;
    public float NodeRadius;
    
    private List<Node> _Path;
    private Node[,] _Grid;
    private float _NodeDiameter;
    private int _GridSizeX, _GridSizeY;

    public int  MaxSize{
        get {return _GridSizeX * _GridSizeY ; }
    }
    public List<Node> Path{
        get {return _Path;}
        set {_Path = value;}
    }
    private void Awake()
    {
        _NodeDiameter = NodeRadius * 2;
        _GridSizeX = Mathf.RoundToInt(GridSize.x / _NodeDiameter);
        _GridSizeY = Mathf.RoundToInt(GridSize.y / _NodeDiameter);
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
        _Grid = new Node[_GridSizeX, _GridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * GridSize.x / 2 - Vector3.up * GridSize.y / 2;
        for (int x = 0; x < _GridSizeX; x++)
        {
            for (int y = 0; y < _GridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _NodeDiameter + NodeRadius) + Vector3.up * (y * _NodeDiameter + NodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, NodeRadius, UnwakableMask));
                _Grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public void UpdateGrid()
    {
        for (int x = 0; x < _GridSizeX; x++)
        {
            for (int y = 0; y < _GridSizeY; y++)
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

        int x = Mathf.RoundToInt((_GridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_GridSizeY - 1) * percentY);

        return _Grid[x, y];
    }

    public List<Node> GetNeighbor(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {

            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y ==0)
                    continue;

                int checkx = node.GridX + x;
                int checky = node.GridY + y;

                if (checkx >= 0 && checkx < _GridSizeX && checky >=0 && checky < _GridSizeY)
                    neighbors.Add(_Grid[checkx, checky]);
            }
        }
        return neighbors;
    }

}
