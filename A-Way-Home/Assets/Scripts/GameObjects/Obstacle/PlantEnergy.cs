using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant, ITrap
{
[SerializeField] private int damage;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private List<Node> lightNodes;

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

    public override void Damage(int damage = 0)
    {
        Debug.Log($"Plant Damage: {damage}");
        hitpoints -= (damage == 0) ? hitpoints : damage;
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult? NodeType.Obstacle: NodeType.Walkable, this);
        if(hitpoints <= 2)
            ClearLightField();
        if(hitpoints <= 0)
            Remove();
    }

    private void SetLightField()
    {
        if(hitpoints < 4)
            return;
        foreach(Node node in lightNodes)
            node.isConductive = true;
        nodes[0].isConductive = true;
    }

    private void ClearLightField()
    {
        foreach(Node node in lightNodes)
            node.isConductive = false;
        nodes[0].isConductive = false;
    }

}
