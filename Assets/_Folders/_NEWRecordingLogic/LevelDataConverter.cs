using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelDataConverter : MonoBehaviour
{
    public static LevelDataConverter instance;
    private UserCreatedLevels userCreatedLevels;


    private int maxTempLevelSaves;
    public static int currentChallengeType = 0;


    public static int currentLevelInstance { get; private set; } = -1;

    // private List<short> idList;
    // private List<ushort> spawnedStepList;
    // private List<Vector2> posList;
    // private List<float> speedList;
    // private List<float> scaleList;
    // private List<float> magList;
    // private List<float> timeList;
    // private List<float> delayList;
    // private List<float> typeList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //0-Normal; 1-Jetpack; 2-BigPig; 3-Tenderizer; 4-Pilot; 5- Missile; 6-BomberPlane; 7-Flappy; 8-Gas; 9-Balloon

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        LevelRecordManager.ResetStaticParameters();



#if UNITY_EDITOR
        {
            var Levels = Resources.Load<LevelContainer>("LevelContainer");

            if (Levels.CheckNullItems())
            {
                UnityEditor.EditorUtility.SetDirty(Levels);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            Resources.UnloadAsset(Levels);
        }
#endif


    }

    public void DeleteAllJsonSaves()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            string[] jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            foreach (string file in jsonFiles)
            {
                try
                {
                    File.Delete(file);
                    Debug.Log("Deleted: " + file);
                }
                catch (IOException e)
                {
                    Debug.LogError("Could not delete file: " + file + "\n" + e.Message);
                }
            }
        }
        else
        {
            Debug.Log("Persistent data directory does not exist.");
        }
    }

    public LevelData ReturnLevelData(int index = -1)
    {
        Debug.Log("Returning Level Data for index: " + index);
        var Levels = Resources.Load<LevelContainer>("LevelContainer");


        if (index == -1)
        {
            Debug.Log("Returning current level instance: " + currentLevelInstance);
            if (currentLevelInstance == -1 || currentLevelInstance >= Levels.levels.Length)
            {
                Debug.LogWarning("Current level instance is not set or out of bounds. Returning null.");
                Resources.UnloadAsset(Levels);
                return null;
            }
            var levelData = Levels.levels[currentLevelInstance];
            Resources.UnloadAsset(Levels);
            return levelData;
        }

        else
        {
            var l = Levels.levels[index];
            Resources.UnloadAsset(Levels);

            return l;
        }


    }
    public LevelData ReturnLevelDataByNum(Vector3Int levelNumber)
    {
        if (levelNumber == Vector3.zero) return null;

        var Levels = Resources.Load<LevelContainer>("LevelContainer");
        for (int i = 0; i < Levels.levels.Length; i++)
        {
            if (Levels.levels[i].levelWorldAndNumber == levelNumber)
            {
                Debug.Log("Returning Level Data for: " + levelNumber);
                var levelData = Levels.levels[i];
                Resources.UnloadAsset(Levels);
                return levelData;
            }
        }
        Resources.UnloadAsset(Levels);
        Debug.LogWarning("No level found for: " + levelNumber);
        return null;
    }




    public void SetCurrentLevelInstance(Vector3Int levelNumber)
    {
        if (levelNumber == Vector3.zero) currentLevelInstance = -1;
        var Levels = Resources.Load<LevelContainer>("LevelContainer");
        for (int i = 0; i < Levels.levels.Length; i++)
        {
            if (Levels.levels[i].levelWorldAndNumber == levelNumber)
            {
                currentLevelInstance = i;
                Resources.UnloadAsset(Levels);
                Debug.Log("Current Level Instance set to: " + currentLevelInstance);
                return;
            }
        }
    }





    public void AddLevel(LevelData level)
    {
#if UNITY_EDITOR
        Debug.Log("Adding level: " + level.LevelName);
        var Levels = Resources.Load<LevelContainer>("LevelContainer");
        Levels.AddLevel(level);
        UnityEditor.EditorUtility.SetDirty(Levels);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Resources.UnloadAsset(Levels);
#endif
    }

    public void DeleteUserCreatedLevelsFile()
    {
        string filePath = GetUserCreatedLevelsFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted the corrupted file at: " + filePath);
        }
        else
        {
            Debug.Log("File not found, nothing to delete: " + filePath);
        }
    }

    public void DeleteAllLevelsAndClearDirectories()
    {
        // Delete all JSON files in the Levels directory.
        string levelsDirectory = GetSaveDirectory();
        if (Directory.Exists(levelsDirectory))
        {
            string[] files = Directory.GetFiles(levelsDirectory, "*.json");
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log("Deleted level file: " + file);
            }
        }
        else
        {
            Debug.Log("Levels directory does not exist: " + levelsDirectory);
        }
    }

    public void DeleteLevelByName(string levelName)
    {
        // Build the asset file path using the level name.
        string filePath = Path.Combine(GetSaveDirectory(), levelName + ".json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted level file: " + filePath);
        }
        else
        {
            Debug.Log("Level file not found: " + filePath);
        }

        // Load the current user-created levels.
        UserCreatedLevels levels = LoadUserCreatedLevels();
        if (levels.UserCreatedLevelNames.Contains(levelName))
        {
            levels.UserCreatedLevelNames.Remove(levelName);
            EditUserCreatedLevels(levels, true);
            Debug.Log("Removed level name from user created levels: " + levelName);
        }
        else
        {
            Debug.Log("Level name not found in user created levels: " + levelName);
        }
    }


    public string GetSaveDirectory()
    {
        string directory = Path.Combine(Application.persistentDataPath, "Levels");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("Created save directory: " + directory);
        }

        return directory;
    }
    public void ConvertDataToArrays(List<RecordableObjectPlacer> obj, LevelData so)
    {
        List<short> idList = new List<short>();
        List<ushort> spawnedStepList = new List<ushort>();
        List<ushort> typeList = new List<ushort>();
        List<Vector2> posList = new List<Vector2>();
        List<float> floatList = new List<float>();
        List<ushort> dataTypes = new List<ushort>();

        for (int i = 0; i < obj.Count; i++)
        {

        }
    }

    public void SaveDataToDevice(List<RecordableObjectPlacer> obj, List<RecordableObjectPlacer> randomObj, string title, List<ushort[]> plSizes, ushort finalSpawnStep, short[] ammos, short lives, LevelData editorData = null)
    {

        List<short> objectTypeList = new List<short>();
        List<short> idList = new List<short>();
        List<ushort> spawnedStepList = new List<ushort>();
        List<ushort> typeList = new List<ushort>();
        List<Vector2> posList = new List<Vector2>();
        List<float> floatList = new List<float>();
        List<short> dataTypes = new List<short>();
        List<ushort> cageAttachments = new List<ushort>();
        List<short> indexAndHealth = new List<short>();








        List<RecordedObjectPositionerDataSave> positionerData = new List<RecordedObjectPositionerDataSave>();

        bool isEditor = editorData != null;
        List<Vector2Int> specialTriggerIndexAndType = new List<Vector2Int>();


        for (int i = 0; i < obj.Count; i++)
        {
            var o = obj[i];

            objectTypeList.Add(o.ObjectType);
            idList.Add(o.Data.ID);
            spawnedStepList.Add(o.Data.spawnedStep);
            dataTypes.Add(o.DataType);

            if (o.Data.hasCageAttachment)
            {
                cageAttachments.Add((ushort)i);
            }
            if (o.Data.health > 1)
            {
                indexAndHealth.Add((short)i);
                indexAndHealth.Add(o.Data.health);
            }

            if (o.IsPositionerObject())
            {
                Debug.LogError("Positioner Object Found: " + o.Data.ID);
                positionerData.Add(o.ReturnPositionerDataSave());

            }

            else
            {
                typeList.Add(o.Data.type);
                posList.Add(o.Data.startPos);

                if (isEditor && o.Data.triggerType != 0)
                {
                    specialTriggerIndexAndType.Add(new Vector2Int(i, o.Data.triggerType));
                }
            }


            int dataType = o.DataType;
            if (dataType > 0 && dataType <= 5)
            {
                float[] values = new float[dataType];
                for (int j = 0; j < dataType; j++)
                {
                    switch (j)
                    {
                        case 0:
                            values[j] = o.Data.float1;
                            break;
                        case 1:
                            values[j] = o.Data.float2;
                            break;
                        case 2:
                            values[j] = o.Data.float3;
                            break;
                        case 3:
                            values[j] = o.Data.float4;
                            break;
                        case 4:
                            values[j] = o.Data.float5;
                            break;
                    }
                }

                for (int n = 0; n < values.Length; n++)
                {
                    floatList.Add(values[n]);
                }

            }

        }

        bool hasRandom = (randomObj != null && randomObj.Count > 0);



        // ushort[] poolData = new ushort[poolSizes.Length];
        // for (int i = 0; i < poolSizes.Length; i++)
        // {
        //     poolData[i] = (ushort)poolSizes[i];

        // }


        if (isEditor)
        {
            title = editorData.LevelName;
            if (editorData.levelDataBossAndRandomLogic != null)
            {
                var extraData = editorData.levelDataBossAndRandomLogic.ReturnCollectableSpawnData();
                if (extraData != null)
                {
                    if (extraData.SpawnEggs)
                    {

                        plSizes[3][0] += 2;
                    }
                    if (extraData.SpawnShotgun)
                    {

                        plSizes[3][1] += 2;
                    }
                }
            }


        }  // Use the editor data title if available
        LevelDataSave save = new LevelDataSave(title, objectTypeList.ToArray(),
        idList.ToArray(), spawnedStepList.ToArray(), typeList.ToArray(),
        dataTypes.ToArray(), finalSpawnStep, posList.ToArray(),
        floatList.ToArray(), plSizes, ammos, lives, indexAndHealth.ToArray());

        if (hasRandom)
        {
            List<float> floatListRand = new List<float>();
            List<Vector2> posListRand = new List<Vector2>();
            List<ushort> objectTypeListRand = new List<ushort>();
            List<ushort> randomLogicSizes = new List<ushort>();
            List<Vector3Int> randomSpawnDataTypeObjectTypeAndID = new List<Vector3Int>();
            List<ushort> spawnStepsRandom = new List<ushort>();


            for (int i = 0; i < randomObj.Count; i++)
            {
                var o = randomObj[i];
                int currentSpawnSet = o.Data.randomSpawnData.x - 1;
                if (currentSpawnSet >= randomLogicSizes.Count)
                {
                    randomLogicSizes.Add(1);
                    Vector3Int v = new Vector3Int();

                    spawnStepsRandom.Add(o.Data.spawnedStep);
                    v.x = o.DataType;
                    v.y = o.ObjectType;
                    v.z = o.Data.ID;
                    randomSpawnDataTypeObjectTypeAndID.Add(v);
                }
                if (o.Data.randomSpawnData.y >= randomLogicSizes[currentSpawnSet])
                {
                    randomLogicSizes[currentSpawnSet]++;
                }

                objectTypeListRand.Add(o.Data.type);
                posListRand.Add(o.Data.startPos);

                int dataType = o.DataType;
                if (dataType > 0 && dataType <= 5)
                {
                    float[] values = new float[dataType];
                    for (int j = 0; j < dataType; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                values[j] = o.Data.float1;
                                break;
                            case 1:
                                values[j] = o.Data.float2;
                                break;
                            case 2:
                                values[j] = o.Data.float3;
                                break;
                            case 3:
                                values[j] = o.Data.float4;
                                break;
                            case 4:
                                values[j] = o.Data.float5;
                                break;
                        }
                    }

                    for (int n = 0; n < values.Length; n++)
                    {
                        floatListRand.Add(values[n]);
                    }

                }


            }


            save.SetRandomData(posListRand.ToArray(), floatListRand.ToArray(), objectTypeListRand.ToArray(), randomLogicSizes.ToArray(), randomSpawnDataTypeObjectTypeAndID.ToArray(), spawnStepsRandom.ToArray());


        }





        save.SetPositionerData(positionerData.ToArray());
        save.SetCageAttachments(cageAttachments.ToArray());
