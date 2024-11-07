using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundariesManager : MonoBehaviour
{
    public static int rightBoundary = 14;

    public static Vector3 vectorThree1 = new Vector3(1, 1, 1);

    public static bool isDay = true;
    public static int leftBoundary = -16;
    public static float rightViewBoundary = 11.9f;
    public static float leftViewBoundary = -11.9f;
    public static float rightPlayerBoundary = 11.8f;
    public static float TopPlayerBoundary = 6.8f;
    public static float TopViewBoundary = 6.8f;
    public static float leftPlayerBoundary = -11.8f;

    public static float GroundSpeed = 4.7f;

    public static float GroundPosition = -4.9f;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.unityLogger.logEnabled = false;


#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#endif
        // Time.timeScale = .1f;

    }

    // Update is called once per frame

}
