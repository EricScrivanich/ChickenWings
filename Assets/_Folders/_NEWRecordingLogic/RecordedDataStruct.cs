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