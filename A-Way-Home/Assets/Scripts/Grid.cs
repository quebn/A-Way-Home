using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask UnwakableMask;
    public Vector2 GridSize;
    public float NodeRadius;
    
    Node[,] _Grid;
    float _NodeDiameter;
    int _GridSizeX, _GridSizeY;

    public int  MaxSize{
        get {return _GridSizeX * _GridSizeY ; }
    }

    void CreateGrid()
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
    public Node NodeWorldPointPos(Vector3 worldpos)
    {
        float percentX = ((byte)(worldpos.x + GridSize.x / 2)) /  GridSize.x;
        float percentY = ((byte)(worldpos.y + GridSize.y / 2)) /  GridSize.y;

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

    // Start is called before the first frame update
    void Start()
    {
        _NodeDiameter = NodeRadius * 2;
        _GridSizeX = Mathf.RoundToInt(GridSize.x / _NodeDiameter);
        _GridSizeY = Mathf.RoundToInt(GridSize.y / _NodeDiameter);
        CreateGrid();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x, GridSize.y, 0));
        // Debug.Assert(_Grid != null);
        if (_Grid != null)
        {
            foreach (Node node in _Grid)
            {
                Gizmos.color = (node.IsWalkable)?Color.white : Color.red;

                if (path != null && path.Contains(node))
                    Gizmos.color = Color.green;
                Gizmos.DrawCube(node.WorldPosition, new Vector3( _NodeDiameter - .1f, _NodeDiameter - .1f, 0));
            }
        }            
    }
}
