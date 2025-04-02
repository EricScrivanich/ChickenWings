using UnityEngine;

[System.Serializable]
public struct RecordedDataStructDynamic
{
    public short ID;
    public ushort type;

    public ushort spawnedStep;
    public ushort unspawnedStep;
    public Vector2 startPos;
    public float float1;
    public float float2;

    public float float3;
    public float float4;
    public float float5;



    public RecordedDataStructDynamic(short id, ushort typ, Vector2 pos, float f1, float f2, float f3, float f4, float f5, ushort spawnStep, ushort unspawnStep)
    {
        this.ID = id;
        this.type = typ;
        this.startPos = pos;
        this.float1 = f1;
        this.float2 = f2;
        this.float3 = f3;
        this.float4 = f4;
        this.float5 = f5;
        this.spawnedStep = spawnStep;
        this.unspawnedStep = unspawnStep;

    }
}