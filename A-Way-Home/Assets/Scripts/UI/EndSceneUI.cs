using System;
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
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI message; 
    private bool isSaved;

    private void Start()
    {
        InitPlayerEndScore();
    }

    private void InitPlayerEndScore()
    {
        // PlayerLevelData data = ScoreSystem.playerLevelData;
        Debug.Assert(ScoreSystem.characterName != null);
        characterImage.sprite = ScoreSystem.characterSprite;
        characterName.text = ScoreSystem.characterName;
        scoreValueText.text = ScoreSystem.scoreLevelData.score.ToString();
        isSaved = false;
    }

    public void SaveToLeaderboard()
    {
        if (String.IsNullOrWhiteSpace(playerNameInput.text))
        {
            message.text = "Invalid Name Input!";
            animator.Play("InputErrorMessage");
            return;
        }
        if (isSaved){
            message.text = "Score Already Saved!";
            animator.Play("AlreadySavedMessage");
            return;
        }
        message.text = "Score Saved to Leaderboards!";
        animator.Play("SaveMessage");
        Debug.Log("Saved to Leaderboard");
        ScoreSystem.SaveScoreData(playerNameInput.text);
        playerNameInput.text = "";
        isSaved = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");        
    }

    public void Leaderboards()
    {
        leaderboards.SetActive(true);
        LeaderboardSlotUI.LoadLeaderbordsInfo();
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
