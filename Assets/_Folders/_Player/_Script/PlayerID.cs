
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{

    public float slowMaxFallSpeed;
    public float slowJumpForce;
    public float slowFlipLeftForceX;
    public float slowFlipLeftForceY;
    public float slowFlipRightForceX;
    public float slowFlipRightForceY;
    
    public bool IsSlowedDown;
    public Material PlayerMaterial;
    public float rotationFactor;
    public float rotationSpeed;
    public float parachuteRotationSpeed;
    public float decelerationFactor;
    public bool IsTwoTouchPoints;
    public float StaminaUsed;
    public float MaxStamina = 100;
    [SerializeField] private int startingAmmo;
    [SerializeField] private int startingScore;
    [SerializeField] private int startingLives;
    private int lives;
    public int Lives
    {
        get 
        {
            return lives;
        }
        set 
        {
            if (value > 3)
            {
                return;
            }
            Debug.Log("Doing it");
            lives = value;
            Debug.Log("LIves " + lives);
            globalEvents.OnUpdateLives?.Invoke(lives);
        }
    }
    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            globalEvents.OnUpdateScore?.Invoke(score);
        }
    }

    public int Ammo;
    public float MaxFallSpeed;
    public float AddEggVelocity;
    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;
    public float parachuteXOffset;
    public float parachuteYOffset;

    public void ResetValues()
    {
        PlayerMaterial.SetFloat("_Alpha", 1);
        Score = startingScore;
        Ammo = startingAmmo;
        Lives = startingLives;
        globalEvents.OnUpdateAmmo?.Invoke();
    }

    public void SlowedGame()
    {


    }


}
