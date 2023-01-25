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
}