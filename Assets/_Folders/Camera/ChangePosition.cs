using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePosition : MonoBehaviour
{
    public CameraID cam;

    [SerializeField] private float xThreshold;
    [SerializeField] private float yThreshold;
    [SerializeField] private float addX;
    [SerializeField] private float addY;
    [SerializeField] private bool setBehind;
    [SerializeField] private bool setBelow;
    [SerializeField] private float timeX;
    [SerializeField] private float timeY;

    // [SerializeField] private float time;
    // [SerializeField] private List<GameObject> posObj;
    // private List<Vector2> positionsList;

    // void Start()
    // {
    //     positionsList = new List<Vector2>();



    // }
    public void TriggerAction()
    {
        // for (int i = 0; i < posObj.Count; i++)
        // {
        //     positionsList.Add(posObj[i].transform.position);
        // }

        // cam.events.OnChangePosition?.Invoke(posObj, time);

        if (addX != 0)
        {
            cam.events.OnChangeXPosition?.Invoke(addX, timeX);
        }

        else if (xThreshold > 0)
        {
            cam.events.AdjustCameraX?.Invoke(xThreshold, setBehind, timeX);
        }

        if (addY != 0)
        {
            cam.events.OnChangeYPosition?.Invoke(addY, timeY);
        }
        else if (yThreshold > 0)
        {
            cam.events.AdjustCameraY?.Invoke(xThreshold, setBehind, timeY);
        }

    }

}
