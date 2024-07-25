using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundariesManager : MonoBehaviour
{
    public static int rightBoundary = 14;
    public static bool isDay;
    public static int leftBoundary = -14;
    public static float rightViewBoundary = 11.4f;
    public static float leftViewBoundary = -11.4f;
    public static float rightPlayerBoundary = 10.8f;
    public static float TopPlayerBoundary = 6.9f;
    public static float leftPlayerBoundary = -10.8f;

    public static float GroundSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        Debug.unityLogger.logEnabled = true;
        // Time.timeScale = .1f;

    }

    // Update is called once per frame

}
