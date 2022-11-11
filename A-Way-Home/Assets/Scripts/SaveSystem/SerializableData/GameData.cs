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
    public static Dictionary<string, Sprite> characterSprites;
    public static SaveFileData loadedLevelData = null;
    public static Dictionary<int,SavedSlotUI> saveSlotUIDict;

    public static void InitGameDataInstance()
    {
        characterSprites = new Dictionary<string, Sprite>();
        saveSlotUIDict = new Dictionary<int, SavedSlotUI>(5);
        Instance = SaveSystem.LoadGameData();
        if (Instance != null)
            return;
        Instance = new GameData();
    }

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
}