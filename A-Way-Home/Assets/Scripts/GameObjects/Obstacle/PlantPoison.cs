using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPoison : Plant
{
    [SerializeField] private int damage; 
    [SerializeField] private GameObject prefabPoisonMiasma; 
    private Dictionary<Vector2Int, Node> tilesPoisoned;
    private HashSet<PoisonMiasma> miasmas;

    public override bool isCorrosive => false;

    protected override void Initialize()
    {
        base.Initialize();
        tilesPoisoned = NodeGrid.GetNeighborNodes(this.nodes[0], NodeGrid.Instance.grid, 1);
        miasmas = new HashSet<PoisonMiasma>();
    }

    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(-damage);
        character.DamageAnimation();
        Damage(1);
    }

    public override void OnLightningHit()
    {
        base.OnLightningHit();
        if(hitpoints < 4 && miasmas.Count != 0)
            RemoveMiasma();
    }

    public override void OnGrow()
    {
        base.OnGrow();
        GeneratePoisonTiles();
    }

    public override void Remove()
    {
        base.Remove();
        if(miasmas.Count != 0)
            RemoveMiasma();
    }

    private void GeneratePoisonTiles()
    {
        foreach(Node node in tilesPoisoned.Values)
        {
            if(!IsCorrosive(node))
                continue;
            miasmas.Add(GameObject.Instantiate(prefabPoisonMiasma, node.worldPosition, Quaternion.identity).GetComponent<PoisonMiasma>());
        } 
    }

    private bool IsCorrosive(Node node)
    {
        if(node.hasPlatform) 
            return node.GetObstacle(true).isCorrosive; 
        if(node.IsWalkable() && !node.hasObstacle)
            return true;
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water))
            return false;
        return node.GetObstacle().isCorrosive;
    }

    private void RemoveMiasma()
    {
        foreach(PoisonMiasma miasma in miasmas)
            Destroy(miasma);
        miasmas.Clear();
        Debug.Log("Poison Plant Cleared");
    }

}
