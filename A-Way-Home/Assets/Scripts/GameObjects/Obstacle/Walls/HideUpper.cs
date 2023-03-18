using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUpper : MonoBehaviour, IHoverable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void OnDehover()
    {
        spriteRenderer.color = Color.white;
    }

    public void OnHover()
    {
        spriteRenderer.color = new Color32(255, 255, 255, 50);
    }
}
