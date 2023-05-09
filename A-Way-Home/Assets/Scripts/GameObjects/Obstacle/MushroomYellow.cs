using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomYellow : Plant
{
    [SerializeField] private GameObject lilypad;
    [SerializeField] private GameObject cactus;
    private List<Node> growthNodes;

    public override bool isFragile => false;
    public override bool isTrampleable => false;

    protected override void OnInitialize()
    {
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        growthNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
    }

    public override void OnGrow()
    {
        GrowCacti();
        hitpoints += hitpoints == 2 ? 0 : 1;
    }

    private void GrowCacti()
    {
        for(int i = 0; i < growthNodes.Count; i++)
        {
            if(growthNodes[i].hasObstacle || growthNodes[i].IsType(NodeType.Terrain))
                continue;
            GameObject.Instantiate(growthNodes[i].IsType(NodeType.Walkable) ? cactus : lilypad,  growthNodes[i].worldPosition, Quaternion.identity);
        }
    }

    protected override string CurrentAnimationName()
    {
        switch(hitpoints)
        {
            case 1:
                return youngling;
            case 2:
                return middle;
            case 3:
                return fullGrown;
            default:
                Debug.Assert(hitpoints <= 0, $"Error: Unexpected hitpoint value reached: {hitpoints}");
                return destroy;
        }
    }
}
