using System.Collections.Generic;
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
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, 0, data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 1:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], 0, 0, 0, 0, data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    break;
                case 2:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], 0, data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
                case 3:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, 0, 0, data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    break;
                case 4:
                    l.Add(new RecordedDataStructDynamic(data.idList[i], data.posList[i], data.speedList[speedIndex], data.scaleList[scaleIndex], data.magList[magIndex], 0, data.delayList[delayIndex], 0, data.spawnSteps[i], 0));
                    speedIndex++;
                    scaleIndex++;
                    magIndex++;
                    delayIndex++;
                    break;
            }
        }
        return l;


    }

    public static void CreateLevelDataFile(List<RecordedDataStructDynamic> data, string levelName, string worldAndLevelNumber = null)
    {

    }

}
