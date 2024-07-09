using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSerializable
{

}

[System.Serializable]
public class RingData
{
    public Vector2 position;
    public Vector3 scale;
    public Quaternion rotation;
    public float speed;



}
[System.Serializable]
public class TriggerObjectData
{
    public Vector2 position;
    public float speed;
}
