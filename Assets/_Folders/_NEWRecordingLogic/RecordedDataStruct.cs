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