using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIntensityManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private PlayerID player;

    [SerializeField] private RandomSpawnIntensity[] spawnIntensitiesNormal;
    [SerializeField] private RandomSpawnIntensity[] spawnIntensitiesHealth;
    private int currentHealthIntensityIndex = 0;
    private bool cycledAllIntensities = false;

    private int currentIntensityIndex = 0;
    private int nextIntensityIndex = 0;
    private int currentIntensityLevel = 0;

    [SerializeField] private int[] nextIntentisityObjectiveValues;
    private int currentObjectivesObtained = 0;

    [SerializeField] string[] objectivesTracked;
    // Start is called before the first frame update

    private void Start()
    {
        UpdateObjective("all", 0);

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

        if (!shouldUpdate || currentIntensityIndex >= nextIntentisityObjectiveValues.Length)
            return;

        if (currentObjectivesObtained >= nextIntentisityObjectiveValues[currentIntensityIndex])
        {

            int newIntensityIndex = nextIntensityIndex;


            Debug.Log("current objectives is: " + currentObjectivesObtained + " Value must be greater than: " + nextIntentisityObjectiveValues[currentIntensityIndex]);

            // Start checking from currentIntensityIndex + 1
            for (int i = currentIntensityIndex; i < nextIntentisityObjectiveValues.Length; i++)
            {
                if (currentObjectivesObtained >= nextIntentisityObjectiveValues[i])
                {
                    newIntensityIndex = i; // Update to the most recent true statement
                    currentIntensityIndex++;
                    Debug.Log("SetNewIntensity");
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
            lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[currentIntensityIndex]);

        }




    }

    public void GoToNextIntensity(int newIntensity)
    {
        currentIntensityIndex = newIntensity;
        currentObjectivesObtained = nextIntentisityObjectiveValues[newIntensity];
        lvlID.SetNewIntensity(currentIntensityIndex);
        lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[currentIntensityIndex]);

    }

    void FinishOverride()
    {
        lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesNormal[currentIntensityIndex]);
    }

    void UpdateHealthIntensity(int lives)
    {
        if (lives == 1 && spawnIntensitiesHealth.Length > 0)
        {
            lvlID.outputEvent.OnSetNewIntensity?.Invoke(spawnIntensitiesHealth[currentHealthIntensityIndex]);
            currentHealthIntensityIndex++;
            if (currentHealthIntensityIndex >= spawnIntensitiesHealth.Length)
            {
                currentHealthIntensityIndex--;
            }



        }
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
