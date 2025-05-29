using System.Runtime.InteropServices;
using UnityEngine;


public readonly struct RecordedDataStruct
{
    public readonly short ID;
    public readonly Vector2 startPos;
    public readonly float speed;
    public readonly float scale;

    public readonly float magPercent;
    public readonly float timeInterval;
    public readonly float delayInterval;
    public readonly short type;



    public RecordedDataStruct(short id, Vector2 pos, float sp, float sc, float m, float t, float d, short b)
    {
        this.ID = id;
        this.startPos = pos;
        this.speed = sp;
        this.scale = sc;
        this.magPercent = m;
        this.timeInterval = t;
        this.delayInterval = d;
        this.type = b;

    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatOne
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes (x, y)
    public readonly float float1;     // 4 bytes

    public DataStructFloatOne(short id, ushort type, Vector2 startPos, float float1)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatTwo
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes

    public DataStructFloatTwo(short id, ushort type, Vector2 startPos, float float1, float float2)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
        this.float2 = float2;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatThree
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes

    public DataStructFloatThree(short id, ushort type, Vector2 startPos, float float1, float float2, float float3)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
        this.float2 = float2;
        this.float3 = float3;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatFour
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes
    public readonly float float4;     // 4 bytes

    public DataStructFloatFour(short id, ushort type, Vector2 startPos, float float1, float float2, float float3, float float4)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
        this.float2 = float2;
        this.float3 = float3;
        this.float4 = float4;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatFive
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes
    public readonly float float4;     // 4 bytes
    public readonly float float5;     // 4 bytes

    public DataStructFloatFive(short id, ushort type, Vector2 startPos, float float1, float float2, float float3, float float4, float float5)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
        this.float2 = float2;
        this.float3 = float3;
        this.float4 = float4;
        this.float5 = float5;
    }



}

[System.Serializable]
public class RecordedObjectPositionerDataSave
{
    public short ID;

    public ushort type; // 2 bytes
    public Vector3 startPosAndRot;
    public float savedTimePerStep;


    
    public Vector2[] positions;
    public float[] values;

    public ushort[] sizeByType;

    public ushort[] startingSteps;
    public ushort[] endingSteps;
    public ushort staringSpawnStep;
    public ushort finalSpawnStep;
    private int currentStepIndex;

    private int currentFloatIndex;

    public Vector2[] ReturnIntervalAndDuration()
    {
        Vector2[] intervalsAndDurations = new Vector2[startingSteps.Length];

        for (int i = 1; i < startingSteps.Length; i++)
        {
            float interval = (endingSteps[i - 1] - startingSteps[i]) * -savedTimePerStep;
            float duration = (finalSpawnStep - startingSteps[i]) * savedTimePerStep;
            intervalsAndDurations[i].x = interval;
            intervalsAndDurations[i].y = duration;
        }

        return intervalsAndDurations;
    

    }

    public RecordedObjectPositionerDataSave(short id, ushort typ,ushort[] sizesByType, Vector3 posRot, Vector2[] positions, float[] values, ushort[] startingSteps, ushort[] endingSteps, ushort startStep, ushort finalSpawnStep)
    {
        this.ID = id;
        this.type = typ;
        this.savedTimePerStep = LevelRecordManager.TimePerStep;
        this.sizeByType = sizesByType;
        this.startPosAndRot = posRot; // Default value, can be set later
        
        this.positions = positions;
        this.values = values;
        this.startingSteps = startingSteps;
        this.endingSteps = endingSteps;
        this.staringSpawnStep = startStep;
        this.finalSpawnStep = finalSpawnStep;
    }



}