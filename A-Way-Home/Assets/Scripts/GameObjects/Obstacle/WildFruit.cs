using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFruit : Plant
{
    [SerializeField] private int heal = 1;

    protected override void OnInitialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool == Tool.Lightning)
            spriteRenderer.color = Color.green;
    }

    public override void OnLightningHit()
    {
        Remove();
    }

    public override void OnGrow()
    {
        return;
    }

    public override void OnTrapTrigger(Character character)
    {
        Debug.Log("Triggered!");
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void Damage(int damage = 0)
    {
        Remove();
    }
}
