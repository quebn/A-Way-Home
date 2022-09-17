using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CharacterType { None , Character1, Character2, Character3 }
public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance {get; private set;}
    [SerializeField] private GameObject CharacterSelectionWindow;
    [SerializeField] private GameObject LoadSelectionWindow;
    [SerializeField] private GameObject LevelSelectionwindow;
    [SerializeField] private GameObject SettingsWindow;
    [SerializeField] private GameObject CloseGameWindow;
    [SerializeField] private GameObject LeaderboardsWindow;
    [SerializeField] private GameObject HowtoPlayWindow;

    public GameObject DeleteConfirmWindow;

    public static bool s_IsActive = true;
    public static CharacterType s_SelectedCharacter;
    
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: MainMenuUI instance is null");
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
        GameData.SaveFileDataList = new List<SaveFileData>(5);
        Debug.Log(GameData.SaveFileDataList.Count);
        s_SelectedCharacter = CharacterType.None;
    }

    #region MainMenu button functions
    public void PlayGame()
    {
        SetWindowActive(CharacterSelectionWindow);
    }

    public void LoadGame()
    {
        GameData.SaveFileDataList =  SaveSystem.InitAllSavedData();
        SetWindowActive(LoadSelectionWindow);
        Debug.Log("Pressed LoadGame Button");
    }

    public void SelectLevel()
    {
        SetWindowActive(LevelSelectionwindow);
        Debug.Log("Pressed SelectLevel Button");
    }
    
    public void Settings()
    {
        SetWindowActive(SettingsWindow);
        Debug.Log("Pressed Settings Button");
    }

    public void HowtoPlay()
    {
        SetWindowActive(HowtoPlayWindow);
        Debug.Log("Pressed HowToPlay Button");
    }

    public void Leaderboards()
    {
        SetWindowActive(LeaderboardsWindow);
        Debug.Log("Pressed Leaderboards Button");
    }

    public void CloseGame()
    {
        SetWindowActive(CloseGameWindow);
        Debug.Log("Pressed CloseGame Button");
    }
    #endregion
    #region Character selection window functions
    public void SelectCharButton(int charactertype)
    {
        Debug.Assert(charactertype <= 3, "Error: Character number exceeded at 3!");
        s_SelectedCharacter = (CharacterType)charactertype;
        Debug.Log("Character Selected: " + s_SelectedCharacter);
    }
    public void CloseCharWindow()
    {
        s_SelectedCharacter = CharacterType.None;
        SetWindowInactive(CharacterSelectionWindow);
    }

    public void StartGame()
    {
        if (s_SelectedCharacter == CharacterType.None)
        {
            Debug.Log("No Character Selected");
            return;
        }
        int CharacterIndex = (int)s_SelectedCharacter - 1;
        NewLevel(GameData.Instance.CurrentCharacterLevel[CharacterIndex]);
    }

    #endregion
    #region Load selection window button functions
    public void ConfirmDeleteSlot()
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        CancelDeleteSlot();
        SavedSlotUI.RefreshSaveSlots();
        Debug.Log("Saved Slot deleted!");
    }
    public void CancelDeleteSlot()
    {
        Debug.Assert(DeleteConfirmWindow != null, "Error: Load slot confrimation window not found!");
        DeleteConfirmWindow.SetActive(false);
    }
    public void CloseLoadWindow()
    {
        SetWindowInactive(LoadSelectionWindow);
    }
    #endregion
    #region Level selection window button functions
    public void NewLevel(string scenelevelname)
    {
        // Debug.Assert(GameData.Instance.UnlockLevels.Contains(scenelevelname), "Error: Scene does not exist");
        GameData.LoadType = LevelLoadType.NewGame;
        if (!GameData.Instance.UnlockLevels.Contains(scenelevelname))
        {
            Debug.Log(scenelevelname + " Does not Exist");
            return;
        }
        SceneManager.LoadScene(scenelevelname);
    }
    public void CloseLevelWindow()
    {
        SetWindowInactive(LevelSelectionwindow);
    }
    #endregion
    #region Settings window button functions

    public void CloseSettingsWindow()
    {
        SetWindowInactive(SettingsWindow);
    }

    #endregion 
    #region Close game window button functions

    public void CloseGameYes()
    {
        SaveSystem.SaveGameData();
        Debug.Log("Application Closed");
        Application.Quit();
    }

    public void CloseGameNo()
    {
        SetWindowInactive(CloseGameWindow);
    }

    #endregion
    #region Leaderboards window button functions
    public void CloseLeaderboardsWindow()
    {
        SetWindowInactive(LeaderboardsWindow);
    }

    #endregion
    #region HowtoPlay window button functions

    public void CloseHowtoPlayWindow()
    {
        SetWindowInactive(HowtoPlayWindow);
    }

    #endregion

    // General window functions
    private void SetWindowInactive(GameObject window)
    {
        Debug.Assert(!s_IsActive, "Main Menu is Active");
        Debug.Assert(window.activeSelf, window + " is not active!");
        s_IsActive = true;
        window.SetActive(false);
    }
    private void SetWindowActive(GameObject Window)
    {
        Debug.Assert(s_IsActive, "Main menu is not Active");
        s_IsActive = false;
        Window.SetActive(true);
    }
    
}
