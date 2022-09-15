using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Public
    public bool IsWalkable;
    public Node Parent;
    public Vector3 WorldPosition;
    public Vector2Int GridPos;
    public int Gcost;
    public int Hcost;
    
    // GetSets
    public int Fcost{
        get{ return Gcost + Hcost ;}
    }

    public Node(bool walkable, Vector3 worldpos, Vector2Int grid)
    {
        IsWalkable = walkable;
        WorldPosition = worldpos;
        GridPos = grid;
    }

    public int  CompareNode(Node node)
    {
        int compare = Fcost.CompareTo(node.Fcost);

        if (compare == 0)
            compare = Hcost.CompareTo(node.Hcost);
        return -compare;
    }
    
    public static List<Node> GetNeighbors(Node node, Node[,] grid, Vector2Int gridsize)
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
                    
                int checkx = node.GridPos.x + x;
                int checky = node.GridPos.y + y;

                if (checkx >= 0 && checkx < gridsize.x && checky >=0 && checky < gridsize.y)
                    neighbors.Add(grid[checkx, checky]);
            }
        }
        return neighbors;        
    }
}