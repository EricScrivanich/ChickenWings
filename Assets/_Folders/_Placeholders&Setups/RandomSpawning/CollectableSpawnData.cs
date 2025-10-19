using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CollectableSpawnIntensities", menuName = "Setups/CollectableIntensity", order = 2)]
public class CollectableSpawnData : ScriptableObject
{

    [Header("Eggs")]
    public bool SpawnEggs;
    public float flipXChanceEgg = .2f;
    public float BaseEggSpawnRate;
    public Vector2 AddedRandomEgg;
    public float EggThreeChance;
    public int TargetEggAmmo;
    public int MaxEggAmmo;
    public float RecheckEggIfMaxTime;
    public float BelowEggTargetTimeChange;
    public float AboveEggTargetTimeChange;
    public float InitialEggDelay = 4;
    public Vector2 InitialEggRandom;


    [Header("Shotgun")]
   
    public bool SpawnShotgun;
    public float flipXChanceShotgun = .2f;
    public float BaseShotgunSpawnRate;
    public Vector2 AddedRandomShotgun;
    public float ShotgunThreeChance;
    public int TargetShotgunAmmo;

    public int MaxShotgunAmmo;
    public float RecheckShotgunIfMaxTime;

    public float BelowShotgunTargetTimeChange;
    public float AboveShotgunTargetTimeChange;
    public float InitialShotgunDelay = 5;
    public Vector2 InitialShotgunRandom;







    [Header("Barns")]
    public bool SpawnBarn;
    public float BaseBarnSpawnRate;
    public Vector2 AddedRandomBarn;

    public float BigBarnChance;
    public float MidBarnChance;
    public float InitialBarnDelay = 7;
    public Vector2 InitialBarnRandom;



    // public float ShotgunThreeChance;

    public float ReturnInitialDelay(int type)
    {

        if (type == 0)
        {
            return (InitialEggDelay + Random.Range(InitialEggRandom.x, InitialEggRandom.y));

        }
        else if (type == 1)
        {
            return (InitialShotgunDelay + Random.Range(InitialShotgunRandom.x, InitialShotgunRandom.y));

        }
        else if (type == 2)
        {
            return (InitialBarnDelay + Random.Range(InitialBarnRandom.x, InitialBarnRandom.y));
        }
        else
            return 3;


    }

    public Vector2 ReturnEggTime(int eggAmount)
    {
        if (eggAmount >= MaxEggAmmo) return Vector2.zero;
        int diff = eggAmount - TargetEggAmmo;
        float addedTime = 0;


        if (diff > 0) addedTime = diff * AboveEggTargetTimeChange;
        else if (diff < 0) addedTime = diff * BelowEggTargetTimeChange;

        addedTime += Random.Range(AddedRandomEgg.x, AddedRandomEgg.y);
        float finalTime = BaseEggSpawnRate + addedTime;
        Debug.LogError("Calcualted egg time is: " + finalTime);
        int isThree = 0;

        if (RandomVal() >= EggThreeChance && diff < 0)
            isThree = 1;

        return new Vector2(finalTime, isThree);

    }

    public Vector2 ReturnShotgunTime(int shotgunAmount)
    {
        if (shotgunAmount >= MaxShotgunAmmo) return Vector2.zero;
        int diff = shotgunAmount - TargetShotgunAmmo;
        float addedTime = 0;


        if (diff > 0) addedTime = diff * AboveShotgunTargetTimeChange;
        else if (diff < 0) addedTime = diff * BelowShotgunTargetTimeChange;

        Debug.LogError("Added time to shotgun is: " + addedTime);

        addedTime += Random.Range(AddedRandomShotgun.x, AddedRandomShotgun.y);
        float finalTime = BaseShotgunSpawnRate + addedTime;
        int isThree = 0;

        if (RandomVal() >= ShotgunThreeChance && diff < 0)
            isThree = 1;

        return new Vector2(finalTime, isThree);
    }

    public Vector2 ReturnBarnTime()
    {
        float finalTime = BaseBarnSpawnRate + Random.Range(AddedRandomBarn.x, AddedRandomBarn.y);
        int barnSize = 2;

        float ran = RandomVal();

        if (ran >= BigBarnChance)
            barnSize = 0;
        else if (ran >= MidBarnChance)
            barnSize = 1;

        return new Vector2(finalTime, barnSize);

    }

    private float RandomVal()
    {
        return Random.Range(0f, 1f);
    }







}
