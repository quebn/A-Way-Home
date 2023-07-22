using System.Collections.Generic;
using UnityEngine;

public class Boulder : Obstacle, ILightning, ITremor
{
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
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
        List<Node> prevNodes = new List<Node>();
        for(int i = 0; i < nodes.Count; i++)
            prevNodes.Add(nodes[i]);
        ClearNodes();
        for(int i = 0; i < prevNodes.Count; i++)
            FireNode.ContinueFire(prevNodes[i]);
        audioSources[0].Play();
        animator.Play("BigBoulder_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnRemove", delay);
    }

    private void OnRemove()
    {
        this.gameObject.SetActive(false);
    }
}