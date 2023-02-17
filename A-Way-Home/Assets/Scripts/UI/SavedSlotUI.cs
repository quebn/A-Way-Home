using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedSlotUI : MonoBehaviour
{
    // create a static list of SaveSlotUI instances
    [SerializeField] private int slotIndexNumber;
    [SerializeField] private GameObject hasData;
    [SerializeField] private GameObject noData;

    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;  
    [SerializeField] private TextMeshProUGUI fileName;  
    [SerializeField] private TextMeshProUGUI energy;  
    [SerializeField] private TextMeshProUGUI moves;  
    [SerializeField] private TextMeshProUGUI lives;  
    [SerializeField] private TextMeshProUGUI level;  
    [SerializeField] private TextMeshProUGUI date;  
    [SerializeField] private TextMeshProUGUI time;  

    public static string FileNameToBeDeleted;

    private void Start()
    {
        GameData.saveSlotUIDict.Add(slotIndexNumber, this);
        InitData();
    }

    private void OnDisable()
    {
        GameData.saveSlotUIDict.Remove(slotIndexNumber);
    }

    public void LoadGame()
    {
        if (noData.activeSelf)
            return;
        GameEvent.LoadGame(slotIndexNumber);
    }

    public void OverwriteFile()
    {
        if (!this.hasData.activeSelf || GameData.savedDataFiles.Count < this.slotIndexNumber)
        {
            Debug.Log("No Data to be Overwritten!");
            return;
        }
        FileNameToBeDeleted = this.fileName.text;
        OptionsUI.Instance.confirmOverwriteWindow.SetActive(true);
    }

    // Delete the exisiting file and replace it with the new save;
    public void DeleteButton(string buttonlocation)
    {
        switch(buttonlocation)
        {
            case "mainmenu":
                MainMenuUI.Instance.deleteConfirmWindow.SetActive(true);
                break;
            case "ingame-save":
                OptionsUI.Instance.deleteConfirmSaveWindow.SetActive(true);
                break;
            case "ingame-load":
                OptionsUI.Instance.deleteConfirmLoadWindow.SetActive(true);
                break;
            default:
                Debug.LogError("no confirm window found");
                break;
        }
        if (noData.activeSelf)
            return;
        FileNameToBeDeleted = this.fileName.text;
        Debug.Log($"{FileNameToBeDeleted} is to be deleted!");
    }

    private void InitData()
    {
        int size = GameData.savedDataFiles.Count;
        if (size == 0 || slotIndexNumber >= size )
        {
            if (this.hasData.activeSelf)
            {
                this.hasData.SetActive(false);
                this.noData.SetActive(true);
            }
            Debug.Log($"Slot number {slotIndexNumber + 1} is empty and has no data");
            return;
        }
        this.noData.SetActive(false);
        this.hasData.SetActive(true);
        SetValues(GameData.savedDataFiles[slotIndexNumber]);
    }
    
    private void SetValues(SaveFileData data)
    {
        this.fileName.text = data.fileName;
        this.characterName.text = data.levelData.characterName;
        // this.characterImage.sprite = GameData.characterSprites[data.levelData.characterName];
        this.energy.text = data.levelData.characterEnergy.ToString();
        this.moves.text = data.levelData.moves.ToString();
        this.lives.text = data.levelData.lives.ToString();
        this.level.text = data.levelData.level.ToString();
        this.date.text = data.date;
        this.time.text = data.time;
    }

    public static void UpdateSaveSlots()
    {
        Debug.Log("Saved slot list Updated");
        GameData.savedDataFiles = SaveSystem.FetchAllSavedFileData();
        for(int i = 0; i < GameData.saveSlotUIDict.Count; i++)
            GameData.saveSlotUIDict[i].InitData();
    }
}