using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CameraID : ScriptableObject
{
    public List<GameObject> list;
    private Vector2 yer;
    public CameraEvents events;
}
