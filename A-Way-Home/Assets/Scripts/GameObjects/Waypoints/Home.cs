using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance {get; private set;}

    [SerializeField] private GameObject lightObject;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if(instance != this)
            instance = this;
    }

    public void ActivateHome()
    {
        lightObject.SetActive(true);
        spriteRenderer.color = Color.white;
    }
}
