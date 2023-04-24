using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFruit : Plant, ITrap
{
    [SerializeField] private int heal = 1;

    protected override void OnInitialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit()
    {
        Remove();
    }

    public override void OnGrow()
    {
        return;
    }

    public void OnTrapTrigger(Character character)
    {
        Debug.Log("Triggered!");
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void Damage(int damage = 0)
    {
        Remove();
    }
}
