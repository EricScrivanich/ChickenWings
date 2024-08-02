using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CollectableData
{
    public float TimeToTrigger;
    public bool isRings;
    public abstract void InitializeCollectable(CollectablePoolManager manger,bool finalTrigger);
    // public abstract void InitializeCollectableRandom(CollectablePoolManager manger,bool finalTrigger, Vector2 minMaxSpeed,Vector2 minMaxY,float xOffset);
}
