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

    void Update()
    {
        // if (Input.GetButtonDown("Jump"))
        // {
        //     Character.Path = FindPath(Character.StartPos, Character.TargetPos);
        //     Character.GoHome();
        // }
    }
    public Vector3[] FindPath(Vector3 startpos, Vector3 targetpos)
    {
        Vector3[] path = new Vector3[0];
        Node startnode = grid.NodeWorldPointPos(startpos);
        Node targetnode = grid.NodeWorldPointPos(targetpos);

        if (!startnode.IsWalkable && !targetnode.IsWalkable)
        {    
            print("No Path Found!");
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
                print("Path Found!");
                path = RetracePath(startnode, targetnode);
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

    Vector3[] RetracePath(Node startnode, Node targetnode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetnode;

        while(currentNode != startnode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> Waypoints = new List<Vector3>();
        Vector2 OldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 NewDirection = new Vector2(path[i-1].GridX - path[i].GridX, path[i-1].GridY - path[i].GridY);
            if (NewDirection != OldDirection)
                Waypoints.Add(path[i].WorldPosition);
            OldDirection = NewDirection;
        }
        return Waypoints.ToArray();
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
