using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : Obstacle
{

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void Clear()
    {
        ClearNodes();
        Debug.Log("Poison Miasma Cleared");
        GameObject.Destroy(this.gameObject);
    }
}
