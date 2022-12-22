using System.Collections.Generic;
using UnityEngine;


public class ObstacleData : MonoBehaviour
{
    public string ID;
    public ManipulationType toolType;
    
    [HideInInspector] public bool isRemoved = false; // should be opposite


    [ContextMenu("Generate Obstacle id")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        PlayerLevelData.gameObjectList.Add(this.ID, this.gameObject);
        if (GameEvent.loadType != LevelLoadType.LoadGame)
            return;
        if (!PlayerLevelData.Instance.levelData.removedObstacles.ContainsKey(this.ID))
            return;
        isRemoved = PlayerLevelData.Instance.levelData.removedObstacles[this.ID];
        this.gameObject.SetActive(!isRemoved);
    }
}