#if UNITY_EDITOR
        if (editorData != null)
        {

            editorData.LoadLevelSaveData(save);
            var arrayData = editorData.ReturnDataArrays(save);
            editorData.specialTriggerIndexAndType = specialTriggerIndexAndType.ToArray();
            UnityEditor.EditorUtility.SetDirty(editorData);
            UnityEditor.EditorUtility.SetDirty(arrayData);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            return;
        }
#endif
        Debug.LogError("Converting To Json");
        string savePath = Path.Combine(GetSaveDirectory(), title + ".json");
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Level saved to: " + savePath);
    }


    public LevelDataSave ConvertDataFromJson()
    {
        string path = PlayerPrefs.GetString("LevelCreatorPath");
        if (path == null || path == "")
        {
            Debug.Log("No path found");
            return null;
        }
        string loadPath = Path.Combine(GetSaveDirectory(), path + ".json");

        if (!File.Exists(loadPath))
        {
            Debug.LogError("Save file not found: " + loadPath);
            return null; // Return null if the file does not exist
        }


        string json = File.ReadAllText(loadPath);
        LevelDataSave loadedData = JsonUtility.FromJson<LevelDataSave>(json);

        Debug.Log("Level loaded from: " + loadPath);
        return loadedData;




    }

    public static string GetLevelNumberStringFormat(Vector3Int n)
    {
        string world = n.x.ToString("00");
        string level = n.y.ToString("00");
        string sub = n.z.ToString("00");
        // Combine with dashes
        string formatted = $"{world}-{level}-{sub}_";
        return formatted;
    }
    public List<RecordedDataStructDynamic> ReturnDynamicData(LevelData data, LevelDataArrays dataArrays)
    {


        int floatIndex = 0;
        int positionerIndex = 0;
        int subrtactIndex = 0;
        int specialTriggerIndex = 0;
        int healthIndex = 0;


        int currentCageIndex = 0;

        bool checkCageAttachments = false;

        if (data.cageAttachments != null && data.cageAttachments.Length > 0)
        {
            checkCageAttachments = true;
        }


        var l = new List<RecordedDataStructDynamic>();
        if (data.idList == null || data.idList.Length == 0)
        {
            Debug.LogError("No data found in the loaded LevelData.");
            return l; // Return an empty list if no data is found
        }

        for (int i = 0; i < data.idList.Length; i++)
        {
            bool hasCageAttachment = false;

            if (checkCageAttachments && i == data.cageAttachments[currentCageIndex])
            {
                hasCageAttachment = true;
                currentCageIndex++;

                if (currentCageIndex >= data.cageAttachments.Length)
                {
                    checkCageAttachments = false; // No more cage attachments to check
                }

            }

            float[] values = new float[5];
            int type = data.dataTypes[i];
            short health = 1;

            short triggerType = 0;

            if (specialTriggerIndex < data.specialTriggerIndexAndType.Length && i == data.specialTriggerIndexAndType[specialTriggerIndex].x)
            {
                triggerType = (short)data.specialTriggerIndexAndType[specialTriggerIndex].y;
                specialTriggerIndex++;
            }
            if (data.objectTypes[i] == 1) // boss type
            {
                if (dataArrays.indexAndHealth != null && healthIndex * 2 < dataArrays.indexAndHealth.Length - 1 && i == dataArrays.indexAndHealth[healthIndex * 2])
                {
                    health = dataArrays.indexAndHealth[(healthIndex * 2) + 1];
                    healthIndex++;
                }
            }


            Debug.Log("Loading Data Type: " + type);

            if (type <= 5 && type >= 0)
            {
                for (int j = 0; j < type; j++)
                {
                    values[j] = dataArrays.floatList[floatIndex];
                    floatIndex++;
                }

                l.Add(new RecordedDataStructDynamic(data.objectTypes[i], data.idList[i], dataArrays.typeList[i - subrtactIndex], dataArrays.posList[i - subrtactIndex], values[0], values[1], values[2], values[3], values[4], data.spawnSteps[i], 0, Vector2Int.zero, hasCageAttachment, triggerType, health));
            }
            else if (type < 0)
            {
                var d = new RecordedDataStructDynamic(data.objectTypes[i], data.idList[i], 0, Vector2.zero, 0, 0, 0, 0, 0, data.spawnSteps[i], 0, Vector2Int.zero, hasCageAttachment, triggerType);
                d.positionerData = data.postionerData[positionerIndex];
                subrtactIndex++;
                positionerIndex++;
                l.Add(d);

            }


        }

        if (dataArrays.randomSpawnDataTypeObjectTypeAndID != null && dataArrays.randomSpawnDataTypeObjectTypeAndID.Length > 0)
        {
            int floatRandIndex = 0;
            int currentSpawnSet = 0;
            int spawnSetCounter = 0;

            for (int i = 0; i < dataArrays.randomSpawnDataTypeObjectTypeAndID.Length; i++)
            {
                var v = dataArrays.randomSpawnDataTypeObjectTypeAndID[i];
                int dataType = v.x;
                ushort spawnStep = dataArrays.spawnStepsRandom[i];
                ushort objectType = (ushort)v.y;
                short id = (short)v.z;



                for (int n = 0; n < dataArrays.randomLogicSizes[i]; n++)
                {
                    float[] values = new float[5];
                    for (int j = 0; j < dataType; j++)
                    {
                        values[j] = dataArrays.floatListRand[floatRandIndex];
                        floatRandIndex++;
                    }
                    l.Add(new RecordedDataStructDynamic((short)v.y, id, dataArrays.typeListRand[i], dataArrays.posListRand[spawnSetCounter], values[0], values[1], values[2], values[3], values[4], spawnStep, 0, new Vector2Int(i + 1, n), false, 0));
                    spawnSetCounter++;
                }



                // spawnSetCounter++;
                // if (spawnSetCounter >= dataArrays.randomLogicSizes[currentSpawnSet])
                // {
                //     currentSpawnSet++;
                //     spawnSetCounter = 0;
                // }
            }
        }
        return l;
    }



    private string GetUserCreatedLevelsFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "UserCreatedLevelNames.json");
    }

    // Saves the current userCreatedLevels to disk.
    public void SaveUserCreatedLevels()
    {
        string filePath = GetUserCreatedLevelsFilePath();
        string json = JsonUtility.ToJson(userCreatedLevels, true);
        File.WriteAllText(filePath, json);
        Debug.Log("User created levels saved to: " + filePath);
    }

    // Edits the current userCreatedLevels. The 'add' flag can be used for future expansion
    // (for example, if you want to remove a level when add is false).
    public void EditUserCreatedLevels(UserCreatedLevels levels, bool add)
    {
        userCreatedLevels = levels;
        // (If add is false, you might remove a level instead; for now, we just overwrite.)
        SaveUserCreatedLevels();
    }

    // Loads the user-created levels from disk. If no file exists, returns a new instance.
    public UserCreatedLevels LoadUserCreatedLevels()
    {
        string filePath = GetUserCreatedLevelsFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            // If the file is empty or only contains whitespace, create a new instance.
            if (string.IsNullOrWhiteSpace(json))
            {
                userCreatedLevels = new UserCreatedLevels();
            }
            else
            {
                try
                {
                    userCreatedLevels = JsonUtility.FromJson<UserCreatedLevels>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to deserialize UserCreatedLevels. Creating new instance. Exception: " + e);
                    userCreatedLevels = new UserCreatedLevels();
                }
            }
            // Ensure the list is initialized.
            if (userCreatedLevels.UserCreatedLevelNames == null)
            {
                userCreatedLevels.UserCreatedLevelNames = new List<string>();
            }
            Debug.Log("User created levels loaded from: " + filePath);
            return userCreatedLevels;
        }
        else
        {
            return new UserCreatedLevels();
        }
    }

    #region Temporary CheckPoint Data
    public string GetTempLevelSaveDirectory()
    {
        string directory = Path.Combine(Application.persistentDataPath, "LevelCheckPointSaves");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("Created save directory: " + directory);
        }

        return directory;
    }


    public void RemoveTemporaryCheckPointData()
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Deleted temporary checkpoint data file: " + savePath);
        }
        else
        {
            Debug.LogWarning("Temporary checkpoint data file does not exist: " + savePath);
        }
    }

    public void OverwriteCheckpointDataForNewLevel(Vector3Int levelAndWorldNumber, int levelDifficulty)
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        TemporaryAllLevelCheckPointData save = new TemporaryAllLevelCheckPointData();
        save.LevelAndWorldNumber = levelAndWorldNumber;
        save.LevelDifficulty = levelDifficulty;
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.LogError("Overwritten checkpoint data for new level: " + levelAndWorldNumber + " Difficulty: " + levelDifficulty);
    }


    public void OverwriteCheckPoint(short checkPointToLoad)
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        string currentSave = File.ReadAllText(savePath);
        var save = JsonUtility.FromJson<TemporaryAllLevelCheckPointData>(currentSave);
        save = save.RemoveCheckPoints(checkPointToLoad);
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);

    }

    public bool CheckIfLevelOverwrite(Vector3Int levelAndWorldNumber, int levelDifficulty)
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        if (!File.Exists(savePath))
        {
            Debug.LogError("Temporary checkpoint data file does not exist: " + savePath);
            return false; // No save file exists, so no overwrite
        }
        string currentSave = File.ReadAllText(savePath);



        var save = JsonUtility.FromJson<TemporaryAllLevelCheckPointData>(currentSave);
        if (save == null)
        {
            Debug.LogError("Failed to deserialize TemporaryAllLevelCheckPointData from: " + savePath);
            return false;
        }
        return !save.CheckIfSameLevel(levelAndWorldNumber, levelDifficulty);
    }
    public bool CheckIfCheckPointOverwrite(short checkPointToLoad)
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Temporary checkpoint data file does not exist: " + savePath);
            return false; // No save file exists, so no overwrite
        }
        string currentSave = File.ReadAllText(savePath);
        var save = JsonUtility.FromJson<TemporaryAllLevelCheckPointData>(currentSave);
        if (save == null) return false;
        return save.CheckIfCurrentCheckPoint(checkPointToLoad);
    }



    public TemporaryAllLevelCheckPointData ReturnAllCheckPointDataForLevel()
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        if (!File.Exists(savePath))
        {

            return null; // No save file exists, so no overwrite
        }
        string currentSave = File.ReadAllText(savePath);

        var save = JsonUtility.FromJson<TemporaryAllLevelCheckPointData>(currentSave);
        if (save == null) return null;
        else return save;

        // if (save.CheckIfSameLevel(lvlID, levelDifficulty))
        // {

        //     return save;
        // }
        // else
        // {

        //     return null;
        // }


    }


    private TemporaryAllLevelCheckPointData currentAllCheckPointData;
    private bool hasAddedCheckPointData = false;
    public TemporaryLevelCheckPointData ReturnCheckPointDataForLevel()
    {
        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");
        if (!File.Exists(savePath))
        {
            currentAllCheckPointData = new TemporaryAllLevelCheckPointData();
            return null; // No save file exists, so no overwrite
        }
        string currentSave = File.ReadAllText(savePath);
        var save = JsonUtility.FromJson<TemporaryAllLevelCheckPointData>(currentSave);
        if (save == null)
        {
            currentAllCheckPointData = new TemporaryAllLevelCheckPointData();
            return null;
        }

        else
        {
            currentAllCheckPointData = save;
            return save.ReturnCheckPointData(0, true);
        }







    }

    public void SaveCheckPointDataToJson()
    {
        if (currentAllCheckPointData == null || !hasAddedCheckPointData)
        {
            Debug.LogWarning("No checkpoint data to save.");
            currentAllCheckPointData = null;
            return;
        }

        Debug.LogError("Saving Checkpoint Data to JSON with length: " + currentAllCheckPointData.LevelCheckPointData.Length);
        hasAddedCheckPointData = false;

        string savePath = Path.Combine(GetTempLevelSaveDirectory(), ".json");



        string json = JsonUtility.ToJson(currentAllCheckPointData, true);
        File.WriteAllText(savePath, json);
        currentAllCheckPointData = null;
    }


    public void SaveTemporaryCheckPointData(LevelChallenges data)
    {
        if (data != null && currentAllCheckPointData != null)
        {
            hasAddedCheckPointData = true;
            Debug.LogError("Adding Checkpoint Data: " + data.CurrentCheckPoint);

            currentAllCheckPointData = currentAllCheckPointData.AddCheckPoint(data);
        }


    }
    #endregion




    #region Permanent Level Data
    private SavedLevelDataByWorld currentLoadedSavedLevelDataByWorld;
    private LevelSavedData currentLoadedSavedLevelData;

    public string GetPermanentLevelSaveDirectory(int levelWorld)
    {

        string directory = Path.Combine(Application.persistentDataPath, "PermanentLevelData_" + "World_" + levelWorld.ToString());

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("Created save directory: " + directory);
        }


        return directory;
    }
