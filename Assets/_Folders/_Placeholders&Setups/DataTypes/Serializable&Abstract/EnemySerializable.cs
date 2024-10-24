
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

[System.Serializable]

// class name: SiloMovement
public struct SiloData
{
    public Vector2 position;
    public int type;

    public float baseHeightMultiplier;

}

[System.Serializable]
// class name: MissilePigScript
public struct MissilePigData
{
    public Vector2 position;
    public int missileType;

    public int movementType;


}


[System.Serializable]
// class name: Windmill

public struct WindMillData
{
    public Vector2 position;
    public int bladeAmount;

    public float bladeScaleMultiplier;
    public float bladeSpeed;

    public int startRot;


}

[System.Serializable]
public struct GasPigData
{
    public Vector2 position;

    public float speed;

    public float delay;

    public float initialDelay;
}

[System.Serializable]

// class name: HotAirBalloon
public struct HotAirBalloonData
{
    public Vector2 position;

    public int type;

    public float xTrigger;

    public float yTarget;

    public float speed;

    public float delay;
}


[System.Serializable]
// class name: FlappyPigMovement
public struct FlappyPigData
{
    public Vector2 position;

    public float scaleFactor;

}

[System.Serializable]
// class name: DropBomb
public struct BomberPlaneData
{
    public float xDropPosition;
    public float dropAreaScaleMultiplier;

    public float speedTarget;


}


