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
    public static List<SaveFileData> saveFileDataList ;
    public static SaveFileData loadedLevelData = null;
    public static Dictionary<int,SavedSlotUI> saveSlotUIDict;


    public static void InitGameDataInstance()
    {
        saveSlotUIDict = new Dictionary<int, SavedSlotUI>(5);
        Instance = SaveSystem.LoadGameData();
        if (Instance != null)
        {
            Debug.Log("Initializing previous existing Gamedata");
            Debug.Log($"audio: {Instance.audio}");
            // Debug.Log($"leaderboards data count: {Instance.leaderboards.Count}");
            return;
        }
        Debug.Log("No existing GameData found creating new gamedata");
        Instance = new GameData();

    }

    private GameData()
    {
        Debug.Log("Creating new Gamedata");
        // Settings
        isFullscreen = true;
        hasAnimations = true;
        audio = 100;
        gameSpeed = 20;
        Debug.Log($"Game data settings [isFullscreen:{isFullscreen}| hasAnimations:{hasAnimations}| audio:{audio}| gameSpeed:{gameSpeed}]");
        currentCharacterLevel = new string[3]{"Char1Level1", "Char2Level1", "Char3Level1"};
        unlockLevels = new List<string>{currentCharacterLevel[0], currentCharacterLevel[1], currentCharacterLevel[2]};
        leaderboards = new List<PlayerScoreData>();
    }
}