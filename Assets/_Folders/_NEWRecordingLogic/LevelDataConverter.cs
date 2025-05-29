using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelDataConverter : MonoBehaviour
{
    public static LevelDataConverter instance;
    private UserCreatedLevels userCreatedLevels;
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
        // DeleteAllLevelsAndClearDirectories();


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

    public void ConvertDataToJson(List<RecordableObjectPlacer> obj, string title, int[] poolSizes, ushort finalSpawnStep, short[] ammos, short lives)
    {
        string savePath = Path.Combine(GetSaveDirectory(), title + ".json");
        List<short> idList = new List<short>();
        List<ushort> spawnedStepList = new List<ushort>();
        List<ushort> typeList = new List<ushort>();
        List<Vector2> posList = new List<Vector2>();
        List<float> floatList = new List<float>();
        List<ushort> dataTypes = new List<ushort>();
        List<RecordedObjectPositionerDataSave> positionerData = new List<RecordedObjectPositionerDataSave>();


        for (int i = 0; i < obj.Count; i++)
        {
            idList.Add(obj[i].Data.ID);
            spawnedStepList.Add(obj[i].Data.spawnedStep);

            if (obj[i].IsPositionerObject())
            {
                positionerData.Add(obj[i].ReturnPositionerDataSave());

            }
            typeList.Add(obj[i].Data.type);
            posList.Add(obj[i].Data.startPos);
            dataTypes.Add(obj[i].DataType);
            int dataType = obj[i].DataType;
            if (dataType <= 5)
            {
                float[] values = new float[dataType];
                for (int j = 0; j < dataType; j++)
                {
                    switch (j)
                    {
                        case 0:
                            values[j] = obj[i].Data.float1;
                            break;
                        case 1:
                            values[j] = obj[i].Data.float2;
                            break;
                        case 2:
                            values[j] = obj[i].Data.float3;
                            break;
                        case 3:
                            values[j] = obj[i].Data.float4;
                            break;
                        case 4:
                            values[j] = obj[i].Data.float5;
                            break;
                    }
                }

                for (int n = 0; n < values.Length; n++)
                {
                    floatList.Add(values[n]);
                }

            }

        }

        ushort[] poolData = new ushort[poolSizes.Length];
        for (int i = 0; i < poolSizes.Length; i++)
        {
            poolData[i] = (ushort)poolSizes[i];
        }

        LevelDataSave save = new LevelDataSave(title, idList.ToArray(), spawnedStepList.ToArray(), typeList.ToArray(), dataTypes.ToArray(), finalSpawnStep, posList.ToArray(), floatList.ToArray(), poolData, ammos, lives);
        if (positionerData.Count > 0)
            save.SetPositionerData(positionerData.ToArray());


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
    public List<RecordedDataStructDynamic> ReturnDynamicData(LevelData data)
    {

        int floatIndex = 0;

        var l = new List<RecordedDataStructDynamic>();
        if (data.idList == null || data.idList.Length == 0)
        {
            Debug.LogError("No data found in the loaded LevelData.");
            return l; // Return an empty list if no data is found
        }

        for (int i = 0; i < data.idList.Length; i++)
        {
            float[] values = new float[5];
            int type = data.dataTypes[i];
            Debug.Log("Loading Data Type: " + type);
            if (type <= 5)
            {
                for (int j = 0; j < type; j++)
                {
                    values[j] = data.floatList[floatIndex];
                    floatIndex++;
                }
                l.Add(new RecordedDataStructDynamic(data.idList[i], data.typeList[i], data.posList[i], values[0], values[1], values[2], values[3], values[4], data.spawnSteps[i], 0));
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



    // public void ConvertDataToArrays(List<RecordableObjectPlacer> obj, LevelData so)
    // {
    //     List<short> idList = new List<short>();
    //     List<ushort> spawnedStepList = new List<ushort>();
    //     List<Vector2> posList = new List<Vector2>();
    //     List<float> speedList = new List<float>();
    //     List<float> scaleList = new List<float>();
    //     List<float> magList = new List<float>();
    //     List<float> timeList = new List<float>();
    //     List<float> delayList = new List<float>();
    //     List<short> typeList = new List<short>();

    //     for (int i = 0; i < obj.Count; i++)
    //     {
    //         var data = obj[i].Data;
    //         idList.Add(data.ID);
    //         spawnedStepList.Add(data.spawnedStep);
    //         posList.Add(data.startPos);
    //         typeList.Add(data.type);

    //         switch (data.ID)
    //         {
    //             case -1:
    //             case -2:// Ring (Speed, Size, Time, Type)
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 timeList.Add(data.timeInterval);


    //                 break;
    //             case 0:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);




    //                 break;
    //             case 1:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);




    //                 break;
    //             case 2:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);



    //                 break;
    //             case 3:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);




    //                 break;
    //             case 4:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);




    //                 break;
    //             case 5:
    //                 speedList.Add(data.speed);

    //                 break;
    //             case 6: //Bomber (size, type)
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 7: // Flappy (Scale, Type)
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 8: // Gas Ground (Speed, Delay, Time)

    //                 speedList.Add(data.speed);
    //                 timeList.Add(data.timeInterval);
    //                 delayList.Add(data.delayInterval);



    //                 break;

    //             case 9:
    //                 timeList.Add(data.timeInterval);



    //                 break;

    //             case 10: // Balloon (Speed, Mag, Time, Delay)
    //                 speedList.Add(data.speed);
    //                 magList.Add(data.magPercent);
    //                 timeList.Add(data.timeInterval);
    //                 delayList.Add(data.delayInterval);
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 11: // Windmill (Speed, Size, Delay)                 
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 delayList.Add(data.delayInterval);


    //                 break;
    //             case 12: // Barn (Type)

    //                 break;


    //             case 15: // Egg Ammo (Speed, Mag, Delay, Type)
    //             case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
    //                 speedList.Add(data.speed);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);
    //                 timeList.Add(data.timeInterval);
    //                 scaleList.Add(data.scale);


    //                 break;

    //         }


    //         so.spawnSteps = new ushort[spawnedStepList.Count];
    //         so.spawnSteps = spawnedStepList.ToArray();
    //         so.idList = new short[idList.Count];
    //         so.idList = idList.ToArray();
    //         so.posList = new Vector2[posList.Count];
    //         so.posList = posList.ToArray();
    //         so.speedList = new float[speedList.Count];
    //         so.speedList = speedList.ToArray();
    //         so.scaleList = new float[scaleList.Count];
    //         so.scaleList = scaleList.ToArray();
    //         so.magList = new float[magList.Count];
    //         so.magList = magList.ToArray();
    //         so.timeList = new float[timeList.Count];
    //         so.timeList = timeList.ToArray();
    //         so.delayList = new float[delayList.Count];
    //         so.delayList = delayList.ToArray();
    //         so.typeList = new short[typeList.Count];
    //         so.typeList = typeList.ToArray();

    //     }

    // }

    // public void ConvertDataToJson(List<RecordableObjectPlacer> obj, string title, int[] poolSizes, ushort finalSpawnStep)
    // {
    //     string savePath = Path.Combine(GetSaveDirectory(), title + ".json");
    //     List<short> idList = new List<short>();
    //     List<ushort> spawnedStepList = new List<ushort>();
    //     List<Vector2> posList = new List<Vector2>();
    //     List<float> speedList = new List<float>();
    //     List<float> scaleList = new List<float>();
    //     List<float> magList = new List<float>();
    //     List<float> timeList = new List<float>();
    //     List<float> delayList = new List<float>();
    //     List<short> typeList = new List<short>();

    //     for (int i = 0; i < obj.Count; i++)
    //     {
    //         var data = obj[i].Data;
    //         idList.Add(data.ID);
    //         spawnedStepList.Add(data.spawnedStep);
    //         posList.Add(data.startPos);
    //         typeList.Add(data.type);

    //         switch (data.ID)
    //         {
    //             case -1:
    //             case -2:// Ring (Speed, Size, Time, Type)
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 timeList.Add(data.timeInterval);


    //                 break;
    //             case 0:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);




    //                 break;
    //             case 1:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);




    //                 break;
    //             case 2:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);



    //                 break;
    //             case 3:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);




    //                 break;
    //             case 4:
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);




    //                 break;
    //             case 5:
    //                 speedList.Add(data.speed);

    //                 break;
    //             case 6: //Bomber (size, type)
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 7: // Flappy (Scale, Type)
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 8: // Gas Ground (Speed, Delay, Time)

    //                 speedList.Add(data.speed);
    //                 timeList.Add(data.timeInterval);
    //                 delayList.Add(data.delayInterval);


    //                 break;
    //             case 9:
    //                 timeList.Add(data.timeInterval);
    //                 break;

    //             case 10: // Balloon (Speed, Mag, Time, Delay)
    //                 speedList.Add(data.speed);
    //                 magList.Add(data.magPercent);
    //                 timeList.Add(data.timeInterval);
    //                 delayList.Add(data.delayInterval);
    //                 scaleList.Add(data.scale);

    //                 break;
    //             case 11: // Windmill (Speed, Size, Delay)                 
    //                 speedList.Add(data.speed);
    //                 scaleList.Add(data.scale);
    //                 delayList.Add(data.delayInterval);


    //                 break;
    //             case 12: // Barn (Type)

    //                 break;


    //             case 15: // Egg Ammo (Speed, Mag, Delay, Type)
    //             case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
    //                 speedList.Add(data.speed);
    //                 magList.Add(data.magPercent);
    //                 delayList.Add(data.delayInterval);
    //                 timeList.Add(data.timeInterval);
    //                 scaleList.Add(data.scale);


    //                 break;

    //         }




    //     }

    //     ushort[] poolData = new ushort[poolSizes.Length];
    //     for (int i = 0; i < poolSizes.Length; i++)
    //     {
    //         poolData[i] = (ushort)poolSizes[i];
    //     }

    //     LevelDataSave save = new LevelDataSave(title, "none", idList.ToArray(), spawnedStepList.ToArray(), finalSpawnStep, posList.ToArray(), speedList.ToArray(), scaleList.ToArray(), magList.ToArray(), timeList.ToArray(), delayList.ToArray(), typeList.ToArray(), poolData);
    //     string json = JsonUtility.ToJson(save, true);
    //     File.WriteAllText(savePath, json);
    //     Debug.Log("Level saved to: " + savePath);
    // }



    //short id, Vector2 pos, float speed, float scale, float mag, float time, float delay, short type , ushort ss, ushort us)
    // public List<RecordedDataStructDynamic> ReturnDynamicData(LevelData data)
    // {

    //     int speedIndex = 0;
    //     int scaleIndex = 0;
    //     int magIndex = 0;
    //     int timeIndex = 0;
    //     int delayIndex = 0;
    //     int typeIndex = 0;

    //     var l = new List<RecordedDataStructDynamic>();

    //     for (int i = 0; i < data.idList.Length; i++)
    //     {
    //         switch (data.idList[i])
    //         {
    //             case -1:
    //             case -2: // Ring (Speed, Size, Time, Type)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], 0, data.timeList[timeIndex], 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 timeIndex++;

    //                 break;
    //             case 0:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 1:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 2:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 3:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 break;
    //             case 4:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 5:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                 data.posList[i],
    //                 data.speedList[speedIndex],
    //                 0, 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 typeIndex++;
    //                 break;
    //             case 6: //Bomber (size, type)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                 data.posList[i],
    //                 0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 scaleIndex++;
    //                 break;
    //             case 7: // Flappy (Scale, Type) 
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                 data.posList[i],
    //                 0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 scaleIndex++;
    //                 break;
    //             case 8: // Gas Ground (Speed, Delay, Time)

    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                                        data.posList[i],
    //                  data.speedList[speedIndex], 0, 0, data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 timeIndex++;
    //                 delayIndex++;

    //                 break;
    //             case 9:
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                                                               data.posList[i],
    //                                                               0, 0, 0, data.timeList[timeIndex], 0, data.typeList[i], data.spawnSteps[i], 0));
    //                 timeIndex++;
    //                 break;

    //             case 10: // Balloon (Speed, Mag, Time, Delay)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 magIndex++;
    //                 timeIndex++;
    //                 delayIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 11: // Windmill (Speed, Size, Delay)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 12: // Barn (Type)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                     data.posList[i],
    //                     0, 0, 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));

    //                 break;

    //             case 15: // Egg Ammo (Speed, Mag, Delay, Type)
    //             case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
    //                 l.Add(new RecordedDataStructDynamic(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
    //                 speedIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 scaleIndex++;
    //                 timeIndex++;

    //                 break;


    //         }
    //     }
    //     return l;


    // }

    // public RecordedDataStruct[] ReturnLevelData(LevelData data)
    // {

    //     int speedIndex = 0;
    //     int scaleIndex = 0;
    //     int magIndex = 0;
    //     int timeIndex = 0;
    //     int delayIndex = 0;


    //     var l = new List<RecordedDataStruct>();

    //     for (int i = 0; i < data.idList.Length; i++)
    //     {

    //         switch (data.idList[i])
    //         {
    //             case -1:
    //             case -2: // Ring (Speed, Size, Time, Type)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], 0, data.timeList[timeIndex], 0, data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 timeIndex++;
    //                 break;
    //             case 0:
    //                 l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 1:
    //                 l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 2:
    //                 l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 3:
    //                 l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, 0, data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 break;
    //             case 4:
    //                 l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 5:
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                 data.posList[i],
    //                 data.speedList[speedIndex],
    //                 0, 0, 0, 0, data.typeList[i]));
    //                 speedIndex++;

    //                 break;
    //             case 6: //Bomber (size, type)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                 data.posList[i],
    //                 0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
    //                 scaleIndex++;
    //                 break;
    //             case 7: // Flappy (Scale, Type)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                 data.posList[i],
    //                 0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
    //                 scaleIndex++;
    //                 break;
    //             case 8: // Gas Ground (Speed, Delay, Time)


    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                                        data.posList[i],
    //                                        data.speedList[speedIndex], 0, 0, data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 timeIndex++;
    //                 delayIndex++;




    //                 break;
    //             case 9:
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                                                              data.posList[i],
    //                                                              0, 0, 0, data.timeList[timeIndex], 0, data.typeList[i]));
    //                 timeIndex++;

    //                 break;


    //             case 10: // Balloon (Speed, Mag, Time, Delay)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 magIndex++;
    //                 timeIndex++;
    //                 delayIndex++;
    //                 scaleIndex++;
    //                 break;
    //             case 11: // Windmill (Speed, Size, Delay)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 scaleIndex++;
    //                 delayIndex++;
    //                 break;
    //             case 12: // Barn (Type)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                     data.posList[i],
    //                     0, 0, 0, 0, 0, data.typeList[i]));

    //                 break;



    //             case 15: // Egg Ammo (Speed, Mag, Delay, Type)
    //             case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
    //                 l.Add(new RecordedDataStruct(data.idList[i],
    //                     data.posList[i],
    //                     data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i]));
    //                 speedIndex++;
    //                 magIndex++;
    //                 delayIndex++;
    //                 scaleIndex++;
    //                 timeIndex++;

    //                 break;


    //         }
    //     }
    //     return l.ToArray();

    // }

    public static void CreateLevelDataFile(List<RecordedDataStruct> data, string levelName, string worldAndLevelNumber = null)
    {

    }

}
