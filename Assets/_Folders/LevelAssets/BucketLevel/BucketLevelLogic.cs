using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketLevelLogic : MonoBehaviour
{
    [SerializeField] private int objectiveRingType;
    [SerializeField] private Vector2Int numberOfRandomRingsToSpawnRange;
    [SerializeField] private Vector2Int numberOfRandomEnemiesToSpawnRange;
    [SerializeField] private PlayerID player;
    private int currentConstantRingTrigger;
    private int numberOfEnemySetupsToSpawn;
    private int setupsSpawned;
    private int numberOfRandomRingsToSpawn;

    private int indexTracker;

    private int currentIntensity;
    private bool spawnRandomRingSet;



    private bool spawningRings;
    private bool spawnPinkRings;
    private bool hasSpawnedLives;

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
        indexTracker = 0;
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
        InvokeRepeating("AddTracker", 1f, .2f);


    }

    private void AddTracker()
    {
        indexTracker++;
    }

    private void SpawnRingSetup()
    {


        if (spawningRings)
        {
            if (spawnPinkRings)
            {
                numberOfRandomRingsToSpawn = 1;
                spawnManager.currentRingType = 1;


                if (setupsSpawned >= numberOfRandomRingsToSpawn)
                {
                    spawnManager.TriggerRandomSpawnWithRings(ringSetupRandom, true);

                    spawnRandomRingSet = false;
                    setupsSpawned = 0;

                    numberOfRandomRingsToSpawn = SetRandomInt(numberOfRandomRingsToSpawnRange);
                    spawningRings = false;
                    spawnPinkRings = false;
                }
                else
                {
                    spawnManager.TriggerRandomSpawnWithRings(ringSetupRandom, false);
                }
                setupsSpawned++;

            }
            else if (spawnRandomRingSet)
            {
                spawnManager.currentRingType = 0;
                if (setupsSpawned >= numberOfRandomRingsToSpawn)
                {
                    spawnManager.TriggerRandomSpawnWithRings(ringSetupRandom, true);

                    spawnRandomRingSet = false;
                    setupsSpawned = 0;

                    numberOfRandomRingsToSpawn = SetRandomInt(numberOfRandomRingsToSpawnRange);
                    spawningRings = false;
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



                if (constantRingTriggerStartEnd.Length > currentConstantRingTrigger + 1 && currentConstantRingTrigger < constantRingTriggerStartEnd[currentConstantRingSetup + 1])
                {
                    Debug.LogWarning("Spawning First trigger" + indexTracker);
                    spawnManager.TriggerNextSpawnFromEvent(ringSetupConstant, currentConstantRingTrigger, false);
                    currentConstantRingTrigger++;

                }
                else
                {
                    Debug.LogWarning("Trying to spawn ring setup with trigger of: " + currentConstantRingTrigger);

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
        Debug.LogWarning("Spawning Enemies with range of: " + numberOfEnemySetupsToSpawn + "    " + indexTracker);
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
            Debug.LogWarning("Spawn Rings" + indexTracker);

            SpawnRingSetup();
        }
        else
        {
            Debug.LogWarning("Spawn Enemies" + indexTracker);

            SpawnRandomEnemyOnlySet();
        }



    }

    private void RingSequenceFinished(bool isCorrect, int index)
    {
        Debug.LogWarning("Finsished Sequence with index of: " + index + "     " + indexTracker);
        indexTracker++;

        if (index == objectiveRingType)
        {

            if (isCorrect)
            {
                currentConstantRingSetup++;
                currentConstantRingTrigger++;
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

        if (index == 1 && !isCorrect)
        {
            spawnPinkRings = false;
        }




    }

    void CheckLives(int lives)
    {
        if (lives == 1 && !hasSpawnedLives)
        {
            hasSpawnedLives = true;
            StartCoroutine(ResetHasSpawnedLives());
            StartCoroutine(PinkRingSpawn());

        }

    }
    private IEnumerator ResetHasSpawnedLives()
    {
        yield return new WaitForSeconds(17);
        hasSpawnedLives = false;
    }

    private IEnumerator PinkRingSpawn()
    {
        while (spawningRings)
        {
            yield return null;
        }
        spawnPinkRings = true;
        numberOfEnemySetupsToSpawn = 1;



    }

    // Update is called once per frame
    private void OnEnable()
    {
        lvlID.outputEvent.ringSequenceFinished += RingSequenceFinished;
        lvlID.outputEvent.triggerFinshed += NextTrigger;
        player.globalEvents.OnUpdateLives += CheckLives;


    }
    private void OnDisable()
    {
        lvlID.outputEvent.ringSequenceFinished -= RingSequenceFinished;
        lvlID.outputEvent.triggerFinshed -= NextTrigger;
        player.globalEvents.OnUpdateLives -= CheckLives;



    }
}
