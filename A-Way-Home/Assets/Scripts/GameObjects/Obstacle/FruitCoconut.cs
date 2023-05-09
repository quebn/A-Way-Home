using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCoconut : Obstacle
{
    [SerializeField] private int heal = 5;
    [SerializeField] private Animator animator;

    public override bool isBurnable => true;
    public override bool isMeltable => true;
    public override bool isCorrosive => true;
    public override bool isTrampleable => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnLightningHit(int damage)
    {
        Remove();
    }

    public void OnTrapTrigger(Character character)
    {
        Debug.Log("Triggered!");
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void Damage(int damage)
    {
        Remove();
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
