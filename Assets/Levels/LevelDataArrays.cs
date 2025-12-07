using UnityEngine;
[CreateAssetMenu(fileName = "Level Data Arrays", menuName = "Setups/LevelDataArrays")]
public class LevelDataArrays : ScriptableObject
{
    // [field: SerializeField] public short[] StartingAmmos { get; private set; }
    // [field: SerializeField] public short StartingLives { get; private set; }

    // [field: SerializeField] public short[] easyStartingAmmos { get; private set; }
    // [field: SerializeField] public short easyStartingLives { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ushort[] pigPoolSizes;
    public ushort[] aiPoolSizes;
    public ushort[] buildingPoolSizes;
    public ushort[] collectablePoolSizes;
    public ushort[] positionerPoolSizes;
    public ushort[] ringPoolSizes;
    public ushort[] cageAttachments;
    public short[] indexAndHealth; // For enemies that have health, store index of enemy and then health value
    public Vector2[] posList;
    public float[] floatList;
    public ushort[] typeList;
    public Vector2[] posListRand;
    public float[] floatListRand;
    public short[] usedRNGIndices;
    public byte[] randomSpawnRanges;

    public ushort[] typeListRand;
    public ushort[] randomLogicSizes;
    public ushort[] randomLogicWaveIndices;
    public ushort[] spawnStepsRandom;
    public Vector3Int[] randomSpawnDataTypeObjectTypeAndID;


    public void LoadLevelSaveData(LevelDataSave lds)
    {
        if (lds == null)
        {
            Debug.LogError("LevelDataSave is null");

            posList = new Vector2[0];

            floatList = new float[0];
            // StartingAmmos = new short[5];
            // StartingAmmos[0] = 3;
            // StartingAmmos[1] = 3;
            // StartingLives = 3;

            pigPoolSizes = new ushort[0];
            aiPoolSizes = new ushort[0];
            buildingPoolSizes = new ushort[0];
            collectablePoolSizes = new ushort[0];
            positionerPoolSizes = new ushort[0];
            ringPoolSizes = new ushort[0];



            return;
        }
        Debug.LogError("Loading Level Data: " + lds.levelName + " with " + lds.spawnSteps.Length + " spawn steps.");

        // levelWorldAndNumber = lds.levelWorldAndNumber;

        floatList = lds.floatList;

        cageAttachments = lds.cageAttachments;
        pigPoolSizes = lds.pigPoolSizes;
        aiPoolSizes = lds.aiPoolSizes;
        buildingPoolSizes = lds.buildingPoolSizes;
        collectablePoolSizes = lds.collectablePoolSizes;
        positionerPoolSizes = lds.positionerPoolSizes;
        ringPoolSizes = lds.ringPoolSizes;

        posList = lds.posList;

        // speedList = lds.speeds;
        // scaleList = lds.scaleList;
        // magList = lds.magList;
        // timeList = lds.timeList;
        // delayList = lds.delayList;
        typeList = lds.typeList;
        indexAndHealth = lds.indexAndHealth;

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




        // if (lds.startingAmmos == null || lds.startingAmmos.Length <= 0)
        // {
        //     Debug.LogError("Starting Ammos is null or empty, setting default values.");
        //     StartingAmmos = new short[5];
        //     StartingAmmos[0] = 3;
        //     StartingAmmos[1] = 3;

        //     startingStats.startingAmmos = new short[5];
        //     startingStats.startingAmmos[0] = 3;
        //     startingStats.startingAmmos[1] = 3;

        // }
        // else
        //     StartingAmmos = lds.startingAmmos;

        // if (lds.StartingLives <= 0)
        //     StartingLives = 3;
        // else
        //     StartingLives = lds.StartingLives;

        // startingStats.SetData(StartingLives, StartingAmmos, -1);



    }

    public void SetData(LevelData data)
    {
        if (data == null)
        {
            Debug.LogError("LevelData is null — cannot copy data.");
            return;
        }

        // Shallow fields (copied by value)
        // StartingLives = data.StartingLives;
        // easyStartingLives = data.easyStartingLives;

        // // Copy all array fields (deep copy for value types)
        // StartingAmmos = data.StartingAmmos != null ? (short[])data.StartingAmmos.Clone() : null;
        // easyStartingAmmos = data.easyStartingAmmos != null ? (short[])data.easyStartingAmmos.Clone() : null;

        pigPoolSizes = data.pigPoolSizes != null ? (ushort[])data.pigPoolSizes.Clone() : null;
        aiPoolSizes = data.aiPoolSizes != null ? (ushort[])data.aiPoolSizes.Clone() : null;
        buildingPoolSizes = data.buildingPoolSizes != null ? (ushort[])data.buildingPoolSizes.Clone() : null;
        collectablePoolSizes = data.collectablePoolSizes != null ? (ushort[])data.collectablePoolSizes.Clone() : null;
        positionerPoolSizes = data.positionerPoolSizes != null ? (ushort[])data.positionerPoolSizes.Clone() : null;
        ringPoolSizes = data.ringPoolSizes != null ? (ushort[])data.ringPoolSizes.Clone() : null;
        cageAttachments = data.cageAttachments != null ? (ushort[])data.cageAttachments.Clone() : null;

        posList = data.posList != null ? (Vector2[])data.posList.Clone() : null;
        floatList = data.floatList != null ? (float[])data.floatList.Clone() : null;
        typeList = data.typeList != null ? (ushort[])data.typeList.Clone() : null;



    }


}
