using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePlaceholder : MonoBehaviour
{
    public PlaneData ID;
    [SerializeField]
    private PlaneManager.PlaneType planeType;
    public float speed;

    public PlaneManager.PlaneType GetPlaneType()
    {
        return planeType;
    }

    public void SpawnPlane()
    {
        ID.GetPlane(speed, transform.position);
    }

}