using UnityEngine;
using System.Collections.Generic;



[System.Serializable]
public class SaveFileData 
{
    public string fileName;
    public string levelSceneName;
    public uint playerLives;
    public uint playerMoves;
    public Dictionary<string, bool> removedObstacles;



    public SaveFileData(string filename, PlayerLevelData leveldata)
    {
        this.fileName = filename;
        this.levelSceneName = leveldata.levelSceneName;
        this.playerLives = leveldata.playerLives;
        this.playerMoves = leveldata.playerMoves;
        this.removedObstacles = leveldata.removedObstacles; 
    }
}
