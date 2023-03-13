using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPoison : Plant
{
    [SerializeField] private GameObject prefabPoisonMiasma; 
    private Dictionary<Vector2Int, Node> tilesPoisoned;

    protected override void Initialize()
    {
        base.Initialize();
        tilesPoisoned = NodeGrid.GetNeighborNodes(this.nodes[0], NodeGrid.Instance.grid, 1);
    }


    public override void OnTrapTrigger(Character character)
    {
        base.OnTrapTrigger(character);
        character.IncrementEnergy(-5);
        DamagePlant(1);
    }

    public override void OnRevealNodeColor()
    {
        Node.RevealNodes(nodes, Node.colorPurple);
    }

    protected override void OnGrow()
    {
        base.OnGrow();
        GeneratePoisonTiles();
    }

    private void GeneratePoisonTiles()
    {
        foreach(KeyValuePair<Vector2Int, Node> pair in tilesPoisoned)
        {
            if(!DestroyObstacles(pair.Value))
                continue;
            PoisonMiasma miasma = GameObject.Instantiate(prefabPoisonMiasma, pair.Value.worldPosition, Quaternion.identity).GetComponent<PoisonMiasma>();
            miasma.AddAsSpawned($"{GameData.levelData.spawnCount += 1}");

        } 
    }

    private bool DestroyObstacles(Node node)
    { 
        if(node.IsWalkable() && !node.hasObstacle)
            return true;
        if(node.hasObstacle)
        {
            if(node.IsObstacle(typeof(TreeThin)))
            {
                TreeThin tree = (TreeThin)node.GetObstacle();
                tree.DestroyCompletely();
            }
            else if(node.IsObstacle(typeof(TreeLog)))
            {
                TreeLog log = (TreeLog)node.GetObstacle();
                log.Clear();
            }
            else if(node.IsObstacle(typeof(GroundSpike)))
            {
                GroundSpike spike = (GroundSpike)node.GetObstacle();
                spike.ForceClear();
                Debug.LogWarning("Cleared Spike");
            }
            else if(node.IsObstacle(typeof(RockCrab)))
            {
                RockCrab crab = (RockCrab)node.GetObstacle();
                crab.ForceClear();
            }
            else if(node.IsObstacle(typeof(PlantEnergy)))
            {
                PlantEnergy plant = (PlantEnergy)node.GetObstacle();
                plant.ForceClear();
                Debug.LogWarning("Cleared Plant");
            }
            else if(node.IsObstacle(typeof(Undead)))
            {
                Undead undead = (Undead)node.GetObstacle();
                undead.TriggerDeath(true);
            }
            return true;
        }

        return false;
    }


    protected override void HarvestPlant()
    {
        base.HarvestPlant();
        foreach(KeyValuePair<Vector2Int, Node> pair in tilesPoisoned)
        {
            if(pair.Value.IsObstacle(typeof(PoisonMiasma)))
            {
                PoisonMiasma miasma = (PoisonMiasma)pair.Value.GetObstacle();
                miasma.Clear();
            }

        }
        Debug.Log("Poison Plant Cleared");
    }

}
