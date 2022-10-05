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
                SetActiveEndGameUI(levelClearUI, new Color32(0, 219, 43, 255));
                InitLivesUI(PlayerLevelData.Instance.levelData.lives);
                break;
            case EndGameType.NoEnergy:
                SetActiveEndGameUI(zeroEnergysUI, Color.white);
                InitLivesUI(PlayerLevelData.Instance.levelData.lives - 1);
                break;
        }
    }

    private void SetActiveEndGameUI(GameObject ui, Color color)
    {
        windowBackground.color = color;
        ui.SetActive(true);
    }

    private void InitLivesUI(uint life)
    {
        livesUI.SetActive(true);
        livesValueText.text = life.ToString();
    }

    public void NextLevel()
    {
        Debug.Log("Next Level Loaded");
        GameEvent.NextLevel();
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
