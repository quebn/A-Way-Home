using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockEssence : Obstacle, ITremor
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject essence;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnTremor()
    {
        Damage(1);
    }

    public override void Remove()
    {
        ForceDehighlight();
        if(hitpoints != 0)
            hitpoints = 0;
        ClearNodes();
        animator.Play("Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnRemove", delay);
        essence.SetActive(true);
    }

    private void OnRemove()
    {
        this.spriteRenderers[0].enabled = false;
    }
}
