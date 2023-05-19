using UnityEngine;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance {get; private set;}
    [SerializeField] private List<string> characters;

    [SerializeField] private GameObject characterSelectionWindow;
    [SerializeField] private GameObject loadSelectionWindow;
    [SerializeField] private GameObject levelSelectionwindow;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject closeGameWindow;
    [SerializeField] private GameObject leaderboardsWindow;
    [SerializeField] private GameObject howtoPlayWindow;
    [SerializeField] private GameObject characterLevelSelectionWindow;
    [SerializeField] private bool unlockedAllLevels;

    // private bool isActive;
    public GameObject deleteConfirmWindow;
    public static bool isAllLevelUnlock;

    private void Awake()
    {
        if (GameData.Instance == null)
            GameData.InitGameDataInstance();
        isAllLevelUnlock = this.unlockedAllLevels;
    }

    private void Start()
    {
        // isActive = true;
        if (Instance == null)
            Instance = this;
        Debug.Assert(Instance != null, "Error: MainMenuUI instance is null");
        Debug.Assert(GameData.Instance != null, "Error: GameData instance is null");
        GameData.selectedCharacter = "NA";
        // Debug.Log(GameData.saveFileDataList.Count);
    }

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

    public void SelectCharButton(int characterIndex)
    {
        Debug.Assert(characterIndex <= 3, "Error: Character number exceeded at 3!");
        // PlayerLevelData.characterName = GameData.characterSprites.ElementAt(characterIndex - 1).Key;
        GameData.selectedCharacter = characters[characterIndex];
        Debug.Log($"Character Selected: {GameData.selectedCharacter}");
    }

    public void CloseCharWindow()
    {
        GameData.selectedCharacter = "NA";
        SetWindowInactive(characterSelectionWindow);
    }

    public void CloseLevelCharWindow()
    {
        GameData.selectedCharacter = "NA";
        SetWindowInactive(characterLevelSelectionWindow);
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

    public void CloseLevelWindow()
    {
        SetWindowInactive(levelSelectionwindow);
    }

    public void StartSelectedLevel()
    {
        if (!characters.Contains(GameData.selectedCharacter))
        {
            Debug.Log("No Character Selected");
            return;
        }
        GameEvent.NewGame(LevelSelectButtonUI.selectedStageLevel);
    }

    public void CloseSettingsWindow()
    {
        SetWindowInactive(settingsWindow);
    }

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

    public void CloseLeaderboardsWindow()
    {
        SetWindowInactive(leaderboardsWindow);
    }

    public void CloseHowtoPlayWindow()
    {
        SetWindowInactive(howtoPlayWindow);
    }

    // General window functions
    private void SetWindowInactive(GameObject window)
    {
        // Debug.Assert(!isActive, "Main Menu is Active");
        Debug.Assert(window.activeSelf, window + " is not active!");
        // isActive = true;
        window.SetActive(false);
    }

    private void SetWindowActive(GameObject window)
    {
        // Debug.Assert(isActive, "Main menu is not Active");
        // isActive = false;
        window.SetActive(true);
    }
}

