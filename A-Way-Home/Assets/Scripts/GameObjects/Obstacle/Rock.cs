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
        hitpoints -= 1;
        Debug.Assert(hitpoints == 0, "ERROR: hp should be but is not 0");
        ClearNodes();
        this.gameObject.SetActive(false);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        if(hitpoints != 0)
            return;
        ClearNodes();
        this.gameObject.SetActive(false);

    }
}
