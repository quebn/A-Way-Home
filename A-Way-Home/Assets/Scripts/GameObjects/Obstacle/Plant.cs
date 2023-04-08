using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Obstacle , ITrap, ILightning, IGrow
{
    [SerializeField] protected Animator animator;
    [SerializeField] private int initialStage = 1;
    [SerializeField] protected GameObject outline;

    public override bool isBurnable => true;
    public override bool isCorrosive => true;
    public override bool isFragile => true;
    public override bool isTrampleable => true;
    public override bool isMeltable => true;

    protected const string youngling    = "Plant_Youngling";
    protected const string middle       = "Plant_Middle";
    protected const string fullGrown    = "Plant_FullGrown";
    protected const string destroy      = "Plant_Destroy";

    protected override int hitpoints {
        get => initialStage;
        set => initialStage = value;
    } 

    private bool isAdult {
        get => hitpoints > 1;
    }

    protected override void Initialize()
    {
        base.Initialize();
        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult? NodeType.Obstacle: NodeType.Walkable, this);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(outline == null  || outline.activeSelf)
            return;
        if((tool == Tool.Grow && !isAdult )|| tool == Tool.Lightning)
            outline.SetActive(true);
    }

    protected override void OnDehighlight(Tool tool)
    {
        if(outline == null || !outline.activeSelf)
            return;
        outline.SetActive(false);
    }

    public virtual void OnLightningHit()
    {
        Damage(isAdult ? 2 : 1);
    }

    public virtual void OnGrow()
    {
        if(hitpoints != 4)
            hitpoints = 4;
        animator.Play(CurrentAnimationName());
        Debug.Assert(isAdult, "ERROR: isnt adult and hitpoints not equal to 1!");
        SetNodes(this.worldPos, NodeType.Obstacle, this);
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
        Debug.Log("22222222222222222222222222");
    }

    public override void Remove()
    {
        if(hitpoints > 0)
        {
            hitpoints = 0;
            Debug.LogWarning("Plant Cleared with hitpoints above 0");
        }
        animator.Play(CurrentAnimationName());
        ClearNodes();
        StartCoroutine(RemoveAnimation());
    }

    public override void Damage(int damage = 0)
    {
        Debug.Log($"Plant Damage: {damage}");
        hitpoints -= (damage == 0) ? hitpoints : damage;
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult? NodeType.Obstacle: NodeType.Walkable, this);
        if(hitpoints <= 0)
            Remove();
    }

    private IEnumerator RemoveAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        this.gameObject.SetActive(false);
    }

    protected virtual string CurrentAnimationName()
    {
        switch(hitpoints)
        {
            case 1:
                return youngling;
            case 2:
                return middle;
            case 4:
                return fullGrown;
            default:
                Debug.Assert(hitpoints <= 0, $"Error: Unexpected hitpoint value reached: {hitpoints}");
                return destroy;
        }
    }

}
