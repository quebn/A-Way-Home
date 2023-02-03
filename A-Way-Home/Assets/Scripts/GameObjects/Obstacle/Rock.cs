using UnityEngine;
using System.Collections.Generic;

public class Rock : Obstacle, IInteractable
{

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public virtual void OnDehighlight()
    {
        if(currentTool != Tool.Lightning && currentTool != Tool.Tremor)
            return;
        this.spriteRenderer.color = Color.white;

    }

    public virtual void OnHighlight()
    {
        if(currentTool != Tool.Lightning && currentTool != Tool.Tremor)
            return;
        this.spriteRenderer.color = Color.green;
    }

    public virtual void OnInteract()
    {
        if(currentTool == Tool.Lightning)
        {
            ClearRock();
        }
    }

    public void ClearRock()
    {
        ClearNodes();
        this.gameObject.SetActive(false);
    }
}
