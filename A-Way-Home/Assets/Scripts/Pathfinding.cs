using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Start()
    {
    }
    public Vector3[] FindPath(Vector3 startpos, Vector3 targetpos)
    {
        Vector3[] path = new Vector3[0];
        Node startnode = grid.NodeWorldPointPos(startpos);
        Node targetnode = grid.NodeWorldPointPos(targetpos);
        Debug.Log("FindPath target node WorldPos:" + targetnode.WorldPosition);

        if (!startnode.IsWalkable && !targetnode.IsWalkable)
        {    
            Debug.Log("No Path Found!");
            return path;
        }
        List<Node> OpenSet = new List<Node>();
        HashSet<Node> ClosedSet = new HashSet<Node>();
        OpenSet.Add(startnode);

        while (OpenSet.Count > 0)
        {
            Node currentNode = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++)
                if (OpenSet[i].Fcost < currentNode.Fcost || OpenSet[i].Fcost == currentNode.Fcost && OpenSet[i].Hcost < currentNode.Hcost)
                    currentNode = OpenSet[i];

            OpenSet.Remove(currentNode);
            ClosedSet.Add(currentNode);

            if (currentNode == targetnode)
            {
                Debug.Log("Path Found!");
                path = RetracePath(startnode, targetnode).ToArray();
                break;
            }

            foreach (Node neighbor in grid.GetNeighbor(currentNode))
            {
                if (!neighbor.IsWalkable || ClosedSet.Contains(neighbor))
                    continue;
                
                int newMovementCostToNeighbor = currentNode.Gcost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.Gcost || !OpenSet.Contains(neighbor))
                {
                    neighbor.Gcost = newMovementCostToNeighbor;
                    neighbor.Hcost = GetDistance(neighbor, targetnode);
                    neighbor.Parent = currentNode;

                    if (!OpenSet.Contains(neighbor))
                        OpenSet.Add(neighbor);
                }
            }
        }
        return path;
        
    }

    List<Vector3> RetracePath(Node startnode, Node targetnode)
    {
        List<Node> path = new List<Node>();
        List<Vector3> waypoints = new List<Vector3>();
        Node currentNode = targetnode;
        while(currentNode != startnode)
        {
            waypoints.Add(currentNode.WorldPosition);
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        grid.path = path;
        waypoints.Reverse();
        foreach (Vector3 waypoint in waypoints)
            Debug.Log(waypoint);
        return waypoints;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
