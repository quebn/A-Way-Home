using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsUI : MonoBehaviour
{
    public InGameUI InGameUI;
    
    [SerializeField]
    private GameObject SaveGameWindow;
    
    [SerializeField]
    private GameObject LoadGameWindow;
    
    [SerializeField]
    private GameObject QuitGameWindow;
    
    public static bool s_IsActive = false;

    private void SetWindowActive(GameObject window)
    {
        Debug.Assert(s_IsActive, "Error: OptionsUI is not active!");
        window.SetActive(true);
        s_IsActive = false;
    }

    private void SetWindowInactive(GameObject window)
    {
        Debug.Assert(!s_IsActive, "Error: OptionsUI is active!");
        window.SetActive(false);
        s_IsActive = true;
    }
    // Options button functions
    public void Resume()
    {
        InGameUI.UnpauseGame();
        InGameUI.OptionUI.SetActive(false);

    }

    public void SaveGame()
    {
        SetWindowActive(SaveGameWindow);
        Debug.Log("Pressed SaveGame Button!");
    }

    public void LoadGame()
    {
        SetWindowActive(LoadGameWindow);
        Debug.Log("Pressed LoadGame Button");
    }

    public void MainMenu()
    {
        InGameUI.UnpauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1 );
        MainMenuUI.s_IsActive = true;
    }

    public void QuitGame()
    {
        SetWindowActive(QuitGameWindow);
    }

    // SaveGame window button functions
    public void CloseSaveGameWindow()
    {
        SetWindowInactive(SaveGameWindow);
    }

    // LoadGame window button functions
    public void CloseLoadGameWindow()
    {
        SetWindowInactive(LoadGameWindow);
    }
    
    // QuitGame window button functions
    public void QuitWindowYes()
    {
        Debug.Log("Quit A-Way Home Game!");
        Application.Quit();
    }
    public void QuitWindowNo()
    {
        SetWindowInactive(QuitGameWindow);
    }

}
