using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Setups/LevelData")]
public class LevelData : ScriptableObject
{


    public string levelName;
    public string levelWorldAndNumber;
    public ushort[] spawnSteps;
    public short[] idList;
    public Vector2[] posList;
    public float[] speedList;
    public float[] scaleList;
    public float[] magList;
    public float[] timeList;
    public float[] delayList;
    public short[] typeList;
    private int spawnSize;

    private ushort currentSpawnStep = 0;
    private ushort currentSpawnIndex;
    private ushort nextSpawnIndex;
    private ushort nextSpawnStep;
    private ushort one = 1;

    private SpawnStateManager spawner;


    private RecordedDataStruct[] Data;


    public void InitializeData(SpawnStateManager s)
    {
        spawner = s;
        currentSpawnStep = 0;
        currentSpawnIndex = 0;
        nextSpawnIndex = 0;


        LoadLevelSaveData(LevelDataConverter.instance.ConvertDataFromJson());
        // spawnSize =
        Data = LevelDataConverter.instance.ReturnLevelData(this);
        GetSpawnSizeForNextTimeStep();
    }

    public void LoadData()
    {
        LoadLevelSaveData(LevelDataConverter.instance.ConvertDataFromJson());

    }

    public void LoadLevelSaveData(LevelDataSave lds)
    {
        levelName = lds.levelName;
        levelWorldAndNumber = lds.levelWorldAndNumber;
        spawnSteps = lds.spawnSteps;
        Debug.Log("SPAWN STEPS: " + spawnSteps.Length);
        idList = lds.idList;
        Debug.Log("IDLIST: " + idList.Length);
        posList = lds.posList;
        speedList = lds.speeds;
        scaleList = lds.scaleList;
        magList = lds.magList;
        timeList = lds.timeList;
        delayList = lds.delayList;
        typeList = lds.typeList;



    }

    public void NextSpawnStep(ushort ss)
    {
        currentSpawnStep = ss;
        if (ss == nextSpawnStep)
        {

            for (ushort i = currentSpawnIndex; i < nextSpawnIndex; i++)
            {
                // Debug.LogError("Spawning Object: ID: " + Data[i].ID + " Pos: " + Data[i].startPos + " Speed: " + Data[i].speed + " Scale: " + Data[i].scale + " Mag: " + Data[i].magPercent + " Time: " + Data[i].timeInterval + " Delay: " + Data[i].delayInterval + " Type: " + Data[i].type);
                spawner.SpawnObject(Data[i]);


            }

            currentSpawnIndex = nextSpawnIndex;

            GetSpawnSizeForNextTimeStep();
        }

    }

    public void GetSpawnSizeForNextTimeStep()
    {
        // Edge case: If all objects have already been spawned
        if (nextSpawnIndex >= spawnSteps.Length)
        {
            Debug.LogWarning("All spawn steps processed. No more objects to spawn.");
            return;
        }

        // Set the next step we are looking for
        nextSpawnStep = spawnSteps[nextSpawnIndex];

        // Start checking from the current index
        for (ushort i = nextSpawnIndex; i < spawnSteps.Length; i++)
        {
            if (spawnSteps[i] > nextSpawnStep) // Look for the next step in sequence
            {
                nextSpawnIndex = i;  // Store the next spawn index correctly
                Debug.LogError("Next Spawn Step: " + nextSpawnStep + ", Next Spawn Index: " + nextSpawnIndex);
                return; // Exit loop early once found
            }
        }

        // If no greater spawn step is found, assume all remaining objects spawn at the same step
        nextSpawnIndex = (ushort)spawnSteps.Length; // Mark as fully processed
        Debug.LogWarning("No further spawn steps found. All objects may have been processed.");
    }













}
