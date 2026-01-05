using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LevelDataSave
{
    public string levelName;
    public short[] objectTypes;
    public short[] idList;
    public ushort[] spawnSteps;
    public ushort finalSpawnStep;
    public ushort[] typeList;

    public Vector2[] posList;
    public short[] dataTypes;

    public float[] floatList;
    public ushort[] poolSizes;
    public ushort[] pigPoolSizes;
    public ushort[] aiPoolSizes;
    public ushort[] buildingPoolSizes;
    public ushort[] collectablePoolSizes;
    public ushort[] positionerPoolSizes;
    public ushort[] ringPoolSizes;
    public short[] indexAndHealth; // For enemies that have health, store index of enemy and then health value


    public Vector2[] posListRand;
    public float[] floatListRand;
    public ushort[] typeListRand;
    public short[] usedRNGIndices;
    public ushort[] randomLogicSizes;
    public ushort[] randomLogicWaveIndices;
   
    public Vector3Int[] randomSpawnDataTypeObjectTypeAndID;
    public ushort[] spawnStepsRandom;

    public ushort[] cageAttachments;

    public byte[] randomSpawnRanges;






    public short[] startingAmmos;
    public short StartingLives;

    public RecordedObjectPositionerDataSave[] postionerData = null;

    public void SetRandomData(Vector2[] posListRand, float[] floatListRand, ushort[] typeListRand, ushort[] randomLogicSizes, ushort[] randomLogicWaveIndices, Vector3Int[] randomSpawnDataTypeObjectTypeAndID, ushort[] spawnStepsRandom, short[] usedRNGIndices, byte[] randomSpawnRanges)
    {
        this.posListRand = posListRand;
        this.floatListRand = floatListRand;
        this.typeListRand = typeListRand;
        this.randomLogicSizes = randomLogicSizes;
        this.randomLogicWaveIndices = randomLogicWaveIndices;
        this.randomSpawnDataTypeObjectTypeAndID = randomSpawnDataTypeObjectTypeAndID;
        this.spawnStepsRandom = spawnStepsRandom;
        this.usedRNGIndices = usedRNGIndices;
        this.randomSpawnRanges = randomSpawnRanges;

        for (int i = 0; i < randomLogicSizes.Length; i++)
        {
            Debug.Log("Random Logic Size " + i + ": " + randomLogicSizes[i]);
        }
        for (int i = 0; i < randomLogicWaveIndices.Length; i++)
        {
            Debug.Log("Random Logic Wave Index " + i + ": " + randomLogicWaveIndices[i]);
        }


    }

    public LevelDataSave(string levelName, short[] objectTypes, short[] idList, ushort[] spawnSteps, ushort[] typeList, short[] dataTypeList, ushort finalStep, Vector2[] posList, float[] floats, List<ushort[]> plList, short[] ammos, short lives, short[] inxAndHlth)
    {
        this.levelName = levelName;
        this.objectTypes = objectTypes;
        this.idList = idList;
        this.typeList = typeList;
        this.dataTypes = dataTypeList;
        this.posList = posList;
        this.spawnSteps = spawnSteps;
        this.finalSpawnStep = finalStep;
        this.floatList = floats;

        this.pigPoolSizes = plList[0];
        this.aiPoolSizes = plList[1];
        this.buildingPoolSizes = plList[2];
        this.collectablePoolSizes = plList[3];
        this.positionerPoolSizes = plList[4];
        this.ringPoolSizes = plList[5];
        this.startingAmmos = ammos;
        this.StartingLives = lives;
        this.cageAttachments = null;
        this.postionerData = null;
        this.indexAndHealth = inxAndHlth;

    }
    public void SetPositionerData(RecordedObjectPositionerDataSave[] data)
    {
        if (data == null || data.Length == 0)
        {
            Debug.LogError("Positioner data is null or empty, not setting it.");
            return;
        }
        this.postionerData = data;
    }
    public void SetCageAttachments(ushort[] cageAttachments)
    {
        if (cageAttachments == null || cageAttachments.Length == 0)
        {
            Debug.LogError("Cage attachments data is null or empty, not setting it.");
            return;
        }
        this.cageAttachments = cageAttachments;
    }



}
