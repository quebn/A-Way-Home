using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : Plant
{
    [SerializeField] private int heal;
    
    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void OnLightningHit()
    {
        if(hitpoints >= 3)
        {
            hitpoints = 1;
            animator.Play(CurrentAnimationName());
            SetNodes(this.worldPos, NodeType.Walkable, this);
        }
        else
            Remove();
    }

    // protected override void OnHighlight(Tool tool)
    // {
    //     if(tool != Tool.Lightning && tool != Tool.Grow && hitpoints >= 3)
    //         return;
    //     spriteRenderer.color = Color.green;
    // }

    public override void OnGrow()
    {
        if(hitpoints >= 3)
            return;
        hitpoints++;
        Debug.LogWarning($"Animation clip: {CurrentAnimationName()} -> hp:{hitpoints}");
        animator.Play(CurrentAnimationName());
        if(hitpoints > 1)
            SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(heal);
        Damage(1);
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
