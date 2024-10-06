
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
    [SerializeField] private int startingShotgunAmmo;
    [SerializeField] private int startingChainedShotgunAmmo;
    private int maxAddedChainShotgunAmmo = 5;
    private int addedChainShotgunAmmo;
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

    private int normalAmmo;
    private int shotgunAmmo;
    private int chainedShotgunAmmo;

    public int Ammo
    {
        get
        {
            return normalAmmo;
        }
        set
        {

            normalAmmo = value;
            globalEvents.OnUpdateAmmo?.Invoke(normalAmmo);

        }
    }

    public int ShotgunAmmo
    {
        get
        {
            return shotgunAmmo;
        }
        set
        {



            shotgunAmmo = value;
            globalEvents.OnUpdateShotgunAmmo?.Invoke(shotgunAmmo);

        }
    }

    public int ChainedShotgunAmmo
    {
        get
        {
            return chainedShotgunAmmo;
        }
        set
        {

            if (value > chainedShotgunAmmo)
            {
                addedChainShotgunAmmo++;

                if (addedChainShotgunAmmo > maxAddedChainShotgunAmmo)
                    return;

                chainedShotgunAmmo = value;
            }

            else if (value == -1)
            {
                addedChainShotgunAmmo = 0;
                chainedShotgunAmmo = startingChainedShotgunAmmo;
                return;

            }

            chainedShotgunAmmo = value;
            globalEvents.OnUpdateChainedShotgunAmmo?.Invoke(chainedShotgunAmmo);

        }
    }

    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;


    public void ResetValues(PlayerStartingStatsForLevels stats)
    {

        resetingValues = true;
        isAlive = true;


        PlayerMaterial.SetFloat("_Alpha", 1);
        Score = startingScore;

        if (stats == null)
        {
            Ammo = startingAmmo;
            shotgunAmmo = startingShotgunAmmo;
        }
        else
        {
            Ammo = stats.startingNormalEggAmmo;
            shotgunAmmo = stats.startingShotgunAmmo;
        }

        chainedShotgunAmmo = startingChainedShotgunAmmo;
        lives = startingLives;

        resetingValues = false;
        addedChainShotgunAmmo = 0;


    }

    public void AddScore(int amount)
    {
        globalEvents.OnAddScore?.Invoke(amount);
        Score += amount;

    }




}
