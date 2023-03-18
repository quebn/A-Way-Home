using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant
{
    [SerializeField] private int damage;
    
    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(-damage);
        Damage(1);
    }

}
