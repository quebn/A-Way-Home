using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public static class ScoreSystem
{
    private const float RemainingMovesMult = .5f;
    private const float CharEnergyMult = 10f; 
    // public static PlayerLevelData playerLevelData;    
    public static LevelData scoreLevelData;
    public static string characterName;
    public static Image characterImage;


    public static uint CalculateScore()
    {
        // Debug.Assert(false, "TODO: Implement Calculate Score");
        uint score = PlayerLevelData.Instance.levelData.score;
        uint energy = PlayerLevelData.Instance.character.energy;
        uint moves = PlayerLevelData.Instance.levelData.moves;
        score += (uint)Mathf.RoundToInt((energy * CharEnergyMult) * (moves * RemainingMovesMult + 1)); 
        Debug.Log($"Calculated Score: {score}");
        return score;
    }
    
    public static void InitScoreData()
    {
        PlayerLevelData playerLevelData = PlayerLevelData.Instance;
        // characterImage = new Image(playerLevelData.character.image);
        characterName = new string(playerLevelData.character.charName);
        scoreLevelData = PlayerLevelData.Instance.levelData;
    }

    public static void SaveScoreData(string playerName)
    {
        GameData.Instance.leaderboards.Add(new PlayerScoreData(characterName, playerName, scoreLevelData.score));
        SaveSystem.SaveGameData();
        // GameData.Instance.leaderboards.Sort();
    }
}

