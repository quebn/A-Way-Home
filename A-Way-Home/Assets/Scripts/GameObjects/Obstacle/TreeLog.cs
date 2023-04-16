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

    public void OnLightningHit()
    {
        Remove();
    }

    public override void Remove()
    {
        hitpoints = 0;
        ClearNodes();
        this.gameObject.SetActive(false);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        // Debug.LogWarning($"loaded hp: {hitpoints}"/);
        if(hitpoints != 0)
            return;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
    }
}
