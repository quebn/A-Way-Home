using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCollider : MonoBehaviour
{
    private PolygonCollider2D waterCollider;

    private void Awake()
    {
        waterCollider = GetComponent<PolygonCollider2D>();
    }
    private void Start()
    {
        if(waterCollider != null)
            waterCollider.enabled = false;
    }
}
