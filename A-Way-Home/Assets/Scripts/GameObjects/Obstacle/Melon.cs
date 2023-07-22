using UnityEngine;

public class Melon : Plant, ITrap
{
    protected override void OnInitialize()
    {
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, NodeType.Walkable, this);
        switch(hitpoints)
        {
            case 1:
                heal = 1;
                break;
            case 2:
                heal = 5;
                break;
            case 3:
                heal = 10;
                break;
        }
    }

    public override void OnLightningHit(int damage)
    {
        Damage(hitpoints);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void OnGrow()
    {
        if(hitpoints > 2)
            return;
        hitpoints++;
        switch(hitpoints)
        {
            case 1:
                heal = 1;
                break;
            case 2:
                heal = 5;
                break;
            case 3:
                heal = 10;
                break;
        }
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
        DamageAnimation();
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