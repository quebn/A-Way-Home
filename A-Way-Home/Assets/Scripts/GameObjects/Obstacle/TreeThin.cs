using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeThin : Obstacle, IInteractable
{
    [SerializeField] private GameObject treeUpper;
    [SerializeField] private GameObject treeLower;
    [SerializeField] private Animator animatorUpper;
    [SerializeField] private Animator animatorLower;
    private SpriteRenderer lowerSpriteRenderer;


    protected override void Initialize()
    {
        base.Initialize();
        spriteRenderer = treeUpper.GetComponent<SpriteRenderer>();
        lowerSpriteRenderer = treeLower.GetComponent<SpriteRenderer>();
        SetNodes(this.transform.position);
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
        spriteRenderer.color = Color.green;
        lowerSpriteRenderer.color = Color.green;

    }

    public void OnDehover()
    {
        ResetMouseCursor();
        spriteRenderer.color = Color.white;
        lowerSpriteRenderer.color = Color.white;
    }

    private void ShowBlockableNodes()
    {
        Debug.Log("To be implemented");
    }
}
