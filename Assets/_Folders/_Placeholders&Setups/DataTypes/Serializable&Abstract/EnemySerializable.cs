
using UnityEngine;


[System.Serializable]
public struct JetPackPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
}

[System.Serializable]
public struct TenderizerPigData 
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
    public bool hasHammer;
}

[System.Serializable]
public struct NormalPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
    
}

[System.Serializable]
public struct BigPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;
    public float yForce;
    public float distanceToFlap;

    public float startingFallSpot;

}

[System.Serializable]
public struct PilotPigData
{
    public Vector2 position;
    public Vector3 scale;
    public float speed;

    public int flightMode;
    public float minY;
    public float maxY;
    public float yForce;
    public float maxYSpeed;
    public float xTrigger;
   

}