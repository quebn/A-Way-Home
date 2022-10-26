using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelLoadType{ NewGame, LoadGame, RestartGame, Sandbox}// <- should not exist

public static class GameEvent
{
    private static PlayerLevelData endData;
    public static LevelLoadType loadType;
    public static uint restartCounter;
    public static bool isPaused = false;
    
    // private static
    public static void LoadEndScene()
    {
        SceneManager.LoadScene("EndScene");
    }
    public static void NextLevel()
    {
        Debug.Log("No other level found Redirecting to main menu!");
        SceneManager.LoadScene("MainMenu");
        // string nextScene = "";// <- TODO: this should contain the name of the next level scene of the character
        // SceneManager.LoadScene(nextScene);

    }
    public static void NewGame(string sceneLevelName)
    {
        loadType = LevelLoadType.NewGame;
        restartCounter = 0;
        if (!GameData.Instance.unlockLevels.Contains(sceneLevelName))
        {
            Debug.Log(sceneLevelName + " Does not Exist");
            return;
        }
        SceneManager.LoadScene(sceneLevelName);
    }

    public static void RestartGame()
    {
        if (loadType == LevelLoadType.Sandbox)
        {
            SceneManager.LoadScene(PlayerLevelData.Instance.levelData.sceneName);
            return;
        }
        if (PlayerLevelData.Instance.levelData.lives > 1)
        {
            loadType = LevelLoadType.RestartGame;
            restartCounter++;       
            Debug.Log($"GameEvent Restart counter :{restartCounter}");
            SceneManager.LoadScene(PlayerLevelData.Instance.levelData.sceneName);
        } else {
            Debug.Log($"You have {PlayerLevelData.Instance.levelData.lives}! you cant restart anymore!");
        }
    }

    public static void LoadGame(int slotNumber)
    {
        if (isPaused)
            UnpauseGame();
        loadType = LevelLoadType.LoadGame;
        restartCounter = 0;
        GameData.loadedLevelData = GameData.saveFileDataList[slotNumber];
        SceneManager.LoadScene(GameData.loadedLevelData.levelData.sceneName);
    }

    public static void SetEndWindowActive(EndGameType endGameType)
    {
        if(loadType == LevelLoadType.Sandbox)
            return;
        InGameUI inGameUI = InGameUI.Instance;
        Debug.Assert(inGameUI != false, $"ERROR:{inGameUI.gameObject.name} instance is null");
        Debug.Assert(inGameUI.getGameEndWindow != false, $"ERROR: Game End window is null/not found!");
        inGameUI.getGameEndWindow.SetActive(true);
        inGameUI.endGameType = endGameType;
    }
    // public static void On
    public static void PauseGame()//TODO: should be in GameEvent.cs
    {
        Debug.Assert(!isPaused, "Game is Already Paused");
        isPaused = true;
        Time.timeScale = 0f;
    }

    public static void UnpauseGame()//TODO: should be in GameEvent.cs
    {
        Debug.Assert(isPaused, "Game is not Paused");
        isPaused = false;
        Time.timeScale = 1f;
    }
}
