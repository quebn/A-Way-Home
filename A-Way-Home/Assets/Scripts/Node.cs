using UnityEngine;

public class Node
{
    // Public
    public bool IsWalkable;
    public Vector3 WorldPosition;
    public int GridX;
    public int GridY;
    public int Gcost;
    public int Hcost;
    
    // Private
    private Node _Parent;

    // GetSets
    public int Fcost{
        get{ return Gcost + Hcost ;}
    }

    public Node Parent{
        get{ return _Parent; }
        set{ _Parent = value; }
    }

    public Node(bool walkable, Vector3 worldpos, int gridx, int gridy)
    {
        IsWalkable = walkable;
        WorldPosition = worldpos;
        GridX = gridx;
        GridY = gridy;
    }

    public int  CompareNode(Node node)
    {
        int compare = Fcost.CompareTo(node.Fcost);

        if (compare == 0)
            compare = Hcost.CompareTo(node.Hcost);
        return -compare;
    }
}