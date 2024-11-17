using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public SceneManagerSO sceneSO; // Scriptable object for scene data

    private GameData gameData;
    private string saveFilePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");

        // Load existing data or initialize a new GameData
        LoadData();
    }

    // Method to update level data based on a bool array (e.g., from SaveLevelCompletion)
    public void UpdateLevelData(int levelId, bool[] data)
    {
        // Find the LevelSavedData for the specified levelId
        LevelSavedData levelData = GetLevelData(levelId);

        if (levelData == null)
        {
            Debug.LogError($"Level data for level {levelId} not found.");
            return;
        }

        // Ensure the data array has enough elements for CompletedLevel, MasteredLevel, and challenges
        if (data.Length < 2 + levelData.ChallengeCompletion.Length)
        {
            Debug.LogError("Data array is not the correct size for this level.");
            return;
        }

        // Update CompletedLevel and MasteredLevel if data contains true values
        if (data[0]) levelData.CompletedLevel = true;
        if (data[1]) levelData.MasteredLevel = true;

        // Update challenges only if data[i + 2] is true
        for (int i = 0; i < levelData.ChallengeCompletion.Length; i++)
        {
            if (data[i + 2]) levelData.ChallengeCompletion[i] = true;
        }

        // Save the modified data
        SaveData();
    }

    // Helper method to get LevelSavedData for a specific level by ID
    private LevelSavedData GetLevelData(int levelId)
    {
        return gameData.Levels.Find(level => level.LevelID == levelId);
    }

    // Save gameData to a JSON file
    private void SaveData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
    }

    public int ReturnCompletedChallenges(int type)
    {
        int amount = 0;
        for (int i = 1; i < sceneSO.ReturnNumberOfLevels(); i ++)
        {
            var data = GetLevelData(i);

            if (data.ChallengeCompletion.Length > 0)
            {
                if (type == 0)
                {
                    foreach (var b in data.ChallengeCompletion)
                    {
                        if (b) amount++;
                    }
                }
                else if (type == 1)
                {
                    if (data.MasteredLevel) amount++;
                }
            }

        }

        return amount;

    }

    public bool HasCompletedLevel(int levelNumber)
    {
        if (levelNumber <= 0) return true;
        else return GetLevelData(levelNumber).CompletedLevel;
    }



    // Load gameData from a JSON file, or initialize new data if file does not exist
    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            InitializeGameData();
            SaveData();
        }
    }

    public void ResetGameDataInMemory()
    {
        InitializeGameData(); // Reinitialize the data in memory
        SaveData();           // Save the new, empty data immediately
    }

    public bool[] GetSavedLevelData(int levelId)
    {
        
        // Find the LevelSavedData for the specified levelId
        LevelSavedData levelData = GetLevelData(levelId);
      

        // If level data does not exist, check if sceneSO has data for this level and initialize it if so
        if (levelData == null || (levelData.ChallengeCompletion.Length < 1 && sceneSO.ReturnChallengeCountByLevel(levelId) > 0))
        {
            int challengeCount = sceneSO.ReturnChallengeCountByLevel(levelId);

            if (challengeCount > 0) // If sceneSO indicates there are challenges for this level
            {
                Debug.Log($"Level data for level {levelId} not found in saved data. Initializing new data with {challengeCount} challenges.");

                // Initialize new LevelSavedData with default values and add it to gameData
                levelData = new LevelSavedData(levelId, challengeCount);
                gameData.Levels.Add(levelData);

                // Save updated data
                SaveData();
            }
            else
            {
                Debug.LogError($"No challenges found in sceneSO for level {levelId}. Cannot initialize data.");
                return new bool[0]; // Return an empty array if level data and challenges are not found
            }
        }
        // else if (sceneSO.ReturnChallengeCountByLevel(levelId) >= levelData.ChallengeCompletion.Length)
        // {
        //     var newData = new LevelSavedData(levelId, sceneSO.ReturnChallengeCountByLevel(levelId));

        //     for (int i = 0; i < levelData.ChallengeCompletion.Length; i++)
        //     {
        //         newData.ChallengeCompletion[i] = levelData.ChallengeCompletion[i];
        //     }

        //     newData.ChallengeCompletion[1] = false;




        // }

        // Create a bool array with a length of 2 (for CompletedLevel and MasteredLevel) + the number of challenges
        bool[] statusArray = new bool[2 + levelData.ChallengeCompletion.Length];

        // Set the first two elements
        statusArray[0] = levelData.CompletedLevel;
        statusArray[1] = levelData.MasteredLevel;

        // Copy the ChallengeCompletion array into the rest of the statusArray
        levelData.ChallengeCompletion.CopyTo(statusArray, 2);

        return statusArray;
    }

    // Initializes gameData with LevelSavedData for each level based on SceneManagerSO
    private void InitializeGameData()
    {
        gameData = new GameData();

        // Populate gameData.Levels with LevelSavedData for each level using sceneSO
        for (int i = 1; i <= sceneSO.ReturnNumberOfLevels(); i++)
        {
            int challengeCount = sceneSO.ReturnChallengeCountByLevel(i);
            gameData.Levels.Add(new LevelSavedData(i, challengeCount));
        }
    }
}