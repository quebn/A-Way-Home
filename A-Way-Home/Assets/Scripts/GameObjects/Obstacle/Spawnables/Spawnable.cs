using System.Collections;
using UnityEngine;

public class Spawnable : Obstacle
{
    private bool finished = false;
    protected bool isLoaded = false;

    public static int spawnCount = 0;

    protected override void Initialize()
    {
        this.id = $"{GameData.levelData.spawnCount += 1}";
        finished = true;
        OnSpawn();
        base.Initialize();
        spawnCount++;
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
        isLoaded = true;
        StartCoroutine(SpawnLoadData(levelData));
    }

    private IEnumerator SpawnLoadData(LevelData levelData)
    {
        while(!finished)
            yield return null;
        base.LoadData(levelData);
    }
}
