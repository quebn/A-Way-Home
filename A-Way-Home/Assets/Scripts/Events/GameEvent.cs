using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public enum LevelLoadType{ NewGame, LoadGame, RestartGame, ContinueGame}// <- should not exist

public static class GameEvent
{
    public static LevelLoadType loadType;
    public static bool isPaused = false;
    public static bool isEndWindowActive {get {return InGameUI.Instance.getGameEndWindow.activeSelf;}}
    private static PlayerLevelData endData;
    private static string  prefabLevelName;

    public static bool isSceneSandbox {get { return SceneManager.GetActiveScene().name == "Sandbox" ;}}
    

    public static void InitializeLevel()
    {
        if(isSceneSandbox)
        {
            GameData.selectedCharacter = "SBChar";
            return;
        }
        // {
            // GameData.selectedCharacter = new CharacterInfo(){name = "SandboxCharacter"};
        // }
        if(isPaused)
            UnpauseGame();
        GameObject levelPrefab = Resources.Load<GameObject>($"Levels/{prefabLevelName}");
        // PlayerActions.onPlayerActions = new List<IOnPlayerAction>();
        // PlayerActions.actionWaitProcesses = new HashSet<IActionWaitProcess>();
        Debug.Assert(levelPrefab != null, $"ERROR: {prefabLevelName} is null/not found!");
        GameObject.Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
    }

    public static void LoadEndScene()
    {
        SceneManager.LoadScene("EndScene");
    }

    public static void UnlockNextStageLevel()
    {
        if(isSceneSandbox)
            return;
        string nextStageLevel = PlayerLevelData.Instance.GetNextStageLevel();
        Debug.LogWarning($"Unlocking {nextStageLevel}............");
        if(!GameData.Instance.unlockedLevels.Contains(nextStageLevel))
            GameData.Instance.unlockedLevels.Add(nextStageLevel);
        Debug.Assert(GameData.Instance.unlockedLevels.Contains(nextStageLevel), $"ERROR: {nextStageLevel} not unlocked");
    }

    public static void NextLevel()
    {
        if(isSceneSandbox)
            return;
        string nextStageLevel = PlayerLevelData.Instance.GetNextStageLevel();
        Debug.Log($"Loading Next Level: {nextStageLevel}......");
        Debug.Assert(GameData.Instance.unlockedLevels.Contains(nextStageLevel), $"ERROR: {nextStageLevel} not unlocked");
        prefabLevelName = nextStageLevel;
        loadType = LevelLoadType.ContinueGame;
        SceneManager.LoadScene("LevelScene");
    }

    public static void NewGame(string levelName)
    {
        Debug.Assert(GameData.Instance.unlockedLevels.Contains(levelName) ||  MainMenuUI.isAllLevelUnlock, $"ERROR: {levelName} does not exist!");
        loadType = LevelLoadType.NewGame;
        prefabLevelName = levelName;
        SceneManager.LoadScene("LevelScene");
    }

    public static void RestartGame()
    {
        if (isSceneSandbox){
            SceneManager.LoadScene("Sandbox");
            return;
        }
        if (GameData.levelData.lives > 1){
            loadType = LevelLoadType.RestartGame;
            SceneManager.LoadScene("LevelScene");
        } else {
            Debug.Log($"You have {GameData.levelData.lives}! you cant restart anymore!");
        }
    }

    public static void LoadGame(int index)
    {
        if (isPaused)
            UnpauseGame();
        loadType = LevelLoadType.LoadGame;
        GameData.levelData = GameData.savedDataFiles[index].levelData;
        // GameData.loadedLevelData = GameData.savedDataFiles[index];
        prefabLevelName = $"Stage{GameData.levelData.stage}Level{GameData.levelData.level}";
        SceneManager.LoadScene("LevelScene");
    }

    public static void SetEndWindowActive(EndGameType endGameType)
    {
        PauseGame();
        Debug.Log("Calling..");
        InGameUI inGameUI = InGameUI.Instance;
        inGameUI.getGameEndWindow.SetActive(true);
        inGameUI.endGameType = endGameType;
    }

    public static void PauseGame()
    {
        Debug.Assert(!isPaused, "Game is Already Paused");
        isPaused = !isPaused;
        Time.timeScale = 0f;
    }

    public static void UnpauseGame()
    {
        Debug.Assert(isPaused, "Game is not Paused");
        isPaused = !isPaused;
        Time.timeScale = 1f;
    }

    public static void GoToEndStoryScene()
    {
        ScoreSystem.InitScoreData();
        SceneManager.LoadScene("EndStory");
    }
}
