using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class EndSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject leaderboards;
    [SerializeField] private GameObject confirmQuitGameWindow;
    [SerializeField] private TextMeshProUGUI scoreValueText;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;



    private void Start()
    {
        InitPlayerEndScore();
    }

    private void InitPlayerEndScore()
    {

    }

    public void SaveToLeaderboard()
    {
        Debug.Log("Saved to Leaderboard");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");        
    }

    public void Leaderboards()
    {
        leaderboards.SetActive(true);
    }

    public void CloseLeaderboards()
    {
        leaderboards.SetActive(false);
    }
    public void QuitGame()
    {
        confirmQuitGameWindow.SetActive(true);
    }

    public void ConfirmQuitGame()
    {
        Debug.Log("Game closed.");
        CloseQuitGameWindow();
        Application.Quit();
    }

    public void CloseQuitGameWindow()
    {
        confirmQuitGameWindow.SetActive(false);
    }
}
