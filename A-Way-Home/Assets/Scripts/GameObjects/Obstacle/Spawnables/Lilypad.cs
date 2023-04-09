using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilypad : Spawnable, ILightning
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

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Inspect)
            return;
        base.OnHighlight(tool);
    }

    public void OnLightningHit()
    {
        Remove();
    }
}
