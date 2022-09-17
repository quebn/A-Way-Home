using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance {get; private set;}
    [SerializeField] private GameObject SaveGameWindow;
    [SerializeField] private GameObject CreateNewFileWindow;
    [SerializeField] private GameObject LoadGameWindow;
    [SerializeField] private GameObject QuitGameWindow;
    [SerializeField] private TMP_InputField FileNameInput;
    [SerializeField] private TMP_InputField OverwriteNameInput;
    // Confirm delete windows
    public GameObject DeleteConfirmSaveWindow;
    public GameObject DeleteConfirmLoadWindow;
    public GameObject ConfirmOverwriteWindow;

    // public InGameUI InGameUI;
    private bool isActive;

    private void Start()
    {
        isActive = true;
        Debug.Assert(ConfirmOverwriteWindow != null, $"{ConfirmOverwriteWindow} is null");
        Debug.Assert(OverwriteNameInput != null, $"{OverwriteNameInput} is null");
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: OptionsUI instance is null");
    }

    #region Pause button functions
    public void Resume()
    {
        InGameUI.Instance.UnpauseGame();
        InGameUI.Instance.OptionUI.SetActive(false);

    }

    public void SaveGame()
    {
        GameData.SaveFileDataList = SaveSystem.InitAllSavedData();
        SetWindowActive(SaveGameWindow);
        Debug.Log("Pressed SaveGame Button!");
    }

    public void LoadGame()
    {
        GameData.SaveFileDataList = SaveSystem.InitAllSavedData();
        SetWindowActive(LoadGameWindow);
        Debug.Log("Pressed LoadGame Button");
    }

    public void MainMenu()
    {
        InGameUI.Instance.UnpauseGame();
        SceneManager.LoadScene("MainMenu");
        MainMenuUI.s_IsActive = true;
    }

    public void QuitGame()
    {
        SetWindowActive(QuitGameWindow);
    }
    #endregion
    #region SaveGame window button functions

    public void CreateNewSaveFile()
    {
        CreateNewFileWindow.SetActive(true);
    }
    public void CloseSaveGameWindow()
    {
        SetWindowInactive(SaveGameWindow);
    }
    public void ConfirmNewSaveFile()//TODO: fix bug where new saved file does not show when refreshed
    {
        if (GameData.SaveFileDataList.Count > 5)
        {
            Debug.LogWarning("SavedSlots is full delete/overwrite an existing slot");
            return;
        }
        SaveSystem.SaveLevelData(FileNameInput.text);
        CancelNewSaveFile();
        Debug.Log($"Saved data as {FileNameInput.text}.save");
        SavedSlotUI.RefreshSaveSlots();
    }
    public void CancelNewSaveFile()
    {
        FileNameInput.text = "";
        CreateNewFileWindow.SetActive(false);
    }
    public void ConfirmOverwriteFile()
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        SaveSystem.SaveLevelData(OverwriteNameInput.text);
        CloseConfirmOverwriteWindow();
        Debug.Log($"{SavedSlotUI.FileNameToBeDeleted} was overwritten by {OverwriteNameInput.text}");
        SavedSlotUI.RefreshSaveSlots();
    }
    public void CloseConfirmOverwriteWindow()
    {
        OverwriteNameInput.text = "";
        ConfirmOverwriteWindow.SetActive(false);
    }
    #endregion
    #region LoadGame window button functions
    // public void LoadSavedFile(int slotnumber)
    // {
        // string SlotFileName = GameData.Instance.SavedStateFileNames[slotnumber];
        // SaveFileData LoadedData =  SaveSystem.LoadLevelData(SlotFileName);
        // Debug.Assert(LoadedData != null, "Load FileData is null");
        // PlayerLevelData.Instance.LoadLevel(LoadedData);
        // // Should be Level Reload
        // CloseLoadGameWindow();
        // Resume();
        // Debug.Log("Loaded data from test.sav");
    // }
    public void CloseLoadGameWindow()
    {
        SetWindowInactive(LoadGameWindow);
    }
    
    #endregion
    #region QuitGame window button functions
    public void QuitWindowYes()
    {
        SaveSystem.SaveGameData();
        Debug.Log("Quit A-Way Home Game!");
        Application.Quit();
    }
    public void QuitWindowNo()
    {
        SetWindowInactive(QuitGameWindow);
    }
    #endregion

    public void ConfirmDeleteSlot(string windoworigin)
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        CloseDeleteWindow(windoworigin);
        SavedSlotUI.RefreshSaveSlots();
    }

    public void CloseDeleteWindow(string windoworigin)
    {
        switch(windoworigin)
        {
            case "ingame-save":
                DeleteConfirmSaveWindow.SetActive(false);
                break;
            case "ingame-load":
                DeleteConfirmLoadWindow.SetActive(false);
                break;
        }
    }

    private void SetWindowActive(GameObject window)
    {
        Debug.Assert(isActive, "Error: OptionsUI is not active!");
        window.SetActive(true);
        isActive = false;
    }
    private void SetWindowInactive(GameObject window)
    {
        Debug.Assert(!isActive, "Error: OptionsUI is active!");
        window.SetActive(false);
        isActive = true;
    }
}
