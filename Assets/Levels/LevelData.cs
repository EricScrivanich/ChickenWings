using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Setups/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int maxWaves;
    private int currentWave = 1;
    public LevelDataRandomSpawnData[] randomSpawnData;
    private int spawnStepsPerRandom;
    [field: SerializeField] public short unlockItemOnComplete { get; private set; } = -1;


    private int currentSpawnStepCount = 0;
    [Header("Starting Stats")]
    [field: SerializeField] public short[] StartingAmmos { get; private set; }
    [field: SerializeField] public short StartingLives { get; private set; }

    [field: SerializeField] public short[] easyStartingAmmos { get; private set; }
    [field: SerializeField] public short easyStartingLives { get; private set; }



    public int[] AvailableAmmos = new int[2] { 0, 1 };

    [Header("Scriptable Objects")]
    [SerializeField] private AllObjectData objData;
    public PlayerStartingStatsForLevels startingStats;

    public LevelDataBossAndRandomLogic levelDataBossAndRandomLogic;

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

    public Vector2Int[] specialTriggerIndexAndType;
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

    public SpawnStateManager spawner { get; private set; }


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
    private int currentSpecialTriggerIndex;


    private bool checkForCage = false;
    private bool checkPointHasBeenCollected = false;
    private bool isRealLevel;



    public void SetRandomEnemySpawnSteps(int stepsPerSpawn)
    {
        currentSpawnStepCount = 0;
        spawnStepsPerRandom = stepsPerSpawn;
    }
    public void InitializeData(SpawnStateManager s, ushort startingStep, LevelDataArrays dataArrays = null, LevelDataSave lds = null, int difficulty = 1, TemporaryLevelCheckPointData checkPointData = null, bool isLevel = false, bool endlessMode = false)
    {
        if (lds != null)
            LoadLevelSaveData(lds);
        spawner = s;
        Difficulty = difficulty;
        currentSpawnStepCount = 0;
        spawnStepsPerRandom = 0;
        currentSpecialTriggerIndex = 0;

        currentWave = 1;
        if (levelDataBossAndRandomLogic != null)
        {
            levelDataBossAndRandomLogic.Initialize(this);
            checkBossAndRandomData = true;
            Debug.Log("Checking for Random:");
        }
        else
            checkBossAndRandomData = false;



        this.isRealLevel = isLevel;
        checkPointHasBeenCollected = false;
        playerID.livesToLoseOnStart = 0;
        currentCheckpointIndex = 0;

        if (levelChallenges != null)
        {
            levelChallenges.skipShowChallenges = !isLevel;
            Debug.Log("Setting skipShowChallenges to: " + levelChallenges.skipShowChallenges);
        }


        int shownEgg = -1;
        int collectableSize0 = 0;
        int collectableSize1 = 0;
        int livesAmount = 0;
        short[] ammos = null;
        bool hasCollectables = false;

        if (!hasCollectables)
        {
            if (difficulty < 1)
            {
                if (easyStartingAmmos != null && easyStartingAmmos.Length > 0)
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
                if (StartingAmmos != null && StartingAmmos.Length > 0)
                    foreach (int a2 in StartingAmmos)
                    {
                        if (a2 > 0)
                        {
                            hasCollectables = true;
                            break;
                        }
                    }

            }
        }


        // Load your level save data first. This sets spawnSteps, idList, etc.

        if (checkPointData != null && playerID != null)
        {


            currentCheckpointIndex = checkPointData.CurrentCheckPoint;

            if (difficulty == 1)
            {
                livesAmount = StartingLives;

                // startingStats.SetData(StartingLives, checkPointData.CurrentAmmos, shownEgg);
            }
            else if (difficulty == 0)
            {
                livesAmount = easyStartingLives;
                //  startingStats.SetData(easyStartingLives, checkPointData.CurrentAmmos, shownEgg);

                // Normal or Hard difficulty, use default values

            }

            ammos = checkPointData.CurrentAmmos.Clone() as short[];

            if (livesAmount > checkPointData.CurrentLives)
            {

                playerID.livesToLoseOnStart = livesAmount - checkPointData.CurrentLives;
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
                // normal difficulty\ammo\

                ammos = StartingAmmos.Clone() as short[]; ;
                livesAmount = StartingLives;

                // startingStats.SetData(StartingLives, StartingAmmos, shownEgg);
                if (levelChallenges != null)
                    levelChallenges.ResetData(levelWorldAndNumber, difficulty, StartingAmmos, StartingLives);
            }
            else if (difficulty == 2)
            {
                // startingStats.SetData(1, StartingAmmos, shownEgg);
                ammos = StartingAmmos.Clone() as short[]; ;
                livesAmount = 1;
                if (levelChallenges != null)
                    levelChallenges.ResetData(levelWorldAndNumber, difficulty, StartingAmmos, StartingLives);
            }


            else if (difficulty == 0)
            {
                ammos = easyStartingAmmos.Clone() as short[]; ;
                livesAmount = easyStartingLives;
                // Normal or Hard difficulty, use default values
                // startingStats.SetData(easyStartingLives, easyStartingAmmos, shownEgg);
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
            if (specialTriggerIndexAndType != null && currentSpecialTriggerIndex < specialTriggerIndexAndType.Length && i >= specialTriggerIndexAndType[currentSpecialTriggerIndex].x)
            {
                currentSpecialTriggerIndex++;
            }


        }

        // Set nextSpawnIndex to match the found starting index.
        nextSpawnIndex = currentSpawnIndex;

        // Reset your data type indexes (assuming you have 5 data types).


        // Create pools for each object type if needed.

        // for (int i = 0; i < poolSizes.Length; i++)
        // {
        //     if (objData.pools[i] != null && poolSizes[i] > 0)
        //         objData.pools[i].CreatePool(poolSizes[i]);
        // }

        // if (dataArrays == null && lds == null)
        // {
        //     if (levelWorldAndNumber == Vector3Int.zero)
        //         dataArrays = Resources.Load<LevelDataArrays>(LevelDataConverter.GetLevelNumberStringFormat(levelWorldAndNumber) + LevelName + "_DataArrays");

        //     else
        //         dataArrays = Resources.Load<LevelDataArrays>(LevelDataConverter.GetLevelNumberStringFormat(levelWorldAndNumber) + LevelName + "_DataArrays");

        // }
        if (dataArrays == null && lds == null)
        {

            dataArrays = Resources.Load<LevelDataArrays>(LevelDataConverter.GetLevelNumberStringFormat(levelWorldAndNumber) + LevelName + "_DataArrays");
            if (dataArrays == null)
            {
                Debug.LogError("Data Arrays is null for level: " + LevelName + " at path: " + "Levels/MainLevelDataArrays/" + levelWorldAndNumber + "_" + LevelName + "_DataArrays");
                return;
            }
            else
            {
                foreach (int a in dataArrays.collectablePoolSizes)
                {
                    if (a > 0)
                    {
                        hasCollectables = true;
                        break;
                    }
                }
                CreatePoolsAndDataStructs(dataArrays, s);
            }

        }
        else if (lds != null)
        {
            foreach (int a in lds.collectablePoolSizes)
            {
                if (a > 0)
                {
                    hasCollectables = true;
                    break;
                }
            }
            CreatePoolsAndDataStructs(lds, s);
        }







        if (!hasCollectables)
        {
            Debug.Log("No collectables found, hiding EggCanvas");
            GameObject.Find("EggCanvas").SetActive(false);
            startingStats.SetData((short)livesAmount, ammos, 0);
        }
        else
        {
            if (ammos[0] == 0 && ammos[1] == 0)
            {
                shownEgg = 0;
            }
            else if (ammos[0] > ammos[1])
            {
                shownEgg = 0;
            }
            else
            {
                shownEgg = 1;
            }
            startingStats.SetData((short)livesAmount, ammos, shownEgg);
        }

        playerID.SetDataForLevel(startingStats, levelChallenges);






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
            dataTypes = new short[0];

            posList = new Vector2[0];
            floatList = new float[0];
            StartingAmmos = new short[5];
            easyStartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;
            easyStartingAmmos[0] = 3;
            easyStartingAmmos[1] = 3;
            StartingLives = 3;
            easyStartingLives = 5;
            startingStats.SetData(StartingLives, StartingAmmos, -1);
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
        dataTypes = lds.dataTypes;
        // pigPoolSizes = lds.pigPoolSizes;
        // aiPoolSizes = lds.aiPoolSizes;
        // buildingPoolSizes = lds.buildingPoolSizes;
        // collectablePoolSizes = lds.collectablePoolSizes;
        // positionerPoolSizes = lds.positionerPoolSizes;
        // ringPoolSizes = lds.ringPoolSizes;

        // posList = lds.posList;


        // typeList = lds.typeList;
        // poolSizes = lds.poolSizes;
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

        startingStats.SetData(StartingLives, StartingAmmos, -1);


        Debug.LogError("Finished loading data for level: " + "Starting Lives" + StartingLives + LevelName + " with " + spawnSteps.Length + " spawn steps and " + idList.Length + " objects.");
    }


    private bool checkBossAndRandomData = false;

    public void CheckRandomSpawnStep()
    {

        if (spawnStepsPerRandom <= 0) return;

        if (currentSpawnStepCount < spawnStepsPerRandom)
            currentSpawnStepCount++;

        else
        {

            byte[] rng = new byte[9];
            for (int i = 0; i < 9; i++)
            {
                rng[i] = (byte)UnityEngine.Random.Range(0, 100);
            }
            for (int i = 0; i < randomSpawnData.Length; i++)
            {
                var rsd = randomSpawnData[i];
                if (rsd == null || rsd.waveIndex != currentWave)
                {
                    Debug.LogError("Skipping Random Spawn Data at index: " + i + " for wave: " + currentWave);
                    continue;
                }

                rsd.GenerateRandomEnemySpawn(rng);
            }
            currentSpawnStepCount = 0;

            currentWave++;

            if (currentWave > maxWaves)
            {
                currentWave = 1;

            }

        }

    }
    public void NextSpawnStep(ushort ss)
    {
        currentSpawnStep = ss;
        currentSpawnStepCount++;






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


                    // if (currentSpecialTriggerIndex < specialTriggerIndexAndType.Length && i == specialTriggerIndexAndType[currentSpecialTriggerIndex].x)
                    // {
                    //     switch (specialTriggerIndexAndType[currentSpecialTriggerIndex].y)
                    //     {
                    //         case 1:
                    //             pool.SpawnBoss(GetDataByType(dataTypes[i]), levelDataBossAndRandomLogic);
                    //             break;


                    //     }
                    //     currentSpecialTriggerIndex++;
                    // }
                    if (objectTypes[i] == 1) // Boss Type
                    {
                        bool hasTrigger = false;
                        Debug.LogError("Spawning Boss for ID: " + idList[i] + " at index: " + i);
                        if (currentSpecialTriggerIndex < specialTriggerIndexAndType.Length && i == specialTriggerIndexAndType[currentSpecialTriggerIndex].x)
                        {
                            hasTrigger = true;
                            currentSpecialTriggerIndex++;
                        }
                        pool.SpawnBoss(GetDataByType(dataTypes[i]), hasTrigger);

                    }
                    else pool.Spawn(GetDataByType(dataTypes[i]));





                    // switch (dataTypes[i])
                    // {
                    //     case 0:
                    //         pool.SpawnSimpleObject(dataStructSimple[dataTypeIndexes[0]]);
                    //         break;
                    //     case 1:
                    //         pool.SpawnFloatOne(dataStructFloatOne[dataTypeIndexes[1]]);
                    //         break;
                    //     case 2:
                    //         pool.SpawnFloatTwo(dataStructFloatTwo[dataTypeIndexes[2]]);
                    //         break;
                    //     case 3:
                    //         pool.SpawnFloatThree(dataStructFloatThree[dataTypeIndexes[3]]);
                    //         break;
                    //     case 4:
                    //         pool.SpawnFloatFour(dataStructFloatFour[dataTypeIndexes[4]]);
                    //         break;
                    //     case 5:
                    //         pool.SpawnFloatFive(dataStructFloatFive[dataTypeIndexes[5]]);
                    //         break;

                    // }

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

        if (checkBossAndRandomData)
        {
            levelDataBossAndRandomLogic.CheckIfSpawnStep(ss);
        }



    }

    private ISpawnData GetDataByType(int dataType)
    {
        switch (dataType)
        {
            case 0: return dataStructSimple[dataTypeIndexes[0]];
            case 1: return dataStructFloatOne[dataTypeIndexes[1]];
            case 2: return dataStructFloatTwo[dataTypeIndexes[2]];
            case 3: return dataStructFloatThree[dataTypeIndexes[3]];
            case 4: return dataStructFloatFour[dataTypeIndexes[4]];
            case 5: return dataStructFloatFive[dataTypeIndexes[5]];
            default:
                Debug.LogWarning($"Unknown data type {dataType}");
                return default;
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

    public LevelDataArrays ReturnDataArrays(LevelDataSave lds = null, bool returnDefualt = false)
    {
        LevelDataArrays dataArrays = null;
        if (returnDefualt)
        {
            dataArrays = Resources.Load<LevelDataArrays>(LevelDataConverter.GetLevelNumberStringFormat(Vector3Int.zero) + "_DataArrays");

        }
        else
        {
            dataArrays = Resources.Load<LevelDataArrays>(LevelDataConverter.GetLevelNumberStringFormat(levelWorldAndNumber) + LevelName + "_DataArrays");
        }

        if (lds != null)
        {
            dataArrays.LoadLevelSaveData(lds);
        }

        return dataArrays;
    }

    #region  Pool And Data Creation
    private void CreatePoolsAndDataStructs(LevelDataArrays d, SpawnStateManager s)
    {

        List<ushort[]> poolList = new List<ushort[]>
        {
            d.pigPoolSizes,
            d.aiPoolSizes,
            d.buildingPoolSizes,
            d.collectablePoolSizes,
            d.positionerPoolSizes,
            d.ringPoolSizes
        };

        for (int i = 0; i < poolList.Count; i++)
        {
            if (i == 5) // ring pool
            {
                objData.ringPool.Initialize(d.ringPoolSizes[0], d.ringPoolSizes[1], s);
            }
            else if (poolList[i] != null && poolList[i].Length > 0)
            {
                var poolToPopulate = objData.GetPoolArrayByObjectType(i);
                for (int j = 0; j < poolList[i].Length; j++)
                {
                    Debug.LogError("Creating Pool for Object Type: " + i + " with size: " + poolList[i][j] + " and ID: " + j);
                    poolToPopulate[j].CreatePool(poolList[i][j], this);
                    Debug.LogError("Created Pool");
                }
            }
        }
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
        int healthIndex = 0;
        for (int i = 0; i < dataTypes.Length; i++)
        {
            short id = idList[i];
            if (objectTypes[i] == 1) // boss object, replace ID with health
            {
                id = 1;
                if (d.indexAndHealth != null && healthIndex * 2 < d.indexAndHealth.Length - 1 && i == d.indexAndHealth[healthIndex * 2])
                {
                    id = d.indexAndHealth[(healthIndex * 2) + 1];
                    healthIndex++;
                }
            }

            if (dataTypes[i] < 0)
            {
                // This is a positioner object
                subrtactIndex++;
                continue;
            }

            switch (dataTypes[i])
            {
                case 0:
                    dataStructSimple[simpleStructIndex] = new DataStructSimple(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex]);

                    simpleStructIndex++;
                    // This is a simple object, already handled above
                    break;
                case 1:
                    dataStructFloatOne[float1Index] = new DataStructFloatOne(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatList[floatListIndex]);
                    float1Index++;
                    floatListIndex++;
                    break;
                case 2:
                    dataStructFloatTwo[float2Index] = new DataStructFloatTwo(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatList[floatListIndex], d.floatList[floatListIndex + 1]);
                    float2Index++;
                    floatListIndex += 2;
                    break;
                case 3:

                    dataStructFloatThree[float3Index] = new DataStructFloatThree(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatList[floatListIndex], d.floatList[floatListIndex + 1], d.floatList[floatListIndex + 2]);
                    float3Index++;
                    floatListIndex += 3;
                    break;
                case 4:
                    dataStructFloatFour[float4Index] = new DataStructFloatFour(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatList[floatListIndex], d.floatList[floatListIndex + 1], d.floatList[floatListIndex + 2], d.floatList[floatListIndex + 3]);
                    float4Index++;
                    floatListIndex += 4;
                    break;
                case 5:
                    dataStructFloatFive[float5Index] = new DataStructFloatFive(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatList[floatListIndex], d.floatList[floatListIndex + 1], d.floatList[floatListIndex + 2], d.floatList[floatListIndex + 3], d.floatList[floatListIndex + 4]);
                    float5Index++;
                    floatListIndex += 5;
                    break;

            }

        }

        // do logic if there is random Enemy Spawn Data
        if (d.randomSpawnDataTypeObjectTypeAndID != null && d.randomSpawnDataTypeObjectTypeAndID.Length > 0)
        {
            Debug.LogError("Creating Random Spawn Data with length: " + d.randomSpawnDataTypeObjectTypeAndID.Length);
            randomSpawnData = new LevelDataRandomSpawnData[d.randomSpawnDataTypeObjectTypeAndID.Length];

            floatListIndex = 0;
            int spawnIndex = 0;
            int usedRngIndex = 0;

            for (int i = 0; i < d.randomSpawnDataTypeObjectTypeAndID.Length; i++)
            {
                LevelDataRandomSpawnData newData = new LevelDataRandomSpawnData();
                List<ISpawnData> spawnDataList = new List<ISpawnData>();
                List<short> rngUsedList = new List<short>();
                int nextRNGIndex = 0;

                for (int j = usedRngIndex; j < d.randomSpawnDataTypeObjectTypeAndID[i].x + 3 + usedRngIndex; j++)
                {
                    rngUsedList.Add(d.usedRNGIndices[j]);
                    nextRNGIndex++;



                }
                usedRngIndex += nextRNGIndex;


                for (int j = 0; j < d.randomLogicSizes[i]; j++)
                {

                    Debug.LogError("Creating random, spawn index: " + spawnIndex + ", floatListIndex: " + floatListIndex + ", type: " + d.randomSpawnDataTypeObjectTypeAndID[i].x);


                    switch (d.randomSpawnDataTypeObjectTypeAndID[i].x)
                    {
                        case 0:
                            spawnDataList.Add(new DataStructSimple(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex]));

                            break;
                        case 1:
                            spawnDataList.Add(new DataStructFloatOne(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex]));

                            floatListIndex++;
                            break;
                        case 2:
                            spawnDataList.Add(new DataStructFloatTwo(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1]));

                            floatListIndex += 2;
                            break;
                        case 3:
                            spawnDataList.Add(new DataStructFloatThree(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2]));

                            floatListIndex += 3;
                            break;
                        case 4:
                            spawnDataList.Add(new DataStructFloatFour(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3]));
                            floatListIndex += 4;
                            break;
                        case 5:
                            spawnDataList.Add(new DataStructFloatFive(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3], d.floatListRand[floatListIndex + 4]));
                            floatListIndex += 5;
                            break;


                    }
                    spawnIndex++;
                }

                Debug.Log("Created Random Spawn Data with wave index: " + d.randomLogicWaveIndices[i]);




                // RecordableObjectPool p = objData.GetPoolArrayByObjectType(d.randomSpawnDataTypeObjectTypeAndID[i].y)[d.randomSpawnDataTypeObjectTypeAndID[i].z];
                newData.Initialize(spawnDataList.ToArray(), rngUsedList.ToArray(), objData.GetPoolArrayByObjectType(d.randomSpawnDataTypeObjectTypeAndID[i].y)[d.randomSpawnDataTypeObjectTypeAndID[i].z], (ushort)d.randomSpawnDataTypeObjectTypeAndID[i].x, d.randomLogicWaveIndices[i], d.randomSpawnRanges[i * 2], d.randomSpawnRanges[(i * 2) + 1]);
                randomSpawnData[i] = newData;
            }
        }
        Resources.UnloadAsset(d);
    }

    private void CreatePoolsAndDataStructs(LevelDataSave d, SpawnStateManager s)
    {

        List<ushort[]> poolList = new List<ushort[]>
        {
            d.pigPoolSizes,
            d.aiPoolSizes,
            d.buildingPoolSizes,
            d.collectablePoolSizes,
            d.positionerPoolSizes,
            d.ringPoolSizes
        };

        for (int i = 0; i < poolList.Count; i++)
        {
            if (i == 5) // ring pool
            {
                objData.ringPool.Initialize(d.ringPoolSizes[0], d.ringPoolSizes[1], s);
            }
            else if (poolList[i] != null && poolList[i].Length > 0)
            {
                var poolToPopulate = objData.GetPoolArrayByObjectType(i);
                for (int j = 0; j < poolList[i].Length; j++)
                {
                    poolToPopulate[j].CreatePool(poolList[i][j], this);
                }
            }
        }
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
        int healthIndex = 0;
        for (int i = 0; i < dataTypes.Length; i++)
        {

            short id = idList[i];
            if (objectTypes[i] == 1) // boss object, replace ID with health
            {
                id = 1;
                if (d.indexAndHealth != null && healthIndex < d.indexAndHealth.Length - 1 && d.indexAndHealth[i * 2] == i)
                {
                    id = d.indexAndHealth[(i * 2) + 1];
                    healthIndex++;
                }
            }

            if (dataTypes[i] < 0)
            {
                // This is a positioner object
                subrtactIndex++;
                continue;
            }

            switch (dataTypes[i])
            {
                case 0:
                    dataStructSimple[simpleStructIndex] = new DataStructSimple(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex]);

                    simpleStructIndex++;
                    // This is a simple object, already handled above
                    break;
                case 1:
                    dataStructFloatOne[float1Index] = new DataStructFloatOne(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatListRand[floatListIndex]);
                    float1Index++;
                    floatListIndex++;
                    break;
                case 2:
                    dataStructFloatTwo[float2Index] = new DataStructFloatTwo(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1]);
                    float2Index++;
                    floatListIndex += 2;
                    break;
                case 3:

                    dataStructFloatThree[float3Index] = new DataStructFloatThree(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2]);
                    float3Index++;
                    floatListIndex += 3;
                    break;
                case 4:
                    dataStructFloatFour[float4Index] = new DataStructFloatFour(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3]);
                    float4Index++;
                    floatListIndex += 4;
                    break;
                case 5:
                    dataStructFloatFive[float5Index] = new DataStructFloatFive(id, d.typeList[i - subrtactIndex], d.posList[i - subrtactIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3], d.floatListRand[floatListIndex + 4]);
                    float5Index++;
                    floatListIndex += 5;
                    break;

            }

        }
        if (d.randomSpawnDataTypeObjectTypeAndID != null && d.randomSpawnDataTypeObjectTypeAndID.Length > 0)
        {
            randomSpawnData = new LevelDataRandomSpawnData[d.randomSpawnDataTypeObjectTypeAndID.Length];

            floatListIndex = 0;
            int spawnIndex = 0;


            for (int i = 0; i < d.randomSpawnDataTypeObjectTypeAndID.Length; i++)
            {
                LevelDataRandomSpawnData newData = new LevelDataRandomSpawnData();
                List<ISpawnData> spawnDataList = new List<ISpawnData>();

                for (int j = 0; j < d.randomLogicSizes[i]; j++)
                {


                    switch (d.randomSpawnDataTypeObjectTypeAndID[i].x)
                    {
                        case 0:
                            spawnDataList.Add(new DataStructSimple(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex]));

                            break;
                        case 1:
                            spawnDataList.Add(new DataStructFloatOne(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex]));

                            floatListIndex++;
                            break;
                        case 2:
                            spawnDataList.Add(new DataStructFloatTwo(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1]));

                            floatListIndex += 2;
                            break;
                        case 3:
                            spawnDataList.Add(new DataStructFloatThree(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2]));

                            floatListIndex += 3;
                            break;
                        case 4:
                            spawnDataList.Add(new DataStructFloatFour(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3]));
                            floatListIndex += 4;
                            break;
                        case 5:
                            spawnDataList.Add(new DataStructFloatFive(0, d.typeListRand[spawnIndex], d.posListRand[spawnIndex], d.floatListRand[floatListIndex], d.floatListRand[floatListIndex + 1], d.floatListRand[floatListIndex + 2], d.floatListRand[floatListIndex + 3], d.floatListRand[floatListIndex + 4]));
                            floatListIndex += 5;
                            break;


                    }
                    spawnIndex++;
                }


                // RecordableObjectPool p = objData.GetPoolArrayByObjectType(d.randomSpawnDataTypeObjectTypeAndID[i].y)[d.randomSpawnDataTypeObjectTypeAndID[i].z];
                newData.Initialize(spawnDataList.ToArray(), null, objData.GetPoolArrayByObjectType(d.randomSpawnDataTypeObjectTypeAndID[i].y)[d.randomSpawnDataTypeObjectTypeAndID[i].z], (ushort)d.randomSpawnDataTypeObjectTypeAndID[i].x, d.randomLogicWaveIndices[i]);
                randomSpawnData[i] = newData;
            }
        }

        // Resources.UnloadAsset(d);


    }

    #endregion








}



