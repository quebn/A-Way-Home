using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform Seeker, Target;
    Grid grid;

    Vector3[] _Path;

    void FindPath(Vector3 startpos, Vector3 targetpos)
    {
        Node startnode = grid.NodeWorldPointPos(startpos);
        Node targetnode = grid.NodeWorldPointPos(targetpos);

        if (!startnode.IsWalkable && !targetnode.IsWalkable)
            return;
        
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
                // _Path = RetracePath(startnode, targetnode);
                RetracePath(startnode, targetnode);
                return;
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
        
    }

    void RetracePath(Node startnode, Node targetnode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetnode;

        while(currentNode != startnode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPath(Seeker.position, Target.position);
            
    }
}
