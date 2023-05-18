using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCollider : MonoBehaviour
{
    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        if(polygonCollider2D != null)
            polygonCollider2D.enabled = false;
        if(boxCollider2D != null)
            boxCollider2D.enabled = false;
    }
}
