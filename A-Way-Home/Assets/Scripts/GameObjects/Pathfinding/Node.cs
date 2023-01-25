using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NodeType{ Walkable, Water, Terrain, Obstacle}
public class Node
{
    public Vector3 worldPosition;
    public Vector2Int gridPos;
    public Node parent;
    public bool containsObject;
    public int gCost;
    public List<int> hCosts ;

    private NodeType originalNodeType;
    private NodeType currentNodeType;
    private INodeInteractable interactableObstacle;

    private Color nodeColor {set => NodeGrid.tilemap.SetColor((Vector3Int)this.gridPos ,value); }
    private bool isWalkable => currentNodeType == NodeType.Walkable ;
    public int minHCost => hCosts.Min(); 
    public int fCost => gCost + minHCost;
    public NodeType currentType {  
        get => this.currentNodeType; 
        set {this.currentNodeType = value; UpdateColor();}
    }

    public static Color colorCyan   => new Color32(0, 255, 255, 150);
    public static Color colorWhite  => new Color32(255, 255, 255, 100);
    public static Color colorRed    => new Color32(255, 0, 0, 150);
    public static Color colorGreen  => new Color32(0, 255, 0, 150);
    public static Color colorBlue   => new Color32(0, 0, 255, 150);
    public static Color colorYellow   => new Color32(255, 255, 0, 150);
    public static Color colorClear  => Color.clear; 
    
    public Node(NodeType nodeType, Vector3 worldpos, Vector2Int grid, bool containsObj)
    {
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

    public bool IsType(NodeType nodeType)
    {
        if(currentNodeType == nodeType)
            return true;
        return false;
    }

    private void UpdateColor()
    {
        if(!NodeGrid.nodesVisibility){
            HideNode();
            return;
        }
        RevealNode();
    }

    public void HideNode()
    {
        nodeColor = colorClear;
    }

    public void UpdateNode()
    {
        UpdateColor();
    }

    public void RevealNode()
    {
        switch(currentNodeType)
        {
            case NodeType.Walkable:
                nodeColor = colorWhite;
                break;
            case NodeType.Terrain:
                HideNode();
                break;
            case NodeType.Water:
                nodeColor = colorBlue;
                break;
            case NodeType.Obstacle:
                nodeColor = colorRed;
                break;
        }
    }

    public void RevealNode(Color color)
    {
        nodeColor = color;
    }

    public void ToggleNode(bool toggle)
    {
        if(toggle)
            RevealNode();
        else
            HideNode();
    }

    public void ToggleNode(Color color, bool toggle)
    {
        if(toggle)
            RevealNode(color);
        else
            HideNode();
    }


    public void SetObstacle(INodeInteractable obstacle)
    {
        this.interactableObstacle = obstacle;
    }

    public void SetObstacle(INodeInteractable obstacle, NodeType nodeType)
    {
        this.interactableObstacle = obstacle;
        this.currentType = nodeType;
    }

    public void InteractObstacle(Tool tool)
    {
        if(interactableObstacle == null)
            return;
        interactableObstacle.OnNodeInteract(tool);
    }

    public static List<Node> GetNeighbors(Node node, Dictionary<Vector2Int, Node> grid, Vector2Int gridsize)
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
                    neighbors.Add(grid[new Vector2Int(checkx, checky)]);
            }
        }
        return neighbors;
    }

    public static void RevealNodes(List<Node> nodeList)
    {
        if (nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.RevealNode();
    }

    public static void RevealNodes(List<Node> nodeList, Color color)
    {
        if (nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.RevealNode(color);
    }

    public static void HideNodes(List<Node> nodeList)
    {
        if (nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.HideNode();
    }

    public static void ToggleNodes(List<Node> nodeList, bool active)
    {
        if (nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.ToggleNode(active);
    }

    public static void ToggleNodes(List<Node> nodeList, Color color,bool active)
    {
        if (nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.ToggleNode(color, active);
    }

    public static void SetNodesObstacle(List<Node> nodeList, INodeInteractable obstacle)
    {
        if(nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.SetObstacle(obstacle);
    }

    public static void TriggerNodesObstacle(List<Node> nodeList, Tool tool)
    {
        if(nodeList.Count == 0)
            return;
        foreach (Node node in nodeList)
            node.InteractObstacle(tool);
    } 
    
    public static void ToggleNodes(List<Node> nodeList, bool toggle, Character character)
    {
        if(nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            if(character.NodeInPath(node))
                node.ToggleNode(Node.colorGreen, toggle);
            else
                node.ToggleNode(toggle);
    }
}