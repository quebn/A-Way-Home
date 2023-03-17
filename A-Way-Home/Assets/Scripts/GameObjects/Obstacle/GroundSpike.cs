using UnityEngine;
using System.Collections;

public class GroundSpike : Obstacle, ITrap, ILightning
{
    [SerializeField] private Animator animator;

    protected override int hitpoints {
        get => animator.GetInteger("hitpoints");
        set => animator.SetInteger("hitpoints", value);
    }

    private bool isTriggered {
        get => animator.GetBool("isTriggered");
        set => animator.SetBool("isTriggered", value);
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnLightningHit()
    {
        Remove();
    }

    protected override void OnHighlight(Tool tool)
    {
        if(!isTriggered)
            return;
        if(tool == Tool.Lightning || isTriggered)
            spriteRenderer.color = Color.green;
    }

    protected override void OnDehighlight(Tool tool)
    {
        if(!isTriggered)
            return;
        if(tool == Tool.Lightning || isTriggered)
            spriteRenderer.color = Color.white;

    }

    public void OnTrapTrigger(Character character)
    {
        PopUp();
        StartCoroutine(Kill(character));
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

    public  IEnumerator Kill(RockCrab rockCrab)
    {
        PopUp();
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(rockCrab);
        PopDown();
    }

    public IEnumerator Kill(Undead undead)
    {
        PopUp();
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
        undead.TriggerDeath();
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
