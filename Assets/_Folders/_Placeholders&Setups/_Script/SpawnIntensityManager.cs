using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIntensityManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private PlayerID player;

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

    private bool canSpawnHealthRings = false;

    private bool ignoreNextIntensityTrigger;
    private int currentLives;
    // Start is called before the first frame update

    private void Start()
    {
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
        for (int i = 0; i < nextIntentisityObjectiveValues.Length; i++)
        {
            if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i] && i > n)
            {
                n = i;
            }
        }

        return n;
    }

    void UpdateObjective(string type, int addedValue)
    {
        Debug.Log("Trying");

        bool shouldUpdate = false;


        if (type == "all")
        {
            currentObjectivesObtained += addedValue;
            shouldUpdate = true;

        }
        else
        {
            foreach (string s in objectivesTracked)
            {
                if (s == type)
                {
                    currentObjectivesObtained += addedValue;
                    shouldUpdate = true;
                    break;
                }
            }
        }

        if (!shouldUpdate || nextIntensityIndex >= nextIntentisityObjectiveValues.Length || ignoreNextIntensityTrigger)
        {
            Debug.Log("Ignoring intesntity index is: " + ignoreNextIntensityTrigger);
            return;
        }


        if (currentObjectivesObtained >= nextIntentisityObjectiveValues[nextIntensityIndex])
        {

            int newIntensityIndex = nextIntensityIndex;


            // Start checking from currentIntensityIndex + 1
            for (int i = currentIntensityIndex; i < nextIntentisityObjectiveValues.Length; i++)
            {
                if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i])
                {
                    newIntensityIndex = i; // Update to the most recent true statement

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

            Debug.Log("inoking event: " + currentIntensityIndex);

            lvlID.SetNewIntensity(currentIntensityIndex);
            var intensity = spawnIntensitiesNormal[currentIntensityIndex];
            lvlID.outputEvent.OnSetNewIntensity?.Invoke(intensity);



        }




    }

    public void GoToNextIntensity(int newIntensity)
    {
        Debug.Log("Set NEW Intesntiy from Go To Next");

        currentIntensityIndex = newIntensity;
        currentObjectivesObtained = nextIntentisityObjectiveValues[newIntensity];
        lvlID.SetNewIntensity(currentIntensityIndex);
        lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[currentIntensityIndex]);

    }

    void FinishOverride()
    {
        Debug.Log("Set NEW Intesntiy from FinsihOverride");
        ignoreNextIntensityTrigger = false;

        if (!canSpawnHealthRings)
        {
            int n;

            if (currentHealthIntensityIndex > delaysToSpawnNextHealthAfterSpawn.Length - 1)
                n = delaysToSpawnNextHealthAfterSpawn.Length - 1;
            else
            {
                n = currentHealthIntensityIndex;
            }
            StartCoroutine(WaitForNextHealthChance(delaysToSpawnNextHealthAfterSpawn[n]));
        }


        lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[CheckCurrentIntensity()]);
    }

    void UpdateHealthIntensity(int lives)
    {
        currentLives = lives;
        if (lives == 1 && spawnIntensitiesHealth.Length > 0 && canSpawnHealthRings)
        {
            Debug.Log("Set NEW Intesntiy from Health override");
            canSpawnHealthRings = false;

            ignoreNextIntensityTrigger = true;

            lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesHealth[currentHealthIntensityIndex]);


            currentHealthIntensityIndex++;

            if (currentHealthIntensityIndex >= spawnIntensitiesHealth.Length)
            {
                currentHealthIntensityIndex--;
            }




        }
    }

    private IEnumerator WaitForNextHealthChance(float delay)
    {
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
