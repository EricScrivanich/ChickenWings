using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder : MonoBehaviour
{
    [SerializeField]
    private PlaneManager.PlaneType planeType;

    public PlaneManager.PlaneType GetPlaneType()
    {
        return planeType;
    }

}