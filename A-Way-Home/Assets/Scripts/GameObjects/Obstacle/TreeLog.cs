using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, IInteractable
{
    private Animator animator;


    protected override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetNodes(this.transform.position);
        nodesType = NodeType.Obstacle;
        // PlaySpawnAnimation();
    }

    public void OnClick()
    {
        if(incorrectTool)
            return;
        RemoveLog();
    }

    public void OnHover()
    {
        if(incorrectTool)
            return;
        SetMouseCursor(this.mouseTexture);
        spriteRenderer.color = Color.green;
        // HighlightObstacle();
    }

    public void OnDehover()
    {
        ResetMouseCursor();
        spriteRenderer.color = Color.white;
    }

    private void RemoveLog()
    {
        this.tag = "Interacted";
        PlayerLevelData.Instance.SetPlayerMoves(-1);
        animator.Play("SmallExplosion_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("UpdateNodes", delay);
    }

}
