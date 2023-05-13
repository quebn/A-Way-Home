using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance {get; private set;}

    [SerializeField] private GameObject lightObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isPortal = false;
    [SerializeField] private GameObject portal;

    void Awake()
    {
        if(instance != this)
            instance = this;
    }

    public void ActivateHome()
    {
        if(isPortal){
            ActivatePortal();
            return;
        }
        lightObject.SetActive(true);
        spriteRenderer.color = Color.green  ;
    }

    private void ActivatePortal()
    {
        spriteRenderer.enabled = false;
        portal.SetActive(true);
    }
}
