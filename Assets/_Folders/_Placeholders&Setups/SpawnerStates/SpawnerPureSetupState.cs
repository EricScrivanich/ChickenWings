using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPureSetupState : SpawnBaseState
{
    private int currentTrigger = 0;
    private bool canContinueToNextSetup;
    private bool hasFinishedFullSetup;

    private bool failedRings;
    private bool setupDone = false;

    private bool isSpecialGoldRing;

    private bool completedRings;
    private int ringTypeSpawned;

    private bool ignoreTriggerReset = false;

    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        hasFinishedFullSetup = false;
        canContinueToNextSetup = false;
        setupDone = false;
        failedRings = false;
        isSpecialGoldRing = spawner.transitionLogic.SpecialGoldRings;

        if (!ignoreTriggerReset)
        {
            currentTrigger = 0;
        }
        else
            ignoreTriggerReset = false;



        // spawner.currentRingType = spawner.currentRandomSpawnIntensityData.GetRingTypeIndex();

        // if (spawner.pureSetups[spawner.CurrentPureSetup] != null)
        if (spawner.pureSetups.Length > spawner.CurrentPureSetup)
            spawner.pureSetups[spawner.CurrentPureSetup].SpawnTrigger(spawner, currentTrigger, true);

        else
            return;
        currentTrigger++;


    }

    public void SetRingType(int type)
    {
        // Debug.Log("Set new ring type in set Ring: " + type);

        ringTypeSpawned = type;
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
        setupDone = true;
        if (failedRings && spawner.pureSetups[spawner.CurrentPureSetup].mustCompleteRingSequence && !isSpecialGoldRing)
        {
            spawner.SwitchStateWithLogic();
        }
        else if (currentTrigger >= spawner.pureSetups[spawner.CurrentPureSetup].collectableTriggerCount
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
            if (isSpecialGoldRing)
            {

                spawner.pureSetups[spawner.CurrentPureSetup].SpawnTrigger(spawner, currentTrigger, !failedRings);

            }

            else
                spawner.pureSetups[spawner.CurrentPureSetup].SpawnTrigger(spawner, currentTrigger, true);
            currentTrigger++;
        }


    }

    public void SetNewCurrentTrigger(int val)
    {
        currentTrigger = val;
        ignoreTriggerReset = true;
    }

    public override void RingsFinished(SpawnStateManager spawner, int type, bool isCorrect)
    {
        if (spawner.pureSetups[spawner.CurrentPureSetup].mustCompleteRingSequence && type == ringTypeSpawned)
        {
            if (isCorrect && !isSpecialGoldRing)
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
                // spawner.SwitchStateWithLogic();
                failedRings = true;

                if (setupDone && !isSpecialGoldRing)
                {
                    spawner.SwitchStateWithLogic();
                }

            }
        }

        else if (type == ringTypeSpawned && isSpecialGoldRing && !isCorrect)
            failedRings = true;


    }
}