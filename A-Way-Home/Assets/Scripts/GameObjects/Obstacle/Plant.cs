using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Obstacle , ITrap, ILightning, IGrow
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
        OnInitialize();
    }


    protected override void OnHighlight(Tool tool)
    {
        if((tool == Tool.Grow && !isAdult )|| tool == Tool.Lightning)
            spriteRenderer.color = Color.green;
    }


    public virtual void OnLightningHit()
    {
        Damage(isAdult ? 2 : 1);
    }

    public virtual void OnGrow()
    {
        if(!isAdult)
            Grow();
    }

    public virtual void OnTrapTrigger(Character character)
    {
        Debug.Assert(!isAdult, "ERROR: consumed plant is adult");
    }

    public override void SaveData(LevelData levelData)
    {
        base.SaveData(levelData);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
    }

    protected virtual void HarvestPlant()
    {
        Harvested();
    }

    protected void Spawn()
    {
        Debug.Assert(!isAdult, "ERROR: isAdult bool true, and hitpoints isnt full");
        animator.Play("Plant_Spawn");
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    protected virtual void OnInitialize()
    {
        switch(hitpoints)
        {
            case 4:
                Grow();
                break;
            case 2:
                HarvestPlant();
                break;
            case 1: 
                Spawn();
                break;
            case 0:
                Remove();
                break;
            default:
                Debug.LogError("ERROR: SHOULD BE UNREACHABLE!");
                break;
        }
    }

    protected virtual void Grow()
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

    public override void Remove()
    {
        // Debug.Assert(hitpoints == 0, "ERROR: Plant Cleared despite hp is above 0!");
        if(hitpoints> 0)
        {
            hitpoints = 0;
            Debug.LogWarning("Plant Cleared with hitpoints above 0");
        }
        StartCoroutine(OnClear());
        ClearNodes();
    }

    public override void Damage(int damage = 0)
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
                Remove();
                break;
        }
    }

    private IEnumerator OnClear()
    {
        animator.Play("Plant_Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
    }

}
