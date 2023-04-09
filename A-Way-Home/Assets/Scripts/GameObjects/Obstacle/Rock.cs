using UnityEngine;
using System.Collections.Generic;

public class Rock : Obstacle, ILightning
{

    public override bool isMeltable => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public virtual void OnLightningHit()
    {
        Remove();
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Tremor && tool != Tool.Inspect)
            return;
        base.OnHighlight(tool);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        if(hitpoints != 0)
            return;
        ClearNodes();
        this.gameObject.SetActive(false);

    }

}
