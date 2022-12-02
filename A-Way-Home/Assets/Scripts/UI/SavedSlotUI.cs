// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedSlotUI : MonoBehaviour
{
    // create a static list of SaveSlotUI instances
    [SerializeField] private int slotIndexNumber;
    [SerializeField] private GameObject hasData;
    [SerializeField] private GameObject noData;

    #region hasData SerializedField Components
    // 8 components Image/Filename/energy/moves/Lives/LevelNum/Time/Date

    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;  
    [SerializeField] private TextMeshProUGUI fileName;  
    [SerializeField] private TextMeshProUGUI energy;  
    [SerializeField] private TextMeshProUGUI moves;  
    [SerializeField] private TextMeshProUGUI lives;  
    [SerializeField] private TextMeshProUGUI level;  
    [SerializeField] private TextMeshProUGUI date;  
    [SerializeField] private TextMeshProUGUI time;  
    #endregion

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
        if (!this.hasData.activeSelf || GameData.saveFileDataList[this.slotIndexNumber] == null)
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
        int Size = 0;
        Size = GameData.saveFileDataList.Count;
        if (Size == 0 || slotIndexNumber >= Size )
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
        SetValues(GameData.saveFileDataList[slotIndexNumber]);
    }
    
    private void SetValues(SaveFileData slotdata)
    {
        this.characterName.text = slotdata.levelData.characterName;
        this.characterImage.sprite = GameData.characterSprites[slotdata.levelData.characterName];
        this.fileName.text = slotdata.fileName;
        this.energy.text = slotdata.levelData.characterEnergy.ToString();
        this.moves.text = slotdata.levelData.moves.ToString();
        this.lives.text = slotdata.levelData.lives.ToString();
        this.level.text = slotdata.levelData.level.ToString();
        this.date.text = slotdata.date;
        this.time.text = slotdata.time;
    }

    public static void RefreshSaveSlots()
    {
        GameData.saveFileDataList = SaveSystem.InitAllSavedData();
        for(int i = 0; i < 5; i++)
        {
            GameData.saveSlotUIDict[i].InitData();
        }
    }
}