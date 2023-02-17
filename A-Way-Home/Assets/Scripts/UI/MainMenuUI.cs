using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance {get; private set;}
    [SerializeField] private List<CharacterInfo> characters;
    // [SerializeField] private Sprite char1Sprite;
    // [SerializeField] private string char1Name;
    // [SerializeField] private Sprite char2Sprite;
    // [SerializeField] private string char2Name;
    // [SerializeField] private Sprite char3Sprite;
    // [SerializeField] private string char3Name;

    [SerializeField] private GameObject characterSelectionWindow;
    [SerializeField] private GameObject loadSelectionWindow;
    [SerializeField] private GameObject levelSelectionwindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject closeGameWindow;
    [SerializeField] private GameObject leaderboardsWindow;
    [SerializeField] private GameObject howtoPlayWindow;

    private bool isActive;
    public GameObject deleteConfirmWindow;

    private void Awake()
    {
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
    }

    private void Start()
    {
        isActive = true;
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: MainMenuUI instance is null");
        Debug.Assert(GameData.Instance != null, "Error: GameData instance is null");
        // Debug.Log(GameData.saveFileDataList.Count);
    }

    #region MainMenu button functions
    public void PlayGame()
    {
        SetWindowActive(characterSelectionWindow);
    }

    public void LoadGame()
    {
        GameData.savedDataFiles =  SaveSystem.FetchAllSavedFileData();
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
    public void SelectCharButton(int characterIndex)
    {
        Debug.Assert(characterIndex <= 3, "Error: Character number exceeded at 3!");
        // PlayerLevelData.characterName = GameData.characterSprites.ElementAt(characterIndex - 1).Key;
        GameData.selectedCharacter = characters[characterIndex];
        Debug.Log($"Character Selected: {GameData.selectedCharacter.name}");
    }

    public void CloseCharWindow()
    {
        GameData.selectedCharacter = new CharacterInfo();
        SetWindowInactive(characterSelectionWindow);
    }

    public void StartGame()
    {
        if (!characters.Contains(GameData.selectedCharacter))
        {
            Debug.Log("No Character Selected");
            return;
        }
        // GameEvent.instance.NewGame(GameData.Instance.currentCharacterLevel[characterIndex - 1]);
        GameEvent.NewGame("Stage1Level1");
    }
    #endregion
    #region Load selection window button functions
    public void ConfirmDeleteSlot()
    {
        SaveSystem.DeleteFileData(SavedSlotUI.FileNameToBeDeleted);
        CancelDeleteSlot();
        SavedSlotUI.UpdateSaveSlots();
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
    
    // public static uint GetStageIndex()
    // {
    //     string name = GameData.levelData.characterName;
    //     if (name == Instance.char1Name)
    //         return 1;
    //     else if (name == Instance.char2Name)
    //         return 2;
    //     else if (name == Instance.char3Name)
    //         return 3;
    //     return 0;
    // }
    
    // public static uint GetStageIndex(string characterName)
    // {
    //     if (characterName == Instance.char1Name)
    //         return 1;
    //     else if (characterName == Instance.char2Name)
    //         return 2;
    //     else if (characterName == Instance.char3Name)
    //         return 3;
    //     return 0;

    // }
}