#if UNITY_EDITOR

    public void SetLevelCompletionManual(LevelData data, bool clearData, bool mastered, bool[] challengeCompletion)
    {

        LoadLevelSavedData(data);



        if (clearData)
        {
            currentLoadedSavedLevelData.FurthestCompletion = 0;
            currentLoadedSavedLevelData.FurthestCompletionEasy = 0;
            for (int i = 0; i < currentLoadedSavedLevelData.ChallengeCompletion.Length; i++)
            {
                currentLoadedSavedLevelData.ChallengeCompletion[i] = false;
            }

            currentLoadedSavedLevelData.CompletedLevel = false;

            currentLoadedSavedLevelData.MasteredLevel = false;
        }

        else
        {
            currentLoadedSavedLevelData.CompletedLevel = true;
            currentLoadedSavedLevelData.FurthestCompletion = data.finalSpawnStep;
            currentLoadedSavedLevelData.MasteredLevel = mastered;

            if (challengeCompletion != null)
            {
                for (int i = 0; i < challengeCompletion.Length; i++)
                {
                    if (i < currentLoadedSavedLevelData.ChallengeCompletion.Length)
                    {
                        currentLoadedSavedLevelData.ChallengeCompletion[i] = challengeCompletion[i];
                    }
                }
            }

        }


        SavePermanentLevelData();
    }

