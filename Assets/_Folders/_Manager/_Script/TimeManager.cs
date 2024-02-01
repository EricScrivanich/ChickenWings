using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private bool DebogLogEnabled;
    public float slowdownFactor = 0.5f;

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         Time.timeScale = slowdownFactor;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Z))
    //     {
    //         Time.timeScale = 1f;
    //     }
    // }

    private void Awake() {
        Debug.unityLogger.logEnabled = DebogLogEnabled;
    }
}