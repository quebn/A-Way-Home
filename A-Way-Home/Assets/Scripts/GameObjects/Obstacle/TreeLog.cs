using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, ILightning
{

    // public bool notSpawned;
    [SerializeField] private Animator animator;
 
    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;
    public override bool isCorrosive => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.transform.position, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }
}