#endif





    public void LoadLevelSavedData(LevelData data)
    {
        for (int i = 0; i < currentLoadedSavedLevelDataByWorld.levels.Length; i++)
        {
            if (currentLoadedSavedLevelDataByWorld.levels[i].LevelName == data.LevelName && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x == data.levelWorldAndNumber.y && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y == data.levelWorldAndNumber.z)
            {

                currentLoadedSavedLevelData = currentLoadedSavedLevelDataByWorld.levels[i];
                if (currentLoadedSavedLevelData.ChallengeCompletion.Length != data.GetLevelChallenges(false, null).NumberOfChallenges)
                {
                    List<bool> challengeCompletion = new List<bool>();
                    var c = data.GetLevelChallenges(false, null);

                    for (int j = 0; j < c.NumberOfChallenges; j++)
                    {
                        if (currentLoadedSavedLevelData.ChallengeCompletion.Length > j && currentLoadedSavedLevelData.ChallengeCompletion[j])
                        {
                            challengeCompletion.Add(true);
                        }
                        else
                        {
                            challengeCompletion.Add(false);
                        }
                    }
                    currentLoadedSavedLevelData.ChallengeCompletion = challengeCompletion.ToArray();


                }
                return;
            }

        }
        Debug.LogError("No saved level data found to load for: " + data.LevelName + " in world: " + data.levelWorldAndNumber.x);

    }
    public LevelSavedData ReturnLevelSavedData()
    {
        if (currentLoadedSavedLevelData == null)
        {
            Debug.LogError("No current loaded saved level data.");
            return null;
        }
        return currentLoadedSavedLevelData;
    }

    public void SaveLevelDataForLevelComplete()
    {
        var data = ReturnLevelData();
        if (data == null)
        {
            Debug.LogError("No level data provided to save for level complete.");
            return;
        }
        var challenges = data.GetLevelChallenges(false, null);
        if (challenges == null || currentLoadedSavedLevelData == null)
        {
            Debug.LogError("No challenges or current loaded saved level data to save for level complete.");
            return;
        }

        if (challenges.LevelDifficulty == 0)
        {
            currentLoadedSavedLevelData.CompletedLevelEasy = true;
            currentLoadedSavedLevelData.FurthestCompletionEasy = data.finalSpawnStep;

        }
        else
        {
            Debug.Log("Saving level complete for normal difficulty: " + data.LevelName);
            currentLoadedSavedLevelData.CompletedLevel = true;
            currentLoadedSavedLevelData.FurthestCompletion = data.finalSpawnStep;
            List<bool> challengeCompletion = new List<bool>();
            bool hasCompletedAllChallenges = true;

            for (int i = 0; i < challenges.NumberOfChallenges; i++)
            {
                if (challenges.ReturnChallengeCompletion(i, true) <= 0) hasCompletedAllChallenges = false;
                if (currentLoadedSavedLevelData.ChallengeCompletion.Length > i && currentLoadedSavedLevelData.ChallengeCompletion[i])
                {
                    challengeCompletion.Add(true);
                }
                else if (challenges.ReturnChallengeCompletion(i, true) > 0)
                {
                    challengeCompletion.Add(true);
                }
                else challengeCompletion.Add(false);
            }

            currentLoadedSavedLevelData.ChallengeCompletion = challengeCompletion.ToArray();

            if (hasCompletedAllChallenges) currentLoadedSavedLevelData.MasteredLevel = true;

            // for (int i = 0; i < currentLoadedSavedLevelData.ChallengeCompletion.Length; i++)
            // {
            //     if (currentLoadedSavedLevelData.ChallengeCompletion[i])
            //     {
            //         Debug.Log("Challenge " + i + " completed.");
            //     }
            //     else
            //     {
            //         Debug.Log("Challenge " + i + " not completed.");
            //     }
            // }

        }
        AddUnlockedItem(data.unlockItemOnComplete, data.levelWorldAndNumber.x, true);

        SavePermanentLevelData();



    }

    public void SavePermanentLevelData(int levelWorld = -1)
    {
        string json = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
        string savePath = "";
        if (levelWorld >= 0)
            savePath = Path.Combine(GetPermanentLevelSaveDirectory(levelWorld), ".json");
        else
            savePath = Path.Combine(GetPermanentLevelSaveDirectory(currentLoadedSavedLevelDataByWorld.LevelWorld), ".json");

        File.WriteAllText(savePath, json);

        Debug.LogError("Saved level data to: " + savePath);
        Debug.LogError("Saved Level Unlocked item is: " + currentLoadedSavedLevelDataByWorld.NextUnlockedItem);
        for (int i = 0; i < currentLoadedSavedLevelDataByWorld.unlockedMapItems.Count; i++)
        {
            Debug.LogError("Unlocked Map Item of : " + i + " is: " + currentLoadedSavedLevelDataByWorld.unlockedMapItems[i]);
        }
    }
    public void SaveLevelDataForDeath(int difficulty, ushort step)
    {
        if (currentLoadedSavedLevelData == null)
        {
            Debug.LogError("No current loaded saved level data to save for death.");
            return;
        }
        bool overwrite = false;


        if (difficulty >= 1 && step > currentLoadedSavedLevelData.FurthestCompletion)
        {
            currentLoadedSavedLevelData.FurthestCompletion = step;
            if (step > currentLoadedSavedLevelData.FurthestCompletionEasy)
                currentLoadedSavedLevelData.FurthestCompletionEasy = step;
            Debug.Log("Saving furthest completion for normal difficulty: " + step);
            overwrite = true;

        }
        else if (difficulty == 0 && step > currentLoadedSavedLevelData.FurthestCompletionEasy)
        {

            currentLoadedSavedLevelData.FurthestCompletionEasy = step;

            Debug.Log("Saving furthest completion for easy difficulty: " + step);
            overwrite = true;


        }

        if (overwrite)
        {
            SavePermanentLevelData();
        }


    }

    public Vector3Int CurrentFurthestLevel()
    {
        if (currentLoadedSavedLevelDataByWorld.levels == null)
        {

            return new Vector3Int(1, 1, 0);
        }
        foreach (var level in currentLoadedSavedLevelDataByWorld.levels)
        {
            if (!level.CompletedLevel)
            {
                return new Vector3Int(currentLoadedSavedLevelDataByWorld.LevelWorld, level.LevelNumber.x, level.LevelNumber.y);
            }
        }
        return new Vector3Int(1, 15, 0);


    }

    public bool CheckIfLevelCompleted(int indexCheck, Vector2Int levelNumber)
    {
        if (currentLoadedSavedLevelDataByWorld == null)
        {
            Debug.LogError("No current loaded saved level data by world to check level completion.");
            return false;
        }
        if (currentLoadedSavedLevelDataByWorld.levels == null || currentLoadedSavedLevelDataByWorld.levels.Length <= indexCheck)
        {
            return false;
        }
        if (currentLoadedSavedLevelDataByWorld.levels[indexCheck].LevelNumber == levelNumber)
        {
            return currentLoadedSavedLevelDataByWorld.levels[indexCheck].CompletedLevel;
        }
        else
        {
            Debug.LogError("Level number mismatch at index: " + indexCheck + ". Expected: " + levelNumber + ", Found: " + currentLoadedSavedLevelDataByWorld.levels[indexCheck].LevelNumber);

            for (int i = 0; i < currentLoadedSavedLevelDataByWorld.levels.Length; i++)
            {
                if (currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber == levelNumber)
                {
                    Debug.LogError("Found matching level number at index: " + i);
                    return currentLoadedSavedLevelDataByWorld.levels[i].CompletedLevel;
                }
            }
            return false;
        }
    }
    public short GetNextUnlockedItem(ushort worldIndex)
    {
        if (currentLoadedSavedLevelDataByWorld != null)
        {
            if (currentLoadedSavedLevelDataByWorld.unlockedMapItems == null || currentLoadedSavedLevelDataByWorld.unlockedMapItems.Count == 0)
            {
                Debug.LogError("No unlocked map items found for world: " + worldIndex);
                return -1;
            }
            return currentLoadedSavedLevelDataByWorld.NextUnlockedItem;
        }
        else return -1;



    }
    public void ResetNextUnlockedIndex(int worldIndex)
    {
        if (currentLoadedSavedLevelDataByWorld == null)
        {
            Debug.LogError("No current loaded saved level data by world to reset next unlocked index.");
            return;
        }
        currentLoadedSavedLevelDataByWorld.AddUnlockableMapItems(-1);
        SavePermanentLevelData(worldIndex);
    }

    public void AddUnlockedItem(short itemID, int worldIndex, bool ignoreSave = false)
    {
        if (currentLoadedSavedLevelDataByWorld == null)
        {
            Debug.LogError("No current loaded saved level data by world to add unlocked item.");
            return;
        }
        currentLoadedSavedLevelDataByWorld.AddUnlockableMapItems(itemID);
        Debug.LogError("Added unlocked item: " + itemID + " to world: " + worldIndex);
        // if (!ignoreSave)
        SavePermanentLevelData(worldIndex);
    }

    public List<short> ReturnUnlockedItems()
    {
        if (currentLoadedSavedLevelDataByWorld == null)
        {
            Debug.LogError("No current loaded saved level data by world to return unlocked items.");
            return new List<short>();
        }
        return currentLoadedSavedLevelDataByWorld.unlockedMapItems;
    }

    public void LoadWorldData(int worldNumber)
    {
        string path = Path.Combine(GetPermanentLevelSaveDirectory(worldNumber), ".json");
        if (!File.Exists(path))
        {
            Debug.Log("No saved data found for world: " + worldNumber + ". Creating new save file." + " save path: " + path);

            currentLoadedSavedLevelDataByWorld = new SavedLevelDataByWorld((short)worldNumber);
            string json = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
            File.WriteAllText(path, json);
        }
        else
        {
            string json = File.ReadAllText(path);
            Debug.Log("Loading level data for world: " + worldNumber + " from path: " + path);
            currentLoadedSavedLevelDataByWorld = JsonUtility.FromJson<SavedLevelDataByWorld>(json);
        }
    }

    public LevelSavedData ReturnAndLoadWorldLevelData(LevelData data, int world = 0)
    {
        // if (world > 0)
        // {
        //     string path = Path.Combine(GetPermanentLevelSaveDirectory(world), ".json");
        //     if (!File.Exists(path))
        //     {



        //         // if (data == null && world > 0)
        //         //     currentLoadedSavedLevelDataByWorld = new SavedLevelDataByWorld((short)world);
        //         // else
        //         //     currentLoadedSavedLevelDataByWorld = new SavedLevelDataByWorld((short)data.levelWorldAndNumber.x);

        //         // string json = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
        //         // File.WriteAllText(path, json);
        //         Debug.LogError("No saved data found for world: " + world + ". Creating new save file." + " save path: " + path);
        //         return null;


        //     }
        //     else
        //     {
        //         string json = File.ReadAllText(path);
        //         Debug.LogError("Loading level data for world: " + world + " from path: " + path);
        //         currentLoadedSavedLevelDataByWorld = JsonUtility.FromJson<SavedLevelDataByWorld>(json);


        //     }
        //     return null;
        // }
        if (currentLoadedSavedLevelDataByWorld == null)
        {
            Debug.Log("No saved data loaded yet for world: " + data.levelWorldAndNumber.x + ". Loading now.");
            return null;
            // string path = Path.Combine(GetPermanentLevelSaveDirectory(data.levelWorldAndNumber.x), ".json");
            // if (!File.Exists(path))
            // {
            //     Debug.Log("No saved data found for world: " + data.levelWorldAndNumber.x + ". Creating new save file." + " save path: " + path);

            //     LevelSavedData save = new LevelSavedData(data);

            //     currentLoadedSavedLevelDataByWorld = new SavedLevelDataByWorld((short)data.levelWorldAndNumber.x);
            //     currentLoadedSavedLevelDataByWorld.AddLevel(save);
            //     string json = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
            //     File.WriteAllText(path, json);


            // }
            // else
            // {
            //     string json = File.ReadAllText(path);
            //     currentLoadedSavedLevelDataByWorld = JsonUtility.FromJson<SavedLevelDataByWorld>(json);


            // }
        }

        else if (currentLoadedSavedLevelDataByWorld != null && currentLoadedSavedLevelDataByWorld.LevelWorld != (short)data.levelWorldAndNumber.x)
        {
            string path = Path.Combine(GetPermanentLevelSaveDirectory(data.levelWorldAndNumber.x), ".json");
            if (!File.Exists(path))
            {
                Debug.Log("No saved data found for world Seocnd: " + data.levelWorldAndNumber.x + ". Creating new save file.");

                LevelSavedData save = new LevelSavedData(data);

                // currentLoadedSavedLevelDataByWorld = new SavedLevelDataByWorld((short)data.levelWorldAndNumber.x);
                currentLoadedSavedLevelDataByWorld.AddLevel(save);
                string json = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
                File.WriteAllText(path, json);


            }
            else
            {
                string json = File.ReadAllText(path);
                currentLoadedSavedLevelDataByWorld = JsonUtility.FromJson<SavedLevelDataByWorld>(json);


            }
        }

        if (currentLoadedSavedLevelDataByWorld.levels != null && currentLoadedSavedLevelDataByWorld.levels.Length > 0)
        {
            Debug.Log("Checking for existing level data for: " + data.LevelName + " in world: " + data.levelWorldAndNumber.x);
            for (int i = 0; i < currentLoadedSavedLevelDataByWorld.levels.Length; i++)
            {
                Debug.Log("Checking level: " + currentLoadedSavedLevelDataByWorld.levels[i].LevelName + " in world: " + currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x + "-" + currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y);
                // if (currentLoadedSavedLevelDataByWorld.levels[i].LevelName == data.LevelName && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x == data.levelWorldAndNumber.y && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y == data.levelWorldAndNumber.z)
                if (currentLoadedSavedLevelDataByWorld.levels[i].LevelName == data.LevelName)
                {
                    Debug.Log("FOund correct level, level number: " + currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x + data.levelWorldAndNumber.y + "-" + currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y + data.levelWorldAndNumber.z);
                    Debug.Log("Found existing level data for: " + data.LevelName);
                    return currentLoadedSavedLevelDataByWorld.levels[i];
                }


            }
        }
        else Debug.Log("No existing level data found for: " + data.LevelName + " in world: " + data.levelWorldAndNumber.x);

        string path2 = Path.Combine(GetPermanentLevelSaveDirectory(currentLoadedSavedLevelDataByWorld.LevelWorld), ".json");
        LevelSavedData newLevel = new LevelSavedData(data);
        Debug.Log("No saved data found for world third: " + data.levelWorldAndNumber.x + ". Creating new save file.");

        currentLoadedSavedLevelDataByWorld.AddLevel(newLevel);
        string json2 = JsonUtility.ToJson(currentLoadedSavedLevelDataByWorld, true);
        File.WriteAllText(path2, json2);
        return newLevel;



    }
    public bool CheckIfCompletedLevel(Vector3Int num, int difficulty = 1)
    {
        if (currentLoadedSavedLevelDataByWorld == null || currentLoadedSavedLevelDataByWorld.levels == null || currentLoadedSavedLevelDataByWorld.levels.Length == 0)
        {
            Debug.LogError("No saved level data found for world: " + num.x);
            return false;
        }

        if (num.x != currentLoadedSavedLevelDataByWorld.LevelWorld)
        {
            return false;
        }

        for (int i = 0; i < currentLoadedSavedLevelDataByWorld.levels.Length; i++)
        {
            if (currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x == num.y && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y == num.z)
            {
                if (difficulty >= 1)
                    return currentLoadedSavedLevelDataByWorld.levels[i].CompletedLevel;
                else
                    return currentLoadedSavedLevelDataByWorld.levels[i].CompletedLevelEasy;
            }
        }

        return false;
    }

    public LevelSavedData ReturnCompletedChallengesForLevel(Vector3Int num)
    {
        if (currentLoadedSavedLevelDataByWorld == null || currentLoadedSavedLevelDataByWorld.levels == null || currentLoadedSavedLevelDataByWorld.levels.Length == 0)
        {
            Debug.LogError("No saved level data found for world: " + num.x);
            return null;
        }
        if (num.x != currentLoadedSavedLevelDataByWorld.LevelWorld)
        {

            return null;
        }

        for (int i = 0; i < currentLoadedSavedLevelDataByWorld.levels.Length; i++)
        {
            if (currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.x == num.y && currentLoadedSavedLevelDataByWorld.levels[i].LevelNumber.y == num.z)
            {
                Debug.Log("Found saved level data for: " + currentLoadedSavedLevelDataByWorld.levels[i].LevelName);
                return currentLoadedSavedLevelDataByWorld.levels[i];
            }
        }

        var d = ReturnLevelDataByNum(num);

        if (num != null)
        {
            var save = new LevelSavedData(d);
            currentLoadedSavedLevelDataByWorld.AddLevel(save);
            SavePermanentLevelData();
            return save;

        }
        return null;






    }



    #endregion

    #region Steroid Data

    public string GetSteroidDataSaveDirectory()
    {

        string directory = Path.Combine(Application.persistentDataPath, "SteroidDataSaves");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Debug.Log("Created save directory: " + directory);
        }

        return directory;
    }

    public void SaveSteroidData(ushort[] equippedSteroids = null, int lastEditedIndex = -1, int addedUnlockedSteroid = -1)
    {
        // Use a writeable path and build a valid file path
        string path = Path.Combine(Application.persistentDataPath, "SteroidData.json");

        try
        {
            SteroidSavedData data;

            // If file doesn't exist (or is empty), create a new one with defaults
            if (!File.Exists(path) || new FileInfo(path).Length == 0)
            {
                data = new SteroidSavedData();
            }
            else
            {
                string existing = File.ReadAllText(path);
                data = string.IsNullOrWhiteSpace(existing)
                    ? new SteroidSavedData()
                    : JsonUtility.FromJson<SteroidSavedData>(existing) ?? new SteroidSavedData();
            }

            // Ensure arrays are non-null
            if (data.equippedSteroids == null) data.equippedSteroids = new ushort[0];
            if (data.unlockedSteroids == null) data.unlockedSteroids = new ushort[0];
            if (data.lastEditedIndex >= 0) data.lastEditedIndex = (ushort)lastEditedIndex;

            // Apply changes
            if (equippedSteroids != null)
            {
                data.equippedSteroids = (ushort[])equippedSteroids.Clone();
            }

            if (addedUnlockedSteroid >= 0)
            {
                var unlockedList = new List<ushort>(data.unlockedSteroids);
                if (!unlockedList.Contains((ushort)addedUnlockedSteroid))
                    unlockedList.Add((ushort)addedUnlockedSteroid);
                data.unlockedSteroids = unlockedList.ToArray();
            }

            // Write back to disk (pretty-printed)
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

#if UNITY_EDITOR
            Debug.Log($"Steroid data saved: {path}");
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save steroid data: {ex.Message}");
        }
    }


    public SteroidSavedData LoadSteroidData()
    {
        string path = Path.Combine(Application.persistentDataPath, "SteroidData.json");

        try
        {
            // If the file doesn't exist, create it with default data
            if (!File.Exists(path))
            {
                var newData = new SteroidSavedData();
                string json = JsonUtility.ToJson(newData, true);
                File.WriteAllText(path, json);
#if UNITY_EDITOR
                Debug.Log($"Created new SteroidData file at: {path}");
#endif
                return newData;
            }

            // Read and parse file
            string jsonText = File.ReadAllText(path);

            // Handle empty or invalid file contents
            if (string.IsNullOrWhiteSpace(jsonText))
            {
                var newData = new SteroidSavedData();
                File.WriteAllText(path, JsonUtility.ToJson(newData, true));
                return newData;
            }

            SteroidSavedData data = JsonUtility.FromJson<SteroidSavedData>(jsonText);

            // If parsing failed or data is null, recreate file
            if (data == null)
            {
                data = new SteroidSavedData();
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
#if UNITY_EDITOR
                Debug.LogWarning("Steroid data file was corrupted or invalid — recreated default file.");
#endif
            }

            // Ensure no null arrays
            if (data.equippedSteroids == null) data.equippedSteroids = new ushort[0];
            if (data.unlockedSteroids == null) data.unlockedSteroids = new ushort[0];

            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading SteroidData: {ex.Message}");
            var fallback = new SteroidSavedData();
            File.WriteAllText(path, JsonUtility.ToJson(fallback, true));
            return fallback;
        }
    }

    #endregion


}
