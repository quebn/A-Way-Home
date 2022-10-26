using UnityEngine;


public class ObstacleData : MonoBehaviour
{
    public string ID;
    public ManipulationType toolType;
    [HideInInspector] public bool isRemoved = false; // should be opposite

    [ContextMenu("Generate Obstacle id")]
    private void GenerateGuid() 
    {
        ID = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        InitObstacle();
    }
    
    private void InitObstacle()
    {
        if (GameEvent.loadType == LevelLoadType.Sandbox)
            return;
        if (GameEvent.loadType == LevelLoadType.NewGame || GameEvent.loadType == LevelLoadType.RestartGame)
            return;
        if (!PlayerLevelData.Instance.levelData.removedObstacles.ContainsKey(this.ID))
            return;
        isRemoved = PlayerLevelData.Instance.levelData.removedObstacles[this.ID];
        this.gameObject.SetActive(!isRemoved);
    }
}
