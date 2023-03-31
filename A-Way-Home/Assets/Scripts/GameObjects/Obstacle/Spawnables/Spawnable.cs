using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : Obstacle
{
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(this.id));
        this.id = $"{GameData.levelData.spawnCount += 1}";
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(this.id));
        base.Initialize();
        OnSpawn();
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
}
