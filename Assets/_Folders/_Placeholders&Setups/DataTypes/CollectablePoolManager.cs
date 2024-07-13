using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePoolManager : MonoBehaviour
{
    public RingPool ringPool;
    public SetupParent setup;
    private int trigger = 0;
    private EnemyPoolManager enemyManager;
    int side = 0;

    private int currentEggCollectableIndex = 0;
    [SerializeField] private int eggCollectableCount;
   
    private int currentRingType;
    private readonly int triggerObjectCount = 3;
    // Start is called before the first frame update
    void Start()
    {
        ringPool.Initialize();
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

        // Invoke("TriggerNextSpawn", 2f);
       
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
        TriggerNextSpawn();


    }

    public void TriggerNextSpawn()
    {
        if (trigger < setup.collectableTriggerCount && trigger < setup.enemyTriggerCount)
        {
            setup.SpawnBoth(this, enemyManager, trigger);
            Debug.Log("SPawnedBoth");
        }
        else if (trigger! < setup.collectableTriggerCount && trigger < setup.enemyTriggerCount)
        {
            setup.SpawnEnemiesOnly(this, enemyManager, trigger);
            Debug.Log("SPawnedEnemies");

        }

        else if (trigger < setup.collectableTriggerCount && trigger! < setup.enemyTriggerCount)
        {
            setup.SpawnCollectablesOnly(this, trigger);
            Debug.Log("SPawnedCollectables");


        }

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
