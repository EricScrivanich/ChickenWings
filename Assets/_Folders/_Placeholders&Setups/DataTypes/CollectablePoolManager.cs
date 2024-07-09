using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePoolManager : MonoBehaviour
{
    public RingPool ringPool;
    public SetupParent setup;
    private int trigger = 0;
    private EnemyPoolManager enemyManager;

    [SerializeField] private RingID[] ringTypes;
    [SerializeField] private GameObject triggerObjectPrefab;
    private Queue<TriggerSetupObject> triggerObjects;
    private int currentRingType;
    private readonly int triggerObjectCount = 3;
    // Start is called before the first frame update
    void Start()
    {
        ringPool.Initialize();
        enemyManager = GetComponent<EnemyPoolManager>();
        triggerObjects = new Queue<TriggerSetupObject>();

        for (int i = 0; i < triggerObjectCount; i++)
        {
            var obj = Instantiate(triggerObjectPrefab);
            var script = obj.GetComponent<TriggerSetupObject>();
            script.manager = this;
            triggerObjects.Enqueue(script);
            obj.SetActive(false);
        }

        Invoke("TriggerNextSpawn", 2f);
    }


    public IEnumerator NextTriggerCourintine(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerNextSpawn();


    }

    public void TriggerNextSpawn()
    {
        setup.SpawnEnemiesOnly(this, enemyManager, trigger);
        trigger++;
    }
    public void GetTriggerObject(Vector2 pos, float speed, float xCord)
    {
        var obj = triggerObjects.Dequeue();
        obj.transform.position = (Vector2)transform.position + pos;
        obj.speed = speed;
        obj.xCordinateTrigger = xCord;
        obj.gameObject.SetActive(true);

        triggerObjects.Enqueue(obj);
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
