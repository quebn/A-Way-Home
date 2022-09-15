using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    public string ID;
    [HideInInspector] public bool IsNotRemoved = true;

    [ContextMenu("Generate Obstacle id")]
    private void GenerateGuid() 
    {
        ID = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        
        if (!this.IsNotRemoved)
        {
            this.gameObject.SetActive(false);
        }
    }

}
