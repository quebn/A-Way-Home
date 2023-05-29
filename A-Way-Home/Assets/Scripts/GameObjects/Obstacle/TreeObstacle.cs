using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObstacle : Obstacle
{
    [SerializeField] protected Animator animator;
    protected List<List<Node>> nodesPlaceable;

    public override bool isCorrosive => true;
    public override bool isMeltable => true;
    public override bool isBurnable => isCutDown;

    protected bool isCutDown => hitpoints == 1;

    protected virtual Action AfterCutDown => () => {};

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
        audioSources[0].Play();
        if(outlines[1].activeSelf)
            outlines[1].SetActive(false);
        yield return new WaitForSeconds(CutDownTreeAnimation());
        audioSources[1].Stop();
        AfterCutDown();
    }

    protected void SetPlaceableLocations()
    {
        nodesPlaceable = new List<List<Node>>();
        for(float f = -1.5f; f < 3; f += 3)
        {
            Vector2 pos = new Vector2(this.worldPos.x + f, this.worldPos.y);
            nodesPlaceable.Add(NodeGrid.GetNodes(pos, 2, 1));
        }
    }

    protected bool IsCursorRight()
    {
        Vector2 mousePos = PlayerActions.Instance.mouseWorldPos;
        return mousePos.x > this.worldPos.x;
    }

    protected float CutDownTreeAnimation()
    {
        audioSources[1].Play();
        animator.Play(IsCursorRight() ? "TreeFall_Right" : "TreeFall_Left");
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    protected override void OnHighlight(Tool tool)
    {
        if(!isCutDown && !outlines[1].activeSelf)
            outlines[1].SetActive(true);
        base.OnHighlight(tool);
    }

    protected override void OnWhileHovered(Tool tool)
    {
        if(tool == Tool.Lightning && !isCutDown)
            HighlightPlaceableNodes();
    }

    protected override void OnDehighlight()
    {
        if(!isCutDown && outlines[1].activeSelf)
            outlines[1].SetActive(false);
        base.OnDehighlight();
        for(int i = 0; i < nodesPlaceable.Count; i++)
            Node.ToggleNodes(nodesPlaceable[i], NodeGrid.nodesVisibility);

    }

    protected void HighlightPlaceableNodes()
    {
        for(int i = 0; i < nodesPlaceable.Count; i++)
            Node.ToggleNodes(nodesPlaceable[i], NodeGrid.nodesVisibility);
        Node.RevealNodes(nodesPlaceable[IsCursorRight()? 1 : 0], Node.colorRed, NodeType.Terrain);
    }

    protected virtual bool LogNotPlaceable(Node node)
    {
        return node.IsType(NodeType.Terrain) || (node.hasObstacle && !node.GetObstacle().isFragile)|| node == Character.instance.currentNode || node.IsStatus(NodeStatus.Burning);
    }

    protected bool isFruitplaceable(Node node)
    {
        return (node.worldPosition.x == NodeGrid.GetMiddle(this.worldPos.x + 2f) || node.worldPosition.x == NodeGrid.GetMiddle(this.worldPos.x + -2f)) && node.IsWalkable(); 
    } 

}
