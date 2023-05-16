using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireNode
{
    public bool shouldBurn;
    public Node childNode;
    public bool hasChild => childNode != null;

    public FireNode()
    {
        shouldBurn = false;
        // this.node = node;
    }

    //TODO: Add Block Fire and Continue Fire functions.
    public static void StartFire(Node node, Vector2Int direction, int count)
    {
        if(!node.fireNode.shouldBurn || !IfBurnable(node))
            return;
        node.SetStatus(NodeStatus.Burning);
        if(node.fireNode.hasChild)
            StartFire(node.fireNode.childNode, direction, count - 1);
        else return;
    }

    public static void StopFire(Node node, Vector2Int direction, int count)
    {
        node.SetStatus(NodeStatus.None);
        if(node.fireNode.hasChild)
            StopFire(node.fireNode.childNode, direction, count - 1);
        else return;
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
        if(!node.fireNode.shouldBurn || !node.IsStatus(NodeStatus.Burning))
            return;
        node.SetStatus(NodeStatus.None);
        if(node.fireNode.hasChild)
            PauseFire(node.fireNode.childNode);
        else return;
    }

    public static bool IfBurnable(Node node)
    {
        if(node.IsType(NodeType.Terrain))
            return false;
        if(!node.hasObstacle)
            return true;
        Debug.Assert(node.hasObstacle);
        return node.GetObstacle().isBurnable;
    }
}
