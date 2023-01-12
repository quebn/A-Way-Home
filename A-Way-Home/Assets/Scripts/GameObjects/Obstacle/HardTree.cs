using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardTree : Obstacle, IInteractable
{
    private Animator animator;
    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.transform.position);
        spriteRenderer = GetComponent<SpriteRenderer>();
        nodesType = NodeType.Obstacle;
    }

    public void OnClick()
    {
        if(incorrectTool)
            return;
        ShowBlockableNodes();
    }

    public void OnHover()
    {
        if(incorrectTool)
            return;
        SetMouseCursor(this.mouseTexture);
        this.spriteRenderer.color = Color.green;
    }
    public void OnDehover()
    {
        ResetMouseCursor();
        this.spriteRenderer.color = Color.white;
    }

    private void ShowBlockableNodes()
    {

    }

    private void DestroyTree()
    {

    }

}
