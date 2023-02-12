using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string filePathDir = Application.persistentDataPath;
    private static BinaryFormatter formatter = new BinaryFormatter(); 
    public static List<ISaveable> saveables;

    public static void DeleteFileData(string filename)
    {
        string path = $"{filePathDir}/SavedFiles/{filename}.save";
        Debug.Log($"Deleting file named: '{filename}.save' in '{filePathDir}/SavedFiles/'. ");
        if (!File.Exists(path))
        {
            Debug.LogError($"{filename}.save does not exist in path: {filePathDir}/SavedFiles/");
            return;
        }
        File.Delete(path);
        Debug.Log($"{filename}.save deleted successfully!");
    }

    public static void SaveLevelData(string filename, PlayerLevelData playerLevelData)
    {
        string path = $"{filePathDir}/SavedFiles/{filename}.save";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveFileData fileData = new SaveFileData(filename ,playerLevelData.SaveGame());
        Debug.Log($"Saving Level Data as '{filename}.save' in '{filePathDir}/SavedFiles/' !");
        formatter.Serialize(stream, fileData);
        stream.Close();
        SaveGameData();
    }

    public static SaveFileData GetSaveFileData(string filepath)
    {
        if (!File.Exists(filepath))
        {
            Debug.Log($"{filepath} does not exist!");
            return null;
        }
        FileStream stream = new FileStream(filepath, FileMode.Open);
        SaveFileData fileData = formatter.Deserialize(stream) as SaveFileData;
        stream.Close(); 

        return fileData;
    }

    public static List<SaveFileData> InitAllSavedData()
    {
        List<SaveFileData> list = new List<SaveFileData>(5);
        string path = $"{filePathDir}/SavedFiles/";
        string[] fileArray = Directory.GetFiles(path, "*.save");
        if (fileArray.Length == 0)
            return list;
        foreach(string filename in fileArray)
        {
            Debug.Log($"Accessing the file: {filename}");
            SaveFileData fileData = GetSaveFileData(filename);
            if(fileData == null)
                Debug.LogWarning($"{filename} is null");
            list.Add(fileData);
        }
        return list;
    }

    public static List<SaveFileData> FetchAllSavedFileData()
    {
        List<SaveFileData> savedFiles = new List<SaveFileData>();
        string path = $"{filePathDir}/SavedFiles/";
        Debug.Log($"Fetching all saved files from {path}");
        string[] fileArray = Directory.GetFiles(path, "*.save");
        if (fileArray.Length == 0)
        {
            Debug.LogWarning("SavedFiles folder is Empty");
            return savedFiles;
        }
        foreach(string filename in fileArray)
        {
            Debug.Log($"Accessing the file: {filename}");
            SaveFileData fileData = GetSaveFileData(filename);
            savedFiles.Add(fileData);
        }
        Debug.Log($"Fetched {savedFiles.Count} saved file data!");
        return savedFiles;
    }


    public static void SaveGameData()//should be implemented in every file saving functions
    {
        string path = $"{filePathDir}/GameData.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, GameData.Instance);
        stream.Close();
        Debug.Log($"GameData saved to {path}");
    }

    public static GameData LoadGameData()
    {
        string path = $"{filePathDir}/GameData.data";
        if (!File.Exists(path))
        {
            Debug.Log($"{path} does not exist!");
            return null;
        }
        FileStream stream = new FileStream(path, FileMode.Open);
        GameData fileData = formatter.Deserialize(stream) as GameData;
        stream.Close(); 

        return fileData;
    }

}

public interface ISaveable
{
    public void SaveData(LevelData levelData);
    public void LoadData(LevelData levelData);
}