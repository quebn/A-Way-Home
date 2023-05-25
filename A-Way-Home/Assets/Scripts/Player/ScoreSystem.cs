using UnityEngine;
using UnityEngine.UI;
public static class ScoreSystem
{
    private const float RemainingMovesMult = 5f;
    private const float CharEnergyMult = 10f; 
    // public static PlayerLevelData playerLevelData;    
    public static LevelData scoreLevelData;
    public static string characterName;
    public static Sprite characterSprite;


    public static void CalculateScore()
    {
        // Debug.Assert(false, "TODO: Implement Calculate Score");
        // int playerScore = ;
        float characterScore = Character.instance.GetScore(CharEnergyMult);
        Debug.LogWarning($"Char Energy: {characterScore} / {CharEnergyMult} -> {characterScore / CharEnergyMult}");
        float levelScore = PlayerLevelData.Instance.GetScore(RemainingMovesMult , 2);
        Debug.LogWarning($"level Score: {levelScore} <- {GameData.levelData.moves * RemainingMovesMult} + {GameData.levelData.lives * 2}");
        Debug.LogWarning($"Prev Calculated Score: {GameData.levelData.score}");
        GameData.levelData.score += Mathf.RoundToInt(characterScore + levelScore); 
        Debug.LogWarning($"New Calculated Score: {GameData.levelData.score}");
        if (GameData.levelData.score < 0)
            Debug.LogError("ERROR: Score was less than zero");
    }
    
    public static void InitScoreData()
    {
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

