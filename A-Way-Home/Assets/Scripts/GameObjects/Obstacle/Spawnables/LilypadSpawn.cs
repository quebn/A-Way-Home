using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilypadSpawn : Spawnable, ILightning
{

    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this , true);
    }

    public override void Remove()
    {
        ClearNodes(NodeType.Water, true);
        base.Remove();
    }


    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }
}
