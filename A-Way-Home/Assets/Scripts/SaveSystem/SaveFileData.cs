using UnityEngine;
using System.Collections.Generic;



[System.Serializable]
public class SaveFileData 
{
    public string FileName;
    public string LevelSceneName;
    public uint PlayerLives;
    public uint PlayerMoves;
    public Dictionary<string, bool> RemovedObstacles;



    public SaveFileData(string filename, PlayerLevelData leveldata)
    {
        this.FileName = filename;
        this.LevelSceneName = leveldata.LevelSceneName;
        this.PlayerLives = leveldata.PlayerLives;
        this.PlayerMoves = leveldata.PlayerMoves;
        this.RemovedObstacles = leveldata.RemovedObstacles; 
    }
}
