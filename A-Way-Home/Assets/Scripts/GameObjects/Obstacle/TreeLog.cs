using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, ILightning
{

    // public bool notSpawned;
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        DestroyNodeObstacles();
        SetNodes(this.transform.position, NodeType.Obstacle, this);
    }

    public void OnLightningHit()
    {
        RemoveLog();
    }

    public void AddAsSpawned(string id)
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        this.id = id;
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(id), "ERROR: obstacle with id of {id} should not exist!");
        base.Initialize();
        animator.Play("Log_Spawn");
    }

    private void DestroyNodeObstacles()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(GroundSpike)))
        {
            GroundSpike groundSpike = (GroundSpike)node.GetObstacle();
            Destroy(node.GetObstacle());
        }
        else if(node.IsObstacle(typeof(RockCrab)))
        {
            RockCrab rockCrab = (RockCrab)node.GetObstacle();
            Destroy(rockCrab);
        }
        else if(node.IsObstacle(typeof(Rock)))
        {
            Rock rock = (Rock)node.GetObstacle();
            Destroy(rock);
        }
        else if(node.IsObstacle(typeof(Plant)))
        {
            Plant plant = (Plant)node.GetObstacle();
            Destroy(plant);
        }
    }

    private void RemoveLog()
    {
        hitpoints -= 1;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
        // if(GameData.levelData.spawneds.ContainsKey(id))
        //     GameData.levelData.spawneds.Remove(id);
        // else if(GameData.levelData.obstacles.ContainsKey(id))
        //     GameData.levelData.obstacles[id] = hitpoints;
    }

    public void Clear()
    {
        ClearNodes();
        hitpoints = 0;
        this.gameObject.SetActive(false);
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        // Debug.LogWarning($"loaded hp: {hitpoints}"/);
        if(hitpoints != 0)
            return;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
    }


}
