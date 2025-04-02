
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Player/ID")]

public class PlayerID : ScriptableObject
{

    public List<float> particleYPos { get; private set; }
    public PlayerStartingStatsForLevels playerStartingStats;
    public bool pressingEggButton;
    public bool pressingDashButton;
    public bool pressingDropButton;
    public bool pressingFlipRButton;
    public bool pressingFlipLButton;
    public bool pressingSwitchButton;
    public bool pressingHideButton;

    public bool scytheIsStuck;

    public bool pressingButton
    {
        get
        {
            if (!pressingFlipLButton && !pressingFlipRButton && !pressingDashButton && !pressingDropButton && !pressingEggButton && !pressingSwitchButton && !pressingHideButton)
                return false;
            else
                return true;
        }
    }


    public bool chamberIsRotating;

    public bool canPressEggButton;
    public bool canUseJoystick;

    public int amountOfWeapons { get; private set; }



    public List<int> PlayerInputs { get; private set; }
    public List<Vector3Int> KilledPigs { get; private set; }




    public bool constantPlayerForceBool;
    public float maxShotgunHoldTime { get; private set; } = 1.2f;

    public float constantPlayerForce;
    private bool resetingValues;
    public bool UsingClocker;

    [HideInInspector]
    public bool isHolding;
    [HideInInspector]
    public bool isAlive;
    public Material PlayerMaterial;
    public bool IsTwoTouchPoints;

    public bool CanParry;
    public bool CanPerectParry;
    [SerializeField] private int startingShotgunAmmo;
    [SerializeField] private int startingChainedShotgunAmmo;
    private int maxAddedChainShotgunAmmo = 5;
    private int addedChainShotgunAmmo;
    [SerializeField] private int startingAmmo;

    private int startingScore = 0;
    public int startingLives { get; private set; }
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
            if (value > startingLives)
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

    public bool usingChainedAmmo;
    // public bool ammosOnZero;
    public bool ammosButtonHidden;


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
            UiEvents.OnCollectAmmo?.Invoke(0);
            // if (ammosOnZero) CheckAmmosOnZero(0);

        }
    }
    public int BoomerangAmmo;

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
            UiEvents.OnCollectAmmo?.Invoke(1);
            // if (ammosOnZero) CheckAmmosOnZero(1);


        }
    }
    public void SetAmountOfWeapons(int n)
    {
        amountOfWeapons = n;
    }
    private int scytheAmmo;
    public int ScytheAmmo
    {
        get
        {
            return scytheAmmo;
        }

        set
        {
            scytheAmmo = value;
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

    public bool ReturnNextAvailableAmmo(int checkType)
    {
        switch (checkType)
        {
            case 0:
                if (Ammo > 0) return true;


                break;
            case 1:
                if (shotgunAmmo > 0) return true;


                break;
        }
        return false;
    }


    public bool trackParrySwipe;

    public PlayerEvents events;
    public UiEvents UiEvents;
    public GlobalPlayerEvents globalEvents;
    // public void CheckAmmosOnZero(int collectedIndex)
    // {
    //     if (!ammosOnZero && ShotgunAmmo <= 0 && Ammo <= 0)
    //     {
    //         ammosOnZero = true;
    //         if (collectedIndex == -1)
    //         {
    //             Debug.Log("ammo zero");
    //             // canPressEggButton = false;
    //             UiEvents.OnSwitchWeapon?.Invoke(0, -2);
    //         }

    //     }
    //     else if (ammosOnZero)
    //     {
    //         ammosOnZero = false;

    //         if (ammosButtonHidden)
    //         {
    //             UiEvents.OnResetSidebarAmmos?.Invoke(collectedIndex);
    //             UiEvents.OnSwitchWeapon?.Invoke(0, collectedIndex);


    //         }
    //     }

    // }
    public void SetStartingStats(PlayerStartingStatsForLevels stats)
    {
        if (stats == null)
        {
            Ammo = 0;
            shotgunAmmo = 0;
            startingLives = 3;

        }
        else
        {
            playerStartingStats = stats;
            Ammo = stats.startingAmmos[0];
            shotgunAmmo = stats.startingAmmos[1];
            startingLives = stats.StartingLives;
        }

        lives = startingLives;

    }
    public void ResetValues()
    {
        // ammosOnZero = false;
        ammosButtonHidden = false;
        trackParrySwipe = false;
        scytheIsStuck = false;

        resetingValues = true;
        canUseJoystick = false;
        isAlive = true;
        particleYPos = new List<float>();
        PlayerInputs = new List<int>();
        KilledPigs = new List<Vector3Int>();
        pressingEggButton = false;


        PlayerMaterial.SetFloat("_Alpha", 1);
        Score = startingScore;



        chainedShotgunAmmo = startingChainedShotgunAmmo;


        resetingValues = false;
        addedChainShotgunAmmo = 0;



        pressingEggButton = false;
        pressingDashButton = false;
        pressingDropButton = false;
        pressingFlipRButton = false;
        pressingFlipLButton = false;
        pressingSwitchButton = false;
        pressingHideButton = false;


    }

    public void AddPlayerInput(int inp)
    {

        PlayerInputs.Add(inp);
    }

    public void AddKillPig(int type, int bulletType, int bulletID)
    {
        KilledPigs.Add(new Vector3Int(type, bulletType, bulletID));
    }

    public void NewGasParticles(float y, bool add)
    {
        if (add)
        {
            particleYPos.Add(y);
        }
        else
        {
            if (particleYPos != null && particleYPos.Count > 0)
            {
                for (int i = 0; i < particleYPos.Count; i++)
                {
                    if (y == particleYPos[i]) particleYPos.Remove(i);
                }
            }
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        globalEvents.OnAddScore?.Invoke(amount);


    }




}
