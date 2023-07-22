using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour, ISaveable
{
    [SerializeField] private string ID;
    [SerializeField] private int energyRestored;
    
    private HomePortal homePortal;
    private Vector3 position;


    public Vector3 worldPosition => position;

    public static int count = 0;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        position = NodeGrid.GetMiddle(this.transform.position);
        Debug.LogWarning($"Essence Position: {position}");
        count++;

    }

    public void OnConsume(Character character)
    {
        AudioManager.instance.PlayAudio("Consume");
        character.IncrementEnergy(energyRestored);
        character.IncrementEssence(-1);
        if(character.noEssenceRequired)
            RemoveOtherEssence();
        this.gameObject.SetActive(false);
    }

    public static List<Vector3> GetCurrentDestinations()
    {
        List<Vector3> destinations = new List<Vector3>();
        Essence[] essences = GetAllActiveEssences();
        for(int i = 0; i < essences.Length; i++)
            destinations.Add(essences[i].worldPosition);
        return destinations;
    }

    private void RemoveOtherEssence()
    {
        Essence[] essences = GetAllActiveEssences();
        for(int i = 0; i < essences.Length; i++)
            if(essences[i].ID != this.ID)
                essences[i].Despawn();
    }

    public static Essence[] GetAllActiveEssences()
    {
        Essence[] essences = GameObject.FindObjectsOfType<Essence>(false);
        return essences;
    }

    public static Essence GetEssence(Vector3 position)
    {
        Essence[] essences = GetAllActiveEssences();
        for(int i = 0; i < essences.Length; i++)
            if(essences[i].worldPosition == position)
                return essences[i];
        return null;
    }

    private void Despawn()
    {
        this.gameObject.SetActive(false);
    }

    [ContextMenu("Generate Essence ID")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }

    public void SaveData(LevelData levelData)
    {
        if(!GameData.levelData.essences.ContainsKey(ID))
            GameData.levelData.essences.Add(ID, this.gameObject.activeSelf);
        Debug.Assert(GameData.levelData.essences.ContainsKey(this.ID), $"ERROR: essences with id of {ID} should be in this dictionary.");
        GameData.levelData.essences[ID] = this.gameObject.activeSelf;
        Debug.Assert(GameData.levelData.essences[ID] == this.gameObject.activeSelf, $"ERROR: wrong active state.");
        Debug.LogWarning($"Saved Essence: essence with id of {ID} should be {levelData.essences[ID]} .");
    }

    public void LoadData(LevelData levelData)
    {
        Debug.Assert(GameData.levelData.essences.ContainsKey(this.ID), $"ERROR: essences with id of {ID} should be in this dictionary.");
        this.gameObject.SetActive(levelData.essences[ID]);
        Debug.LogWarning($"Loaded Essence: essences with id of {ID} is {levelData.essences[ID]} .");
    }
}