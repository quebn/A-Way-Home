using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusFruitSpawn : SpawnablePlant, ITrap
{

    protected override void OnSpawn()
    {
        base.OnSpawn();
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
