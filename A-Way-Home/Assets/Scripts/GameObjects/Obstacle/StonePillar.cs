using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePillar : Obstacle, ITremor
{
    [SerializeField] private Animator animator;
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
    }

    private void OnRemove()
    {
        this.gameObject.SetActive(false);
    }
}
