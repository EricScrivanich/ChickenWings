using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60; // Set to your desired frame rate
    }
}