using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public static GameData Instance;
    // Settings Data
    // LeaderBoards Data
    public string[] CurrentCharacterLevel;

    public List<string> UnlockLevels;

    public static List<SaveFileData> SaveFileDataList ;
    public static SaveFileData LoadedLevelData = null;
    public static LevelLoadType ?LoadType = null;
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
        CurrentCharacterLevel = new string[3]{"Char1Level1", "Char2Level1", "Char3Level1"};
        UnlockLevels = new List<string>{CurrentCharacterLevel[0], CurrentCharacterLevel[1], CurrentCharacterLevel[2]};
    }
}
