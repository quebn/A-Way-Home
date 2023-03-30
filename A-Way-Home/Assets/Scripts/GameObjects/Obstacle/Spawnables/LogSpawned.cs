using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawned : Spawnable, ILightning
{

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

    private void DestroyNodeObstacle()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.hasObstacle)
            Destroy(node.GetObstacle());
    }

}
