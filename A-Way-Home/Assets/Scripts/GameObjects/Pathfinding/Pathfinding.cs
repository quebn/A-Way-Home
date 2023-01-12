using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static Vector3[] FindPath(Vector3 startingPos, List<Vector3> targetPos, bool canWalkWater = false)
    {
        NodeGrid nodeGrid = NodeGrid.Instance;
        Vector3[] path = new Vector3[0];

        Node startNode = NodeGrid.NodeWorldPointPos(startingPos);
        List<Node> endNodes = NodeGrid.NodeWorldPointPos(targetPos);

        if (!startNode.IsWalkable())
        {
            Debug.Log("No Path Found!");
            return path;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // 
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].minHCost < currentNode.minHCost)
                    currentNode = openSet[i];
            //

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (endNodes.Contains(currentNode))
            {
                path = RetracePath(startNode, currentNode).ToArray();
                Debug.Log("Path Found! Path Total Nodes: " + path.Length);
                break;
            }
            foreach (Node neighbor in Node.GetNeighbors(currentNode, nodeGrid.grid, nodeGrid.gridSizeInt))
            {
                if (!neighbor.IsWalkable(canWalkWater) || closedSet.Contains(neighbor))
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

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            waypoints.Add(currentNode.worldPosition);
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
