using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EndGameType { None, GameOver, LevelClear, NoEnergy, TimeRanOut}

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private Image windowBackground;
    [SerializeField] private GameObject endRunConfirmWindow;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI redButtonText;
    [SerializeField] private TextMeshProUGUI greenButtonText;
    [SerializeField] private TextMeshProUGUI livesValueText;
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;

    private void Start()
    {
        InitGameEndUI();
        GameEvent.PauseGame();
        PlayerLevelData.Instance.levelData.score = ScoreSystem.CalculateScore();
    }

    private void OnDisable()
    {
        GameEvent.UnpauseGame();
    }

    private void InitGameEndUI()
    {
        EndGameType type = InGameUI.Instance.endGameType;
        
        switch(type)
        {
            case EndGameType.None:
                Debug.LogError("ERROR: InGame Instance endgametype = None!");
                break;
            case EndGameType.GameOver:
                InitializeContentsUI(
                    color: Color.red,
                    title: "game over",
                    message: "looks like you have no more lives left to try again! proceed to end screen?.",
                    redButtonText: "main menu",
                    greenButtonText: "proceed"
                );
                break;
            case EndGameType.LevelClear:
                InitializeContentsUI(
                    color: new Color32(0, 219, 43, 255),
                    title: "level complete",
                    message: "your character reached its target safetly!",
                    redButtonText: "end run",
                    greenButtonText: "proceed",
                    lifeIncrement: 0
                );
                // UnlockNextLevel();
                break;
            case EndGameType.NoEnergy:
                InitializeContentsUI(
                    color: Color.white, 
                    title: "level failed",
                    message: "your character ran out of energy before reaching to its home!",
                    redButtonText: "end run",
                    greenButtonText: "try again"
                );
                break;
            case EndGameType.TimeRanOut:
                if (PlayerLevelData.Instance.levelData.lives > 1){
                    InitializeContentsUI(
                        color: Color.white,
                        title: "level failed",
                        message: "you ran out of time before solving the level!",
                        redButtonText: "end run",
                        greenButtonText: "try again"
                    );
                    InGameUI.Instance.endGameType = EndGameType.NoEnergy;
                }else{
                    InitializeContentsUI(
                        color: Color.red,
                        title: "game over",
                        message: "you ran out of time before solving the level!",
                        redButtonText: "main menu",
                        greenButtonText: "proceed"
                    );
                    InGameUI.Instance.endGameType = EndGameType.GameOver;
                }
                // Debug.Assert(false, "Unimplemented!");
                break;
        }
    }
    private void InitializeContentsUI(Color color, string title, string message, string redButtonText, string greenButtonText, int lifeIncrement = -1)
    {
        this.windowBackground.color = color;
        this.title.text = title;
        this.message.text = message;
        this.redButtonText.text = redButtonText;
        this.greenButtonText.text = greenButtonText;
        this.livesValueText.text = $"{PlayerLevelData.Instance.levelData.lives + lifeIncrement}";
    }

    private void UnlockNextLevel()
    {
        if (PlayerLevelData.Instance.levelData.level == 5)
        {
            Debug.Log($"Max Level ({PlayerLevelData.Instance.levelData.level}) reached!");
            return;
        }
        string sceneLevelName = GameEvent.GetNextLevel();
        Debug.Log($"Next Level Scene Name: {sceneLevelName}");
        if (!GameData.allLevels.Contains(sceneLevelName))
        {
            Debug.Assert(false, $"Scene: {sceneLevelName} does not exist!");
            return;
        }
        GameData.Instance.unlockLevels.Add(sceneLevelName);
    }


    public void RedButton()
    {
        switch(InGameUI.Instance.endGameType)
        {
            case EndGameType.GameOver:
                OptionsUI.Instance.MainMenu();
                break;
            default:
                EndRun();
                break;
        }
    }

    public void GreenButton()
    {
        Debug.Log("Green Button Pressed!");
        switch(InGameUI.Instance.endGameType)
        {
            case EndGameType.GameOver:
                ConfirmEndRun();
                break;
            case EndGameType.LevelClear:
                NextLevel();
                break;
            case EndGameType.NoEnergy:
                TryAgain();
                break;
        }
    }

    private void NextLevel()
    {
        Debug.Log("Loading Next Level......");
        if(GameData.Instance.unlockLevels.Contains(GameEvent.GetNextLevel()))
            GameEvent.NextLevel();
        else
            ConfirmEndRun();
    }

    private void TryAgain()
    {
        GameEvent.RestartGame();
    }

    private void EndRun()
    {
        Debug.Assert(!endRunConfirmWindow.gameObject.activeSelf, "ERROR: EndRun confirm window is already active!");
        endRunConfirmWindow.SetActive(true);
    } 

    private void CancelEndRun() 
    {
        Debug.Assert(endRunConfirmWindow.gameObject.activeSelf, "ERROR: EndRun confirm window is already active!");
        endRunConfirmWindow.SetActive(false);
    }

    private void ConfirmEndRun()
    {
        ScoreSystem.InitScoreData();
        GameEvent.LoadEndScene();
    }
}
