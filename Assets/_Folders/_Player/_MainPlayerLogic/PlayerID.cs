
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{


    [SerializeField] private LevelManagerID lvlID;
    public bool constantPlayerForceBool;


    public float constantPlayerForce;
    private bool resetingValues;
    public bool UsingClocker;

    [HideInInspector]
    public bool isHolding;
    [HideInInspector]
    public bool isAlive;
    public Material PlayerMaterial;
    public bool IsTwoTouchPoints;
    [SerializeField] private int startingAmmo;
    private int startingScore = 0;
    [SerializeField] private int startingLives;
    private int lives;
    [HideInInspector]
    public bool infiniteLives;
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

    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;


    public void ResetValues()
    {
        
        resetingValues = true;
        isAlive = true;


        PlayerMaterial.SetFloat("_Alpha", 1);
        Score = startingScore;
        Ammo = startingAmmo;
        Lives = startingLives;

        resetingValues = false;

    }

    public void AddScore(int amount)
    {
        globalEvents.OnAddScore?.Invoke(amount);
        Score += amount;

    }




}
