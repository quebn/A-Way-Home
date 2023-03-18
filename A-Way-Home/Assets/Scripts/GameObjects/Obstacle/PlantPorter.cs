using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPorter : Plant
{
    [SerializeField] private int damage;
    private bool isTeleporting = false;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnTrapTrigger(Character character)
    {
        if(isTeleporting)
            return;
        Damage();
        character.TriggerDeath();
    }

    private void TeleportCharacter(Vector2 location)
    {
        Character.instance.Relocate(location);
        Character.instance.IncrementEnergy(-damage);
    }


    protected override void Grow()
    {
        isTeleporting = true;
        Vector2 location = nodes[0].worldPosition;
        TeleportCharacter(location);
        Damage();
    }
}

