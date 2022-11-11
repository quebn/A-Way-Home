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
        int score = PlayerLevelData.Instance.levelData.score;
        int energy = PlayerLevelData.Instance.character.energy;
        int moves = PlayerLevelData.Instance.levelData.moves;
        score += Mathf.RoundToInt((energy * CharEnergyMult) * (moves * RemainingMovesMult + 1)); 
        Debug.Log($"Calculated Score: {score}");
        if (score < 0)
            Debug.LogError("ERROR: Score was less than zero");
        return score;
    }
    
    public static void InitScoreData()
    {
        PlayerLevelData playerLevelData = PlayerLevelData.Instance;
        characterSprite = playerLevelData.character.characterImage.sprite;
        characterName = new string(playerLevelData.levelData.characterName);
        scoreLevelData = PlayerLevelData.Instance.levelData;
    }

    public static void SaveScoreData(string playerName)
    {
        GameData.Instance.leaderboards.Add(new PlayerScoreData(characterName, playerName, scoreLevelData.score));
        SaveSystem.SaveGameData();
        // GameData.Instance.leaderboards.Sort();
    }
}

