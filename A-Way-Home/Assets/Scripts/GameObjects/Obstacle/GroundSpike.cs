using UnityEngine;

public class GroundSpike : Obstacle, IInteractable, ITrap
{
    private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnClick()
    {
        if (incorrectTool)
            return;
        Debug.Log($"Click on Ground Spike with {PlayerActions.Instance.currentTool}");
    }


    public void OnHover()
    {

    }

    public void OnDehover()
    {

    }

    public void OnTrapTrigger(Character character)
    {
        animator.SetBool("triggered", true);
        character.TriggerDeath();
    }
}
