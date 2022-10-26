using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [HideInInspector] public Vector2 worldPos;
    [HideInInspector] public Waypoint pointingTo;

    public static uint waypointCount;

}
