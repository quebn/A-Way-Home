using System;

[System.Serializable] 
public class SaveFileData {
    public string fileName;
    public LevelData levelData;
    public string date;
    public string time;

    public SaveFileData(string filename, LevelData leveldata)
    {
        DateTime dateTime = DateTime.Now;
        this.fileName = filename;
        this.levelData = leveldata;
        this.date = dateTime.ToShortDateString();        
        this.time = dateTime.ToShortTimeString();        
    }
}


