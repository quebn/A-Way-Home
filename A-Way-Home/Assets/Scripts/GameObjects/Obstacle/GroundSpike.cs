using UnityEngine;
using System.Collections;

public class GroundSpike : Obstacle, ITrap, ILightning
{
    [SerializeField] private Animator animator;

    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isCorrosive => true;
    public override bool isMeltable => true;


    private bool isTriggered {
        get => animator.GetBool("isTriggered");
        set => animator.SetBool("isTriggered", value);
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnLightningHit(int damage)
    {
        if(isTriggered)
            Damage(damage);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning && tool != Tool.Inspect && tool != Tool.Tremor)
            return;
        if(tool == Tool.Lightning && !isTriggered)
            return;
        base.OnHighlight(tool);
    }


    public void OnTrapTrigger(Character character)
    {
        PopUp();
        StartCoroutine(Kill(character));
    }

    public override void Destroy(Obstacle obstacle)
    {
        StartCoroutine(Kill(obstacle));
    }

    public override void Remove()
    {
        ClearNodes();
        hitpoints -= 1;
        StartCoroutine(DeathAnimation());
    }

    private void PopUp()
    {
        isTriggered = true;
    }

    private IEnumerator Kill(Character character)
    {
        while(character.currentPosition != this.nodes[0].worldPosition)
            yield return null;
        // character.TriggerDeath(animator.GetCurrentAnimatorClipInfo(0).Length);
        character.TriggerDeath(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    private  IEnumerator Kill(Obstacle obstacle)
    {
        PopUp();
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
        base.Destroy(obstacle);
        PopDown();
    }


    private IEnumerator DeathAnimation()
    {
        // this.animator.Play("SmallExplosion_Death");
        // yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorClipInfo(0).Length);
        // yield return new WaitForSeconds(.6f);
        this.gameObject.SetActive(false);
    }

    private void PopDown()
    {
        isTriggered = false;
    }
}
