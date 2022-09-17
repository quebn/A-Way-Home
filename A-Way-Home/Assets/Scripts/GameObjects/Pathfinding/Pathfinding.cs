using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Grid _Grid;
    
    private void Awake()
    {
        // Grid should be from Platform GameObject
        _Grid = GetComponent<Grid>();
    }

    public Vector3[] FindPath(Vector3 startpos, Vector3 targetpos)
    {
        Vector3[] path = new Vector3[0];
        Node startnode = _Grid.NodeWorldPointPos(startpos);
        Node targetnode = _Grid.NodeWorldPointPos(targetpos);

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
            // 
            Node CurrentNode = OpenSet[0];
            for (int i = 1; i < OpenSet.Count; i++)
                if (OpenSet[i].Fcost < CurrentNode.Fcost || OpenSet[i].Fcost == CurrentNode.Fcost && OpenSet[i].Hcost < CurrentNode.Hcost)
                    CurrentNode = OpenSet[i];

            OpenSet.Remove(CurrentNode);
            ClosedSet.Add(CurrentNode);

            if (CurrentNode == targetnode)
            {
                path = RetracePath(startnode, targetnode).ToArray();
                Debug.Log("Path Found! Path Total Nodes: " + path.Length);
                break;
            }
            // 
            foreach (Node Neighbor in Node.GetNeighbors(CurrentNode, _Grid.NodeGrid, _Grid.GridSizeInt))
            {
                if (!Neighbor.IsWalkable || ClosedSet.Contains(Neighbor))
                    continue;
                
                int newMovementCostToNeighbor = CurrentNode.Gcost + GetDistance(CurrentNode, Neighbor);
                if (newMovementCostToNeighbor < Neighbor.Gcost || !OpenSet.Contains(Neighbor))
                {
                    Neighbor.Gcost = newMovementCostToNeighbor;
                    Neighbor.Hcost = GetDistance(Neighbor, targetnode);
                    Neighbor.Parent = CurrentNode;

                    if (!OpenSet.Contains(Neighbor))
                        OpenSet.Add(Neighbor);
                }
            }
        }
        if (path.Length <= 0)
            Debug.Log("path is null");
        return path;
        
    }

    private List<Vector3> RetracePath(Node startnode, Node targetnode)
    {
        List<Node> path = new List<Node>();
        List<Vector3> waypoints = new List<Vector3>();
        Node CurrentNode = targetnode;
        while(CurrentNode != startnode)
        {
            waypoints.Add(CurrentNode.WorldPosition);
            path.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        path.Reverse();
        _Grid.Path = path;
        waypoints.Reverse();
        return waypoints;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridPos.x - nodeB.GridPos.x);
        int dstY = Mathf.Abs(nodeA.GridPos.y - nodeB.GridPos.y);

        if (dstX > dstY)
            return dstY + 10 * (dstX - dstY);
        return dstX + 10 * (dstY - dstX);
    }


}
