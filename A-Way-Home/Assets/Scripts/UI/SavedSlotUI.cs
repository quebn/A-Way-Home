using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SavedSlotUI : MonoBehaviour
{
    // create a static list of SaveSlotUI instances
    [SerializeField] private int slotIndexNumber;
    [SerializeField] private GameObject hasData;
    [SerializeField] private GameObject noData;

    #region hasData SerializedField Components
    // 8 components Image/Filename/energy/moves/Lives/LevelNum/Time/Date
    [SerializeField] private TextMeshProUGUI fileName;  
    [SerializeField] private TextMeshProUGUI energy;  
    [SerializeField] private TextMeshProUGUI moves;  
    // [SerializeField] private TextMeshProUGUI Lives;  
    // [SerializeField] private TextMeshProUGUI LevelNum;  
    // [SerializeField] private TextMeshProUGUI Time;  
    // [SerializeField] private TextMeshProUGUI Date;  
    #endregion

    public static string FileNameToBeDeleted;

    private void Start()
    {
        // Debug.Log($"Slot Number {GameData.saveSlotUIDict.Count} => slotIndexNumber: {slotIndexNumber} Capacity = {GameData.saveSlotUIDict.Count}");
        GameData.saveSlotUIDict.Add(slotIndexNumber, this);
        InitData();
    }

    private void OnDisable()
    {
        GameData.saveSlotUIDict.Remove(slotIndexNumber);
    }

    public void LoadGame()
    {
        // GameEvent.instance.LoadGame(slotIndexNumber);
        if (noData.activeSelf)
            return;
        GameEvent.LoadGame(slotIndexNumber);
    }

    public void OverwriteFile()
    {
        // TODO: Implement function
        // Delete the exisiting file and replace it with the new save;
        if (!this.hasData.activeSelf || GameData.saveFileDataList[this.slotIndexNumber] == null)
        {
            Debug.Log("No Data to be Overwritten!");
            return;
        }
        FileNameToBeDeleted = this.fileName.text;
        OptionsUI.Instance.confirmOverwriteWindow.SetActive(true);
    }

    public void DeleteButton(string buttonlocation)
    {
        // TODO: fix mainmenu bug where window does not pop up when delete button is pressed
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
        fileName.text = slotdata.fileName;
        moves.text = slotdata.levelData.moves.ToString();
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
