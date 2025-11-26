using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecordedDataStructDynamic
{
    public short ObjectType;
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

    public short triggerType;

    public bool hasCageAttachment;
    public short health;

    //wave index, subWave index, pouint within subwave index
    public Vector3Int randomSpawnData;


    public RecordedObjectPositionerDataSave positionerData;





    public RecordedDataStructDynamic(short objectType, short id, ushort typ, Vector2 pos, float f1, float f2, float f3, float f4, float f5, ushort spawnStep, ushort unspawnStep, Vector3Int randomSpawn, bool hasCage = false, short triggerType = 0, short health = 1)
    {
        this.ObjectType = objectType;
        this.ID = id;
        this.type = typ;
        this.startPos = pos;
        this.float1 = f1;
        this.float2 = f2;
        this.float3 = f3;
        this.float4 = f4;
        this.float5 = f5;


        positionerData = null;

        this.spawnedStep = spawnStep;
        this.unspawnedStep = unspawnStep;
        this.hasCageAttachment = hasCage;
        this.triggerType = triggerType;
        this.health = health;
        this.randomSpawnData = randomSpawn;

    }
}


// [System.Serializable]
// public struct RecordedDataStructPositionDynamic
// {
//     public short ID;
//     public ushort type;
//     public Vector2 startPos;
//     public ushort spawnedStep;
//     public ushort unspawnedStep;

//     public float[] floats;


//     public RecordedDataStructPositionDynamic(short id, ushort typ)
//     {
//         this.ID = id;
//         this.type = typ;
//         this.easeTypes = new List<ushort>();
//         easeTypes.Add(0);

//         this.positions = new List<Vector2>();
//         this.delayValues = new List<float>();
//         delayValues.Add(0);
//         this.values = new List<float>();
//         values.Add(0);
//         this.appraochTimes = new List<float>();
//         appraochTimes.Add(0);




//     }
// }

[System.Serializable]
public class RecordedDataStructTweensDynamic
{
    public short ID;
    public ushort type;
    public List<ushort> easeTypes;
    public List<Vector2> positions;

    public List<float> values;
    // public List<ushort> other;
    public List<ushort> startingSteps;
    public List<ushort> endingSteps;

    public bool boolSwitch;

    // public RecordedDataStructTweensDynamic Clone()
    // {
    //     var copy = new RecordedDataStructTweensDynamic(this.ID, this.type);
    //     copy.easeTypes = new List<ushort>(this.easeTypes);
    //     copy.positions = new List<Vector2>(this.positions);
    //     copy.values = new List<float>(this.values);
    //     copy.startingSteps = new List<ushort>(this.startingSteps);
    //     copy.endingSteps = new List<ushort>(this.endingSteps);
    //     return copy;
    // }

    public void RemoveAt(int index)
    {
        if (easeTypes.Count > index)
            easeTypes.RemoveAt(index);

        if (positions.Count > index)
            positions.RemoveAt(index);
        // other.RemoveAt(index);
        // delayValues.RemoveAt(index);
        if (values.Count > index)
            values.RemoveAt(index);

        startingSteps.RemoveAt(index);
        endingSteps.RemoveAt(index);
    }

    public void AddDuplicateItems(string t)
    {
        easeTypes.Add(easeTypes[easeTypes.Count - 1]);
        positions.Add(positions[positions.Count - 1]);
        // other.Add(other[other.Count - 1]);

        values.Add(values[values.Count - 1]);
        if (t == "Timers" && startingSteps.Count > 1)
        {
            int newStartStep = endingSteps[endingSteps.Count - 1] + (startingSteps[startingSteps.Count - 1] - endingSteps[endingSteps.Count - 2]);
            int newEndStep = newStartStep + (endingSteps[endingSteps.Count - 1] - startingSteps[startingSteps.Count - 1]);
            startingSteps.Add((ushort)newStartStep);
            endingSteps.Add((ushort)newEndStep);
        }
        else
        {
            startingSteps.Add(endingSteps[endingSteps.Count - 1]);
            endingSteps.Add((ushort)(endingSteps[endingSteps.Count - 1] + 20));
        }

    }

    public void AdjustBaseTimeStep(int timeStep)
    {
        int dif = timeStep - startingSteps[0];
        for (int i = 0; i < startingSteps.Count; i++)
        {

            int s = startingSteps[i] + dif;
            int e = endingSteps[i] + dif;
            startingSteps[i] = (ushort)s;
            endingSteps[i] = (ushort)e;

        }

    }

    public RecordedDataStructTweensDynamic(short id, ushort typ, ushort step)
    {
        this.ID = id;
        this.type = typ;
        this.easeTypes = new List<ushort>();
        easeTypes.Add(3);

        this.positions = new List<Vector2>();
        this.positions.Add(Vector2.zero);
        // this.delayValues = new List<float>();
        // delayValues.Add(0);
        this.values = new List<float>();
        values.Add(0);
        this.startingSteps = new List<ushort>();
        this.startingSteps.Add(step);
        this.endingSteps = new List<ushort>();
        this.endingSteps.Add(step);
        // this.other = new List<ushort>();
        // this.other.Add(0);




    }
}