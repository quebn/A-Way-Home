using UnityEngine;

public class PlantPorter : Plant, ITrap
{
    [SerializeField] private int damage;
    private static bool isTeleported = false;
    
    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnTrapTrigger(Character character)
    {
        if(isTeleported)
        {
            isTeleported = false;
            return;
        }
        Remove();
        character.TriggerDeath();
    }

    public override void OnGrow()
    {
        isTeleported = true;
        Vector2 location = nodes[0].worldPosition;
        TeleportCharacter(location);
        RemoveAllPorters(this.id);
    }

    private static void RemoveAllPorters(string excludedID)
    {
        PlantPorter[] plantPorters = GameObject.FindObjectsOfType<PlantPorter>(false);
        for(int i = 0; i < plantPorters.Length; i++)
                plantPorters[i].Remove();
    }

    private void TeleportCharacter(Vector2 location)
    {
        Character.instance.Relocate(location);
        Character.instance.IncrementEnergy(-damage);
    }
}