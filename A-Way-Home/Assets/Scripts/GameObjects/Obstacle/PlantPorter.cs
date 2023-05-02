using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Make other instance of plants porter o dissapear when one is eaten.
public class PlantPorter : Plant, ITrap
{
    [SerializeField] private int damage;
    private static bool isTeleported = false;
    private static HashSet<PlantPorter> porters = new HashSet<PlantPorter>();
    
    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
        Debug.Assert(porters != null);
        if(!porters.Contains(this))
            porters.Add(this);
    }

    public void OnTrapTrigger(Character character)
    {
        if(isTeleported)
        {
            isTeleported = false;
            return;
        }
        Damage(hitpoints);
        character.TriggerDeath();
    }

    public override void OnGrow()
    {
        isTeleported = true;
        Vector2 location = nodes[0].worldPosition;
        TeleportCharacter(location);
        Damage(hitpoints);
        RemoveOtherPorter(this.id);
    }

    private static void RemoveOtherPorter(string excludedID)
    {
        foreach(PlantPorter porter in porters)
            if(porter.id != excludedID)
                porter.Remove();
        porters = new HashSet<PlantPorter>();
    }

    private void TeleportCharacter(Vector2 location)
    {
        Character.instance.Relocate(location);
        Character.instance.IncrementEnergy(-damage);
    }
}

