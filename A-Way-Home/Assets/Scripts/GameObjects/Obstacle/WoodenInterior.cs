using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenInterior : Obstacle, ILightning
{
    public override bool isBurnable => true;
    public override bool isMeltable => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning)
            return;
        base.OnHighlight(tool);
    }

    public override void Damage(int value)
    {
        if(hitpoints > 0)
            hitpoints -= value;
        if(hitpoints > 0)
            return;
        Remove();
    }

    public override void Remove()
    {
        if(hitpoints > 0)
            hitpoints = 0;
        ClearNodes();
        this.gameObject.SetActive(false);
        // StartCoroutine(OnRemove());
    }

    // private IEnumerator OnRemove()
    // {
        // ClearNodes();
        // // animator.Play("OnDestroy");
        // // float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        // // yield return new WaitForSeconds(delay);
        // this.gameObject.SetActive(false);
    // }
}
