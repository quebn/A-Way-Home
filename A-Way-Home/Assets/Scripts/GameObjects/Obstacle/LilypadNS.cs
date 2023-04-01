using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilypadNS : Obstacle, ILightning
{
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(this.id));
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this, true);
    }

    public override void Remove()
    {
        ClearNodes(NodeType.Water, true);
        base.Remove();
    }

    public void OnLightningHit()
    {
        Remove();
    }
}
