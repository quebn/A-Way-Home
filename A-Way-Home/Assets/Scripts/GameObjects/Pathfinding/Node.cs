using System.Collections.Generic;
using UnityEngine;

public enum NodeType{ Walkable, Water, Terrain, Obstacle}//, Platform}
public class Node
{
    public GameObject tileObject;
    public SpriteRenderer tileSprite;
    public Vector3 worldPosition;
    public Vector2Int gridPos;
    public NodeType type;
    public Node parent;
    public bool containsObject;
    public int gCost;
    public int hCost;
    public int fCost{ get{ return gCost + hCost ;} }
    private bool isWalkable {get { return type == NodeType.Walkable ;}}

    public Node(NodeType type, Vector3 worldpos, Vector2Int grid, GameObject tilePrefab, Transform parent)
    {
        tileObject = GameObject.Instantiate(tilePrefab, worldpos, Quaternion.identity, parent);
        tileSprite = tileObject.GetComponent<SpriteRenderer>();
        worldPosition = worldpos;
        gridPos = grid;
        this.type = type;
        containsObject = false;
        tileObject.SetActive(false);
    }
    public bool IsWalkable(bool canWalkWater = false)
    {
        if (canWalkWater)
            return (isWalkable || type == NodeType.Water);
        return isWalkable;
    }
    public int CompareNode(Node node)
    {
        int compare = fCost.CompareTo(node.fCost);

        if (compare == 0)
            compare = hCost.CompareTo(node.hCost);
        return -compare;
    }
    
    public static List<Node> GetNeighbors(Node node, Dictionary<Vector2, Node> grid, Vector2Int gridsize)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Prevent middle node and diagonal nodes from being include in neighbors of the node.
                // (0, 0), (1, 1), (-1, -1), (-1 , 1), (1, -1)
                if (x == y || x + y == 0)
                    continue;
                    
                int checkx = node.gridPos.x + x;
                int checky = node.gridPos.y + y;

                if (checkx >= 0 && checkx < gridsize.x && checky >=0 && checky < gridsize.y)
                    neighbors.Add(grid[new Vector2(checkx, checky)]);
            }
        }
        return neighbors;
    }

}