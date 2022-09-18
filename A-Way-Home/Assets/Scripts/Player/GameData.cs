using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public static GameData Instance;
    // Settings Data
    // LeaderBoards Data
    public string[] currentCharacterLevel;

    public List<string> unlockLevels;

    public static List<SaveFileData> saveFileDataList ;
    public static SaveFileData loadedLevelData = null;
    public static LevelLoadType? loadType = null;
    // public static string SelectedFile;
    

    public static void InitGameDataInstance()
    {
        Instance = SaveSystem.LoadGameData();
        if (Instance != null)
        {
            Debug.Log("Initializing previous existing Gamedata");
            return;
        }
        Debug.Log("No existing GameData found creating new gamedata");
        Instance = new GameData();
    }

    // TODO: add a function to be saved in a file for game data persistence
    private GameData()
    {
        Debug.Log("Creating new Gamedata");
        currentCharacterLevel = new string[3]{"Char1Level1", "Char2Level1", "Char3Level1"};
        Debug.Log(currentCharacterLevel[0] + " is instantiated in currentCharacterLevel array");
        Debug.Log(currentCharacterLevel[1] + " is instantiated in currentCharacterLevel array");
        Debug.Log(currentCharacterLevel[2] + " is instantiated in currentCharacterLevel array");
        unlockLevels = new List<string>{currentCharacterLevel[0], currentCharacterLevel[1], currentCharacterLevel[2]};
    }
}
