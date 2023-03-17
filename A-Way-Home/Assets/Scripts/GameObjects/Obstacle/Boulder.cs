using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boulder : Obstacle, ILightning, ITremor
{

    [SerializeField] private Animator animator;

    protected override int hitpoints{
        get => animator.GetInteger("hitpoints");
        set => animator.SetInteger("hitpoints", value);
    } 

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

    public void OnLightningHit()
    {
        Damage(2);
    }

    public void OnTremor()
    {
        Damage(1);
    }

    public override void Damage(int value)
    {
        if(hitpoints > 0)
            hitpoints -= value;
        Debug.Log("Hit Boulder");
        if(hitpoints > 0)
            return;
        ClearNodes();
        animator.Play("BigBoulder_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnDestroy", delay);
    } 

    public override void LoadData(LevelData levelData)
    {
        Debug.LogWarning($"{levelData.obstacles[id].position} position");
        base.LoadData(levelData);
        if(hitpoints == 0)
            this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        this.gameObject.SetActive(false);
    }


}