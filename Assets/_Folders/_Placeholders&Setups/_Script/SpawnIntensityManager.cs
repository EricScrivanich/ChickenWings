using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIntensityManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private PlayerID player;

    private bool waitingOnHealth = false;

    private bool returningToPrevInt = false;


    [SerializeField] private RandomSpawnIntensity[] spawnIntensitiesNormal;
    [SerializeField] private RandomSpawnIntensity[] spawnIntensitiesHealth;
    [SerializeField] private int[] delaysToSpawnNextHealthAfterSpawn;




    private SpawnStateTransitionLogic lastTransition;


    private int currentHealthIntensityIndex = 0;
    private bool cycledAllIntensities = false;

    private int currentIntensityIndex = 0;
    private int nextIntensityIndex = 0;
    private int currentIntensityLevel = 0;

    [SerializeField] private int[] nextIntentisityObjectiveValues;
    private int currentObjectivesObtained = 0;

    [SerializeField] string[] objectivesTracked;

    [SerializeField] private float initialDelayForHealthRingsToSpawn;
    [SerializeField] private int maxNumberOfHealthSpawns;

    private bool canSpawnHealthRings = false;

    private int currentHealthDelayIndex;

    private bool ignoreNextIntensityTriggerFromHealth;
    private bool ignoreNextIntensityTrigger;
    private int returnToFinishNonLoopedTranstionIndex = -1;
    private int currentLives;
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (var item in spawnIntensitiesNormal)
        {
            item.ResetIntensities();
        }
        foreach (var item in spawnIntensitiesHealth)
        {
            item.ResetIntensities();
        }
    }
    private void Start()
    {
        currentHealthDelayIndex = 0;

        UpdateObjective("all", 0);

        if (initialDelayForHealthRingsToSpawn > 0)
        {
            StartCoroutine(WaitForNextHealthChance(initialDelayForHealthRingsToSpawn));

        }
        else
            canSpawnHealthRings = true;

    }

    private int CheckCurrentIntensity()
    {
        int n = 0;
        if (returnToFinishNonLoopedTranstionIndex > -1)
        {
            returningToPrevInt = true;
            return returnToFinishNonLoopedTranstionIndex;
        }
        for (int i = currentIntensityIndex; i < nextIntentisityObjectiveValues.Length; i++)
        {
            if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i] && i > n)
            {
                n = i;
            }
            else
            {
                break; // Stop checking if the condition is not met
            }
        }

        return n;
    }

    void UpdateObjective(string type, int addedValue)
    {
        bool revertingOverride = false;

        bool shouldUpdate = false;


        if (type == "all")
        {
            currentObjectivesObtained += addedValue;
            shouldUpdate = true;

        }
        else if (type == "revert")
        {
            shouldUpdate = true;
            revertingOverride = true;
            Debug.Log("WE ARE REVERTING BUH");
        }
        else
        {
            foreach (string s in objectivesTracked)
            {
                if (s == type)
                {
                    // Debug.LogError("Tracked Pig Kill");
                    currentObjectivesObtained += addedValue;
                    shouldUpdate = true;
                    break;
                }
            }
        }

        if (!shouldUpdate || (nextIntensityIndex >= nextIntentisityObjectiveValues.Length && !revertingOverride) || (ignoreNextIntensityTrigger && !revertingOverride))
        {
            Debug.Log("Ignoring intesntity index is: " + ignoreNextIntensityTrigger);
            return;
        }


        if (revertingOverride)
        {





            int newIntensityIndex = 0;

            for (int i = 0; i < nextIntentisityObjectiveValues.Length; i++)
            {
                if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i])
                {
                    if (!spawnIntensitiesNormal[i].hasCompleted)
                    {
                        newIntensityIndex = i; // Update to the most recent true statement
                        if (spawnIntensitiesNormal[i].mustComplete && !spawnIntensitiesNormal[i].hasCompleted)
                        {
                            Debug.LogError("WE GOTTA FINSIH another first bud");
                            break;
                        }




                        Debug.Log("SetNewIntensity From Update Objective: " + i);
                    }


                }
                else
                {
                    Debug.Log("But why tho bruh");
                    break; // Stop checking if the condition is not met
                }
            }



            // Update the currentIntensityIndex to the most recent valid intensity
            currentIntensityIndex = newIntensityIndex;
            nextIntensityIndex = currentIntensityIndex + 1;


            if (currentIntensityIndex < spawnIntensitiesNormal.Length)
            {
                lvlID.SetNewIntensity(currentIntensityIndex);
                var intensity = spawnIntensitiesNormal[currentIntensityIndex];
                ignoreNextIntensityTrigger = intensity.IgnoreItensityTriggers;
                lvlID.outputEvent.OnSetNewIntensity?.Invoke(intensity, false);
            }
            else
            {
                Debug.LogWarning("THIS should not happen");
            }

        }






        else if (currentObjectivesObtained >= nextIntentisityObjectiveValues[nextIntensityIndex])
        {
            Debug.LogWarning("HERE WE Go, time to check");


            int newIntensityIndex = nextIntensityIndex;


            // Start checking from currentIntensityIndex + 1
            for (int i = currentIntensityIndex; i < nextIntentisityObjectiveValues.Length; i++)
            {
                if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i] && !spawnIntensitiesNormal[i].hasCompleted)
                {

                    newIntensityIndex = i; // Update to the most recent true statement
                    if (spawnIntensitiesNormal[i].mustComplete && !spawnIntensitiesNormal[i].hasCompleted)
                    {
                        Debug.LogError("WE GOTTA FINSIH another first bud");
                        break;
                    }




                    Debug.Log("SetNewIntensity From Update Objective");
                }
                else
                {
                    break; // Stop checking if the condition is not met
                }
            }



            // Update the currentIntensityIndex to the most recent valid intensity
            currentIntensityIndex = newIntensityIndex;
            nextIntensityIndex = currentIntensityIndex + 1;


            if (currentIntensityIndex < spawnIntensitiesNormal.Length)
            {
                lvlID.SetNewIntensity(currentIntensityIndex);
                var intensity = spawnIntensitiesNormal[currentIntensityIndex];
                ignoreNextIntensityTrigger = intensity.IgnoreItensityTriggers;
                lvlID.outputEvent.OnSetNewIntensity?.Invoke(intensity, false);
            }






        }

        // else if (spawnIntensitiesNormal[currentIntensityIndex].hasCompleted)
        // {
        //     for (int i = currentIntensityIndex - 1; i >= 0; i--)
        //     {
        //         if (!spawnIntensitiesNormal[i].hasCompleted)
        //         {
        //             currentIntensityIndex = i;
        //             break;

        //         }
        //     }

        //     nextIntensityIndex = currentIntensityIndex + 1;


        //     if (currentIntensityIndex < spawnIntensitiesNormal.Length)
        //     {
        //         lvlID.SetNewIntensity(currentIntensityIndex);
        //         var intensity = spawnIntensitiesNormal[currentIntensityIndex];
        //         ignoreNextIntensityTrigger = intensity.IgnoreItensityTriggers;
        //         lvlID.outputEvent.OnSetNewIntensity?.Invoke(intensity, false);
        //     }

        // }




    }

    // public void GoToNextIntensity(int newIntensity)
    // {
    //     Debug.Log("Set NEW Intesntiy from Go To Next");

    //     currentIntensityIndex = newIntensity;
    //     currentObjectivesObtained = nextIntentisityObjectiveValues[newIntensity];
    //     lvlID.SetNewIntensity(currentIntensityIndex);
    //     lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[currentIntensityIndex]);

    // }

    void FinishOverride()
    {
        Debug.Log("Set NEW Intesntiy from FinsihOverride");
        ignoreNextIntensityTrigger = false;

        if (!canSpawnHealthRings && !waitingOnHealth)
        {
            int n;
            waitingOnHealth = true;


            if (currentHealthDelayIndex > delaysToSpawnNextHealthAfterSpawn.Length - 1)
                n = delaysToSpawnNextHealthAfterSpawn.Length - 1;
            else
            {
                n = currentHealthDelayIndex;
            }
            StartCoroutine(WaitForNextHealthChance(delaysToSpawnNextHealthAfterSpawn[n]));
            currentHealthDelayIndex++;
        }

        // int nextInt = CheckCurrentIntensity();

        // lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[nextInt], returningToPrevInt);
        UpdateObjective("revert", 0);

        // ignoreNextIntensityTrigger = spawnIntensitiesNormal[nextInt].IgnoreItensityTriggers;

        // if (returningToPrevInt)
        // {
        //     returnToFinishNonLoopedTranstionIndex = -1;
        //     returningToPrevInt = false;
        // }
    }

    void UpdateHealthIntensity(int lives)
    {

        if (maxNumberOfHealthSpawns != -1 && currentHealthIntensityIndex >= maxNumberOfHealthSpawns) return;
        currentLives = lives;


        if (lives == 1 && spawnIntensitiesHealth.Length > 0 && canSpawnHealthRings)
        {
            waitingOnHealth = false;
            Debug.Log("Set NEW Intesntiy from Health override");
            canSpawnHealthRings = false;

            if (ignoreNextIntensityTrigger)
            {
                Debug.LogError("Setting return intnesity");
                returnToFinishNonLoopedTranstionIndex = currentIntensityIndex;
            }
            ignoreNextIntensityTrigger = true;

            lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesHealth[currentHealthIntensityIndex], false);


            currentHealthIntensityIndex++;

            if (currentHealthIntensityIndex >= spawnIntensitiesHealth.Length && maxNumberOfHealthSpawns == -1)
            {
                currentHealthIntensityIndex--;
            }




        }
    }

    private IEnumerator WaitForNextHealthChance(float delay)
    {
        Debug.LogWarning("WAiting for next helath chance, delay of: " + delay);
        yield return new WaitForSeconds(delay);

        canSpawnHealthRings = true;

        UpdateHealthIntensity(currentLives);



    }

    private void OnEnable()
    {
        lvlID.inputEvent.OnUpdateObjective += UpdateObjective;
        lvlID.inputEvent.finishedOverrideStateLogic += FinishOverride;
        player.globalEvents.OnUpdateLives += UpdateHealthIntensity;

    }
    private void OnDisable()
    {
        lvlID.inputEvent.OnUpdateObjective -= UpdateObjective;
        lvlID.inputEvent.finishedOverrideStateLogic -= FinishOverride;
        player.globalEvents.OnUpdateLives -= UpdateHealthIntensity;


    }
}
