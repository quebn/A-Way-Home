using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPlatformSpawn : Spawnable, ILightning
{
    protected override void OnSpawn()
    {
        base.OnSpawn();
        if(!isLoaded && audioSources.Count != 0)
            audioSources[0].Play();
        SetNodes(this.worldPos, NodeType.Walkable, this, true);
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
