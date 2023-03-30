using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, ILightning
{

    // public bool notSpawned;
    [SerializeField] private Animator animator;

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
        hitpoints -= 1;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
    }

    public void Clear()
    {
        ClearNodes();
        hitpoints = 0;
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
