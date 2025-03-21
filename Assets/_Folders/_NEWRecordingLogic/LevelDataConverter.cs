using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelDataConverter : MonoBehaviour
{
    public static LevelDataConverter instance;
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
    }



    public void ConvertDataToArrays(List<RecordableObjectPlacer> obj, LevelData so)
    {
        List<short> idList = new List<short>();
        List<ushort> spawnedStepList = new List<ushort>();
        List<Vector2> posList = new List<Vector2>();
        List<float> speedList = new List<float>();
        List<float> scaleList = new List<float>();
        List<float> magList = new List<float>();
        List<float> timeList = new List<float>();
        List<float> delayList = new List<float>();
        List<short> typeList = new List<short>();

        for (int i = 0; i < obj.Count; i++)
        {
            var data = obj[i].Data;
            idList.Add(data.ID);
            spawnedStepList.Add(data.spawnedStep);
            posList.Add(data.startPos);
            typeList.Add(data.type);

            switch (data.ID)
            {
                case 0:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);




                    break;
                case 1:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);




                    break;
                case 2:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);



                    break;
                case 3:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);




                    break;
                case 4:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);




                    break;
                case 5:
                    speedList.Add(data.speed);

                    break;
                case 6: //Bomber (size, type)
                    scaleList.Add(data.scale);

                    break;
                case 7: // Flappy (Scale, Type)
                    scaleList.Add(data.scale);

                    break;
                case 8: // Gas Ground (Speed, Delay, Time)

                    speedList.Add(data.speed);
                    timeList.Add(data.timeInterval);
                    delayList.Add(data.delayInterval);



                    break;

                case 9:
                    timeList.Add(data.timeInterval);



                    break;

                case 10: // Balloon (Speed, Mag, Time, Delay)
                    speedList.Add(data.speed);
                    magList.Add(data.magPercent);
                    timeList.Add(data.timeInterval);
                    delayList.Add(data.delayInterval);
                    scaleList.Add(data.scale);

                    break;
                case 11: // Windmill (Speed, Size, Delay)                 
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    delayList.Add(data.delayInterval);


                    break;
                case 12: // Barn (Type)

                    break;
                case 13:
                case 14:// Ring (Speed, Size, Time, Type)
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    timeList.Add(data.timeInterval);


                    break;

                case 15: // Egg Ammo (Speed, Mag, Delay, Type)
                case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
                    speedList.Add(data.speed);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);


                    break;

            }


            so.spawnSteps = new ushort[spawnedStepList.Count];
            so.spawnSteps = spawnedStepList.ToArray();
            so.idList = new short[idList.Count];
            so.idList = idList.ToArray();
            so.posList = new Vector2[posList.Count];
            so.posList = posList.ToArray();
            so.speedList = new float[speedList.Count];
            so.speedList = speedList.ToArray();
            so.scaleList = new float[scaleList.Count];
            so.scaleList = scaleList.ToArray();
            so.magList = new float[magList.Count];
            so.magList = magList.ToArray();
            so.timeList = new float[timeList.Count];
            so.timeList = timeList.ToArray();
            so.delayList = new float[delayList.Count];
            so.delayList = delayList.ToArray();
            so.typeList = new short[typeList.Count];
            so.typeList = typeList.ToArray();

        }

    }

    public void ConvertDataToJson(List<RecordableObjectPlacer> obj, string title)
    {
        string savePath = Path.Combine(GetSaveDirectory(), title + ".json");
        List<short> idList = new List<short>();
        List<ushort> spawnedStepList = new List<ushort>();
        List<Vector2> posList = new List<Vector2>();
        List<float> speedList = new List<float>();
        List<float> scaleList = new List<float>();
        List<float> magList = new List<float>();
        List<float> timeList = new List<float>();
        List<float> delayList = new List<float>();
        List<short> typeList = new List<short>();

        for (int i = 0; i < obj.Count; i++)
        {
            var data = obj[i].Data;
            idList.Add(data.ID);
            spawnedStepList.Add(data.spawnedStep);
            posList.Add(data.startPos);
            typeList.Add(data.type);

            switch (data.ID)
            {
                case 0:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);




                    break;
                case 1:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);




                    break;
                case 2:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);



                    break;
                case 3:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);




                    break;
                case 4:
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);




                    break;
                case 5:
                    speedList.Add(data.speed);

                    break;
                case 6: //Bomber (size, type)
                    scaleList.Add(data.scale);

                    break;
                case 7: // Flappy (Scale, Type)
                    scaleList.Add(data.scale);

                    break;
                case 8: // Gas Ground (Speed, Delay, Time)

                    speedList.Add(data.speed);
                    timeList.Add(data.timeInterval);
                    delayList.Add(data.delayInterval);


                    break;
                case 9:
                    timeList.Add(data.timeInterval);
                    break;

                case 10: // Balloon (Speed, Mag, Time, Delay)
                    speedList.Add(data.speed);
                    magList.Add(data.magPercent);
                    timeList.Add(data.timeInterval);
                    delayList.Add(data.delayInterval);
                    scaleList.Add(data.scale);

                    break;
                case 11: // Windmill (Speed, Size, Delay)                 
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    delayList.Add(data.delayInterval);


                    break;
                case 12: // Barn (Type)

                    break;
                case 13:
                case 14:// Ring (Speed, Size, Time, Type)
                    speedList.Add(data.speed);
                    scaleList.Add(data.scale);
                    timeList.Add(data.timeInterval);


                    break;

                case 15: // Egg Ammo (Speed, Mag, Delay, Type)
                case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
                    speedList.Add(data.speed);
                    magList.Add(data.magPercent);
                    delayList.Add(data.delayInterval);


                    break;

            }




        }

        LevelDataSave save = new LevelDataSave(title, "none", idList.ToArray(), spawnedStepList.ToArray(), posList.ToArray(), speedList.ToArray(), scaleList.ToArray(), magList.ToArray(), timeList.ToArray(), delayList.ToArray(), typeList.ToArray());
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Level saved to: " + savePath);
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
    public LevelDataSave ConvertDataFromJson()
    {
        string loadPath = Path.Combine(Application.persistentDataPath, "Levels", LevelRecordManager.LevelPath + ".json");

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

    //short id, Vector2 pos, float speed, float scale, float mag, float time, float delay, short type , ushort ss, ushort us)
    public List<RecordedDataStructDynamic> ReturnDynamicData(LevelData data)
    {
        int mainIndex = 0;
        int speedIndex = 0;
        int scaleIndex = 0;
        int magIndex = 0;
        int timeIndex = 0;
        int delayIndex = 0;
        int typeIndex = 0;

        var l = new List<RecordedDataStructDynamic>();

        for (int i = 0; i < data.idList.Length; i++)
        {
            switch (data.idList[i])
            {
                case 0:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 1:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 2:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
                case 3:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    break;
                case 4:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
                case 5:
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                    data.posList[i],
                    data.speedList[speedIndex],
                    0, 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    typeIndex++;
                    break;
                case 6: //Bomber (size, type)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                    data.posList[i],
                    0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    scaleIndex++;
                    break;
                case 7: // Flappy (Scale, Type) 
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                    data.posList[i],
                    0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));
                    scaleIndex++;
                    break;
                case 8: // Gas Ground (Speed, Delay, Time)

                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                                           data.posList[i],
                     data.speedList[speedIndex], 0, 0, data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    timeIndex++;
                    delayIndex++;

                    break;
                case 9:
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                                                                  data.posList[i],
                                                                  0, 0, 0, data.timeList[timeIndex], 0, data.typeList[i], data.spawnSteps[i], 0));
                    timeIndex++;
                    break;

                case 10: // Balloon (Speed, Mag, Time, Delay)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    magIndex++;
                    timeIndex++;
                    delayIndex++;
                    scaleIndex++;
                    break;
                case 11: // Windmill (Speed, Size, Delay)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    delayIndex++;
                    break;
                case 12: // Barn (Type)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                        data.posList[i],
                        0, 0, 0, 0, 0, data.typeList[i], data.spawnSteps[i], 0));

                    break;
                case 13:
                case 14: // Ring (Speed, Size, Time, Type)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], 0, data.timeList[timeIndex], 0, data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    timeIndex++;

                    break;
                case 15: // Egg Ammo (Speed, Mag, Delay, Type)
                case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
                    l.Add(new RecordedDataStructDynamic(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], 0, data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i], data.spawnSteps[i], 0));
                    speedIndex++;
                    magIndex++;
                    delayIndex++;

                    break;


            }
        }
        return l;


    }

    public RecordedDataStruct[] ReturnLevelData(LevelData data)
    {

        int speedIndex = 0;
        int scaleIndex = 0;
        int magIndex = 0;
        int timeIndex = 0;
        int delayIndex = 0;


        var l = new List<RecordedDataStruct>();

        for (int i = 0; i < data.idList.Length; i++)
        {

            switch (data.idList[i])
            {
                case 0:
                    l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 1:
                    l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 2:
                    l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
                case 3:
                    l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, 0, data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    break;
                case 4:
                    l.Add(new RecordedDataStruct(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
                case 5:
                    l.Add(new RecordedDataStruct(data.idList[i],
                    data.posList[i],
                    data.speedList[speedIndex],
                    0, 0, 0, 0, data.typeList[i]));
                    speedIndex++;

                    break;
                case 6: //Bomber (size, type)
                    l.Add(new RecordedDataStruct(data.idList[i],
                    data.posList[i],
                    0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
                    scaleIndex++;
                    break;
                case 7: // Flappy (Scale, Type)
                    l.Add(new RecordedDataStruct(data.idList[i],
                    data.posList[i],
                    0, data.scaleList[scaleIndex], 0, 0, 0, data.typeList[i]));
                    scaleIndex++;
                    break;
                case 8: // Gas Ground (Speed, Delay, Time)


                    l.Add(new RecordedDataStruct(data.idList[i],
                                           data.posList[i],
                                           data.speedList[speedIndex], 0, 0, data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    timeIndex++;
                    delayIndex++;




                    break;
                case 9:
                    l.Add(new RecordedDataStruct(data.idList[i],
                                                                 data.posList[i],
                                                                 0, 0, 0, data.timeList[timeIndex], 0, data.typeList[i]));
                    timeIndex++;

                    break;


                case 10: // Balloon (Speed, Mag, Time, Delay)
                    l.Add(new RecordedDataStruct(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], data.timeList[timeIndex], data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    magIndex++;
                    timeIndex++;
                    delayIndex++;
                    scaleIndex++;
                    break;
                case 11: // Windmill (Speed, Size, Delay)
                    l.Add(new RecordedDataStruct(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    delayIndex++;
                    break;
                case 12: // Barn (Type)
                    l.Add(new RecordedDataStruct(data.idList[i],
                        data.posList[i],
                        0, 0, 0, 0, 0, data.typeList[i]));

                    break;
                case 13:
                case 14: // Ring (Speed, Size, Time, Type)
                    l.Add(new RecordedDataStruct(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], data.scaleList[scaleIndex], 0, data.timeList[timeIndex], 0, data.typeList[i]));
                    speedIndex++;
                    scaleIndex++;
                    timeIndex++;

                    break;
                case 15: // Egg Ammo (Speed, Mag, Delay, Type)
                case 16: // Shotgun Ammo (Speed, Mag, Delay, Type)
                    l.Add(new RecordedDataStruct(data.idList[i],
                        data.posList[i],
                        data.speedList[speedIndex], 0, data.magList[magIndex], 0, data.delayList[delayIndex], data.typeList[i]));
                    speedIndex++;
                    magIndex++;
                    delayIndex++;

                    break;


            }
        }
        return l.ToArray();

    }

    public static void CreateLevelDataFile(List<RecordedDataStruct> data, string levelName, string worldAndLevelNumber = null)
    {

    }

}
