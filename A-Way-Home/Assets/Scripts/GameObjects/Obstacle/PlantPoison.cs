using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPoison : Plant
{
    [SerializeField] private int damage; 
    [SerializeField] private GameObject prefabPoisonMiasma; 
    private Dictionary<Vector2Int, Node> tilesPoisoned;

    public override bool isCorrosive => false;

    protected override void Initialize()
    {
        base.Initialize();
        tilesPoisoned = NodeGrid.GetNeighborNodes(this.nodes[0], NodeGrid.Instance.grid, 1);
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(-damage);
        Damage(1);
    }

    public override void OnRevealNodeColor()
    {
        Node.RevealNodes(nodes, Node.colorPurple);
    }

    public override void OnLightningHit()
    {
        base.OnLightningHit();
        if(hitpoints == 2)
            RemoveMiasma();
        
    }

    public override void OnGrow()
    {
        base.OnGrow();
        GeneratePoisonTiles();
    }

    private void GeneratePoisonTiles()
    {
        foreach(KeyValuePair<Vector2Int, Node> pair in tilesPoisoned)
        {
            if(!IsCorrosive(pair.Value))
                continue;
            GameObject.Instantiate(prefabPoisonMiasma, pair.Value.worldPosition, Quaternion.identity);
        } 
    }

    private bool IsCorrosive(Node node)
    { 
        if(node.IsWalkable() && !node.hasObstacle)
            return true;
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water))
            return false;
        return node.GetObstacle().isCorrosive;
    }


    private void RemoveMiasma()
    {
        foreach(KeyValuePair<Vector2Int, Node> pair in tilesPoisoned)
        {
            if(pair.Value.IsObstacle(typeof(PoisonMiasma)))
            
            {
                PoisonMiasma miasma = (PoisonMiasma)pair.Value.GetObstacle();
                miasma.Remove();
            }

        }
        Debug.Log("Poison Plant Cleared");
    }

}
