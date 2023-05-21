using UnityEngine;
using System.Collections.Generic;

public class Rock : Obstacle, ILightning, ITremor, ITrap
{

    public override bool isMeltable => true;

    protected override void Initialize()
    {
        base.Initialize();
        // SetNodes(this.worldPos, NodeType.Obstacle, this);
        SetNodes(this.worldPos, Character.IsName("Terra") ? NodeType.Walkable : NodeType.Obstacle, this);

    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public void OnTremor()
    {
        Damage(1);
    }

    public override void Remove()
    {
        audioSources[0].Play();
        base.Remove();
    }

    public void OnTrapTrigger(Character character)
    {
        Remove();
    }
}
