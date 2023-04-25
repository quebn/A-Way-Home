using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : Obstacle, ILightning
{

    public override bool isBurnable => true;
    public override bool isMeltable => true;
    public override bool isFragile => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Inspect && tool != Tool.Lightning)
            return;
        base.OnHighlight(tool);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }
}
