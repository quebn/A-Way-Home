using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boulder : Obstacle, IObstacle
{

    private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        spriteRenderer.color = Color.green;
    }

    public void OnDehover()
    {
        spriteRenderer.color = Color.white;
    }

    private void RemoveBoulder()
    {
        InGameUI.Instance.SetPlayerMoves(-1);
        animator.Play("BigBoulder_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("UpdateBoulder", delay);
    }

    private void UpdateBoulder()
    {
        nodesType = NodeType.Walkable;
        this.gameObject.SetActive(false);
    }
}