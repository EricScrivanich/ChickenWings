using UnityEditor;
using UnityEngine;
using System.IO;

public class SaveDataEditorTools
{
    // This method creates a new menu item in Unity's toolbar
    [MenuItem("Tools/Reset Save Data")]
    public static void ResetSaveData()
    {
        // Define the path to the save file
        string saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");

        // Check if the file exists and delete it
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save data reset successfully.");
        }
        else
        {
            Debug.LogWarning("No save data found to reset.");
        }

        // If you want to reset in-memory data too (only if you are in play mode), you can do this:
        if (Application.isPlaying && SaveManager.instance != null)
        {
            SaveManager.instance.ResetGameDataInMemory();
        }
    }
}