using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance {get; private set;}
    [SerializeField] private GameObject saveGameWindow;
    [SerializeField] private GameObject createNewFileWindow;
    [SerializeField] private GameObject loadGameWindow;
    [SerializeField] private GameObject quitGameWindow;
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private TMP_InputField overwriteNameInput;
    // Confirm delete windows
    public GameObject deleteConfirmSaveWindow;
    public GameObject deleteConfirmLoadWindow;
    public GameObject confirmOverwriteWindow;

    // public InGameUI InGameUI;
    private bool isActive;

    private void Start()
    {
        isActive = true;
        Debug.Assert(confirmOverwriteWindow != null, $"{confirmOverwriteWindow} is null");
        Debug.Assert(overwriteNameInput != null, $"{overwriteNameInput} is null");
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: OptionsUI instance is null");
    }

    #region Pause button functions
    public void Resume()
    {
        GameEvent.UnpauseGame();
        this.gameObject.SetActive(false);

    }

    public void SaveGame()
    {
        // List<SaveFileData> list = SaveSystem.InitAllSavedData();
        GameData.saveFileDataList = SaveSystem.InitAllSavedData();
        SetWindowActive(saveGameWindow);
        Debug.Log("Pressed SaveGame Button!");
    }

    public void LoadGame()
    {
        GameData.saveFileDataList = SaveSystem.InitAllSavedData();
        SetWindowActive(loadGameWindow);
        Debug.Log("Pressed LoadGame Button");
    }

    public void MainMenu()
    {
        GameEvent.UnpauseGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SetWindowActive(quitGameWindow);
    }
    #endregion
    #region SaveGame window button functions

    public void CreateNewSaveFile()
    {
        createNewFileWindow.SetActive(true);
    }
    public void CloseSaveGameWindow()
    {
        SetWindowInactive(saveGameWindow);
    }
    public void ConfirmNewSaveFile()//TODO: fix bug where new saved file does not show when refreshed
    {
        if (GameData.saveFileDataList.Count > 5)
        {
            Debug.LogWarning("SavedSlots is full delete/overwrite an existing slot");
            return;
        }
        SaveSystem.SaveLevelData(fileNameInput.text);
        CancelNewSaveFile();
        Debug.Log($"Saved data as {fileNameInput.text}.save");
        SavedSlotUI.RefreshSaveSlots();
    }
    public void CancelNewSaveFile()
    {
        fileNameInput.text = "";
        createNewFileWindow.SetActive(false);
    }
    public void ConfirmOverwriteFile()
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        SaveSystem.SaveLevelData(overwriteNameInput.text);
        CloseConfirmOverwriteWindow();
        Debug.Log($"{SavedSlotUI.FileNameToBeDeleted} was overwritten by {overwriteNameInput.text}");
        SavedSlotUI.RefreshSaveSlots();
    }
    public void CloseConfirmOverwriteWindow()
    {
        overwriteNameInput.text = "";
        confirmOverwriteWindow.SetActive(false);
    }
    #endregion
    #region LoadGame window button functions
    public void CloseLoadGameWindow()
    {
        SetWindowInactive(loadGameWindow);
    }
    
    #endregion
    #region QuitGame window button functions
    public void QuitWindowYes()
    {
        SetWindowInactive(quitGameWindow);
        SaveSystem.SaveGameData();
        Debug.Log("Quit A-Way Home Game!");
        Application.Quit();
    }
    public void QuitWindowNo()
    {
        SetWindowInactive(quitGameWindow);
    }
    #endregion
    #region Class none button Functions
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
                deleteConfirmSaveWindow.SetActive(false);
                break;
            case "ingame-load":
                deleteConfirmLoadWindow.SetActive(false);
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
    #endregion
}
