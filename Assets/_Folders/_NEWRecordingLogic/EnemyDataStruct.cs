using UnityEngine;

public readonly struct EnemyDataStruct
{
    public readonly int enemyID;
    public readonly Vector2 startPos;
    public readonly float scale;
    public readonly float speed;
    public readonly float magPercent;
    public readonly float timeInterval;
    public readonly float delayInterval;
    public readonly byte type;


    public EnemyDataStruct(int id, Vector2 pos, float sc, float sp, float m, float t, float d, byte b)
    {
        enemyID = id;
        startPos = pos;
        scale = sc;
        speed = sp;
        magPercent = m;
        timeInterval = t;
        delayInterval = d;
        type = b;

    }
}