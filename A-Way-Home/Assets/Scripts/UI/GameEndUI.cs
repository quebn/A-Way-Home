using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EndGameType { None, GameOver, LevelClear, NoEnergy}

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private Image windowBackground;
    [SerializeField] private GameObject zeroEnergysUI;
    [SerializeField] private GameObject levelClearUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject endRunConfirmWindow;
    [SerializeField] private GameObject livesUI;
    [SerializeField] private TextMeshProUGUI livesValueText;

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
        Debug.Log($"endGameType: {type}");
        switch(type)
        {
            case EndGameType.None:
                Debug.LogError("ERROR: InGame Instance endgametype = None!");
                break;
            case EndGameType.GameOver:
                SetActiveEndGameUI(gameOverUI, Color.red);
                break;
            case EndGameType.LevelClear:
                // UnlockNextLevel();
                SetActiveEndGameUI(levelClearUI, new Color32(0, 219, 43, 255));
                InitLivesUI(PlayerLevelData.Instance.levelData.lives);
                break;
            case EndGameType.NoEnergy:
                SetActiveEndGameUI(zeroEnergysUI, Color.white);
                InitLivesUI(PlayerLevelData.Instance.levelData.lives - 1);
                break;
        }
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

    private void SetActiveEndGameUI(GameObject ui, Color color)
    {
        windowBackground.color = color;
        ui.SetActive(true);
    }

    private void InitLivesUI(int life)
    {
        if (life < 0)
            Debug.LogError("ERROR: Life less than zer0");
        livesUI.SetActive(true);
        livesValueText.text = life.ToString();
    }

    public void NextLevel()
    {
        Debug.Log("Loading Next Level......");
        if(GameData.Instance.unlockLevels.Contains(GameEvent.GetNextLevel()))
            GameEvent.NextLevel();
        ConfirmEndRun();
    }

    public void TryAgain()
    {
        GameEvent.RestartGame();
    }

    public void EndRun()
    {
        Debug.Assert(!endRunConfirmWindow.gameObject.activeSelf, "ERROR: EndRun confirm window is already active!");
        endRunConfirmWindow.SetActive(true);
    } 

    public void CancelEndRun() 
    {
        Debug.Assert(endRunConfirmWindow.gameObject.activeSelf, "ERROR: EndRun confirm window is already active!");
        endRunConfirmWindow.SetActive(false);
    }

    public void ConfirmEndRun()
    {
        ScoreSystem.InitScoreData();
        GameEvent.LoadEndScene();
    }
}
