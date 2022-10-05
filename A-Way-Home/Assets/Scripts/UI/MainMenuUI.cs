using System.Collections.Generic;
using UnityEngine;

public enum CharacterType { None , Character1, Character2, Character3 }
public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance {get; private set;}
    [SerializeField] private GameObject characterSelectionWindow;
    [SerializeField] private GameObject loadSelectionWindow;
    [SerializeField] private GameObject levelSelectionwindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject closeGameWindow;
    [SerializeField] private GameObject leaderboardsWindow;
    [SerializeField] private GameObject howtoPlayWindow;

    private bool isActive;
    public GameObject deleteConfirmWindow;
    public CharacterType selectedCharacter;
    

    private void Start()
    {
        isActive = true;
        selectedCharacter = CharacterType.None;
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: MainMenuUI instance is null");
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
        Debug.Assert(GameData.Instance != null, "Error: GameData instance is null");
        GameData.saveFileDataList = new List<SaveFileData>(5);
        // Debug.Log(GameData.saveFileDataList.Count);
    }

    #region MainMenu button functions
    public void PlayGame()
    {
        SetWindowActive(characterSelectionWindow);
    }

    public void LoadGame()
    {
        GameData.saveFileDataList =  SaveSystem.InitAllSavedData();
        SetWindowActive(loadSelectionWindow);
        Debug.Log("Pressed LoadGame Button");
    }

    public void SelectLevel()
    {
        SetWindowActive(levelSelectionwindow);
        Debug.Log("Pressed SelectLevel Button");
    }
    
    public void Settings()
    {
        SetWindowActive(settingsWindow);
        Debug.Log("Pressed Settings Button");
    }

    public void HowtoPlay()
    {
        SetWindowActive(howtoPlayWindow);
        Debug.Log("Pressed HowToPlay Button");
    }

    public void Leaderboards()
    {
        SetWindowActive(leaderboardsWindow);
        Debug.Log("Pressed Leaderboards Button");
    }

    public void CloseGame()
    {
        SetWindowActive(closeGameWindow);
        Debug.Log("Pressed CloseGame Button");
    }
    #endregion
    #region Character selection window functions
    public void SelectCharButton(int charactertype)
    {
        Debug.Assert(charactertype <= 3, "Error: Character number exceeded at 3!");
        selectedCharacter = (CharacterType)charactertype;
        Debug.Log("Character Selected: " + selectedCharacter);
    }
    public void CloseCharWindow()
    {
        selectedCharacter = CharacterType.None;
        SetWindowInactive(characterSelectionWindow);
    }

    public void StartGame()
    {
        if (selectedCharacter == CharacterType.None)
        {
            Debug.Log("No Character Selected");
            return;
        }
        int characterIndex = (int)this.selectedCharacter;
        // GameEvent.instance.NewGame(GameData.Instance.currentCharacterLevel[characterIndex - 1]);
        GameEvent.NewGame(GameData.Instance.currentCharacterLevel[characterIndex - 1]);
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
        Debug.Assert(deleteConfirmWindow != null, "Error: Load slot confrimation window not found!");
        deleteConfirmWindow.SetActive(false);
    }
    public void CloseLoadWindow()
    {
        SetWindowInactive(loadSelectionWindow);
    }
    #endregion
    #region Level selection window button functions
    public void NewLevel(string scenelevelname)
    {
        // GameEvent.instance.NewGame(scenelevelname);
        GameEvent.NewGame(scenelevelname);
        // Debug.Assert(GameData.Instance.unlockLevels.Contains(scenelevelname), "Error: Scene does not exist");
        // GameData.loadType = LevelLoadType.NewGame;
        // if (!GameData.Instance.unlockLevels.Contains(scenelevelname))
        // {
        //     Debug.Log(scenelevelname + " Does not Exist");
        //     return;
        // }
        // SceneManager.LoadScene(scenelevelname);
    }
    public void CloseLevelWindow()
    {
        SetWindowInactive(levelSelectionwindow);
    }
    #endregion
    #region Settings window button functions

    public void CloseSettingsWindow()
    {
        SetWindowInactive(settingsWindow);
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
        SetWindowInactive(closeGameWindow);
    }

    #endregion
    #region Leaderboards window button functions
    public void CloseLeaderboardsWindow()
    {
        SetWindowInactive(leaderboardsWindow);
    }

    #endregion
    #region HowtoPlay window button functions

    public void CloseHowtoPlayWindow()
    {
        SetWindowInactive(howtoPlayWindow);
    }

    #endregion

    // General window functions
    private void SetWindowInactive(GameObject window)
    {
        Debug.Assert(!isActive, "Main Menu is Active");
        Debug.Assert(window.activeSelf, window + " is not active!");
        isActive = true;
        window.SetActive(false);
    }
    private void SetWindowActive(GameObject window)
    {
        Debug.Assert(isActive, "Main menu is not Active");
        isActive = false;
        window.SetActive(true);
    }
    
}
