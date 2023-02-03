using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, IInteractable//, IInteractable, INodeInteractable
{

    // public bool notSpawned;
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.transform.position, NodeType.Obstacle, this);
    }

    public void OnInteract()
    {
        if(currentTool == Tool.Lightning)
            RemoveLog();
    }

    public void OnHighlight()
    {
        if(currentTool != Tool.Lightning)
            return;
        this.spriteRenderer.color = Color.green;
    }

    public void OnDehighlight()
    {
        if(currentTool != Tool.Lightning)
            return;
        this.spriteRenderer.color = Color.white;
    }

    private void RemoveLog()
    {
        PlayerLevelData.Instance.IncrementPlayerMoves(-1);
        ClearNodes();
        this.gameObject.SetActive(false);
    }
}
