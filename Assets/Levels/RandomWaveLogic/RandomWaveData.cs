using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Wave Data", menuName = "Setups/RandomWaveData")]
public class RandomWaveData : ScriptableObject
{

    public LevelDataRandomSpawnData[] randomSpawnData;
    private int currentSpawnStepCount = 0;
    public int spawnStepsPerRandom = 2;
    private int currentWave = 1;
    public int maxWaves = 5;

    public Vector2[] posListRand;
    public float[] floatListRand;
    public short[] usedRNGIndices;
    public byte[] randomSpawnRanges;

    public ushort[] typeListRand;
    public ushort[] randomLogicSizes;
    public ushort[] randomLogicWaveIndices;
    public ushort[] spawnStepsRandom;
    public Vector3Int[] randomSpawnDataTypeObjectTypeAndID;


    [Header("Random Seed Settings")]
    public bool useFixedSeed = false;
    public bool useFixedSeedCollectable = false;
    public int fixedSeed = 12345;
    public int fixedSeedCollectable = 12345;

    private System.Random randomSeed;
    private System.Random randomSeedCollectable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void InitializeRNG()
    {
        rngIndex = 0;
        if (useFixedSeed)
        {
            randomSeed = new System.Random(fixedSeed);

            Debug.Log($"Using FIXED RNG seed: {fixedSeed}");
        }
        else
        {
            int randomSeedValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            randomSeed = new System.Random(randomSeedValue);
            Debug.Log($"Using RANDOM RNG seed: {randomSeedValue}");
        }
        if (useFixedSeedCollectable)
        {
            randomSeedCollectable = new System.Random(fixedSeedCollectable);

            Debug.Log($"Using FIXED Collectable RNG seed: {fixedSeedCollectable}");
        }
        else
        {
            int randomSeedValueCollectable = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            randomSeedCollectable = new System.Random(randomSeedValueCollectable);
            Debug.Log($"Using RANDOM Collectable RNG seed: {randomSeedValueCollectable}");
        }
    }
    private int rngIndex = 0;
    public byte GetRNG()
    {

        byte rngValue = (byte)randomSeed.Next(0, 100);
        Debug.Log("Generated RNG value: " + rngValue + " at index: " + rngIndex);
        rngIndex++;
        return rngValue;

    }

    public byte GetCollectableRNG()
    {

        return (byte)randomSeedCollectable.Next(0, 100);



    }
    public void PopulateData(AllObjectData objData)
    {
        if (randomSpawnDataTypeObjectTypeAndID == null || randomSpawnDataTypeObjectTypeAndID.Length <= 0)
            return;
        InitializeRNG();
        currentWave = 0;
        currentSpawnStepCount = 0;



        randomSpawnData = new LevelDataRandomSpawnData[randomSpawnDataTypeObjectTypeAndID.Length];

        int floatListIndex = 0;
        int spawnIndex = 0;
        int usedRngIndex = 0;

        for (int i = 0; i < randomSpawnDataTypeObjectTypeAndID.Length; i++)
        {
            LevelDataRandomSpawnData newData = new LevelDataRandomSpawnData();
            List<ISpawnData> spawnDataList = new List<ISpawnData>();
            List<short> rngUsedList = new List<short>();
            int nextRNGIndex = 0;

            for (int j = usedRngIndex; j < randomSpawnDataTypeObjectTypeAndID[i].x + 3 + usedRngIndex; j++)
            {
                rngUsedList.Add(usedRNGIndices[j]);
                nextRNGIndex++;



            }
            usedRngIndex += nextRNGIndex;


            for (int j = 0; j < randomLogicSizes[i]; j++)
            {

                Debug.LogError("Creating random, spawn index: " + spawnIndex + ", floatListIndex: " + floatListIndex + ", type: " + randomSpawnDataTypeObjectTypeAndID[i].x);


                switch (randomSpawnDataTypeObjectTypeAndID[i].x)
                {
                    case 0:
                        spawnDataList.Add(new DataStructSimple(0, typeListRand[spawnIndex], posListRand[spawnIndex]));

                        break;
                    case 1:
                        spawnDataList.Add(new DataStructFloatOne(0, typeListRand[spawnIndex], posListRand[spawnIndex], floatListRand[floatListIndex]));

                        floatListIndex++;
                        break;
                    case 2:
                        spawnDataList.Add(new DataStructFloatTwo(0, typeListRand[spawnIndex], posListRand[spawnIndex], floatListRand[floatListIndex], floatListRand[floatListIndex + 1]));

                        floatListIndex += 2;
                        break;
                    case 3:
                        spawnDataList.Add(new DataStructFloatThree(0, typeListRand[spawnIndex], posListRand[spawnIndex], floatListRand[floatListIndex], floatListRand[floatListIndex + 1], floatListRand[floatListIndex + 2]));

                        floatListIndex += 3;
                        break;
                    case 4:
                        spawnDataList.Add(new DataStructFloatFour(0, typeListRand[spawnIndex], posListRand[spawnIndex], floatListRand[floatListIndex], floatListRand[floatListIndex + 1], floatListRand[floatListIndex + 2], floatListRand[floatListIndex + 3]));
                        floatListIndex += 4;
                        break;
                    case 5:
                        spawnDataList.Add(new DataStructFloatFive(0, typeListRand[spawnIndex], posListRand[spawnIndex], floatListRand[floatListIndex], floatListRand[floatListIndex + 1], floatListRand[floatListIndex + 2], floatListRand[floatListIndex + 3], floatListRand[floatListIndex + 4]));
                        floatListIndex += 5;
                        break;


                }
                spawnIndex++;
            }

            Debug.Log("Created Random Spawn Data with wave index: " + randomLogicWaveIndices[i]);




            // RecordableObjectPool p = objData.GetPoolArrayByObjectType(d.randomSpawnDataTypeObjectTypeAndID[i].y)[d.randomSpawnDataTypeObjectTypeAndID[i].z];
            newData.Initialize(this, spawnDataList.ToArray(), rngUsedList.ToArray(), objData.GetPoolArrayByObjectType(randomSpawnDataTypeObjectTypeAndID[i].y)[randomSpawnDataTypeObjectTypeAndID[i].z], (ushort)randomSpawnDataTypeObjectTypeAndID[i].x, randomLogicWaveIndices[i], randomSpawnRanges[i * 2], randomSpawnRanges[(i * 2) + 1]);
            randomSpawnData[i] = newData;
        }
        if (randomLogicWaveIndices != null && randomLogicWaveIndices.Length > 0)
            maxWaves = randomLogicWaveIndices[randomLogicWaveIndices.Length - 1];

    }

