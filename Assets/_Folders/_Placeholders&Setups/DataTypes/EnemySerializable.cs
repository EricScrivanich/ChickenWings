
using UnityEngine;

public class EnemySerializable
{
   
}


[System.Serializable]
public class JetPackPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
}

[System.Serializable]
public class TenderizerPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
    public bool hasHammer;
}