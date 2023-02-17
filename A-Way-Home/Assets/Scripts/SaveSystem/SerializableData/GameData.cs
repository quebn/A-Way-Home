using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] 
public class GameData {
    public static GameData Instance;

    // Settings Data
    public bool isFullscreen;
    public bool hasAnimations;
    public uint audio;
    public uint gameSpeed;
    public Dictionary<string, string> keybinds;
    // LeaderBoards Data
    public List<PlayerScoreData> leaderboards;
    public string currentStageLevel;
    public List<string> unlockedLevels;


    // Statics;
    public static LevelData levelData;
    public static CharacterInfo selectedCharacter;
    public static Dictionary<int, SavedSlotUI> saveSlotUIDict;
    public static List<SaveFileData> savedDataFiles;
    public static List<string> allLevels = new List<string>(){"Stage1Level1", "Stage2Level1", "Stage3Level1"};

    private GameData()
    {
        Debug.Log("Creating new Gamedata");
        // Settings
        isFullscreen = true;
        hasAnimations = true;
        audio = 100;
        gameSpeed = 5;
        // GameData 
        currentStageLevel = "Stage1Level1";
        unlockedLevels = new List<string>{currentStageLevel};
        leaderboards = new List<PlayerScoreData>();
    }

    public static void InitGameDataInstance()
    {
        // characterSprites = new Dictionary<string, Sprite>(3);
        saveSlotUIDict = new Dictionary<int, SavedSlotUI>(5);
        savedDataFiles = new List<SaveFileData>(5);
        Instance = SaveSystem.LoadGameData();
        if (Instance == null)
            Instance = new GameData();
        Debug.LogWarning($"Current Level: {Instance.currentStageLevel}");
        foreach(string levels in Instance.unlockedLevels)
            Debug.LogWarning($"Unlocked Levels: [{levels}]");
    }

    public static void IncrementPlayerMoves(int increment)
    {
        levelData.moves += increment;
        InGameUI.Instance.playerMovesUI = levelData.moves;
    }



}

[System.Serializable]
public class ObstacleData
{
    public string typeName;
    public int hitpoints;
    public SerializedVector3 position;

    public ObstacleData(Type obstacleType, int hp, Vector3 location)
    {
        this.typeName = obstacleType.Name;
        this.hitpoints = hp;
        this.position = location;
    }
    
}

[System.Serializable]
public class LevelData
{

    // Character Data
    public string characterName;
    public int characterEnergy;
    public int characterRequiredEssence;
    public SerializedVector3 characterPosition;
    // Player Data 
    public uint stage;
    public uint level;
    public int lives;
    public int moves;
    public int score;
    public uint spawnCount;
    public float secondsLeft;
    public Dictionary<string, ObstacleData> obstacles;
    public Dictionary<string, bool> essences;
}

[System.Serializable]
public class CharacterInfo{
    public string name;
    public GameObject prefab;

}