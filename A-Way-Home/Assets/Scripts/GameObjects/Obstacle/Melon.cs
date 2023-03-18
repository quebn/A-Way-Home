using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melon : Plant
{
    [SerializeField] private int heal;
    [SerializeField] private int growthStage = 2;

    protected override void OnInitialize()
    {
        hitpoints = growthStage;
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit()
    {
        Remove();
    }

    public override void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void OnGrow()
    {
        Grow();
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool == Tool.Lightning || tool == Tool.Grow)
            spriteRenderer.color = Color.green;
    }

    protected override void Grow()
    {
        if(hitpoints > 2)
            return;
        hitpoints++;
    }

    public override void Damage(int damage = 0)
    {
        hitpoints -= damage;
    }

}
