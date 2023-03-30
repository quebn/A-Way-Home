using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPlatform : Spawnable, ILightning
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void Remove()
    {
        ClearNodes(NodeType.Water);
        base.Remove();
    }

    public void OnLightningHit()
    {
        Remove();
    }
}
