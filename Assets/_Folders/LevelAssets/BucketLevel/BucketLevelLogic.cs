using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketLevelLogic : MonoBehaviour
{
    [SerializeField] private int objectiveRingType;
    [SerializeField] private Vector2Int numberOfRandomRingsToSpawnRange;
    [SerializeField] private Vector2Int numberOfRandomEnemiesToSpawnRange;
    private int currentConstantRingTrigger;
    private int numberOfEnemySetupsToSpawn;
    private int setupsSpawned;
    private int numberOfRandomRingsToSpawn;

    private int currentIntensity;
    private bool spawnRandomRingSet;

    private bool spawningRings;
    private int currentConstantRingSetup;
    private int numberOfConstantRingsSpawned;
    [SerializeField] private int[] constantRingTriggerStartEnd;
    private CollectablePoolManager spawnManager;
    [SerializeField] private LevelManagerID lvlID;

    [SerializeField] private SetupParent ringSetupConstant;
    [SerializeField] private SetupParent ringSetupRandom;
    [SerializeField] private SetupParent[] enemyOnlySetups;
    public Vector4[] spawnEnemySetRatioByIntensity;

    private int currentRingType;

    [SerializeField] private int startRingType;
    // Start is called before the first frame update
    void Start()
    {
        setupsSpawned = 0;
        currentConstantRingTrigger = 0;
        spawnRandomRingSet = true;
        spawningRings = false;
        currentRingType = startRingType;
        numberOfConstantRingsSpawned = 0;
        spawnManager = GetComponent<CollectablePoolManager>();

        numberOfEnemySetupsToSpawn = SetRandomInt(numberOfRandomEnemiesToSpawnRange);
        numberOfRandomRingsToSpawn = SetRandomInt(numberOfRandomRingsToSpawnRange);

        Invoke("NextTrigger", 2f);


    }

    private void SpawnRingSetup()
    {

        if (spawningRings)
        {
            if (spawnRandomRingSet)
            {
                spawnManager.currentRingType = 0;
                if (setupsSpawned >= numberOfRandomRingsToSpawn)
                {
                    spawnManager.TriggerRandomSpawnWithRings(ringSetupRandom, true);
                    spawningRings = false;
                    spawnRandomRingSet = false;
                    setupsSpawned = 0;

                    numberOfRandomRingsToSpawn = SetRandomInt(numberOfRandomRingsToSpawnRange);
                }
                else
                {
                    spawnManager.TriggerRandomSpawnWithRings(ringSetupRandom, false);
                }
                setupsSpawned++;
            }
            else
            {
                spawnManager.currentRingType = 2;

                if (currentConstantRingTrigger < constantRingTriggerStartEnd[currentConstantRingSetup])
                {
                    spawnManager.TriggerNextSpawnFromEvent(ringSetupConstant, currentConstantRingTrigger, false);
                    currentConstantRingTrigger++;

                }
                else
                {
                    Debug.Log("Trying to spawn ring setup with trigger of: " + currentConstantRingTrigger);

                    spawnManager.TriggerNextSpawnFromEvent(ringSetupConstant, currentConstantRingTrigger, true);
                    spawningRings = false;
                    spawnRandomRingSet = true;




                }
            }

        }

        else
        {
            setupsSpawned = 0;
            SpawnRandomEnemyOnlySet();
        }

    }

    private void SpawnRandomEnemyOnlySet()
    {
        if (setupsSpawned > numberOfEnemySetupsToSpawn)
        {
            setupsSpawned = 0;
            spawningRings = true;
            numberOfEnemySetupsToSpawn = SetRandomInt(numberOfRandomEnemiesToSpawnRange);
            SpawnRingSetup();
        }
        else
        {
            float randomChanceOfSpawn = Random.Range(0f, 1f);

            float xChance = spawnEnemySetRatioByIntensity[currentIntensity].x;
            float yChance = spawnEnemySetRatioByIntensity[currentIntensity].y + xChance;
            float zChance = spawnEnemySetRatioByIntensity[currentIntensity].z + yChance;
            float wChance = spawnEnemySetRatioByIntensity[currentIntensity].w + zChance;
            SetupParent pickedEnemySetup;


            // Debug.Log("Random chance is: " + randomChanceOfSpawn + " :wChance: " + wChance + " :xChance: " + xChance + " :yChance: " + yChance + " :zChance: " + zChance);
            if (randomChanceOfSpawn < xChance)
            {
                pickedEnemySetup = enemyOnlySetups[0];
            }
            else if (randomChanceOfSpawn < yChance)
            {
                pickedEnemySetup = enemyOnlySetups[1];
            }
            else if (randomChanceOfSpawn < zChance)
            {
                pickedEnemySetup = enemyOnlySetups[2];
            }
            else if (randomChanceOfSpawn < wChance)
            {
                pickedEnemySetup = enemyOnlySetups[3];
            }
            else
            {
                pickedEnemySetup = enemyOnlySetups[0];
            }

            spawnManager.TriggerRandomEnemySpawn(pickedEnemySetup);
            setupsSpawned++;
        }

    }

    private int SetRandomInt(Vector2Int range)
    {
        return Random.Range(range.x, range.y + 1);

    }
    private void NextTrigger()
    {

        if (spawningRings)
        {
            SpawnRingSetup();
        }
        else
        {
            SpawnRandomEnemyOnlySet();
        }



    }

    private void RingSequenceFinished(bool isCorrect, int index)
    {
        if (index == objectiveRingType)
        {

            if (isCorrect)
            {
                

                currentConstantRingSetup++;
                currentConstantRingTrigger++;
                Debug.Log("Passed Goal Ring");
            }
            else
            {
                currentConstantRingTrigger = constantRingTriggerStartEnd[currentConstantRingSetup];
            }


        }

        if (!isCorrect && spawningRings)
        {
            spawningRings = false;
            setupsSpawned = 0;
            numberOfRandomRingsToSpawn = SetRandomInt(numberOfRandomRingsToSpawnRange);

        }

        spawningRings = false;


    }

    // Update is called once per frame
    private void OnEnable()
    {
        lvlID.outputEvent.ringSequenceFinished += RingSequenceFinished;
        lvlID.outputEvent.triggerFinshed += NextTrigger;


    }
    private void OnDisable()
    {
        lvlID.outputEvent.ringSequenceFinished -= RingSequenceFinished;
        lvlID.outputEvent.triggerFinshed -= NextTrigger;


    }
}
