using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    public string ID;
    [HideInInspector] public bool IsNotRemoved = true; // should be opposite

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
        if (GameData.LoadType == LevelLoadType.NewGame)
            return;
        if (!PlayerLevelData.Instance.RemovedObstacles.ContainsKey(this.ID))
        {
            Debug.Log($"Removed obstacle list count: {PlayerLevelData.Instance.RemovedObstacles.Count}");
            return;
        }
        IsNotRemoved = PlayerLevelData.Instance.RemovedObstacles[this.ID];
        this.gameObject.SetActive(IsNotRemoved);
    }
}
