using UnityEngine;

[System.Serializable]
public readonly struct RecordedDataStruct
{
    public readonly int ID;
    public readonly Vector2 startPos;
    public readonly float scale;
    public readonly float speed;
    public readonly float magPercent;
    public readonly float timeInterval;
    public readonly float delayInterval;
    public readonly byte type;


    public RecordedDataStruct(int id, Vector2 pos, float sc, float sp, float m, float t, float d, byte b)
    {
        ID = id;
        startPos = pos;
        scale = sc;
        speed = sp;
        magPercent = m;
        timeInterval = t;
        delayInterval = d;
        type = b;

    }
}