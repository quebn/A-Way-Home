using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantParalyze : Obstacle, IInteractable, ITrap
{
    private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnClick()
    {
        if(incorrectTool)
            return;
        Debug.Log("Click on paralyzing plant.");
    }

    public void OnHover()
    {
        if(incorrectTool)
            return;
        SetMouseCursor(this.mouseTexture);
    }

    public void OnDehover()
    {
        ResetMouseCursor();
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(-5);
    }
}
