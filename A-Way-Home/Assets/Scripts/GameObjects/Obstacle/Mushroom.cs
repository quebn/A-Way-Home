using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Obstacle, ILightning
{

    [SerializeField] protected Animator animator;

    public override bool isBurnable => true;
    public override bool isCorrosive => true;
    public override bool isFragile => true;
    public override bool isTrampleable => true;
    public override bool isMeltable => true;


    protected override void Initialize()
    {
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnLightningHit()
    {
        Remove();
    }

    private IEnumerator RemoveAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        this.gameObject.SetActive(false);
    }

    public override void Remove()
    {
        ForceDehighlight();
        if(hitpoints > 0)
            hitpoints = 0;
        animator.Play("Destroy");
        ClearNodes();
        StartCoroutine(RemoveAnimation());
    }
}
