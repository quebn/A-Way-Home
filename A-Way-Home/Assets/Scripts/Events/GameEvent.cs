using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelLoadType{ NewGame, LoadGame, RestartGame}// <- should not exist

public static class GameEvent
{
    private static PlayerLevelData endData;
    public static LevelLoadType loadType;
    private static int restartCounter;
    // private static
    public static void LoadEndScene()
    {

        SceneManager.LoadScene("EndScene");
    }
    public static void NextLevel()
    {
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
        loadType = LevelLoadType.RestartGame;
        restartCounter++;       
        SceneManager.LoadScene(PlayerLevelData.Instance.levelSceneName);
    }

    public static void LoadGame(int slotNumber)
    {
        loadType = LevelLoadType.LoadGame;
        restartCounter = 0;
        GameData.loadedLevelData = GameData.saveFileDataList[slotNumber];
        SceneManager.LoadScene(GameData.loadedLevelData.levelSceneName);
    }

}
