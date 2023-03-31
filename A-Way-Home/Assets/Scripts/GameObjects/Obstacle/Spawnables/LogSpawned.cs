using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawned : Spawnable, ILightning
{

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;
    public override bool isCorrosive => true;

    protected override void OnSpawn()
    {
        // Destroy Obstacle on spawn.
        DestroyNodeObstacle();
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit()
    {
        Remove();
    }

}
