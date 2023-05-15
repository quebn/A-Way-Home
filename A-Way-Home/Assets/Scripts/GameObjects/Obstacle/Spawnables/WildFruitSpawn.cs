using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFruitSpawn : SpawnablePlant, ITrap
{

    [SerializeField] private int heal = 1;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit(int damage)
    {
        Damage(damage);
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

    public override void Damage(int damage)
    {
        Remove();
    }
}
