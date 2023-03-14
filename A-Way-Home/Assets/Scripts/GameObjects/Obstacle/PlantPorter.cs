using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPorter : Plant
{

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
    }

    private void TeleportCharacter(Vector2 location)
    {
        Character.instance.Relocate(location);
    }

    protected override void OnGrow()
    {
        Vector2 location = nodes[0].worldPosition;
        DamagePlant();
        TeleportCharacter(location);
    }
}
