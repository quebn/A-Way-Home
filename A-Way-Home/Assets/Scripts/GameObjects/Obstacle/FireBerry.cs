using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBerry : Plant, ITrap
{
    private List<Node> explosionNodes;

    protected override void OnInitialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
        explosionNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
    }

    public void OnTrapTrigger(Character character)
    {
        Damage(1);
        character.IncrementEnergy(heal);
    }

    public override void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void OnGrow()
    {
        Detonate();
    }

    private void Detonate()
    {
        animator.Play("Explosion");
        for(int i = 0; i < explosionNodes.Count; i++)
        {
            if(explosionNodes[i].hasObstacle)
                explosionNodes[i].GetObstacle().Damage(1);
        }
    }
}
