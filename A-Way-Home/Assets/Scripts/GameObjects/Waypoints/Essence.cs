using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    [SerializeField] private string ID;
    [SerializeField] private int energyRestored;
    private HomePortal homePortal;


    public Vector3 worldPosition => NodeGrid.SetToMid(this.transform.position);
    public static Dictionary<Vector2, Essence> list;
    
    private void Start()
    {
        Debug.Log($"Essence Position: {worldPosition}");
        Initialize();
    }

    private void Initialize()
    {
        list.Add(worldPosition, this);
        PlayerLevelData.Instance.currentDestinations.Add(this.worldPosition);
    }

    public void OnConsume(Character character)
    {
        this.gameObject.SetActive(false);
        list.Remove(this.worldPosition);
        PlayerLevelData.Instance.currentDestinations.Remove(this.worldPosition);
        character.Initialize(energyRestored, -1);
    }

    
    [ContextMenu("Generate Essence ID")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }
}
