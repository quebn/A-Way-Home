using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Obstacle, IInteractable
{

    private int hp = 2;

    protected override int hitpoints { 
        get => hp; 
        set => hp = value; 
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }


    public void OnDehighlight()
    {
        spriteRenderer.color = Color.white;
    }

    public void OnHighlight()
    {
        if(currentTool == Tool.Lightning || currentTool == Tool.Grow )
            spriteRenderer.color = Color.green;
    }

    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }
}
