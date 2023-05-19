using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomRedSpawn : Spawnable, ITrap, ILightning
{
    [SerializeField] protected Animator animator;

    public override bool isBurnable => true;
    public override bool isCorrosive => true;
    public override bool isFragile => true;
    public override bool isTrampleable => true;
    public override bool isMeltable => true;

    protected override void OnSpawn()
    {
        DestroyNodeObstacle();
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnTrapTrigger(Character character)
    {
        Remove();
        character.IncrementEnergy(-10);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Inspect)
            return;
        base.OnHighlight(tool);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
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
        audioSources[0].Play();
        ClearNodes();
        StartCoroutine(RemoveAnimation());
    }

}
