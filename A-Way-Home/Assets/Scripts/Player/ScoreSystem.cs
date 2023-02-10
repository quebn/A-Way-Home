using UnityEngine;
using UnityEngine.UI;
public static class ScoreSystem
{
    private const float RemainingMovesMult = .5f;
    private const float CharEnergyMult = 10f; 
    // public static PlayerLevelData playerLevelData;    
    public static LevelData scoreLevelData;
    public static string characterName;
    public static Sprite characterSprite;


    public static int CalculateScore()
    {
        // Debug.Assert(false, "TODO: Implement Calculate Score");
        int playerScore = GameData.levelData.score;
        int characterScore = Character.instance.GetScore(1);
        int levelScore = PlayerLevelData.Instance.GetScore(1 , 1);
        playerScore += Mathf.RoundToInt((characterScore * CharEnergyMult) * (levelScore * RemainingMovesMult + 1)); 
        Debug.Log($"Calculated Score: {playerScore}");
        if (playerScore < 0)
            Debug.LogError("ERROR: Score was less than zero");
        return playerScore;
    }
    
    public static void InitScoreData()
    {
        PlayerLevelData playerLevelData = PlayerLevelData.Instance;
        characterSprite = Character.instance.image;
        characterName = new string(GameData.levelData.characterName);
        scoreLevelData = GameData.levelData;
    }

    public static void SaveScoreData(string playerName)
    {
        GameData.Instance.leaderboards.Add(new PlayerScoreData(characterName, playerName, scoreLevelData.score));
        SaveSystem.SaveGameData();
        // GameData.Instance.leaderboards.Sort();
    }
}