    public void SetRandomEnemySpawnSteps(int stepsPerSpawn)
    {
        currentSpawnStepCount = 0;
        spawnStepsPerRandom = stepsPerSpawn;
    }

    public void CheckRandomSpawnStep()
    {

        if (spawnStepsPerRandom <= 0) return;

        if (currentSpawnStepCount < spawnStepsPerRandom)
        {
            currentSpawnStepCount++;
            Debug.Log("Incrementing spawn step count to: " + currentSpawnStepCount);

        }

        else
        {

            byte[] rngBytes = new byte[9];
            for (int i = 0; i < rngBytes.Length; i++)
            {
                rngBytes[i] = GetRNG();
            }
            if (randomSpawnData != null && randomSpawnData.Length > 0)
                for (int i = 0; i < randomSpawnData.Length; i++)
                {
                    var rsd = randomSpawnData[i];
                    if (rsd == null || rsd.waveIndex != currentWave)
                    {
                        Debug.LogError("Skipping Random Spawn Data at index: " + i + " for wave: " + currentWave);
                        continue;
                    }

                    rsd.GenerateRandomEnemySpawn(rngBytes);
                }
            currentSpawnStepCount = 0;

            currentWave++;

            if (currentWave > maxWaves)
            {
                currentWave = 1;

            }

        }

    }

    public void CopyFromDataArray(LevelDataArrays lds)
    {
        if (lds != null && lds.randomSpawnDataTypeObjectTypeAndID != null && lds.randomSpawnDataTypeObjectTypeAndID.Length > 0)
        {
            usedRNGIndices = lds.usedRNGIndices;
            posListRand = lds.posListRand;
            floatListRand = lds.floatListRand;
            typeListRand = lds.typeListRand;
            randomLogicSizes = lds.randomLogicSizes;
            randomSpawnDataTypeObjectTypeAndID = lds.randomSpawnDataTypeObjectTypeAndID;
            spawnStepsRandom = lds.spawnStepsRandom;
            randomLogicWaveIndices = lds.randomLogicWaveIndices;
            randomSpawnRanges = lds.randomSpawnRanges;
        }
        else
        {
            usedRNGIndices = null;
            posListRand = null;
            floatListRand = null;
            typeListRand = null;
            randomLogicSizes = null;
            randomSpawnDataTypeObjectTypeAndID = null;
            spawnStepsRandom = null;
            randomLogicWaveIndices = null;
            randomSpawnRanges = null;
        }

    }


    public void LoadLevelSaveData(LevelDataSave lds)
    {
        if (lds.randomSpawnDataTypeObjectTypeAndID != null && lds.randomSpawnDataTypeObjectTypeAndID.Length > 0)
        {
            usedRNGIndices = lds.usedRNGIndices;
            posListRand = lds.posListRand;
            floatListRand = lds.floatListRand;
            typeListRand = lds.typeListRand;
            randomLogicSizes = lds.randomLogicSizes;
            randomSpawnDataTypeObjectTypeAndID = lds.randomSpawnDataTypeObjectTypeAndID;
            spawnStepsRandom = lds.spawnStepsRandom;
            randomLogicWaveIndices = lds.randomLogicWaveIndices;
            randomSpawnRanges = lds.randomSpawnRanges;
        }
        else
        {
            usedRNGIndices = null;
            posListRand = null;
            floatListRand = null;
            typeListRand = null;
            randomLogicSizes = null;
            randomSpawnDataTypeObjectTypeAndID = null;
            spawnStepsRandom = null;
            randomLogicWaveIndices = null;
            randomSpawnRanges = null;
        }

    }
}
