using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRandomSetupEnemyAndRingState : SpawnBaseState
{
    private int amount;
    private int currentTriggerAmount;
    private int ringType;
    public int lastRingTypeIndex = 0;
    private bool completedRings;
    private bool canContinue;
    private int ringTypeSpawned;

    private bool hasSwitched;
    private bool breakSafe;
    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        Debug.Log("Enterted ring state");
        breakSafe = false;
        hasSwitched = false;
        completedRings = false;
        canContinue = false;
        // currentTriggerAmount = 1;
        currentTriggerAmount = 0;
        currentTriggerAmount++;


        // spawner.NextRingType();


        amount = spawner.currentRandomSpawnIntensityData.GetRandomRingSetupAmountIndex();


        spawner.randomRingSetups[spawner.currentRandomSpawnIntensityData.GetRingDifficultyIndex()].SpawnRandomSetWithRings(spawner, currentTriggerAmount >= amount);


    }

    public override void ExitState(SpawnStateManager spawner)
    {
        completedRings = false;
        currentTriggerAmount = 0;

    }

    public override void SetSpeedAndPos(SpawnStateManager spawner)
    {

    }

    public void SetRingType(int type)
    {
        Debug.Log("Set new ring type in Ran Ring: " + type);
        ringTypeSpawned = type;

    }

    // public override void UpdateState(SpawnStateManager spawner)
    // {

    // }

    public override void SetNewIntensity(SpawnStateManager spawner, RandomSpawnIntensity spawnIntensity)
    {



    }
    public override void SetupHitTarget(SpawnStateManager spawner)
    {
        if (breakSafe)
        {
            spawner.SwitchStateWithLogic();
            return;
        }


        if (currentTriggerAmount > amount)
        {
            canContinue = true;

            if (completedRings)
            {
                spawner.SwitchStateWithLogic();
            }

            return;


        }
        else
        {
            spawner.randomRingSetups[spawner.currentRandomSpawnIntensityData.GetRingDifficultyIndex()].SpawnRandomSetWithRings(spawner, currentTriggerAmount >= amount);
            currentTriggerAmount++;
        }



    }

    public override void RingsFinished(SpawnStateManager spawner, int type, bool isCorrect)
    {
        // if (isCorrect)
        // {

        // }
        // else
        // {


        // }

        if (type == ringTypeSpawned)
        {
            if (isCorrect)
            {
                completedRings = true;

                if (canContinue)
                {
                    completedRings = false;
                    spawner.SwitchStateWithLogic();


                }
            }
            else
            {
                breakSafe = true;
                if (canContinue)
                {
                    spawner.SwitchStateWithLogic();
                }
            }

        }




    }
}