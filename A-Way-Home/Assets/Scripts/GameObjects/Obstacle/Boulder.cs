using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boulder : Obstacle, IInteractable
{

    private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetNodes(this.transform.position);
        nodesType = NodeType.Obstacle;
    }

    public void OnClick()
    {
        if (incorrectTool)
            return;
        RemoveBoulder();
    }

    public void OnHover()
    {
        if(incorrectTool)
            return;
        SetMouseCursor(this.mouseTexture);
        spriteRenderer.color = Color.green;
    }

    public void OnDehover()
    {
        ResetMouseCursor();
        spriteRenderer.color = Color.white;
    }

    private void RemoveBoulder()
    {
        PlayerLevelData.Instance.SetPlayerMoves(-1);
        animator.Play("BigBoulder_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("UpdateNodes", delay);
    }

}