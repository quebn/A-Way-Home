using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant
{


    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(5);
        DamagePlant(1);
    }

}
