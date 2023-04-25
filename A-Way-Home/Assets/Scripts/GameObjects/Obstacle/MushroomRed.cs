using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomRed : Mushroom, ITrap
{

    public void OnTrapTrigger(Character character)
    {
        Remove();
        character.IncrementEnergy(-10);
    }
}
