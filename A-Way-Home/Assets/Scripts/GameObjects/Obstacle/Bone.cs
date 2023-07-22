using UnityEngine;

public class Bone : Obstacle, ILightning, ITremor
{
    [SerializeField] private Animator animator;

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public void OnTremor()
    {
        Damage(2);
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public override void Remove()
    {
        ForceDehighlight();
        if(hitpoints != 0)
            hitpoints = 0;
        ClearNodes();
        audioSources[0].Play();
        animator.Play("Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnRemove", delay);
    }

    private void OnRemove()
    {
        this.gameObject.SetActive(false);
    }
}