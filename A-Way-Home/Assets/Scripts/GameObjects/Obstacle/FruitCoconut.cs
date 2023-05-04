using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCoconut : Obstacle
{
    [SerializeField] private int heal = 5;

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;
    public override bool isCorrosive => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnLightningHit(int damage)
    {
        Remove();
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
