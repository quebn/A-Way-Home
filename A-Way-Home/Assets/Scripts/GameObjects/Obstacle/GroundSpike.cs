using UnityEngine;
using System.Collections;

public class GroundSpike : Obstacle, ITrap, IInteractable
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

    public void OnInteract()
    {
        // if(currentTool == Tool.Tremor && !isTriggered)
        //     PopUp();
        if(currentTool == Tool.Lightning && isTriggered)
        {
            TriggerDeath();
        }

    }

    public void OnHighlight()
    {
        if(!isTriggered)
            return;
        if(currentTool == Tool.Lightning || isTriggered)
            spriteRenderer.color = Color.green;
    }

    public void OnDehighlight()
    {
        if(!isTriggered)
            return;
        if(currentTool == Tool.Lightning || isTriggered)
            spriteRenderer.color = Color.white;

    }

    public void OnTrapTrigger(Character character)
    {
        PopUp();
        StartCoroutine(KillCharacter(character));
    }

    public void TriggerDeath()
    {
        ClearNodes();
        hitpoints -= 1;
        StartCoroutine(DeathAnimation());
    }

    private void PopUp()
    {
        isTriggered = true;
    }

    private IEnumerator KillCharacter(Character character)
    {
        while(character.currentPosition != this.nodes[0].worldPosition)
            yield return null;
        // character.TriggerDeath(animator.GetCurrentAnimatorClipInfo(0).Length);
        character.TriggerDeath(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator Kill(RockCrab rockCrab)
    {
        PopUp();
        // Debug.LogWarning($"GroundSpike SmallExplosion_Death Time: {this.animator.GetCurrentAnimatorClipInfo(0).Length}");
        // Debug.LogWarning($"GroundSpike state Time: {this.animator.GetCurrentAnimatorStateInfo(0).length}");
        // yield return new WaitForSeconds(this.animator.GetCurrentAnimatorClipInfo(0).Length);
        yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
        rockCrab.TriggerDeath();
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
