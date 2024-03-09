using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] private ResetManager reset;
    [SerializeField] private SpriteRenderer sprite;
    private bool is60;
    private static int frameRate = 60;
    void Awake()
    {


        


        
        
        

        Application.targetFrameRate = 60; // Set to your desired frame rate
    }

    public void SwitchFrameRate()
    {
        if (Application.targetFrameRate == 60)
        {
            frameRate = 120;
            is60 = false;
        }
        else {
        frameRate = 60;

        }
        
        reset.SpecialReset();

    }
}