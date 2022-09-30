
[System.Serializable] 
public class SaveFileData {
    public string fileName;
    public LevelData levelData;

    public SaveFileData(string filename, LevelData leveldata)
    {
        this.fileName = filename;
        this.levelData.sceneName = leveldata.sceneName;
        this.levelData.lives = leveldata.lives;
        this.levelData.moves = leveldata.moves;
        this.levelData.score = leveldata.score; 
        this.levelData.removedObstacles = leveldata.removedObstacles;
    }
}


