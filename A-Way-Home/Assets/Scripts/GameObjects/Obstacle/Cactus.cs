using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : Plant
{
    [SerializeField] private GameObject fruit;

    public override bool isFragile => false;
    public override bool isTrampleable => false;
    
    protected override void OnInitialize()
    {
        animator.Play(CurrentAnimationName());
        // SetNodes(this.worldPos, NodeType.Obstacle, this);
        SetNodes(this.worldPos, Character.IsName("Gaia") ? NodeType.Walkable : NodeType.Obstacle, this);

    }

    public override void OnLightningHit(int damage)
    {
        int hp = hitpoints;
        Remove();
        if(hp == 2)
            GameObject.Instantiate(fruit, this.worldPos, Quaternion.identity);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool == Tool.Grow && isAdult)
            return;
        base.OnHighlight(tool);
    }

    public override void OnGrow()
    {
        if(isAdult)
            return;
        hitpoints++;
        animator.Play(CurrentAnimationName());
    }

    protected override string CurrentAnimationName()
    {
        switch(hitpoints)
        {
            case 1:
                return middle;
            case 2:
                return fullGrown;
            default:
                Debug.Assert(hitpoints <= 0, $"Error: Unexpected hitpoint value reached: {hitpoints}");
                return destroy;
        }
    }
}
