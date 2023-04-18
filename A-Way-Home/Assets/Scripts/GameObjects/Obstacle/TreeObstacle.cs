using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObstacle : Obstacle
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject outlineUpper;

    protected float currentCursorLocation = 0;
    protected Dictionary<float, List<Node>> placeableNodes;

    public override bool isCorrosive => true;
    public override bool isMeltable => true;
    public override bool isBurnable => isCutDown;

    protected bool isCutDown => hitpoints == 1;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        if(isCutDown)
            animator.Play("Tree_AfterCut");
        SetPlaceableLocations();
    }

    public override void Damage(int value)
    {
        base.Damage(value);
        Debug.LogWarning("Tree Damaged");
        if(isCutDown)
            StartCoroutine(CutDown());
    }

    protected IEnumerator CutDown()
    {
        Debug.LogWarning("Cutting Down");
        if(outlineUpper.activeSelf)
            outlineUpper.SetActive(false);
        yield return new WaitForSeconds(CutDownTreeAnimation());
        OnCutDown();
    }

    protected virtual void OnCutDown()
    {
        Debug.Assert(false, "ERROR: Should be implemented in subclasses");
    }

    protected void SetPlaceableLocations()
    {
        placeableNodes  = new Dictionary<float, List<Node>>(2);
        for(float f = -1.5f; f < 3; f += 3){
            Vector2 pos = new Vector2(this.worldPos.x + f, this.worldPos.y);
            placeableNodes.Add(pos.x, NodeGrid.GetNodes(pos, 2, 1));
        }
    }

    protected float GetCursorDirection()
    {
        Vector2 mousePos = PlayerActions.Instance.mouseWorldPos;
        return mousePos.x > this.worldPos.x ? worldPos.x + 1.5f : worldPos.x - 1.5f;
    }

    protected float CutDownTreeAnimation()
    {
        if (currentCursorLocation > this.worldPos.x)
            animator.Play("TreeFall_Right");
        else if (currentCursorLocation < this.worldPos.x)
            animator.Play("TreeFall_Left");
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    protected override void OnHighlight(Tool tool)
    {
        if(!isCutDown && !outlineUpper.activeSelf)
            outlineUpper.SetActive(true);
        base.OnHighlight(tool);
    }

    protected override void OnWhileHighlight(Tool tool)
    {
        if(tool == Tool.Lightning && !isCutDown)
            HighlightPlaceableNodes();
    }

    protected override void OnDehighlight()
    {
        if(!isCutDown && outlineUpper.activeSelf)
            outlineUpper.SetActive(false);
        base.OnDehighlight();
        if(currentCursorLocation != 0)
            Node.ToggleNodes(placeableNodes[currentCursorLocation], NodeGrid.nodesVisibility);
    }

    protected void HighlightPlaceableNodes()
    {
        float xLocationCursor = GetCursorDirection(); 
        Debug.Assert(placeableNodes.ContainsKey(xLocationCursor));
        if(currentCursorLocation != 0)
            Node.ToggleNodes(placeableNodes[currentCursorLocation], NodeGrid.nodesVisibility);
        currentCursorLocation = xLocationCursor;
        Node.RevealNodes(placeableNodes[currentCursorLocation], Node.colorRed, NodeType.Terrain);
    }
}