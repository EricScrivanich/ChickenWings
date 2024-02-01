using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] private ResetManager reset;
    [SerializeField] private SpriteRenderer sprite;
    private bool is60;
    private static int frameRate = 60;
    void Start()
    {
        if (frameRate != 60 && frameRate != 120)
        {
            frameRate = 60;
        }

        
        if (sprite != null)
        {
            if (frameRate == 60)
            {
                sprite.color = Color.white;
            }
            else
            {
                sprite.color = Color.red;

            }

        }

        
        
        

        Application.targetFrameRate = frameRate; // Set to your desired frame rate
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