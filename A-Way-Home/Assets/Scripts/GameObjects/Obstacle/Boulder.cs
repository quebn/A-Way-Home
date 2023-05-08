using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boulder : Obstacle, ILightning, ITremor
{

    [SerializeField] private Animator animator;


    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    //     animator = GetComponent<Animator>();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     InitializeNodes(this.transform.position);
    //     SetNodesType(NodeType.Obstacle, this);
    //     hitpoints = 4;
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
        // Debug.LogWarning($"{this.gameObject.name} damage->{damage} >>>> hp->{hitpoints}");
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
        Debug.Assert(hitpoints == 0, "ERROR: Boulder HP isnt Zero");
        ClearNodes();
        animator.Play("BigBoulder_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnRemove", delay);
    }

    private void OnRemove()
    {
        this.gameObject.SetActive(false);
    }
}