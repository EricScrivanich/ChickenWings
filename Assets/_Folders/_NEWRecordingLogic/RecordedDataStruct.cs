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
public readonly struct DataStructSimple : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes (x, y)



    public void ApplyTo(SpawnedObject obj) => obj.ApplySimpleData(this);



    public Vector2 GetStartPos()
    {
        return startPos;
    }

    ushort ISpawnData.GetType()
    {
        return type;
    }

    public DataStructSimple(short id, ushort type, Vector2 startPos)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;

    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatOne : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes (x, y)
    public readonly float float1;     // 4 bytes
    public void ApplyTo(SpawnedObject obj) => obj.ApplyFloatOneData(this);
    public Vector2 GetStartPos()
    {
        return startPos;
    }
    ushort ISpawnData.GetType()
    {
        return type;
    }
    public DataStructFloatOne(short id, ushort type, Vector2 startPos, float float1)
    {
        this.ID = id;
        this.type = type;
        this.startPos = startPos;
        this.float1 = float1;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct DataStructFloatTwo : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public void ApplyTo(SpawnedObject obj) => obj.ApplyFloatTwoData(this);
    public Vector2 GetStartPos()
    {
        return startPos;
    }

    ushort ISpawnData.GetType()
    {
        return type;
    }

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
public readonly struct DataStructFloatThree : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes
    public void ApplyTo(SpawnedObject obj) => obj.ApplyFloatThreeData(this);
    public Vector2 GetStartPos()
    {
        return startPos;
    }
    ushort ISpawnData.GetType()
    {
        return type;
    }
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
public readonly struct DataStructFloatFour : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes
    public readonly float float4;     // 4 bytes
    public void ApplyTo(SpawnedObject obj) => obj.ApplyFloatFourData(this);
    public Vector2 GetStartPos()
    {
        return startPos;
    }
    ushort ISpawnData.GetType()
    {
        return type;
    }
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
public readonly struct DataStructFloatFive : ISpawnData
{
    public readonly short ID;         // 2 bytes
    public readonly ushort type;      // 2 bytes
    public readonly Vector2 startPos; // 8 bytes
    public readonly float float1;     // 4 bytes
    public readonly float float2;     // 4 bytes
    public readonly float float3;     // 4 bytes
    public readonly float float4;     // 4 bytes
    public readonly float float5;     // 4 bytes
    public void ApplyTo(SpawnedObject obj) => obj.ApplyFloatFiveData(this);
    public Vector2 GetStartPos()
    {
        return startPos;
    }
    ushort ISpawnData.GetType()
    {
        return type;
    }
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
    public Vector2 startPos;
    public float startRot;


    public float savedTimePerStep;

    public ushort[] sizeByType;
    public float perecnt;
    public Vector2[] positions;
    public float[] values;



    public ushort[] startingSteps;
    public ushort[] endingSteps;
    public ushort[] easeTypes;
    public ushort staringSpawnStep;
    public ushort finalSpawnStep;

    public void ReturnIntervalAndDuration(ushort t, out float[] intervals, out float[] durations, out ushort[] eases)
    {
        int length = sizeByType[t];
        int current = 0;
        for (int i = 0; i < t; i++) current += sizeByType[i];
        intervals = new float[length];
        durations = new float[length];
        Debug.LogError("Length of intervals and durations is: " + length + " for type: " + t + " and current: " + current);

        if (t == 2)
            eases = new ushort[0];
        else
            eases = new ushort[length];
        for (int i = current; i < current + length; i++)
        {
            float interval = 0;
            if (i == current)
                interval = (staringSpawnStep - startingSteps[i]) * -savedTimePerStep;
            else
                interval = (endingSteps[i - 1] - startingSteps[i]) * -savedTimePerStep;
            float duration = (endingSteps[i] - startingSteps[i]) * savedTimePerStep;
            intervals[i - current] = interval;
            durations[i - current] = duration;
            Debug.LogError("Interval for type: " + t + " at index: " + i + " is: " + interval + " and duration is: " + duration);
            if (t < 2)
                eases[i - current] = easeTypes[i];
        }

    }

    public void SetDataForRecording(RecordedDataStructTweensDynamic data)
    {
        int t = data.type;
        int length = sizeByType[t];
        int current = 0;
        for (int i = 0; i < t; i++) current += sizeByType[i];
        for (int i = current; i < current + length; i++)
        {
            data.startingSteps.Add(startingSteps[i]);
            data.endingSteps.Add(endingSteps[i]);

            if (t == 2) data.easeTypes.Add(0);
            else data.easeTypes.Add(easeTypes[i]);

        }

        if (t == 0)
        {
            foreach (var p in positions)
            {
                data.positions.Add(p);
            }
        }
        else if (t == 1) data.values.AddRange(values);



    }



    public RecordedObjectPositionerDataSave(short id, ushort typ, ushort[] sizesByType, float p, Vector3 posRot, Vector2[] positions, float[] values, ushort[] eases, ushort[] startingSteps, ushort[] endingSteps, ushort startStep, ushort finalSpawnStep)
    {
        this.ID = id;
        this.type = typ;
        this.savedTimePerStep = LevelRecordManager.TimePerStep;
        this.sizeByType = sizesByType;
        for (int i = 0; i < sizesByType.Length; i++)
        {
            Debug.LogError("Size by type of type:" + i + " is: " + sizesByType[i]);
        }
        this.perecnt = p;
        this.startPos = new Vector2(posRot.x, posRot.y); // Default value, can be set later
        this.startRot = posRot.z; // Default value, can be set later
        this.positions = positions;
        this.values = values;
        this.easeTypes = eases;
        this.startingSteps = startingSteps;
        this.endingSteps = endingSteps;
        this.staringSpawnStep = startStep;
        this.finalSpawnStep = finalSpawnStep;
    }



}