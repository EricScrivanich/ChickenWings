using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Setups/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Difficulty overrides")]

    [SerializeField] private short[] easyStartingAmmos;
    [SerializeField] private short easyStartingLives;



    [SerializeField] private AllObjectData objData;
    public PlayerStartingStatsForLevels startingStats;



    public PlayerID playerID;
    public string LevelName;
    public Vector3 levelWorldAndNumber;
    public ushort[] spawnSteps;
    public ushort finalSpawnStep;
    public short[] idList;
    public short[] dataTypes;

    public Vector2[] posList;
    public float[] floatList;
    public ushort[] typeList;


    public ushort[] poolSizes;
    public ushort[] cageAttachments;

    private int currentCageIndex = 0;


    private ushort currentSpawnStep = 0;
    private ushort currentSpawnIndex;
    private ushort nextSpawnIndex;
    private ushort nextSpawnStep;
    private ushort one = 1;
    private int[] dataTypeIndexes;

    [SerializeField] private ushort[] checkPointSteps;

    private SpawnStateManager spawner;
    public short[] StartingAmmos;
    public short StartingLives;


    private RecordedDataStruct[] Data;
    private DataStructFloatOne[] dataStructFloatOne;
    private DataStructFloatTwo[] dataStructFloatTwo;
    private DataStructFloatThree[] dataStructFloatThree;
    private DataStructFloatFour[] dataStructFloatFour;
    private DataStructFloatFive[] dataStructFloatFive;
    public RecordedObjectPositionerDataSave[] postionerData = null;
    private int currentPositionerObjectIndex = 0;



    private bool checkForCage = false;


    public void LoadJsonToMemory()
    {
        LoadLevelSaveData(LevelDataConverter.instance.ConvertDataFromJson());
    }
    public void InitializeData(SpawnStateManager s, ushort startingStep, int preloadIndex = -1, int difficulty = 1)
    {
        spawner = s;
        objData.ringPool.Initialize(s);


        // Load your level save data first. This sets spawnSteps, idList, etc.

        if (playerID != null)
        {
            if (difficulty == 1)
            {
                // normal difficulty

                startingStats.SetData(StartingLives, StartingAmmos);
            }
            else if (difficulty == 0)
            {
                // Normal or Hard difficulty, use default values
                startingStats.SetData(easyStartingLives, easyStartingAmmos);
            }

            playerID.SetStartingStats(startingStats);
        }
        dataTypeIndexes = new int[5];

        // Set currentSpawnStep to the specified starting step.
        if (preloadIndex >= 0) startingStep = (ushort)preloadIndex;
        Debug.LogError("Starting Step: " + startingStep + " preloadIndex: " + preloadIndex);
        currentSpawnStep = startingStep;

        currentCageIndex = 0;

        // Find the first index in spawnSteps where the step is >= startingStep.
        currentSpawnIndex = 0;
        currentPositionerObjectIndex = 0;

        if (cageAttachments == null || cageAttachments.Length <= 0)
        {
            checkForCage = false;
        }
        else
        {
            checkForCage = true;

        }
        for (ushort i = 0; i < spawnSteps.Length; i++)
        {
            if (spawnSteps[i] >= startingStep)
            {
                currentSpawnIndex = i;
                break;
            }
            if (checkForCage && i == cageAttachments[currentCageIndex])
            {
                currentCageIndex++;

                if (currentCageIndex >= cageAttachments.Length)
                {
                    checkForCage = false; // No more cages to spawn
                }
            }

            if (idList[i] >= 0)
            {
                dataTypeIndexes[dataTypes[i] - 1]++;

                if (dataTypes[i] < 0)
                {

                    currentPositionerObjectIndex++;

                }


            }
            else if (idList[i] < 0)
            {

                dataTypeIndexes[2]++;

            }


        }

        // Set nextSpawnIndex to match the found starting index.
        nextSpawnIndex = currentSpawnIndex;

        // Reset your data type indexes (assuming you have 5 data types).


        // Create pools for each object type if needed.
        for (int i = 0; i < poolSizes.Length; i++)
        {
            if (objData.pools[i] != null && poolSizes[i] > 0)
                objData.pools[i].CreatePool(poolSizes[i]);
        }

        CreateDataStructs();
        GetSpawnSizeForNextTimeStep();
    }

    public void SetDefaults(AllObjectData objData, PlayerStartingStatsForLevels stats, PlayerID id)
    {
        this.objData = objData;
        this.startingStats = stats;
        this.playerID = id;

        finalSpawnStep = 300;
        spawnSteps = new ushort[0];
        idList = new short[0];
        posList = new Vector2[0];
        dataTypes = new short[0];
        floatList = new float[0];
        startingStats.startingAmmos = new short[5];
        startingStats.startingAmmos[0] = 3;
        startingStats.startingAmmos[1] = 3;
        startingStats.StartingLives = 3;


    }

    private void CreateDataStructs()
    {
        int[] floatSizes = new int[5];
        for (int i = 0; i < dataTypes.Length; i++)
        {
            if (dataTypes[i] <= floatSizes.Length && dataTypes[i] >= 0)
                floatSizes[dataTypes[i] - 1]++;
            else
                Debug.LogError("Data Type: " + dataTypes[i] + " is not valid. Please check the data type.");

        }
        dataStructFloatOne = new DataStructFloatOne[floatSizes[0]];
        dataStructFloatTwo = new DataStructFloatTwo[floatSizes[1]];
        dataStructFloatThree = new DataStructFloatThree[floatSizes[2]];
        dataStructFloatFour = new DataStructFloatFour[floatSizes[3]];
        dataStructFloatFive = new DataStructFloatFive[floatSizes[4]];

        // foreach (int n in floatSizes)
        // {

        // }

        int floatListIndex = 0;
        int subrtactIndex = 0;

        int float1Index = 0;
        int float2Index = 0;
        int float3Index = 0;
        int float4Index = 0;
        int float5Index = 0;
        for (int i = 0; i < dataTypes.Length; i++)
        {

            if (dataTypes[i] < 0)
            {
                // This is a positioner object
                subrtactIndex++;
                continue;
            }
            switch (dataTypes[i])
            {
                case 1:
                    dataStructFloatOne[float1Index] = new DataStructFloatOne(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex], floatList[floatListIndex]);
                    float1Index++;
                    floatListIndex++;
                    break;
                case 2:
                    dataStructFloatTwo[float2Index] = new DataStructFloatTwo(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex], floatList[floatListIndex], floatList[floatListIndex + 1]);
                    float2Index++;
                    floatListIndex += 2;
                    break;
                case 3:
                    dataStructFloatThree[float3Index] = new DataStructFloatThree(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex], floatList[floatListIndex], floatList[floatListIndex + 1], floatList[floatListIndex + 2]);
                    float3Index++;
                    floatListIndex += 3;
                    break;
                case 4:
                    dataStructFloatFour[float4Index] = new DataStructFloatFour(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex], floatList[floatListIndex], floatList[floatListIndex + 1], floatList[floatListIndex + 2], floatList[floatListIndex + 3]);
                    float4Index++;
                    floatListIndex += 4;
                    break;
                case 5:
                    dataStructFloatFive[float5Index] = new DataStructFloatFive(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex], floatList[floatListIndex], floatList[floatListIndex + 1], floatList[floatListIndex + 2], floatList[floatListIndex + 3], floatList[floatListIndex + 4]);
                    float5Index++;
                    floatListIndex += 5;
                    break;

            }

        }


    }




    public void LoadLevelSaveData(LevelDataSave lds)
    {
        if (lds == null)
        {
            Debug.LogError("LevelDataSave is null");
            finalSpawnStep = 300;
            spawnSteps = new ushort[0];
            idList = new short[0];
            posList = new Vector2[0];
            dataTypes = new short[0];
            floatList = new float[0];
            StartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;
            StartingLives = 3;
            startingStats.SetData(StartingLives, StartingAmmos);



            return;
        }
        Debug.LogError("Loading Level Data: " + lds.levelName + " with " + lds.spawnSteps.Length + " spawn steps.");
        LevelName = lds.levelName;
        // levelWorldAndNumber = lds.levelWorldAndNumber;
        spawnSteps = lds.spawnSteps;
        finalSpawnStep = lds.finalSpawnStep;
        floatList = lds.floatList;
        idList = lds.idList;
        cageAttachments = lds.cageAttachments;

        posList = lds.posList;
        dataTypes = lds.dataTypes;
        // speedList = lds.speeds;
        // scaleList = lds.scaleList;
        // magList = lds.magList;
        // timeList = lds.timeList;
        // delayList = lds.delayList;
        typeList = lds.typeList;
        poolSizes = lds.poolSizes;
        if (lds.postionerData != null)
        {
            Debug.LogError("Postioner Data is not null with length: " + lds.postionerData.Length);
            postionerData = lds.postionerData;
        }

        if (lds.startingAmmos == null || lds.startingAmmos.Length <= 0)
        {
            StartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;

            startingStats.startingAmmos = new short[5];
            startingStats.startingAmmos[0] = 3;
            startingStats.startingAmmos[1] = 3;

        }
        else
            StartingAmmos = lds.startingAmmos;

        if (lds.StartingLives <= 0)
            StartingLives = 3;
        else
            StartingLives = lds.StartingLives;

        startingStats.SetData(StartingLives, StartingAmmos);


        Debug.LogError("Finished loading data for level: " + LevelName + " with " + spawnSteps.Length + " spawn steps and " + idList.Length + " objects.");
    }



    public void NextSpawnStep(ushort ss)
    {
        currentSpawnStep = ss;
        if (ss == checkPointSteps[0])
        {
            objData.SpawnCheckPoint(false);
        }
        if (ss == nextSpawnStep)
        {
            Debug.LogError("Next Spawn Step: " + ss + " is the same as current spawn step: " + currentSpawnStep + ". Spawning objects now.");

            for (ushort i = currentSpawnIndex; i < nextSpawnIndex; i++)
            {
                // Debug.LogError("Spawning Object: ID: " + Data[i].ID + " Pos: " + Data[i].startPos + " Speed: " + Data[i].speed + " Scale: " + Data[i].scale + " Mag: " + Data[i].magPercent + " Time: " + Data[i].timeInterval + " Delay: " + Data[i].delayInterval + " Type: " + Data[i].type);
                // spawner.SpawnObject(Data[i]);


                if (idList[i] >= 0)
                {
                    if (dataTypes[i] < 0)
                    {
                        objData.pools[idList[i]].SpawnPositionerData(postionerData[currentPositionerObjectIndex]);
                        currentPositionerObjectIndex++;
                        if (checkForCage && i == cageAttachments[currentCageIndex])
                        {
                            // Spawn a cage object
                            Debug.LogError("Spawning Cage for ID: " + idList[i] + " at index: " + currentCageIndex);
                            var attachedObject = objData.pools[idList[i]].GetCageAttachment();
                            var cage = Instantiate(objData.cageObject, attachedObject.ReturnPosition(), Quaternion.identity);
                            // cage.gameObject.SetActive(false);
                            cage.GetComponent<CustomHingeJoint2D>().SetAttatchedObject(attachedObject);
                            currentCageIndex++;

                            if (currentCageIndex >= cageAttachments.Length)
                            {
                                checkForCage = false; // No more cages to spawn
                            }
                        }

                        continue; // Skip to the next iteration for positioner objects

                    }


                    switch (dataTypes[i])
                    {
                        case 1:
                            objData.pools[idList[i]].SpawnFloatOne(dataStructFloatOne[dataTypeIndexes[0]]);
                            break;
                        case 2:
                            objData.pools[idList[i]].SpawnFloatTwo(dataStructFloatTwo[dataTypeIndexes[1]]);
                            break;
                        case 3:
                            objData.pools[idList[i]].SpawnFloatThree(dataStructFloatThree[dataTypeIndexes[2]]);
                            break;
                        case 4:
                            objData.pools[idList[i]].SpawnFloatFour(dataStructFloatFour[dataTypeIndexes[3]]);
                            break;
                        case 5:
                            objData.pools[idList[i]].SpawnFloatFive(dataStructFloatFive[dataTypeIndexes[4]]);
                            break;

                    }

                    if (checkForCage && i == cageAttachments[currentCageIndex])
                    {
                        // Spawn a cage object
                        Debug.LogError("Spawning Cage for ID: " + idList[i] + " at index: " + currentCageIndex);
                        var attachedObject = objData.pools[idList[i]].GetCageAttachment();
                        var cage = Instantiate(objData.cageObject, attachedObject.ReturnPosition(), Quaternion.identity);
                        // cage.gameObject.SetActive(false);
                        cage.GetComponent<CustomHingeJoint2D>().SetAttatchedObject(attachedObject);
                        currentCageIndex++;

                        if (currentCageIndex >= cageAttachments.Length)
                        {
                            checkForCage = false; // No more cages to spawn
                        }
                    }




                    dataTypeIndexes[dataTypes[i] - 1]++;
                }
                else if (idList[i] == -2)
                {
                    objData.ringPool.SpawnRingByData(dataStructFloatThree[dataTypeIndexes[2]]);
                    dataTypeIndexes[2]++;

                }
                else if (idList[i] == -1)
                {
                    objData.ringPool.SpawnBucketByData(dataStructFloatThree[dataTypeIndexes[2]]);
                    dataTypeIndexes[2]++;
                }



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



