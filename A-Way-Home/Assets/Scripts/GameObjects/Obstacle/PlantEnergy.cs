using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant, ITrap
{
    [SerializeField] private List<Sprite> sprites;
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
                mainSpriteRenderer.sprite = sprites[2];
                break;
            case 2:
                mainSpriteRenderer.sprite = sprites[1];
                break;
            case 1:
                mainSpriteRenderer.sprite = sprites[0];
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
        character.IncrementEnergy(heal);
        Damage(hitpoints);
    }

    public override void OnGrow()
    {
        base.OnGrow();
        SetLightField();
    }

    public override void Damage(int damage)
    {
        DamageAnimation();
        hitpoints -= damage;
        if(hitpoints < 2)
            hitpoints = 0;
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult ? NodeType.Obstacle: NodeType.Walkable, this);
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
            lightNodes[i].SetStatus(NodeStatus.Conductive);
        nodes[0].SetStatus(NodeStatus.Conductive);
    }

    private void ClearLightField()
    {
        for(int i = 0; i < lightNodes.Count; i++)
            lightNodes[i].SetStatus(NodeStatus.None);
        nodes[0].SetStatus(NodeStatus.None);
    }

    public override void Remove()
    {
        if(isAdult)
            ClearLightField();
        base.Remove();
    }
}