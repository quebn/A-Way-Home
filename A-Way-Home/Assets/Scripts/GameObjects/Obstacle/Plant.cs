using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Obstacle , IInteractable, ITrap
{
    [System.Serializable] 
    public enum Stage {Juvenile, Adult, Harvested}
    
    [SerializeField] private Stage currentStage;
    [SerializeField] private Animator animator;


    // private bool juvenile => stage == Stage.Juvenile;
    // private bool adult => stage == Stage.Adult;
    // private bool harvested => stage == Stage.Harvested;

    protected override void Initialize()
    {
        base.Initialize();
        SetStage(currentStage);
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     animator = GetComponent<Animator>();
    //     InitializeNodes(worldPos);
    //     if(adult)
    //         GrowUp();
    }

    public void OnDehighlight()
    {
        if(currentTool == Tool.Grow || currentTool == Tool.Grow)
            spriteRenderer.color = Color.white;

    }

    public void OnHighlight()
    {
        if(currentTool == Tool.Grow && currentStage == Stage.Juvenile || currentTool == Tool.Lightning && currentStage == Stage.Adult)
            spriteRenderer.color = Color.green;
    }

    public void OnInteract()
    {
        switch(currentTool)
        {
            case Tool.Grow:
                if(currentStage == Stage.Juvenile)
                    Grow();
                break;
            case Tool.Lightning:
                if(currentStage == Stage.Adult)
                    HarvestPlant();
                else if(currentStage == Stage.Juvenile)
                    DestroyPlant();
                break;
        }
    }

    public void OnTrapTrigger(Character character)
    {
        if(currentStage != Stage.Juvenile)
            return;
        character.IncrementEnergy(-5);
        ClearPlant();
    }


    private void HarvestPlant()
    {
        Harvested();
    }

    private void SetStage(Stage stage)
    {
        switch(stage)
        {
            case Stage.Juvenile:
                Spawn();
                break;
            case Stage.Adult:
                Grow();
                break;
            case Stage.Harvested:
                Harvested();
                break;
        }
    }


    private void Spawn()
    {
        if(currentStage != Stage.Juvenile)
            currentStage = Stage.Juvenile;
        animator.Play("Plant_Spawn");
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    private void Grow()
    {
        if(currentStage != Stage.Adult)
            currentStage = Stage.Adult;
        animator.Play("Plant_Grow");
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    private void Harvested()
    {
        if(currentStage != Stage.Harvested)
            currentStage = Stage.Harvested;
        animator.Play("Plant_Harvested");
    }


    private void SpawnFruit()
    {
        Debug.Assert(false, "ERROR: Not implemented");
    }

    public void ClearPlant()
    {
        StartCoroutine(OnClear());
        ClearNodes();
    }

    private void DestroyPlant()
    {
        ClearPlant();
    }


    public IEnumerator OnClear()
    {
        animator.Play("Plant_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
