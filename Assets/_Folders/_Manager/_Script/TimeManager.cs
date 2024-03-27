using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
   [SerializeField] private bool debogLogEnabled;
    public static bool DebogLogEnabled;
    public float slowdownFactor;
    public float deltaScale;

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

    private void Awake()
    {
        DebogLogEnabled = debogLogEnabled;
        Debug.unityLogger.logEnabled = DebogLogEnabled;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = .02f * deltaScale;
        // Time.timeScale = slowdownFactor;
    }
}