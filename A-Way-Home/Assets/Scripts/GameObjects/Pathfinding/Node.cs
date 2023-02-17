using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public enum NodeType{ Walkable, Water, Terrain, Obstacle}
public class Node
{
    public Vector3 worldPosition;
    public Vector2Int gridPos;
    public Node parent;
    public int gCost;
    public List<int> hCosts ;

    private NodeType currentNodeType;
    private IInteractable interactableObstacle;//Maybe should be an Obstacle instead of of IInteractable.

    private Color nodeColor {set => NodeGrid.tilemap.SetColor((Vector3Int)this.gridPos ,value); }
    private bool isWalkable => currentNodeType == NodeType.Walkable ;
    public int hCost => MinHCost(); 
    public int fCost => gCost + hCost;
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
    
    public Node(NodeType nodeType, Vector3 worldpos, Vector2Int grid)
    {
        this.worldPosition = worldpos;
        this.gridPos = grid;
        this.currentType = nodeType;
        this.hCosts = new List<int>();
    }

    private int MinHCost()
    {
        if (!hCosts.Any<int>())
            Debug.LogWarning($"Node[{gridPos.x}, {gridPos.y}] has no Contents");

            // return 0;
        return hCosts.Min<int>();
        // int hcost = hCosts.Min<int>();
        // Debug.LogWarning($"Node[{gridPos.x}, {gridPos.y}] => MIN hCost: {hcost}");
        // return hcost;
    }

    public bool IsWalkable(NodeType nodeType = NodeType.Walkable, Type obstacleType = null)
    {
        if(obstacleType != null && interactableObstacle != null)
            return IsObstacle(obstacleType);
        return (currentNodeType == nodeType || currentNodeType == NodeType.Walkable );
    }

    public bool IsType(NodeType nodeType)
    {
        return currentNodeType == nodeType;
    }

    public bool IsObstacle(Type type)
    {
        return interactableObstacle != null && type.IsAssignableFrom(interactableObstacle.GetType());
    }

    public IInteractable GetObstacle()
    {
        return interactableObstacle;
    }

    public bool IsEmpty(NodeType walkableNodeType = NodeType.Walkable)
    {
        return interactableObstacle == null && IsWalkable(walkableNodeType);
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

    public void UpdateNodeColor()
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

    public void SetInteractable(IInteractable obstacle, NodeType nodeType)
    {
        this.interactableObstacle = obstacle;
        this.currentNodeType = nodeType;
        UpdateColor();
    }

    public bool InteractObstacle()
    {
        if(interactableObstacle == null)
            return false;
        interactableObstacle.OnInteract();
        return true;
    }

    public bool InteractObstacleAfterShock()
    {
        if(interactableObstacle == null)
            return false;
        interactableObstacle.OnAfterShock();
        return true;
    }

    public static void RevealNodes(List<Node> nodeList)
    {
        if (nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.RevealNode();
    }

    public static void RevealNodes(List<Node> nodeList, Color color)
    {
        if (nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.RevealNode(color);
    }

    public static void HideNodes(List<Node> nodeList)
    {
        if (nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.HideNode();
    }

    public static void ToggleNodes(List<Node> nodeList, bool active)
    {
        if (nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.ToggleNode(active);
    }

    public static void ToggleNodes(List<Node> nodeList, Color color,bool active)
    {
        if (nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.ToggleNode(color, active);
    }


    public static void SetNodesInteractable(List<Node> nodeList, NodeType nodeType, IInteractable interactable = null)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.SetInteractable(interactable, nodeType);
    }

    public static void TriggerNodesObstacle(List<Node> nodeList)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return;
        foreach (Node node in nodeList)
            node.InteractObstacle();
    }

    public static void TriggerObstacleAfterShock(List<Node> nodeList)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return;
        foreach (Node node in nodeList)
            node.InteractObstacleAfterShock();
    }
    
    public static void ToggleNodes(List<Node> nodeList, bool toggle, Character character)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            if(character.NodeInPath(node))
                node.ToggleNode(Node.colorGreen, toggle);
            else
                node.ToggleNode(toggle);
    }

    public static void SetNodesType(List<Node> nodeList, NodeType type)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return;
        foreach(Node node in nodeList)
            node.currentNodeType = type;
    }

    public static bool CheckNodesType(List<Node> nodeList, NodeType nodeType, Type type)
    {
        if(nodeList == null ||nodeList.Count == 0)
            return false;
        foreach (Node node in nodeList)
            if(node.IsWalkable(nodeType, type))
                return true;
        return false;
    }

    public static List<IInteractable> GetNodesInteractable(List<Node> nodeList)
    {
        List<IInteractable> interactables = new List<IInteractable>();
        foreach(Node node in nodeList)
            if(node.interactableObstacle != null && !interactables.Contains(node.interactableObstacle))
                interactables.Add(node.interactableObstacle);
        return interactables;
    }

    public static Vector3 GetRandomWorldPos(Dictionary<Vector2Int, Node> grid)
    {
        return grid.Values.ToList()[UnityEngine.Random.Range(0, grid.Count)].worldPosition;
    }

    public static List<Vector3> GetRandomWorldPos(Dictionary<Vector2Int, Node> grid, int count)
    {
        Debug.Assert(count > 0, "ERROR: Count should be greater than 0!");
        List<Vector3> positions = new List<Vector3>(count);
        for (int i = 0; i < count ; i++)
            positions.Add(GetRandomWorldPos(grid));
        return positions;
    }
}