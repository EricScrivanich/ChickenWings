using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePoolManager : MonoBehaviour
{
    [SerializeField] private bool TestRandom;
    [SerializeField] private bool triggerEvents;
    [SerializeField] private int testTrigger;
    public RingPool ringPool;
    public SetupParent setup;
    public LevelManagerID LvlID;
    private int randomEnemyIntensity = 0;
    public SetupParent randomRing;
    public SetupParent[] enemyOnlySetsByIntensity;
    public Vector4[] spawnEnemySetRatioByIntensity;

    private int trigger = 0;
    private EnemyPoolManager enemyManager;
    int side = 0;

    private int currentEggCollectableIndex = 0;
    [SerializeField] private int eggCollectableCount;

    public int currentRingType;
    private readonly int triggerObjectCount = 3;

    private int randomCountBeforeRings;
    private int currentAmountOfSpawns;
    private int numberOfRingSpawns;
    private bool spawnRingsBool;
    // Start is called before the first frame update
    void Start()
    {
        ringPool.Initialize();
        currentAmountOfSpawns = 0;

        if (testTrigger > 0)
        {
            trigger = testTrigger;
        }

        randomCountBeforeRings = Random.Range(3, 6);
        numberOfRingSpawns = Random.Range(3, 5);
        Debug.Log("Number of Ring spawns is: " + numberOfRingSpawns);
        spawnRingsBool = false;

        enemyManager = GetComponent<EnemyPoolManager>();
        // eggCollectables = new EggCollectableMovement[eggCollectableCount];

        // for (int i = 0; i < eggCollectableCount; i++)
        // {
        //     var obj = Instantiate(eggCollectablePrefab);
        //     var script = obj.GetComponent<EggCollectableMovement>();
        //     eggCollectables[i] = script;
        // }

        // var barnObj = Instantiate(barnPrefab);
        // barnScript = barnObj.GetComponent<BarnMovement>();
        // barnObj.SetActive(false);

        if (triggerEvents)
        {
return;
        }
        else if (TestRandom)
            Invoke("TriggerNextRandomSpawn", 2f);
        else
            Invoke("TriggerNextSpawn", 2f);

    }
    private void OnEnable()
    {
        foreach (var type in ringPool.RingType)
        {
            type.ringEvent.OnCreateNewSequence += SequenceFinished;
        }

    }
    private void OnDisable()
    {
        foreach (var type in ringPool.RingType)
        {
            type.ringEvent.OnCreateNewSequence -= SequenceFinished;
        }

    }


    // public void GetEggCollectable(Vector2 pos, bool isThree, float speed)
    // {
    //     if (currentEggCollectableIndex! < eggCollectableCount) currentEggCollectableIndex = 0;
    //     var obj = eggCollectables[currentEggCollectableIndex];
    //     obj.transform.position = pos;
    //     obj.EnableAmmo(isThree, speed);

    // }

    // public void GetBarn()
    // {
    //     if (side > 3) side = 0;

    //     barnScript.transform.position = new Vector2(13.5f, -4.64f);
    //     Debug.Log("Barn Index Should Be: " + side);
    //     barnScript.Initilaize(side);
    //     side++;
    // }



    public IEnumerator NextTriggerCourintine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (triggerEvents)
        {
            LvlID.outputEvent.triggerFinshed?.Invoke();
        }
        else
            TriggerNextSpawn();


    }
    public IEnumerator NextRandomTriggerCourintine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (triggerEvents)
        {
            LvlID.outputEvent.triggerFinshed?.Invoke();
        }
        else
            TriggerNextRandomSpawn();


    }

    public void TriggerNextRandomSpawn()
    {
        if (currentAmountOfSpawns >= randomCountBeforeRings && !spawnRingsBool)
        {
            spawnRingsBool = true;
            currentAmountOfSpawns = 0;
        }

        if (spawnRingsBool)
        {
            if (currentAmountOfSpawns >= numberOfRingSpawns)
            {
                randomRing.SpawnRandomSetWithRings(this, enemyManager, true);
                spawnRingsBool = false;
                currentAmountOfSpawns = 0;
                Debug.Log("Final Ring Spawn");

            }
            else
            {
                randomRing.SpawnRandomSetWithRings(this, enemyManager, false);
                Debug.Log("Normal Ring Spawn, current number is: " + currentAmountOfSpawns + " - number of ring spawns: " + numberOfRingSpawns);
            }

        }

        else
        {
            float randomChanceOfSpawn = Random.Range(0f, 1f);

            float xChance = spawnEnemySetRatioByIntensity[randomEnemyIntensity].x;
            float yChance = spawnEnemySetRatioByIntensity[randomEnemyIntensity].y + xChance;
            float zChance = spawnEnemySetRatioByIntensity[randomEnemyIntensity].z + yChance;
            float wChance = spawnEnemySetRatioByIntensity[randomEnemyIntensity].w + zChance;
            SetupParent pickedEnemySetup;


            Debug.Log("Random chance is: " + randomChanceOfSpawn + " :wChance: " + wChance + " :xChance: " + xChance + " :yChance: " + yChance + " :zChance: " + zChance);
            if (randomChanceOfSpawn < xChance)
            {
                pickedEnemySetup = enemyOnlySetsByIntensity[0];
            }
            else if (randomChanceOfSpawn < yChance)
            {
                pickedEnemySetup = enemyOnlySetsByIntensity[1];
            }
            else if (randomChanceOfSpawn < zChance)
            {
                pickedEnemySetup = enemyOnlySetsByIntensity[2];
            }
            else if (randomChanceOfSpawn < wChance)
            {
                pickedEnemySetup = enemyOnlySetsByIntensity[3];
            }
            else
            {
                pickedEnemySetup = enemyOnlySetsByIntensity[0];
            }
            pickedEnemySetup.SpawnRandomEnemies(this, enemyManager);
        }

        currentAmountOfSpawns++;

    }

    public void TriggerRandomSpawnWithRings(SetupParent setupVar, bool finalRing)
    {
        setupVar.SpawnRandomSetWithRings(this, enemyManager, finalRing);

    }
    public void TriggerRandomEnemySpawn(SetupParent setupVar)
    {
        setupVar.SpawnRandomEnemies(this, enemyManager);
    }
    public void TriggerNextSpawnFromEvent(SetupParent setupVar, int triggerVar, bool finalRing)
    {
        setupVar.SpawnTrigger(this, enemyManager, triggerVar, finalRing);

    }
    private void BoostLevelIntensity(int newIntensity)
    {
        randomEnemyIntensity = newIntensity;

    }

    private void ChangeRingSetup()
    {

    }
    public void TriggerNextSpawn()
    {
        Debug.Log("Trigger Next Spawn");

        setup.SpawnTrigger(this, enemyManager, trigger, false);


        trigger++;
    }


    // Update is called once per frame
    public void GetRing(Vector2 position, Vector3 scale, Quaternion rotation, float speed, bool isBucket)
    {

        if (!isBucket)
        {
            ringPool.RingType[currentRingType].GetRing((Vector2)transform.position + position, rotation, scale, speed);
        }
        else ringPool.RingType[currentRingType].GetBucket((Vector2)transform.position + position, rotation, scale, speed);

    }

    public void SequenceFinished(bool correctSequence, int index)
    {

        if (!correctSequence)
        {

            StartCoroutine(SequenceFinishedCourintine(0, index));

            // SpawnRings(3.4f);
        }
        else
        {
            StartCoroutine(SequenceFinishedCourintine(1.5f, index));

            // Pool.RingType[index].ResetVariables();
            // SpawnRings(4f);
        }

        if (triggerEvents)
        {
            LvlID.outputEvent.ringSequenceFinished?.Invoke(correctSequence, index);
        }
        randomCountBeforeRings = Random.Range(3, 6);
        numberOfRingSpawns = Random.Range(3, 6);
        spawnRingsBool = false;
        currentAmountOfSpawns = 0;

    }

    private IEnumerator SequenceFinishedCourintine(float time, int index)
    {
        yield return new WaitForSeconds(time);

        switch (index)
        {
            case 0:
                StartCoroutine(ringPool.FadeOutRed());
                break;
            case 1:
                StartCoroutine(ringPool.FadeOutPink());

                break;
            case 2:

                StartCoroutine(ringPool.FadeOutGold());
                break;
            case 3:

                StartCoroutine(ringPool.FadeOutPurple());
                break;
            default:
                break;
        }


    }

    private void TestSpawn()
    {

    }

    public void NextTrigger()
    {
        //     Debug.Log("Triggered Next: enemy count: " + setup.enemyTriggerCount + "collectible count: " + setup.collectableTriggerCount + "Trigger Number: " + trigger);

        //     if (trigger < setup.collectableTriggerCount && trigger < setup.enemyTriggerCount)
        //     {

        //         setup.SpawnBoth(this, enemyManager, trigger);
        //         Debug.Log("Spawned Both");
        //     }

        //     else if (trigger < setup.collectableTriggerCount && trigger >= setup.enemyTriggerCount)
        //     {
        //         setup.SpawnCollectablesOnly(this, trigger);
        //         Debug.Log("Spawned Collectables");


        //     }

        //     else if (trigger >= setup.collectableTriggerCount && trigger < setup.enemyTriggerCount)

        //     {
        //         setup.SpawnEnemiesOnly(this, enemyManager, trigger);
        //         Debug.Log("Spawned Enemies");

        //     }

        //     trigger++;

        // }

        // private void OnEnable()
        // {

        //     foreach (var ringId in ringPool.RingType)
        //     {
        //         ringId.ringEvent.OnSpawnRings += SequenceFinished;

        //         ringId.ringEvent.OnCreateNewSequence += SequenceFinished;

        //     }



        // }
        // private void OnDisable()
        // {
        //     foreach (var ringId in ringPool.RingType)
        //     {

        //         ringId.ringEvent.OnSpawnRings -= SequenceFinished;

        //         ringId.ringEvent.OnCreateNewSequence -= SequenceFinished;
        //         // Subscribe to other events as needed
        //     }




        // }
    }
}
