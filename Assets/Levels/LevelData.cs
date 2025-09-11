using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Setups/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Starting Stats")]
    [field: SerializeField] public short[] StartingAmmos { get; private set; }
    [field: SerializeField] public short StartingLives { get; private set; }

    [field: SerializeField] public short[] easyStartingAmmos { get; private set; }
    [field: SerializeField] public short easyStartingLives { get; private set; }



    public int[] AvailableAmmos = new int[2] { 0, 1 };

    [Header("Scriptable Objects")]
    [SerializeField] private AllObjectData objData;
    public PlayerStartingStatsForLevels startingStats;

    [ExposedScriptableObject]
    [SerializeField] private LevelChallenges levelChallenges;
    [ExposedScriptableObject]
    public TutorialData tutorialData;



    public PlayerID playerID;
    public string LevelName;
    public Vector3Int levelWorldAndNumber;
    public ushort[] spawnSteps;
    public ushort finalSpawnStep;
    public short[] objectTypes;

    public short[] idList;
    public short[] dataTypes;

    public Vector2[] posList;
    public float[] floatList;
    public ushort[] typeList;


    public ushort[] poolSizes;
    public ushort[] pigPoolSizes;
    public ushort[] aiPoolSizes;
    public ushort[] buildingPoolSizes;
    public ushort[] collectablePoolSizes;
    public ushort[] positionerPoolSizes;
    public ushort[] ringPoolSizes;
    public ushort[] cageAttachments;

    private int currentCageIndex = 0;


    private ushort currentSpawnStep = 0;
    private ushort currentSpawnIndex;
    private ushort nextSpawnIndex;
    private ushort nextSpawnStep;
    private ushort one = 1;
    private int[] dataTypeIndexes;

    public ushort[] checkPointSteps;

    private SpawnStateManager spawner;


    private RecordedDataStruct[] Data;
    private DataStructSimple[] dataStructSimple;
    private DataStructFloatOne[] dataStructFloatOne;
    private DataStructFloatTwo[] dataStructFloatTwo;
    private DataStructFloatThree[] dataStructFloatThree;
    private DataStructFloatFour[] dataStructFloatFour;
    private DataStructFloatFive[] dataStructFloatFive;
    public RecordedObjectPositionerDataSave[] postionerData = null;
    private int currentPositionerObjectIndex = 0;
    private int currentCheckpointIndex = 0;
    private bool checkForCheckpoint = false;
    public bool usedCheckPoint { get; private set; } = false;
    public int Difficulty = 1;


    private bool checkForCage = false;
    private bool checkPointHasBeenCollected = false;
    private bool isRealLevel;



    public void LoadJsonToMemory()
    {
        LoadLevelSaveData(LevelDataConverter.instance.ConvertDataFromJson());
    }
    public void InitializeData(SpawnStateManager s, ushort startingStep, int difficulty = 1, TemporaryLevelCheckPointData checkPointData = null, bool isLevel = false)
    {
        spawner = s;
        Difficulty = difficulty;
        this.isRealLevel = isLevel;


        checkPointHasBeenCollected = false;
        playerID.livesToLoseOnStart = 0;
        currentCheckpointIndex = 0;

        if (levelChallenges != null)
        {
            levelChallenges.skipShowChallenges = !isLevel;
            Debug.Log("Setting skipShowChallenges to: " + levelChallenges.skipShowChallenges);
        }
        bool hasCollectables = false;

        foreach (int a in collectablePoolSizes)
        {
            if (a > 0)
            {
                hasCollectables = true;
                break;
            }
        }

        if (!hasCollectables)
        {
            if (difficulty < 1)
            {
                foreach (int a2 in easyStartingAmmos)
                {
                    if (a2 > 0)
                    {
                        hasCollectables = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (int a2 in easyStartingAmmos)
                {
                    if (a2 > 0)
                    {
                        hasCollectables = true;
                        break;
                    }
                }

            }
        }

        if (!hasCollectables)
        {
            Debug.Log("No collectables found, hiding EggCanvas");
            GameObject.Find("EggCanvas").SetActive(false);
        }

        // Load your level save data first. This sets spawnSteps, idList, etc.

        if (checkPointData != null && playerID != null)
        {


            currentCheckpointIndex = checkPointData.CurrentCheckPoint;

            if (difficulty == 1)
            {
                startingStats.SetData(StartingLives, checkPointData.CurrentAmmos);
            }
            else if (difficulty == 0)
            {
                startingStats.SetData(easyStartingLives, checkPointData.CurrentAmmos);

                // Normal or Hard difficulty, use default values

            }
            if (startingStats.StartingLives > checkPointData.CurrentLives)
            {

                playerID.livesToLoseOnStart = startingStats.StartingLives - checkPointData.CurrentLives;
            }




            checkPointHasBeenCollected = true;
            usedCheckPoint = true;
            if (levelChallenges != null)
                levelChallenges.LoadData(levelWorldAndNumber, difficulty, checkPointData);
        }

        else if (playerID != null)
        {
            usedCheckPoint = false;
            if (difficulty == 1 || difficulty == 3)
            {
                Debug.LogError("Setting Starting Stats for normal Difficulty");
                // normal difficulty

                startingStats.SetData(StartingLives, StartingAmmos);
                if (levelChallenges != null)
                    levelChallenges.ResetData(levelWorldAndNumber, difficulty, StartingAmmos, StartingLives);
            }
            else if (difficulty == 2)
            {
                startingStats.SetData(1, StartingAmmos);
                if (levelChallenges != null)
                    levelChallenges.ResetData(levelWorldAndNumber, difficulty, StartingAmmos, StartingLives);
            }


            else if (difficulty == 0)
            {
                // Normal or Hard difficulty, use default values
                startingStats.SetData(easyStartingLives, easyStartingAmmos);
                if (levelChallenges != null)
                    levelChallenges.ResetData(levelWorldAndNumber, difficulty, easyStartingAmmos, easyStartingLives);
            }




        }

        if (tutorialData != null && isLevel)
        {
            Debug.LogError("Setting Tutorial Data for level: " + LevelName + " with difficulty: " + difficulty);
            s.SetTutorialData(tutorialData);
            tutorialData.Initialize(s, startingStep, playerID);

        }
        else if (tutorialData == null)
        {
            playerID.UiEvents.OnShowPlayerUI?.Invoke(true, 10, 0);
        }
        startingStats.AvailableAmmos = AvailableAmmos;
        playerID.SetDataForLevel(startingStats, levelChallenges);
        dataTypeIndexes = new int[6];

        // Set currentSpawnStep to the specified starting step.


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

        if (checkPointSteps == null || checkPointSteps.Length <= 0)
        {
            checkForCheckpoint = false;
        }
        else
        {
            checkForCheckpoint = true;
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
                dataTypeIndexes[dataTypes[i]]++;

                if (dataTypes[i] < 0)
                {

                    currentPositionerObjectIndex++;

                }


            }
            else if (idList[i] < 0)
            {

                dataTypeIndexes[3]++;

            }


        }

        // Set nextSpawnIndex to match the found starting index.
        nextSpawnIndex = currentSpawnIndex;

        // Reset your data type indexes (assuming you have 5 data types).


        // Create pools for each object type if needed.
        List<ushort[]> poolList = new List<ushort[]>
        {
            pigPoolSizes,
            aiPoolSizes,
            buildingPoolSizes,
            collectablePoolSizes,
            positionerPoolSizes,
            ringPoolSizes
        };

        for (int i = 0; i < poolList.Count; i++)
        {
            if (i == 5) // ring pool
            {
                objData.ringPool.Initialize(ringPoolSizes[0], ringPoolSizes[1], s);
            }
            else if (poolList[i] != null && poolList[i].Length > 0)
            {
                var poolToPopulate = objData.GetPoolArrayByObjectType(i);
                for (int j = 0; j < poolList[i].Length; j++)
                {
                    poolToPopulate[j].CreatePool(poolList[i][j]);
                }
            }
        }
        // for (int i = 0; i < poolSizes.Length; i++)
        // {
        //     if (objData.pools[i] != null && poolSizes[i] > 0)
        //         objData.pools[i].CreatePool(poolSizes[i]);
        // }

        CreateDataStructs();
        GetSpawnSizeForNextTimeStep();
    }

    public void SetDefaults(AllObjectData objData, PlayerStartingStatsForLevels stats, PlayerID id, LevelChallenges challenges)
    {
        this.objData = objData;
        this.startingStats = stats;
        this.playerID = id;
        this.levelChallenges = challenges;



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
        int simpleStructSize = 0;
        int[] floatSizes = new int[5];
        for (int i = 0; i < dataTypes.Length; i++)
        {
            if (dataTypes[i] == 0)
            {
                simpleStructSize++;

            }
            else if (dataTypes[i] <= floatSizes.Length && dataTypes[i] > 0)
                floatSizes[dataTypes[i] - 1]++;
            else
                Debug.LogError("Data Type: " + dataTypes[i] + " is not valid. Please check the data type.");

        }
        dataStructSimple = new DataStructSimple[simpleStructSize];
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
        int simpleStructIndex = 0;
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
                case 0:
                    dataStructSimple[simpleStructIndex] = new DataStructSimple(idList[i], typeList[i - subrtactIndex], posList[i - subrtactIndex]);

                    simpleStructIndex++;
                    // This is a simple object, already handled above
                    break;
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

    public void SaveLevelCheckPoint(ushort checkPointIndex)
    {

        levelChallenges.SetCheckPoint(checkPointIndex);
        LevelDataConverter.instance.SaveTemporaryCheckPointData(levelChallenges);

    }

    public void EditAmmos(int index, int amount)
    {
        Debug.LogError("Editing ammos in level data");
        StartingAmmos[index] = (short)amount;
    }
    public void EditLives(int amount)
    {
        StartingLives = (short)amount;
        Debug.LogError("Editing lives in level data to: " + StartingLives);
    }




    public void LoadLevelSaveData(LevelDataSave lds)
    {
        if (lds == null)
        {
            Debug.LogError("LevelDataSave is null");
            finalSpawnStep = 300;
            spawnSteps = new ushort[0];
            objectTypes = new short[0];
            idList = new short[0];
            posList = new Vector2[0];
            dataTypes = new short[0];
            floatList = new float[0];
            StartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;
            StartingLives = 3;
            startingStats.SetData(StartingLives, StartingAmmos);
            pigPoolSizes = new ushort[0];
            aiPoolSizes = new ushort[0];
            buildingPoolSizes = new ushort[0];
            collectablePoolSizes = new ushort[0];
            positionerPoolSizes = new ushort[0];
            ringPoolSizes = new ushort[0];



            return;
        }
        Debug.LogError("Loading Level Data: " + lds.levelName + " with " + lds.spawnSteps.Length + " spawn steps.");
        LevelName = lds.levelName;
        // levelWorldAndNumber = lds.levelWorldAndNumber;
        spawnSteps = lds.spawnSteps;
        finalSpawnStep = lds.finalSpawnStep;
        floatList = lds.floatList;
        objectTypes = lds.objectTypes;
        idList = lds.idList;
        cageAttachments = lds.cageAttachments;
        pigPoolSizes = lds.pigPoolSizes;
        aiPoolSizes = lds.aiPoolSizes;
        buildingPoolSizes = lds.buildingPoolSizes;
        collectablePoolSizes = lds.collectablePoolSizes;
        positionerPoolSizes = lds.positonerPoolSizes;
        ringPoolSizes = lds.ringPoolSizes;

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
            Debug.LogError("Starting Ammos is null or empty, setting default values.");
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


        Debug.LogError("Finished loading data for level: " + "Starting Lives" + StartingLives + LevelName + " with " + spawnSteps.Length + " spawn steps and " + idList.Length + " objects.");
    }



    public void NextSpawnStep(ushort ss)
    {
        currentSpawnStep = ss;

        if (ss > finalSpawnStep)
        {
            objData.SpawnFinishLine();
            spawner.FinishLevel();
            return;
        }

        if (checkForCheckpoint && ss == checkPointSteps[currentCheckpointIndex])
        {
            // Debug.LogError("Spawning Checkpoint at step: " + ss + " with index: " + currentCheckpointIndex);
            objData.SpawnCheckPoint(checkPointHasBeenCollected, (ushort)currentCheckpointIndex, this);
            if (checkPointHasBeenCollected) checkPointHasBeenCollected = false; // Only spawn the checkpoint once
            currentCheckpointIndex++;
            if (currentCheckpointIndex >= checkPointSteps.Length)
            {
                checkForCheckpoint = false; // No more checkpoints to spawn
            }
        }
        if (ss == nextSpawnStep)
        {
            // Debug.LogError("Next Spawn Step: " + ss + " is the same as current spawn step: " + currentSpawnStep + ". Spawning objects now.");


            for (ushort i = currentSpawnIndex; i < nextSpawnIndex; i++)
            {
                // Debug.LogError("Object Type: " + objectTypes[i] + ", ID: " + idList[i]);
                // Debug.LogError("Spawning Object: ID: " + Data[i].ID + " Pos: " + Data[i].startPos + " Speed: " + Data[i].speed + " Scale: " + Data[i].scale + " Mag: " + Data[i].magPercent + " Time: " + Data[i].timeInterval + " Delay: " + Data[i].delayInterval + " Type: " + Data[i].type);
                // spawner.SpawnObject(Data[i]);


                if (objectTypes[i] != 5)
                {

                    if (dataTypes[i] < 0)
                    {
                        objData.positionerPools[idList[i]].SpawnPositionerData(postionerData[currentPositionerObjectIndex]);
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
                    var poolArray = objData.GetPoolArrayByObjectType(objectTypes[i]);
                    var pool = poolArray[idList[i]];

                    switch (dataTypes[i])
                    {
                        case 0:
                            pool.SpawnSimpleObject(dataStructSimple[dataTypeIndexes[0]]);
                            break;
                        case 1:
                            pool.SpawnFloatOne(dataStructFloatOne[dataTypeIndexes[1]]);
                            break;
                        case 2:
                            pool.SpawnFloatTwo(dataStructFloatTwo[dataTypeIndexes[2]]);
                            break;
                        case 3:
                            pool.SpawnFloatThree(dataStructFloatThree[dataTypeIndexes[3]]);
                            break;
                        case 4:
                            pool.SpawnFloatFour(dataStructFloatFour[dataTypeIndexes[4]]);
                            break;
                        case 5:
                            pool.SpawnFloatFive(dataStructFloatFive[dataTypeIndexes[5]]);
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




                    dataTypeIndexes[dataTypes[i]]++;
                }
                else if (idList[i] == 0)
                {

                    objData.ringPool.SpawnRingByData(dataStructFloatThree[dataTypeIndexes[3]]);
                    dataTypeIndexes[3]++;

                }
                else if (idList[i] == 1)
                {

                    objData.ringPool.SpawnBucketByData(dataStructFloatThree[dataTypeIndexes[3]]);
                    dataTypeIndexes[3]++;
                }
                else
                {
                    Debug.LogError("Unknown ID: " + idList[i] + " at index: " + i);
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
            // Debug.LogWarning("All spawn steps processed. No more objects to spawn.");
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
                // Debug.LogError("Next Spawn Step: " + nextSpawnStep + ", Next Spawn Index: " + nextSpawnIndex);
                return; // Exit loop early once found
            }
        }

        // If no greater spawn step is found, assume all remaining objects spawn at the same step
        nextSpawnIndex = (ushort)spawnSteps.Length; // Mark as fully processed
        // Debug.LogWarning("No further spawn steps found. All objects may have been processed.");
    }



    public LevelChallenges GetLevelChallenges(bool loadData, TemporaryLevelCheckPointData data, bool overrideSkipChallengesFalse = false)
    {
        if (levelChallenges == null)
        {
            Debug.LogError("Level Challenges is null");
            return null;
        }
        if (overrideSkipChallengesFalse)
            levelChallenges.skipShowChallenges = false;

        if (data == null && loadData)
        {
            levelChallenges.ResetData(levelWorldAndNumber, Difficulty, StartingAmmos, StartingLives);
            return levelChallenges;
        }
        else if (loadData)
        {
            levelChallenges.LoadData(levelWorldAndNumber, Difficulty, data);
            levelChallenges.SetCheckPoint(data.CurrentCheckPoint);

            // Debug.LogError("Loaded Level Challenges for level: " + levelWorldAndNumber + " with difficulty: " + difficulty);
        }
        levelChallenges.SetDifficulty(Difficulty);

        return levelChallenges;
    }









}



