using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant, ITrap
{
[SerializeField] private int damage;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private List<Node> lightNodes;

    protected override void OnHighlight(Tool tool)
    {
        if(isAdult)
            return;
        base.OnHighlight(tool);
    }

    private void OnValidate()
    {
        switch(hitpoints)
        {
            case 4:
                spriteRenderer.sprite = sprites[2];
                break;
            case 2:
                spriteRenderer.sprite = sprites[1];
                break;
            case 1:
                spriteRenderer.sprite = sprites[0];
                break;
            default:
                break;
        }
    }

    public override void OnLightningHit(int damage)
    {
        Damage(isAdult ? 0 : damage);
    }

    protected override void Initialize()
    {
        base.Initialize();
        lightNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
        SetLightField();
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(-damage);
        Damage(1);
    }

    public override void OnGrow()
    {
        base.OnGrow();
        SetLightField();
    }

    public override void Damage(int damage)
    {
        Debug.Log($"Plant Damage: {damage}");
        hitpoints -= damage;
        if(hitpoints < 2)
            hitpoints = 0;
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult? NodeType.Obstacle: NodeType.Walkable, this);
        if(hitpoints <= 2)
            ClearLightField();
        if(hitpoints <= 0)
            Remove();
    }

    private void SetLightField()
    {
        if(hitpoints < 3)
            return;
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].SetConduction(true);
        nodes[0].SetConduction(true);
    }

    private void ClearLightField()
    {
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].SetConduction(false);
        nodes[0].SetConduction(false);
    }

}
