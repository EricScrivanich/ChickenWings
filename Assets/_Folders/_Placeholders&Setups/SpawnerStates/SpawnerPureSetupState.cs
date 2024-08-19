using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPureSetupState : SpawnBaseState
{
    private int currentTrigger = 0;
    private bool canContinueToNextSetup;
    private bool hasFinishedFullSetup;
    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        hasFinishedFullSetup = false;
        canContinueToNextSetup = false;
        currentTrigger = 0;
        spawner.currentRingType = spawner.currentRandomSpawnIntensityData.GetRingTypeIndex();
        spawner.pureSetups[spawner.CurrentPureSetup].SpawnTrigger(spawner, currentTrigger);
        currentTrigger++;


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
        if (currentTrigger >= spawner.pureSetups[spawner.CurrentPureSetup].collectableTriggerCount
&& currentTrigger >= spawner.pureSetups[spawner.CurrentPureSetup].enemyTriggerCount && !hasFinishedFullSetup)
        {
            hasFinishedFullSetup = true;

            if (!spawner.pureSetups[spawner.CurrentPureSetup].mustCompleteRingSequence || canContinueToNextSetup)
            {
                spawner.ChangePureSetupIndex(spawner.CurrentPureSetup + 1);
                spawner.SwitchStateWithLogic();
            }

        }
        else
        {
            spawner.pureSetups[spawner.CurrentPureSetup].SpawnTrigger(spawner, currentTrigger);
            currentTrigger++;
        }


    }

    public override void RingsFinished(SpawnStateManager spawner, bool isCorrect)
    {
        if (spawner.pureSetups[spawner.CurrentPureSetup].mustCompleteRingSequence)
        {
            if (isCorrect)
            {

                if (spawner.pureSetups[spawner.CurrentPureSetup].mustCompleteRingSequence)
                {
                    if (hasFinishedFullSetup)
                    {
                        spawner.ChangePureSetupIndex(spawner.CurrentPureSetup + 1);
                        spawner.SwitchStateWithLogic();

                    }

                    else
                        canContinueToNextSetup = true;
                }

            }
            else
            {
                spawner.SwitchStateWithLogic();

            }




        }

    }
}