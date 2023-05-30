using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string filePathDir = Application.persistentDataPath;
    private static BinaryFormatter formatter = new BinaryFormatter(); 

    public static void DeleteFileData(string filename)
    {
        string path = $"{filePathDir}/SavedFiles/{filename}.save";
        Debug.Log($"Deleting file named: '{filename}.save' in '{filePathDir}/SavedFiles/'. ");
        if (!File.Exists(path))
        {
            Debug.LogError($"{filename}.save does not exist in p    ath: {filePathDir}/SavedFiles/");
            return;
        }
        File.Delete(path);
        Debug.Log($"{filename}.save deleted successfully!");
    }

    public static void SaveLevelData(string filename, PlayerLevelData playerLevelData)
    {
        string path = $"{filePathDir}/SavedFiles/{filename}.save";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveFileData fileData = new SaveFileData(filename , playerLevelData.SaveLevelData());
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
        for(int i = 0; i < fileArray.Length; i++)
        {
            Debug.Log($"Accessing the file: {fileArray[i]}");
            SaveFileData fileData = GetSaveFileData(fileArray[i]);
            if(fileData == null)
                Debug.LogWarning($"{fileArray[i]} is null");
            list.Add(fileData);
        }
        return list;
    }

    public static List<SaveFileData> FetchAllSavedFileData()
    {
        List<SaveFileData> savedFiles = new List<SaveFileData>();
        string path = $"{filePathDir}/SavedFiles/";
        Debug.Log($"Fetching all saved files from {path}");
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
        string[] fileArray = Directory.GetFiles(path, "*.save");
        if (fileArray.Length == 0)
            return savedFiles;
        for(int i = 0; i < fileArray.Length; i++)
        {
            Debug.Log($"Accessing the file: {fileArray[i]}");
            SaveFileData fileData = GetSaveFileData(fileArray[i]);
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
        GameData fileGameData = formatter.Deserialize(stream) as GameData;
        stream.Close(); 

        return fileGameData;
    }

}

public interface ISaveable
{
    public void SaveData(LevelData levelData);
    public void LoadData(LevelData levelData);
}