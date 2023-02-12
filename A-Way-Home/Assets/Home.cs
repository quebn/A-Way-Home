using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance {get; private set;}

    void Awake()
    {
        if(instance != this)
            instance = this;
    }
}
