using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] 
public class GameData 
{
    public const int PLAYER_STARTING_LIVES = 3;
    public static GameData Instance;

    // Settings Data
    public bool isFullscreen;
    public bool isMuted;
    public uint master;
    public uint bgm;
    public uint ambience;
    public uint sfx;
    public Dictionary<string, string> keybinds;
    // LeaderBoards Data
    public List<PlayerScoreData> leaderboards;
    public string currentStageLevel;
    public List<string> unlockedLevels;

    // Statics;
    public static LevelData levelData;
    public static string selectedCharacter = "NA";
    public static Dictionary<int, SavedSlotUI> saveSlotUIDict = new Dictionary<int, SavedSlotUI>(5);
    public static List<SaveFileData> savedDataFiles = new List<SaveFileData>(5);
    public static List<string> allLevels = new List<string>(){"Stage1Level1", "Stage2Level1", "Stage3Level1"};

    private GameData()
    {
        Debug.Log("Creating new Gamedata");
        // Settings
        this.isFullscreen = true;
        this.isMuted = false;
        this.master = 100;
        this.bgm = 100;
        this.ambience = 100;
        this.sfx = 100;
        // GameData 
        this.currentStageLevel = "Stage1Level1";
        this.unlockedLevels = new List<string>{currentStageLevel};
        this.leaderboards = new List<PlayerScoreData>();
    }

    public static void InitGameDataInstance()
    {
        // characterSprites = new Dictionary<string, Sprite>(3);
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
    public Dictionary<string, int> valuePairs = new Dictionary<string, int>();
    public SerializedVector3 position;

    public ObstacleData(Type obstacleType, int hp, Vector3 location)
    {
        this.typeName = obstacleType.Name;
        this.valuePairs.Add("hp", hp);
        this.position = location;
    }

    public int GetValue(string key)
    {
        if(!valuePairs.ContainsKey(key))
            throw new Exception("ERROR: Key does not exist in valuePairs");
        return this.valuePairs[key];
    }
}

[System.Serializable]
public class LevelData
{
    // Character Data
    public string characterName;
    //public byte[] characterImage;
    public int characterEnergy;
    public int characterRequiredEssence;
    public SerializedVector3 characterPosition;
    public List<SerializedVector3> conductivePositions;
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
public class CharacterInfo
{
    public string name;
    public GameObject prefab;

}