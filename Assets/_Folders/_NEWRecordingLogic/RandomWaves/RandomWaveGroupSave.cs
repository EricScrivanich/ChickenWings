using UnityEngine;

public class RandomWaveGroupSave
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector2[] posListRand;
    public float[] floatListRand;
    public ushort[] typeListRand;
    public short[] usedRNGIndices;
    public ushort[] randomLogicSizes;
    public ushort[] randomLogicWaveIndices;

    public Vector3Int[] randomSpawnDataTypeObjectTypeAndID;
    public ushort[] spawnStepsRandom;

  

    public byte[] randomSpawnRanges;

    public RandomWaveGroupSave(Vector2[] posListRand, float[] floatListRand, ushort[] typeListRand, ushort[] randomLogicSizes, ushort[] randomLogicWaveIndices, Vector3Int[] randomSpawnDataTypeObjectTypeAndID, ushort[] spawnStepsRandom, short[] usedRNGIndices, byte[] randomSpawnRanges)
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
        
    }

}
