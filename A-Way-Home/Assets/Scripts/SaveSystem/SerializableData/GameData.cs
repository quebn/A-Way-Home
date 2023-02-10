using System.Collections.Generic;
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
    public string[] currentCharacterLevel;
    public List<string> unlockLevels;


    // Statics;
    public static LevelData levelData;
    public static SaveFileData loadedLevelData = null;
    public static Dictionary<string, Sprite> characterSprites;
    public static Dictionary<int, SavedSlotUI> saveSlotUIDict;
    public static List<SaveFileData> savedDataFiles;
    public static List<string> allLevels = new List<string>(){"Char1Level1", "Char2Level1", "Char3Level1"};

    private GameData()
    {
        Debug.Log("Creating new Gamedata");
        // Settings
        isFullscreen = true;
        hasAnimations = true;
        audio = 100;
        gameSpeed = 5;
        // GameData 
        currentCharacterLevel = new string[3]{"Char1Level1", "Char2Level1", "Char3Level1"};
        unlockLevels = new List<string>{currentCharacterLevel[0], currentCharacterLevel[1], currentCharacterLevel[2]};
        leaderboards = new List<PlayerScoreData>();
    }

    public static void InitGameDataInstance()
    {
        characterSprites = new Dictionary<string, Sprite>(3);
        saveSlotUIDict = new Dictionary<int, SavedSlotUI>(5);
        savedDataFiles = new List<SaveFileData>(5);
        Instance = SaveSystem.LoadGameData();
        if (Instance == null)
            Instance = new GameData();
    }

    public static void IncrementPlayerMoves(int increment)
    {
        levelData.moves += increment;
        InGameUI.Instance.playerMovesUI = levelData.moves;
    }
}

[System.Serializable]
public class SpawnedData
{
    public string typeName;
    public int hitpoints;
    public SerializedVector3 spawnedLocation;

    public SpawnedData(string obstacleType, int hp, Vector3 location)
    {
        typeName = obstacleType;
        hitpoints = hp;
        spawnedLocation = location;
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
    public uint level;
    public int lives;
    public int moves;
    public int score;
    public float secondsLeft;
    public Dictionary<string, int> obstacles;
    public Dictionary<string, SpawnedData> spawneds;
    public Dictionary<string, bool> essences;
}
