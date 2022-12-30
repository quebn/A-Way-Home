using System.Collections.Generic;
using UnityEngine;

public enum NodeType{ Walkable, Water, Terrain, Obstacle}
public class Node
{
    public GameObject tileObject;
    public SpriteRenderer tileRenderer;
    public Vector3 worldPosition;
    public Vector2Int gridPos;
    private NodeType originalNodeType;
    private NodeType currentNodeType;
    public Node parent;
    public bool containsObject;
    public int gCost;
    public int hCost;
    
    public int fCost => gCost + hCost ;
    private bool isWalkable => currentNodeType == NodeType.Walkable ;
    public NodeType currentType {  
        get => this.currentNodeType; 
        set { this.currentNodeType = value; UpdateColor();} 
    }
    
    public static Color colorWhite  => new Color32(255, 255, 255, 100);   
    public static Color colorRed    => new Color32(255, 0, 0, 150);       
    public static Color colorBlue   => new Color32(0, 0, 255, 150);       
    public static Color colorClear  => Color.clear; 
    
    public Node(NodeType nodeType, Vector3 worldpos, Vector2Int grid, GameObject tilePrefab, Transform parent, bool containsObj)
    {
        this.tileObject = GameObject.Instantiate(tilePrefab, worldpos, Quaternion.identity, parent);
        this.tileRenderer = tileObject.GetComponent<SpriteRenderer>();
        this.worldPosition = worldpos;
        this.gridPos = grid;
        this.originalNodeType = nodeType;
        this.currentType = nodeType;
        this.containsObject = containsObj;
    }

    public void RevertNode()
    {
        this.containsObject = false;
        this.currentType = this.originalNodeType;
    }

    public bool IsWalkable(bool canWalkWater = false)
    {
        if (canWalkWater)
            return (isWalkable || currentNodeType == NodeType.Water);
        return isWalkable;
    }

    public int CompareNode(Node node)
    {
        int compare = fCost.CompareTo(node.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(node.hCost);
        return -compare;
    }

    public void UpdateColor()
    {
        switch(currentNodeType)
        {
            case NodeType.Walkable:
                tileRenderer.color = colorWhite;
                break;
            case NodeType.Terrain:
                tileRenderer.color = colorClear;
                break;
            case NodeType.Water:
                tileRenderer.color = colorBlue;
                break;
            case NodeType.Obstacle:
                tileRenderer.color = colorRed;
                break;
        }
    }

    public static List<Node> GetNeighbors(Node node, Dictionary<Vector2, Node> grid, Vector2Int gridsize)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++){
            for (int y = -1; y <= 1; y++){
                // Prevent middle node and diagonal nodes from being included in neighbors of the node.
                // Grid coords that are excluded: (0, 0), (1, 1), (-1, -1), (-1 , 1), (1, -1)
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