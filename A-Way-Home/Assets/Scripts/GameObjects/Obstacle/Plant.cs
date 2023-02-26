using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Obstacle , IInteractable, ITrap
{
    
    [SerializeField] protected Animator animator;
    
    protected override int hitpoints {
        get => animator.GetInteger("hitpoints");
        set => animator.SetInteger("hitpoints", value);
    } 
    private bool isAdult {
        get => hitpoints > 1;
    }

    protected override void Initialize()
    {
        base.Initialize();
        switch(hitpoints)
        {
            case 4:
                OnGrow();
                break;
            case 3:
                Debug.LogError("ERROR: SHOULD BE UNREACHABLE!");
                break;
            case 2:
                HarvestPlant();
                break;
            case 1: 
                Spawn();
                break;
            case 0:
                DestroyPlant();
                break;
        }
        Debug.Assert(GameData.levelData.obstacles.ContainsKey(id), $"ERROR: {id} Not in obstacle dictionary:");
    }

    public virtual void OnDehighlight()
    {
        if(currentTool == Tool.Grow || currentTool == Tool.Lightning)
            spriteRenderer.color = Color.white;

    }

    public virtual void OnHighlight()
    {
        if((currentTool == Tool.Grow && !isAdult )|| currentTool == Tool.Lightning)
            spriteRenderer.color = Color.green;
    }

    public virtual void OnInteract()
    {
        switch(currentTool)
        {
            case Tool.Grow:
                if(!isAdult)
                    OnGrow();
                break;
            case Tool.Lightning:
                DamagePlant(isAdult ? 2 : 1);
                break;
        }
    }

    public virtual void OnTrapTrigger(Character character)
    {
        if(isAdult)
            return;
    }

    public override void SaveData(LevelData levelData)
    {
        base.SaveData(levelData);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
    }

    protected void HarvestPlant()
    {
        Harvested();
    }

    protected void Spawn()
    {
        Debug.Assert(!isAdult, "ERROR: isAdult bool true, and hitpoints isnt full");
        animator.Play("Plant_Spawn");
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    protected void OnGrow()
    {
        if(hitpoints != 4)
            hitpoints = 4;
        Debug.Assert(isAdult, "ERROR: isnt adult and hitpoints not equal to 1!");
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    protected void Harvested()
    {
        // if(isAdult && hitpoints > 2)
            // currentStage = Stage.Harvested;
        Debug.Assert(hitpoints == 2, "ERROR: hitpoints not equal to 2!");
        animator.Play("Plant_Middle");
    }

    private void DestroyPlant()
    {
        Debug.Assert(hitpoints == 0, "ERROR: Plant Cleared despite hp is above 0!");
        StartCoroutine(OnClear());
        ClearNodes();
    }

    public void DamagePlant(int damage = 0)
    {
        Debug.Log($"Plant Damage: {damage}");
        hitpoints -= (damage == 0) ? hitpoints : damage;
        switch(hitpoints)
        {
            case 4:
            case 1:
                Debug.LogError("ERROR: SHOULD BE UNREACHABLE!");
                break;
            case 2:
                HarvestPlant();
                break;
            case 0:
                DestroyPlant();
                break;
        }
    }

    public IEnumerator OnClear()
    {
        animator.Play("Plant_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }
}
