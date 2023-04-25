using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRock : Obstacle, ILightning, ITremor
{
    [SerializeField] private Animator animator;
    private List<Node> lightNodes;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        lightNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
        SetLightningField();
    }

    private void SetLightningField()
    {
        if(hitpoints < 1)
            return;
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].isConductive = true;
        nodes[0].isConductive = true;
    }

    private void ClearLightningField()
    {
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].isConductive = false;
        nodes[0].isConductive = false;
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public void OnTremor()
    {
        Detonate();
    }

    public IEnumerator Explode()
    {
        Detonate();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        this.gameObject.SetActive(false);
    }

    public override void Remove()
    {
        ForceDehighlight();
        hitpoints = 0;
        StartCoroutine(Explode());
    }

    public void Detonate()
    {
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].ShockObstacle(1);
        animator.Play("Explosion");
    }
}
