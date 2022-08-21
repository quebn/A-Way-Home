using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject CharacterSelectionWindow;
    public GameObject LoadSelectionWindow;
    public GameObject LevelSelectionwindow;
    public GameObject SettingsWindow;
    public GameObject CloseGameWindow;
    public GameObject LeaderboardsWindow;
    public GameObject HowtoPlayWindow;
    public static bool s_IsActive = true;

    private void Start()// Unity Start Function
    {
        Debug.Log("Main Menu Loaded!");
    }

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

    // Main Menu Button Functions
    public void PlayGame()
    {
        SetWindowActive(CharacterSelectionWindow);
    }

    public void LoadGame()
    {
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


    // Character selection window button functions
    //      - character selection buttons to be added.
    // ........
    public void CloseCharWindow()
    {
        SetWindowInactive(CharacterSelectionWindow);
    }


    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Load selection window button functions
    //      -file selection functions to be added.
    // ..........
    public void CloseLoadWindow()
    {
        SetWindowInactive(LoadSelectionWindow);
    }
    
    // Level selection window button functions
    public void CloseLevelWindow()
    {
        SetWindowInactive(LevelSelectionwindow);
    } 

    // Settings window button functions
    public void CloseSettingsWindow()
    {
        SetWindowInactive(SettingsWindow);
    }

    // Close game window button functions
    public void CloseGameYes()
    {
        Debug.Log("Application Closed");
        Application.Quit();
    }

    public void CloseGameNo()
    {
        SetWindowInactive(CloseGameWindow);
    }

    // Leaderboards window button functions
    public void CloseLeaderboardsWindow()
    {
        SetWindowInactive(LeaderboardsWindow);
    }

    // HowtoPlay window button functions
    public void CloseHowtoPlayWindow()
    {
        SetWindowInactive(HowtoPlayWindow);
    }

}
