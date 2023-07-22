using UnityEngine;

public class FireNode
{
    public bool shouldBurn;
    public Node childNode;
    public bool hasChild => childNode != null;

    public FireNode()
    {
        shouldBurn = false;
    }

    public static void StartFire(Node node, Vector2Int direction, int count, bool burn = true)
    {
        if(count <= 0)
            return;
        node.fireNode.shouldBurn = true;
        bool keepBurning = IfBurnable(node) && burn;
        if(keepBurning)
            node.SetStatus(NodeStatus.Burning);
        if(!NodeGrid.Instance.grid.ContainsKey(node.gridPos + direction))
            return;
        node.fireNode.childNode = NodeGrid.Instance.grid[node.gridPos + direction];
        StartFire(node.fireNode.childNode, direction, count - 1, keepBurning);
    }

    public static void StopFire(Node node)
    {
        node.fireNode.shouldBurn = false;
        if(node.IsStatus(NodeStatus.Burning))
            node.SetStatus(NodeStatus.None);
        if(!node.fireNode.hasChild)
            return;
        StopFire(node.fireNode.childNode);
        node.fireNode.childNode = null;
    }

    public static void ContinueFire(Node node)
    {
        if(!node.fireNode.shouldBurn || !IfBurnable(node))
            return;
        node.SetStatus(NodeStatus.Burning);
        if(node.fireNode.hasChild)
            ContinueFire(node.fireNode.childNode);
        else return;
    }

    public static void PauseFire(Node node)
    {
        if(!node.IsStatus(NodeStatus.Burning))
            return;
        node.SetStatus(NodeStatus.None);
        if(node.fireNode.hasChild)
            PauseFire(node.fireNode.childNode);
        else return;
    }

    public static bool IfBurnable(Node node)
    {
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water) || node.hasPlatform)
            return false;
        if(!node.hasObstacle)
            return true;
        bool isBurnable = node.GetObstacle().isBurnable; 
        if(isBurnable)
            node.GetObstacle().Remove();
        return isBurnable;
    }
}