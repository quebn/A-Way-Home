using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideUpperTileMap : MonoBehaviour, IHoverable
{
    [SerializeField] private Tilemap tilemap; 

    public void OnDehover()
    {
        tilemap.color = Color.white;
    }

    public void OnHover()
    {
        tilemap.color = new Color32(255, 255, 255, 50);
    }
}
