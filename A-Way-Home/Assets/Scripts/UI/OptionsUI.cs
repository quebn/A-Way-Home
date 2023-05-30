using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] public TMP_InputField overwriteNameInput;
    [SerializeField] private GameObject SaveFileErrorMsg;

    public GameObject deleteConfirmSaveWindow;
    public GameObject deleteConfirmLoadWindow;
    public GameObject confirmOverwriteWindow;

    private bool isActive => this.gameObject.activeSelf;

    private void Start()
    {
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
        SetWindowActive(saveGameWindow);
        SavedSlotUI.LoadAllSaveSlotsUI();
        Debug.Log("Pressed SaveGame Button!");
    }

    public void LoadGame()
    {
        SetWindowActive(loadGameWindow);
        SavedSlotUI.LoadAllSaveSlotsUI();
        Debug.Log("Pressed LoadGame Button");
    }

    public void MainMenu()
    {
        GameEvent.UnpauseGame();
        SceneManager.LoadScene("MainMenu");
        if(PlayerActions.Instance != null)
            PlayerActions.Instance.SetCurrentTool(0);

    }

    public void QuitGame()
    {
        SetWindowActive(quitGameWindow);
    }
    #endregion
    #region SaveGame window button functions

    public void DisplayError()
    {
        if(SaveFileErrorMsg == null)
            return;
        Debug.LogWarning("SaveSlots Full");
        StartCoroutine(Error());
    }

    private IEnumerator Error()
    {
        SaveFileErrorMsg.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        SaveFileErrorMsg.SetActive(false);
    }

    public void CreateNewSaveFile()
    {
        int count = SaveSystem.FetchAllSavedFileData().Count; 
        if(count >= 5)
        {
            DisplayError();
            return;
        }
        createNewFileWindow.SetActive(true);
        fileNameInput.text = $"SaveFile{count+1}";
    }

    public void CloseSaveGameWindow()
    {
        SetWindowInactive(saveGameWindow);
    }

    public void ConfirmNewSaveFile()
    {
        if (SaveSystem.FetchAllSavedFileData().Count == 5)
        {
            Debug.LogWarning("SavedSlots is full delete/overwrite an existing slot");
            return;
        }
        SaveSystem.SaveLevelData(fileNameInput.text, PlayerLevelData.Instance);
        CloseNewSaveFile();
        Debug.Log($"Saved data as {fileNameInput.text}.save");
        SavedSlotUI.LoadAllSaveSlotsUI();
    }

    public void CloseNewSaveFile()
    {
        fileNameInput.text = "";
        createNewFileWindow.SetActive(false);
    }

    public void ConfirmOverwriteFile()
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        SaveSystem.SaveLevelData(overwriteNameInput.text, PlayerLevelData.Instance);
        CloseConfirmOverwriteWindow();
        Debug.Log($"{SavedSlotUI.FileNameToBeDeleted} was overwritten by {overwriteNameInput.text}");
        SavedSlotUI.LoadAllSaveSlotsUI();
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
        SavedSlotUI.LoadAllSaveSlotsUI();
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
    }

    private void SetWindowInactive(GameObject window)
    {
        Debug.Assert(isActive, "Error: OptionsUI is active!");
        window.SetActive(false);
    }
    #endregion
}
