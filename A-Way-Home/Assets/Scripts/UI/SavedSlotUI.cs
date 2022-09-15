using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SavedSlotUI : MonoBehaviour
{
    // create a static list of SaveSlotUI instances
    [SerializeField] private int SlotIndexNumber;
    [SerializeField] private GameObject HasData;
    [SerializeField] private GameObject NoData;

    #region HasData SerializedField Components
    // 8 components Image/Filename/Energy/Moves/Lives/LevelNum/Time/Date
    [SerializeField] private TextMeshProUGUI FileName;  
    [SerializeField] private TextMeshProUGUI Energy;  
    [SerializeField] private TextMeshProUGUI Moves;  
    // [SerializeField] private TextMeshProUGUI Lives;  
    // [SerializeField] private TextMeshProUGUI LevelNum;  
    // [SerializeField] private TextMeshProUGUI Time;  
    // [SerializeField] private TextMeshProUGUI Date;  
    #endregion

    public static List<SavedSlotUI> SaveSlotsUIList = new List<SavedSlotUI>(5);
    public static string FileNameToBeDeleted;
    private void Start()
    {
        Debug.Log($"Slot Number {SaveSlotsUIList.Count + 1} => SlotIndexNumber: {SlotIndexNumber}");
        SaveSlotsUIList.Insert(SlotIndexNumber, this);
        InitData();
    }

    public void LoadGame()
    {
        GameData.LoadedLevelData = GameData.SaveFileDataList[SlotIndexNumber];
        GameData.LoadType = LevelLoadType.LoadGame;
        SceneManager.LoadScene(GameData.LoadedLevelData.LevelSceneName);
    }

    public void OverwriteFile()
    {
        // TODO: Implement function
        // Delete the exisiting file and replace it with the new save;
        if (!this.HasData.activeSelf || GameData.SaveFileDataList[this.SlotIndexNumber] == null)
        {
            Debug.Log("No Data to be Overwritten!");
            return;
        }
        FileNameToBeDeleted = this.FileName.text;
        OptionsUI.Instance.ConfirmOverwriteWindow.SetActive(true);
    }

    public void DeleteButton(string buttonlocation)
    {
        // TODO: fix mainmenu bug where window does not pop up when delete button is pressed
        switch(buttonlocation)
        {
            case "mainmenu":
                MainMenuUI.Instance.DeleteConfirmWindow.SetActive(true);
                break;
            case "ingame-save":
                OptionsUI.Instance.DeleteConfirmSaveWindow.SetActive(true);
                break;
            case "ingame-load":
                OptionsUI.Instance.DeleteConfirmLoadWindow.SetActive(true);
                break;
            default:
                Debug.LogError("no confirm window found");
                break;
        }
        if (NoData.activeSelf)
            return;
        FileNameToBeDeleted = this.FileName.text;
        Debug.Log($"{FileNameToBeDeleted} is to be deleted!");
    }

    private void InitData()
    {
        int Size = 0;
        Size = GameData.SaveFileDataList.Count;
        if (Size == 0 || SlotIndexNumber >= Size )
        {
            if (this.HasData.activeSelf)
            {
                this.HasData.SetActive(false);
                this.NoData.SetActive(true);
            }
            Debug.Log($"Slot number {SlotIndexNumber + 1} is empty and has no data");
            return;
        }
        this.NoData.SetActive(false);
        this.HasData.SetActive(true);
        SetValues(GameData.SaveFileDataList[SlotIndexNumber]);
    }
    
    private void SetValues(SaveFileData slotdata)
    {
        FileName.text = slotdata.FileName;
        Moves.text = slotdata.PlayerMoves.ToString();
    }

    public static void RefreshSaveSlots()
    {
        GameData.SaveFileDataList = SaveSystem.InitAllSavedData();
        for(int i = 0; i < 5; i++)
        {
            SaveSlotsUIList[i].InitData();
        }
    }
}
