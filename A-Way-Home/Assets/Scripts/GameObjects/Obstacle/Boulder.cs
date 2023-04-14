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

    public void OnLightningHit()
    {
        Damage(1);
    }

    public void OnTremor()
    {
        Damage(1);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Tremor)
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

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        if(hitpoints == 0)
            this.gameObject.SetActive(false);
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