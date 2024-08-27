using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRandomSetupEnemyState : SpawnBaseState
{
    private int amount;
    private int currentAmountSpawned;

    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        currentAmountSpawned = 0;
        amount = spawner.currentRandomSpawnIntensityData.GetRandomEnemySetupAmountIndex();
        Debug.LogError("Amount of random setups to spawn is: " + amount);

        if (currentAmountSpawned < amount && amount != 0)
        {
            spawner.randomEnemySetups[spawner.currentRandomSpawnIntensityData.GetRandomEnemySetupDifficultyIndex()].SpawnRandomEnemies(spawner);
            currentAmountSpawned++;

        }
        else
        {

        }


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
        if (currentAmountSpawned < amount && amount != 0)
        {
            spawner.randomEnemySetups[spawner.currentRandomSpawnIntensityData.GetRandomEnemySetupDifficultyIndex()].SpawnRandomEnemies(spawner);
            currentAmountSpawned++;

        }
        else
        {
            spawner.SwitchStateWithLogic();

        }

    }
    public override void RingsFinished(SpawnStateManager spawner, int n, bool isCorrect)
    {

    }
}