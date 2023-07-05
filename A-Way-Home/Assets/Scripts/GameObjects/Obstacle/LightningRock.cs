using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningRock : Obstacle, ILightning, ITremor, ITrap
{
    [SerializeField] private Animator animator;
    [SerializeField] Light2D light2d;
    [SerializeField] GameObject gemLights;

    private List<Node> lightNodes;

    public override bool isWalkableByTerra => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        lightNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
        SetLightningField();
    }

    private void SetLightningField()
    {
        UpdateLigths(hitpoints == 2);
        if(hitpoints < 2)
            return;
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].SetStatus(NodeStatus.Conductive);
        nodes[0].SetStatus(NodeStatus.Conductive);
    }

    private void ClearLightningField()
    {
        UpdateLigths(hitpoints < 2);
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].SetStatus(NodeStatus.None);
        nodes[0].SetStatus(NodeStatus.None);
    }

    private void UpdateLigths(bool isActive)
    {
        light2d.enabled = isActive;
        gemLights.SetActive(isActive);
    }

    public void OnLightningHit(int damage)
    {
        if(hitpoints == 2){
            Detonate();
            Damage(1);
        }else{
            hitpoints++;
            SetLightningField();
        }
    }

    public void OnTremor()
    {
        Damage(2);
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
        int hp = hitpoints; 
        hitpoints = 0;
        this.spriteRenderers[0].enabled = false;
        if(hp == 2)
            StartCoroutine(Explode());
        else
            this.gameObject.SetActive(false);
    }

    public override void Damage(int value)
    {
        DamageAnimation();
        hitpoints -= value > hitpoints ? hitpoints : value;
        if(hitpoints <= 0)
            Remove();
    }

    public void Detonate()
    {
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].ShockObstacle(1);
        ClearLightningField();
        audioSources[0].Play(); 
        animator.Play("Explosion");
    }

    public void OnTrapTrigger(Character character)
    {
        Remove();
    }
}
