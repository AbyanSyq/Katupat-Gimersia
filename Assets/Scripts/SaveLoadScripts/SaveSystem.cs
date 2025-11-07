using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public struct SaveData
{
    
}

public class SaveSystem
{
    public static SaveData saveData = new SaveData();

    // public static string SaveFileName => Path.Combine(Application.persistentDataPath, "save.json");
    public static string SaveFileName => Path.Combine(Application.dataPath, "Resources/save.json");

    public static bool IsSaveFileExists => File.Exists(SaveFileName);

    public static void Save()
    {
        HandleSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveFileName, json);
        Debug.Log("Saved to: " + SaveFileName);
    }

    private static void HandleSaveData()
    {
        // call all method to save data
    }

    public static void Load()
    {
        if (File.Exists(SaveFileName))
        {
            string json = File.ReadAllText(SaveFileName);
            saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Loaded from: " + SaveFileName);
        }
        else
        {
            Debug.LogWarning("Save file not found, creating default save.");
            CreateDefaultSave();
            Save(); 
        }

        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        //call all method to load data
    }

    private static void CreateDefaultSave()
    {
        saveData = new SaveData
        {
            
        };
    }
}
