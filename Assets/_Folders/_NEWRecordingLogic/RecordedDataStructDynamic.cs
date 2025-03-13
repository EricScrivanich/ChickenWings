using UnityEngine;

[System.Serializable]
public struct RecordedDataStructDynamic
{
    public short ID;
    public ushort spawnedStep;
    public ushort unspawnedStep;
    public Vector2 startPos;
    public float speed;
    public float scale;

    public float magPercent;
    public float timeInterval;
    public float delayInterval;
    public short type;



    public RecordedDataStructDynamic(short id, Vector2 pos, float sp, float sc, float m, float t, float d, short b, ushort ss, ushort us)
    {
        this.ID = id;
        this.startPos = pos;
        this.speed = sp;
        this.scale = sc;
        this.magPercent = m;
        this.timeInterval = t;
        this.delayInterval = d;
        this.type = b;
        this.spawnedStep = ss;
        this.unspawnedStep = us;

    }
}