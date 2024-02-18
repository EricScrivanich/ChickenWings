
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
    public int Lives;
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
        Score = startingScore;
        Ammo = startingAmmo;
        globalEvents.OnUpdateAmmo?.Invoke();
    }

    public void SlowedGame()
    {


    }


}
