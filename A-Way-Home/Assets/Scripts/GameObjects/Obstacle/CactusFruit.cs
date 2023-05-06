using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusFruit : Plant, ITrap
{
    [SerializeField] private int heal = 5;

    protected override void OnInitialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnTrapTrigger(Character character)
    {
        Remove();
        character.IncrementEnergy(heal);
    }

    public override void OnGrow()
    {
        return;
    }
}
