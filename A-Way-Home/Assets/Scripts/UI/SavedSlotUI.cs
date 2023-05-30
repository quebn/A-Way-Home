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
    [SerializeField] private TextMeshProUGUI essence;  
    [SerializeField] private TextMeshProUGUI moves;  
    [SerializeField] private TextMeshProUGUI lives;  
    [SerializeField] private TextMeshProUGUI stage;  
    [SerializeField] private TextMeshProUGUI level;  
    [SerializeField] private TextMeshProUGUI date;  
    [SerializeField] private TextMeshProUGUI time;  
    private SaveFileData saveFileData;

    public static string FileNameToBeDeleted;

    public void LoadGame()
    {
        if (noData.activeSelf)
            return;
        Debug.Assert(saveFileData != null, "ERROR: saveFileData is null");
        GameEvent.LoadGame(saveFileData);
    }

    public void OverwriteFile()
    {
        if (!this.hasData.activeSelf || SaveSystem.FetchAllSavedFileData().Count < this.slotIndexNumber)
            return;
        FileNameToBeDeleted = this.fileName.text;
        OptionsUI.Instance.confirmOverwriteWindow.SetActive(true);
        OptionsUI.Instance.overwriteNameInput.text = this.fileName.text;
    }

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

    private void InitData(List<SaveFileData> datas)
    {
        Debug.LogWarning($"DataCount:{datas.Count} -> SlotNumber{this.slotIndexNumber}");
        if(datas.Count == 0 || datas.Count <= this.slotIndexNumber)
        {
            if (this.hasData.activeSelf)
            {
                this.hasData.SetActive(false);
                this.noData.SetActive(true);
            }
            Debug.Log($"Slot number {slotIndexNumber + 1} is empty and has no data");
            return;
        }
        this.saveFileData = datas[slotIndexNumber];
        this.noData.SetActive(false);
        this.hasData.SetActive(true);
        SetValues(saveFileData);
    }
    
    private void SetValues(SaveFileData data)
    {
        Sprite sprite = Resources.Load<Sprite>($"Characters/Images/{data.levelData.characterName}");
        this.fileName.text = data.fileName;
        this.characterName.text = data.levelData.characterName;
        this.characterImage.sprite = sprite;
        this.energy.text = data.levelData.characterEnergy.ToString();
        this.essence.text = data.levelData.characterRequiredEssence.ToString();
        this.moves.text = data.levelData.moves.ToString();
        this.lives.text = data.levelData.lives.ToString();
        this.stage.text = data.levelData.stage.ToString();
        this.level.text = data.levelData.level.ToString();
        this.date.text = data.date;
        this.time.text = data.time;
    }

    public static void LoadAllSaveSlotsUI()
    {
        List<SaveFileData> saveFileDatas = SaveSystem.FetchAllSavedFileData();
        SavedSlotUI[] slots = GameObject.FindObjectsOfType<SavedSlotUI>();
        Debug.LogWarning($"SLOTS COUNT -> {slots.Length}");
        for(int i = 0; i < slots.Length; i++)
            slots[i].InitData(saveFileDatas);
    }
}