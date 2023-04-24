using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melon : Plant, ITrap
{
    [SerializeField] private int heal;

    protected override void OnInitialize()
    {
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit()
    {
        Damage(hitpoints);
    }

    public void OnTrapTrigger(Character character)
    {
        switch(hitpoints)
        {
            case 1:
                character.IncrementEnergy(1);
                break;
            case 2:
                character.IncrementEnergy(heal);
                break;
            case 3:
                character.IncrementEnergy(heal * 2);
                break;
        }
        Remove();
    }

    public override void OnGrow()
    {
        if(hitpoints > 2)
            return;
        hitpoints++;
        animator.Play(CurrentAnimationName());
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool == Tool.Grow && hitpoints == 3)
            return;
        base.OnHighlight(tool);
    }

    public override void Damage(int damage = 0)
    {
        hitpoints -= damage;
        animator.Play(CurrentAnimationName());
        if(hitpoints <= 0)
            Remove();
    }

    protected override string CurrentAnimationName()
    {
        switch(hitpoints)
        {
            case 1:
                return youngling;
            case 2:
                return middle;
            case 3:
                return fullGrown;
            default:
                Debug.Assert(hitpoints <= 0, $"Error: Unexpected hitpoint value reached: {hitpoints}");
                return destroy;
        }
    }
}
