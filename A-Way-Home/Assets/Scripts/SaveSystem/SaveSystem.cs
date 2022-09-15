using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string FilePathDir = Application.persistentDataPath;
    private static BinaryFormatter Formatter = new BinaryFormatter(); 
    
    // public static Dictionary<string, SaveFileData> SavedStateFileData = new Dictionary<string, SaveFileData>(5); 
    public static void DeleteFileData(string filename)
    {
        string Path = $"{FilePathDir}/SavedFiles/{filename}.save";

        if (!File.Exists(Path))
        {
            Debug.LogError($"{filename}.save does not exist in path: {FilePathDir}/SavedFiles/");
            return;
        }
        File.Delete(Path);
        Debug.Log($"{filename}.save deleted successfully!");
    }

    public static void SaveLevelData(string savefilename)
    {
        string Path = $"{FilePathDir}/SavedFiles/{savefilename}.save";
        
        FileStream Stream = new FileStream(Path, FileMode.Create);

        SaveFileData FileData = new SaveFileData(savefilename, PlayerLevelData.Instance);
        Formatter.Serialize(Stream, FileData);
        Stream.Close();
        SaveGameData();
    }

    public static SaveFileData GetSaveFileData(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Debug.Log($"{filepath} does not exist!");
            return null;
        }
        FileStream Stream = new FileStream(filepath, FileMode.Open);
        SaveFileData FileData = Formatter.Deserialize(Stream) as SaveFileData;
        Stream.Close(); 

        return FileData;
    }

    public static List<SaveFileData> InitAllSavedData()
    {
        List<SaveFileData> list = new List<SaveFileData>(5);
        string path = $"{FilePathDir}/SavedFiles/";
        string[] fileArray = Directory.GetFiles(path, "*.save");
        if (fileArray.Length == 0)
            return list;
        foreach(string filename in fileArray)
        {
            Debug.Log($"Accessing the file: {filename}");
            SaveFileData fileData = GetSaveFileData(filename);
            list.Add(fileData);
        }
        return list;
    }

    public static void SaveGameData()//should be implemented in every file saving functions
    {
        string Path = $"{FilePathDir}/GameData.data";
        FileStream Stream = new FileStream(Path, FileMode.Create);
        Formatter.Serialize(Stream, GameData.Instance);
        Stream.Close();
        Debug.Log($"GameData saved to {Path}");
    }

    public static GameData LoadGameData()
    {
        string Path = $"{FilePathDir}/GameData.data";
        if (!File.Exists(Path))
        {
            Debug.Log($"{Path} does not exist!");
            return null;
        }
        FileStream Stream = new FileStream(Path, FileMode.Open);
        GameData FileData = Formatter.Deserialize(Stream) as GameData;
        Stream.Close(); 

        return FileData;
    }
}
