using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFruit : Plant, ITrap
{

    protected override void OnInitialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void OnGrow()
    {
        // hitpoints++;
        heal++;
        return;
    }

    public void OnTrapTrigger(Character character)
    {
        Debug.Log("Triggered!");
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void Damage(int damage)
    {
        Remove();
    }
}
