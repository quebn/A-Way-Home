using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelLoadType{ NewGame, LoadGame, RestartGame}// <- should not exist

public static class GameEvent
{
    public static LevelLoadType loadType;
    public static int restartCounter;
    public static bool isPaused = false;
    public static bool isEndWindowActive {get {return InGameUI.Instance.getGameEndWindow.activeSelf;}}
    private static PlayerLevelData endData;
    private static string  prefabLevelName;

    public static bool isSceneSandbox {get { return SceneManager.GetActiveScene().name == "Sandbox" ;}}

    public static void InitializeLevel()
    {
        if(isSceneSandbox)
            return;
        GameObject levelPrefab = Resources.Load<GameObject>($"Levels/{prefabLevelName}");
        GameObject.Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
    }

    public static void LoadEndScene()
    {
        SceneManager.LoadScene("EndScene");
    }

    public static string GetNextLevel()
    {
        uint currentChar = MainMenuUI.GetCharacterIndex();
        uint currentLevel = PlayerLevelData.Instance.levelData.level;
        Debug.Assert(currentChar != 0, "ERROR: character index is 0");
        return $"Char{currentChar}Level{currentLevel+1}";
    }

    public static void NextLevel()
    {
        NewGame(GetNextLevel());
    }

    public static void NewGame(string levelName)
    {
        if (!GameData.Instance.unlockLevels.Contains(levelName))
        {
            Debug.Log(levelName + " Does not Exist");
            return;
        }
        restartCounter = 0;
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
        if (PlayerLevelData.Instance.levelData.lives > 1){
            loadType = LevelLoadType.RestartGame;
            restartCounter++;       
            Debug.Log($"GameEvent Restart counter :{restartCounter}");
            SceneManager.LoadScene("LevelScene");
        } else {
            Debug.Log($"You have {PlayerLevelData.Instance.levelData.lives}! you cant restart anymore!");
        }
    }

    public static void LoadGame(int slotNumber)
    {
        if (isPaused)
            UnpauseGame();
        restartCounter = 0;
        loadType = LevelLoadType.LoadGame;
        GameData.loadedLevelData = GameData.saveFileDataList[slotNumber];
        prefabLevelName = $"Char{MainMenuUI.GetCharacterIndex(GameData.loadedLevelData.levelData.characterName)}Level{GameData.loadedLevelData.levelData.level}";
        SceneManager.LoadScene("LevelScene");
    }

    public static void SetEndWindowActive(EndGameType endGameType)
    {
        Debug.Log("Calling..");
        PlayerLevelData.Instance.character.isGoingHome = false;
        if(isSceneSandbox)
            return;
        InGameUI inGameUI = InGameUI.Instance;
        inGameUI.getGameEndWindow.SetActive(true);
        inGameUI.endGameType = endGameType;
    }

    public static void PauseGame()
    {
        Debug.Assert(!isPaused, "Game is Already Paused");
        isPaused = true;
        Time.timeScale = 0f;
    }

    public static void UnpauseGame()
    {
        Debug.Assert(isPaused, "Game is not Paused");
        isPaused = false;
        Time.timeScale = 1f;
    }

}
