using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class RingParentSpawner : MonoBehaviour
{
    private Pool ringPool;
    private Pool pigPool;
    public PlayerID player;

    [SerializeField] private int TriggerEventSectionNumber;

    [SerializeField] private bool listenForNextSectionEventRingsPigs;
    [SerializeField] private bool listenForNextSectionEventRingsOnly;
    private bool hasTriggeredEvent;
    [SerializeField] private int RingIndex;
    [SerializeField] private LevelManagerID lvlID;

    [SerializeField] private bool spawnPigsOnly;
    [SerializeField] private int Index;

    [SerializeField] private Vector2 disableSpawnRanges;

    [SerializeField] private float timeAfterPigSpawn;
    [SerializeField] private float timeAfterRingSpawn;
    public int currentRingParentType;
    public float waitTime;
    [SerializeField] private Vector2 ringSpawnMinMaxDelay;
    private Coroutine currentCourintine;

    [SerializeField] private List<GameObject> ringParentTypes;
    [SerializeField] private List<Vector2> minMax;

    private Queue<Vector2> pigCannotSpawnRanges;
    private Queue<Vector2> ringCannotSpawnRanges;

    [SerializeField] private float basePigSpawnDelay;
    [SerializeField] private Vector2 pigSpawnMinMaxDelay;
    [SerializeField] private Vector2 pigSpawnPositionRange;
    // Start is called before the first frame update
    void Start()
    {
        hasTriggeredEvent = false;
        ringPool = PoolKit.GetPool("RingPool");

        if (spawnPigsOnly)
        {
            pigPool = PoolKit.GetPool("PigPool");
            StartCoroutine(SpawnJustPigsCourintine());


        }

    }

    public void SpawnRings(int section)
    {

        if (section == TriggerEventSectionNumber && !hasTriggeredEvent)
        {
            currentCourintine = StartCoroutine(SpawnRingsCourintine());
            hasTriggeredEvent = true;
            Debug.Log("SPawningRIngs");


        }

    }

    private IEnumerator SpawnRingsCourintine()
    {
        yield return new WaitForSeconds(1.5f);
        while (player.isAlive)
        {
            Vector2 position = new Vector2(BoundariesManager.rightBoundary, Random.Range(minMax[RingIndex].x, minMax[RingIndex].y));
            ringPool.Spawn(ringParentTypes[RingIndex], position, Vector3.zero);
            yield return new WaitForSeconds(waitTime);
        }


    }

    private IEnumerator SpawnJustPigsCourintine()
    {
        yield return new WaitForSeconds(3f);

        while (player.isAlive)
        {
            Vector2 position = new Vector2(BoundariesManager.rightBoundary, Random.Range(pigSpawnPositionRange.x, pigSpawnPositionRange.y));
            pigPool.Spawn("NormalPig", position, Vector3.zero);
            float randomDelay = Random.Range(pigSpawnMinMaxDelay.x, pigSpawnMinMaxDelay.y);
            yield return new WaitForSeconds(basePigSpawnDelay + randomDelay);

        }
    }


    private IEnumerator SpawnRingsWithPigsCoroutine(int index)
    {


        yield return new WaitForSeconds(1.5f);
        while (player.isAlive)
        {

            Vector2 position;
            bool isValidPosition;
            int attempts = 0;
            const int maxAttempts = 4;

            do
            {
                position = new Vector2(BoundariesManager.rightBoundary, Random.Range(minMax[index].x, minMax[index].y));
                isValidPosition = true;

                foreach (var item in ringCannotSpawnRanges)
                {
                    if (position.y > item.x && position.y < item.y)
                    {
                        isValidPosition = false;
                        break;
                    }
                }

                attempts++;
                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("Max attempts reached, using most recent position attempt.");
                    break;
                }

            } while (!isValidPosition);

            pigCannotSpawnRanges.Enqueue(new Vector2(position.y + disableSpawnRanges.x, position.y + disableSpawnRanges.y));
            Invoke("RemoveQueueItemPig", timeAfterRingSpawn);

            ringPool.Spawn(ringParentTypes[index], position, Vector3.zero);
            float randomDelay = Random.Range(ringSpawnMinMaxDelay.x, ringSpawnMinMaxDelay.y);
            yield return new WaitForSeconds(waitTime + randomDelay);
        }
    }




    private IEnumerator SpawnPigsCoroutine()
    {
        float randomDelayInitial = Random.Range(.5f, 3f);
        Debug.Log("streted pigggggg");
        yield return new WaitForSeconds(2 + randomDelayInitial);

        while (player.isAlive)
        {
            Vector2 position;
            bool isValidPosition;
            int attempts = 0;
            const int maxAttempts = 4;

            do
            {
                position = new Vector2(BoundariesManager.rightBoundary, Random.Range(pigSpawnPositionRange.x, pigSpawnPositionRange.y));
                isValidPosition = true;

                foreach (var item in pigCannotSpawnRanges)
                {
                    if (position.y > item.x && position.y < item.y)
                    {
                        isValidPosition = false;
                        break;
                    }
                }

                attempts++;
                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("Max attempts reached, using most recent position attempt.");
                    break;
                }

            } while (!isValidPosition);

            ringCannotSpawnRanges.Enqueue(new Vector2(position.y + disableSpawnRanges.x, position.y + disableSpawnRanges.y));
            Invoke("RemoveQueueItemRing", timeAfterPigSpawn);



            pigPool.Spawn("NormalPig", position, Vector3.zero);
            float randomDelay = Random.Range(pigSpawnMinMaxDelay.x, pigSpawnMinMaxDelay.y);
            yield return new WaitForSeconds(basePigSpawnDelay + randomDelay);
        }
    }
    private void RemoveQueueItemPig()
    {
        if (pigCannotSpawnRanges.Count > 0)
        {
            var item = pigCannotSpawnRanges.Dequeue();

        }
    }

    private void RemoveQueueItemRing()
    {
        if (ringCannotSpawnRanges.Count > 0)
        {
            var item = ringCannotSpawnRanges.Dequeue();

        }
    }

    public void SpawnRingsAndPigs(int section)
    {
        if (section == TriggerEventSectionNumber && !hasTriggeredEvent)
        {
            Debug.Log("SPawningRIngs");
            hasTriggeredEvent = true;


            pigPool = PoolKit.GetPool("PigPool");
            pigCannotSpawnRanges = new Queue<Vector2>();
            ringCannotSpawnRanges = new Queue<Vector2>();

            StartCoroutine(SpawnRingsWithPigsCoroutine(RingIndex));
            StartCoroutine(SpawnPigsCoroutine());

        }


    }

    private void OnEnable()
    {
        if (listenForNextSectionEventRingsPigs)
            lvlID.outputEvent.nextSection += SpawnRingsAndPigs;

        if (listenForNextSectionEventRingsOnly)
            lvlID.outputEvent.nextSection += SpawnRings;

    }

    private void OnDisable()
    {
        if (listenForNextSectionEventRingsPigs)
            lvlID.outputEvent.nextSection -= SpawnRingsAndPigs;

        if (listenForNextSectionEventRingsOnly)
            lvlID.outputEvent.nextSection -= SpawnRings;



    }

}
