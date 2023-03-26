using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnergy : Plant
{
    [SerializeField] private int damage;
    [SerializeField] private GameObject lightRange;

    private List<Node> lightNodes;

    protected override void Initialize()
    {
        base.Initialize();
        lightNodes = NodeGrid.GetNeighborNodeList(nodes[0], 1);
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
        if(lightRange.activeSelf)
            return;
        lightRange.SetActive(true);
        foreach(Node node in lightNodes)
            node.isConductive = true;
        nodes[0].isConductive = true;
    }

    private void ClearLightField()
    {
        if(!lightRange.activeSelf)
            return;
        lightRange.SetActive(false);
        foreach(Node node in lightNodes)
            node.isConductive = false;
        nodes[0].isConductive = false;
    }

}
