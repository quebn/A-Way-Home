using System;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{

    public static List<Node> FindPath(Vector3 startingPos, List<Vector3> targetPos, Dictionary<Vector2Int, Node> grid, NodeType walkableNodeType = NodeType.Walkable, Type type = null)
    {
        Debug.Assert(targetPos.Count > 0, "ERROR: No Target in list");
        List<Node> path = new List<Node>();
        Node startNode = NodeGrid.NodeWorldPointPos(startingPos);
        List<Node> endNodes = NodeGrid.NodeWorldPointPos(targetPos);
        if (!startNode.Is(walkableNodeType, type) && !Node.CheckNodesType(endNodes, walkableNodeType, type))
        {
            return path;
        }
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            //
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (endNodes.Contains(currentNode))
            {
                path = RetracePath(startNode, currentNode);
                break;
            }
            foreach (Node neighbor in NodeGrid.GetPathNeighborNodes(currentNode, grid))
            {
                if (!neighbor.IsWalkable() || closedSet.Contains(neighbor))
                    continue;
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCosts = GetDistances(neighbor, endNodes);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return path;
    }

    public static List<Node> FindPath(Vector3 startingPos, List<Vector3> targetPos, NodeType walkableNodeType = NodeType.Walkable, Type type = null)
    {
        return FindPath(startingPos, targetPos, NodeGrid.Instance.grid, walkableNodeType, type);
    }

    public static List<Node> FindPathPhased(Vector3 startingPos, List<Vector3> targetPos,Dictionary<Vector2Int, Node> grid, Type type = null)
    {
        Debug.Assert(targetPos.Count > 0, "ERROR: No Target in list");
        List<Node> path = new List<Node>();
        Node startNode = NodeGrid.NodeWorldPointPos(startingPos);
        List<Node> endNodes = NodeGrid.NodeWorldPointPos(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            //
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (endNodes.Contains(currentNode))
            {
                path = RetracePath(startNode, currentNode);
                break;
            }
            foreach (Node neighbor in NodeGrid.GetPathNeighborNodes(currentNode, grid))
            {
                if (closedSet.Contains(neighbor))
                    continue;
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCosts = GetDistances(neighbor, endNodes);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return path;
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> waypoints = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            waypoints.Add(currentNode);
            currentNode = currentNode.parent;
        }
        waypoints.Reverse();
        return waypoints;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int dstY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if (dstX > dstY)
            return dstY + 10 * (dstX - dstY);
        return dstX + 10 * (dstY - dstX);
    }

    private static List<int> GetDistances(Node nodeA, List<Node> nodes)
    {
        List<int> distances = new List<int>();
        foreach (Node node in nodes)
            distances.Add(GetDistance(nodeA, node));
        return distances;
    }
}
