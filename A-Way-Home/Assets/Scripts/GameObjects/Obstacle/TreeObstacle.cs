using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObstacle : Obstacle, ILightning
{

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit()
    {
        
    }
}
