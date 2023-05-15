using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusFruitSpawn : SpawnablePlant, ITrap
{
    [SerializeField] private int heal = 5;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
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