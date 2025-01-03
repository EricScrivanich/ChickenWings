using UnityEngine;
using System;

public class FrameRateManager : MonoBehaviour
{
    // [SerializeField] private ResetManager reset;
    // [SerializeField] private SpriteRenderer sprite;
    private bool is60;
    private static int frameRate = 60;
    [SerializeField] private int targetFrameRate;
    public static readonly float BaseTimeScale = .86f;
    public static float TargetTimeScale = .86f;

    public static Action<bool> OnChangeGameTimeScale;


    public static bool under1;
    public static bool under085;
    void Awake()
    {
        Time.timeScale = TargetTimeScale;
        Application.targetFrameRate = targetFrameRate; 

    }

    // public void SwitchFrameRate()
    // {
    //     if (Application.targetFrameRate == 60)
    //     {
    //         frameRate = 120;
    //         is60 = false;
    //     }
    //     else {
    //     frameRate = 60;

    //     }

    //     reset.SpecialReset();

    // }
}