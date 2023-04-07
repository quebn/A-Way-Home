using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : Obstacle
{
    [SerializeField] private Animator animator;
    private bool finished = false;

    protected override void Initialize()
    {
        // Debug.Assert(!GameData.levelData.obstacles.ContainsKey(this.id));
        this.id = $"{GameData.levelData.spawnCount += 1}";
        // Debug.Assert(GameData.levelData.obstacles[this.id].typeName == this.GetType().Name, "ERROR: type mismatch");
        base.Initialize();
        OnSpawn();
        finished = true;
    }

    protected virtual void OnSpawn()
    {
        Debug.LogWarning($"Spawned #{id} !");
    }

    protected void DestroyNodeObstacle()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.hasObstacle)
            Destroy(node.GetObstacle());
    }

    public override void LoadData(LevelData levelData)
    {
        StartCoroutine(SpawnLoadData(levelData));
    }

    private IEnumerator SpawnLoadData(LevelData levelData)
    {
        while(!finished)
            yield return null;
        base.LoadData(levelData);
    }
}
