using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant
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

    protected override void Initialize()
    {
        base.Initialize();
        lightNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
        SetLightField();
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
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
        base.Damage(damage);
        if(hitpoints <= 2)
            ClearLightField();
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
