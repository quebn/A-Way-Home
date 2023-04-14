using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Obstacle
{


    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }


    // public void OnDehighlight()
    // {
    //     spriteRenderer.color = Color.white;
    // }


    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }
}
