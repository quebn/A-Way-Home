using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPurple : Mushroom, ITrap
{


    public void OnTrapTrigger(Character character)
    {
        Remove();
        character.IncrementEnergy(heal);
    }
}
