using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPurple : Mushroom, ITrap
{

    [SerializeField] private int heal = 7;

    public void OnTrapTrigger(Character character)
    {
        Remove();
        character.IncrementEnergy(heal);
    }
}
