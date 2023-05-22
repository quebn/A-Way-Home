using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Plant, ITrap
{
    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    public override void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void OnGrow()
    {
        if(hitpoints < 2)
            hitpoints += 1;
        animator.Play(CurrentAnimationName());
        Debug.Assert(isAdult, "ERROR: isnt adult and hitpoints not equal to 1!");
        SetNodes(this.worldPos, Character.IsName("Gaia") ? NodeType.Walkable : NodeType.Obstacle, this);
    }
    public void OnTrapTrigger(Character character)
    {
        Damage(hitpoints);
        character.IncrementEnergy(heal);
    }

    protected override string CurrentAnimationName()
    {
        switch(hitpoints)
        {
            case 1:
                return youngling;
            case 2:
                return fullGrown;
            default:
                Debug.Assert(hitpoints <= 0, $"Error: Unexpected hitpoint value reached: {hitpoints}");
                return destroy;
        }
    }
}
