using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRandomSetupEnemyAndRingState : SpawnBaseState
{
    private int amount;
    private int currentTriggerAmount;
    private int ringType;
    private RandomSpawnIntensity currentIntensityData;
    private bool hasSwitched;
    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        hasSwitched = false;
        currentTriggerAmount = 1;
        spawner.currentRingType = spawner.currentRandomSpawnIntensityData.GetRandomRingTypeIndex();

        amount = spawner.currentRandomSpawnIntensityData.GetRandomRingSetupAmountIndex();

        spawner.randomRingSetups[0].SpawnRandomSetWithRings(spawner, currentTriggerAmount >= amount);
        currentTriggerAmount++;

    }

    public override void ExitState(SpawnStateManager spawner)
    {

    }

    public override void SetSpeedAndPos(SpawnStateManager spawner)
    {

    }

    // public override void UpdateState(SpawnStateManager spawner)
    // {

    // }

    public override void SetNewIntensity(SpawnStateManager spawner, RandomSpawnIntensity spawnIntensity)
    {



    }
    public override void SetupHitTarget(SpawnStateManager spawner)
    {
        if (currentTriggerAmount > amount)
        {
            return;
        }

        spawner.randomRingSetups[0].SpawnRandomSetWithRings(spawner, currentTriggerAmount >= amount);
        currentTriggerAmount++;

    }

    public override void RingsFinished(SpawnStateManager spawner, bool isCorrect)
    {
        if (isCorrect)
        {

        }
        else
        {


        }
        spawner.SwitchStateWithLogic();
        currentTriggerAmount = 0;
    }
}