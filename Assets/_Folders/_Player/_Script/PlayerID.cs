
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{
    
    public bool testingNewGravity;
    public float playerJumpForce;
    public float playerAddDownForce;
    public float startAddDownForce;
    public float endAddDownForce;
    public float flipGravity;
    public float stopFlipGravity;
    public float addJumpForce;
    public float startLerp;
    public float lerpTarget;
    public float lerpTime;
    public float flipSpeed;
    public float slowMaxFallSpeed;
    public float slowJumpForce;
    private bool resetingValues;
    public float slowFlipLeftForceX;
    public float CurrentManaTarget;
    public float slowFlipLeftForceY;
    public float slowFlipRightForceX;
    public float slowFlipRightForceY;
    public int MaxMana = 360;
    public int numberOfPowersThatCanBeUsed;
    private float currentMana;
    public bool UsingClocker;
    private bool hasHitMaxMana;

    public bool isHolding;
    public bool isAlive;
    public float addScoreTime;
    public float CurrentMana

   
    {
        get
        {
            return currentMana;
        }
        set
        {
            if (value > currentMana && !resetingValues)
            {
                currentMana = value;
                if (currentMana >= MaxMana)
                {
                    currentMana = MaxMana;
                    if (hasHitMaxMana)
                    {
                        return;
                    }
                    hasHitMaxMana = true;
                }
                globalEvents.AddMana?.Invoke();
                if (currentMana >= ManaNeeded * (numberOfPowersThatCanBeUsed + 1))
                {
                    
                    globalEvents.AddPowerUse?.Invoke((ManaNeeded * (numberOfPowersThatCanBeUsed + 1)) / MaxMana);
                    numberOfPowersThatCanBeUsed += 1;
                }
            }
            else
            {
                currentMana = value;
                if (currentMana < MaxMana)
                {
                    hasHitMaxMana = false;
                }
            }
        }
    }
    public float ManaNeeded;
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

            lives = value;



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
        resetingValues = true;
        isAlive = true;

        numberOfPowersThatCanBeUsed = 0;
        CurrentMana = 0;
        PlayerMaterial.SetFloat("_Alpha", 1);
        Score = startingScore;
        Ammo = startingAmmo;
        Lives = startingLives;
        globalEvents.OnUpdateAmmo?.Invoke();
        resetingValues = false;
        numberOfPowersThatCanBeUsed = Mathf.FloorToInt(CurrentMana / ManaNeeded);

        if (CurrentMana < MaxMana)
        {
            hasHitMaxMana = false;
        }
    }

    public void AddScore(int amount)
    {
        globalEvents.OnAddScore?.Invoke(amount);
        Score += amount;
        


    }

    public void SlowedGame()
    {


    }


}
