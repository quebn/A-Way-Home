using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour, ISaveable
{
    [SerializeField] private string ID;
    [SerializeField] private int energyRestored;
    private HomePortal homePortal;



    public Vector3 worldPosition => NodeGrid.GetMiddle(this.transform.position);

    public static Dictionary<Vector2, Essence> list; // <- maybe remove and put in GameData.levelData.essences
    
    private void Start()
    {
        Debug.Log($"Essence Position: {worldPosition}");
        Initialize();
    }

    private void Initialize()
    {
        Debug.Assert(list != null, "ERROR: List is null");
        list.Add(worldPosition, this);
        if(!GameData.levelData.essences.ContainsKey(ID))
            GameData.levelData.essences.Add(ID, this.gameObject.activeSelf);
    }

    public void OnConsume(Character character)
    {
        this.gameObject.SetActive(false);
        list.Remove(this.worldPosition);
        character.IncrementEnergy(energyRestored);
        character.IncrementEssence(-1);
    }

    public static List<Vector3> GetCurrentDestinations()
    {
        List<Vector3> destinations = new List<Vector3>();
        Debug.Assert(list.Count > 0, "ERROR: No Essences found");
        foreach(Vector2 pos in list.Keys)
            destinations.Add(pos);
        return destinations;
    }

    public static void InitializeAll()
    {

    }

    [ContextMenu("Generate Essence ID")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }

    public void SaveData(LevelData levelData)
    {
        Debug.Assert(GameData.levelData.essences.ContainsKey(this.ID), $"ERROR: essences with id of {ID} should be in this dictionary.");
        GameData.levelData.essences[ID] = this.gameObject.activeSelf;
    }

    public void LoadData(LevelData levelData)
    {
        Debug.Assert(GameData.levelData.essences.ContainsKey(this.ID), $"ERROR: essences with id of {ID} should be in this dictionary.");
        this.gameObject.SetActive(levelData.essences[ID]);
    }
}
