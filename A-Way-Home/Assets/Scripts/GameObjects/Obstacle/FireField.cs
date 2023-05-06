using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireField : MonoBehaviour, ITrap
{
    public void OnTrapTrigger(Character character)
    {
        character.TriggerDeath();
    }
}
