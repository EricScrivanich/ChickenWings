using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class RingParentSpawner : MonoBehaviour
{
    private Pool ringPool;
    [SerializeField] private SpawnStateManager enemySpawner;

    [SerializeField] private float delayToSpawnRandomEnemies;

    [SerializeField] private RingID goldRingID;
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
    private bool doNotEnableEnemySpawner = false;
    private Coroutine currentCourintine;

    [SerializeField] private List<GameObject> ringParentTypes;
    [SerializeField] private List<Vector2> minMax;

    private Queue<Vector2> pigCannotSpawnRanges;

    private bool pigsCannotSpawnEverAtRangeBool = false;

    [SerializeField] private int setPigCanNeverSpawnAfterThisManyObjectives;
    [SerializeField] private int disablePigCanNeverSpawnAfterThisManyObjectives;
    [SerializeField] private Vector2 pigsWillNotSpawnEverRange;
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
        goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", 0);
        goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", 0);
        goldRingID.defaultMaterial.SetFloat("_Alpha", 1);
        goldRingID.highlightedMaterial.SetFloat("_Alpha", 1);
        goldRingID.CenterColor = new Color(goldRingID.CenterColor.r, goldRingID.CenterColor.g, goldRingID.CenterColor.b, 1);

    }
    private int amountused = 0;
    public void GetSpecialRingSetup(int type, Vector2 pos, float duration)
    {
        amountused++;
        Debug.Log("GetSpeical " + amountused);
        StartCoroutine(FadeInSpecialRingSetup(type, pos, 1f));

    }

    private IEnumerator FadeInSpecialRingSetup(int type, Vector2 pos, float duration)
    {
        float startFade = .7f;
        float time = 0;

        float duration1 = duration * .4f;
        float duration2 = duration * .6f;

        goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", startFade);
        goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", startFade);
        goldRingID.defaultMaterial.SetFloat("_Alpha", 0);
        goldRingID.highlightedMaterial.SetFloat("_Alpha", 0);
        goldRingID.CenterColor = new Color(goldRingID.CenterColor.r, goldRingID.CenterColor.g, goldRingID.CenterColor.b, 0);

        yield return new WaitForSecondsRealtime(.1f);
        ringPool.Spawn(ringParentTypes[type], pos, Vector3.zero);
        yield return new WaitForSecondsRealtime(.15f);
        lvlID.outputEvent.SpecialRingFadeIn?.Invoke(.5f);

        while (time < .32f)
        {
            time += Time.unscaledDeltaTime;
            float fadeAmountAlpha = Mathf.Lerp(0, .2f, time / .32f);
            goldRingID.defaultMaterial.SetFloat("_Alpha", fadeAmountAlpha);
            goldRingID.highlightedMaterial.SetFloat("_Alpha", fadeAmountAlpha);


            yield return null;
        }
        time = 0;

        while (time < .28f)
        {
            time += Time.unscaledDeltaTime;
            float fadeAmount = Mathf.Lerp(.7f, .4f, time / .28f);
            float fadeAmountAlpha = Mathf.Lerp(.2f, .5f, time / .28f);
            goldRingID.defaultMaterial.SetFloat("_Alpha", fadeAmountAlpha);
            goldRingID.highlightedMaterial.SetFloat("_Alpha", fadeAmountAlpha);

            goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", fadeAmount);
            goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", fadeAmount);



            yield return null;
        }

        time = 0;
        while (time < .2f)
        {
            time += Time.unscaledDeltaTime;
            float fadeAmount = Mathf.Lerp(.4f, .12f, time / .2f);



            goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", fadeAmount);
            goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", fadeAmount);

            yield return null;
        }



        time = 0;
        while (time < .13f)
        {
            time += Time.unscaledDeltaTime;
            float fadeAmount = Mathf.Lerp(.12f, 0, time / .13f);
            float fadeAmountAlpha = Mathf.Lerp(.5f, 1f, time / .13f);
            goldRingID.defaultMaterial.SetFloat("_Alpha", fadeAmountAlpha);
            goldRingID.highlightedMaterial.SetFloat("_Alpha", fadeAmountAlpha);

            goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", fadeAmount);
            goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", fadeAmount);

            yield return null;
        }
        goldRingID.defaultMaterial.SetFloat("_HitEffectBlend", 0);
        goldRingID.highlightedMaterial.SetFloat("_HitEffectBlend", 0);
        goldRingID.defaultMaterial.SetFloat("_Alpha", 1);
        goldRingID.highlightedMaterial.SetFloat("_Alpha", 1);
        goldRingID.CenterColor = new Color(goldRingID.CenterColor.r, goldRingID.CenterColor.g, goldRingID.CenterColor.b, 1);

    }

    private void StartSpawning(int type)
    {
        if (type == 0)
        {
            StartCoroutine(SpawnRingsCourintine());
        }
        else if (type == 1)
        {
            SpawnRingsAndPigs();
        }
        else if (type == 2)
        {
            StartCoroutine(SpawnPigsCoroutine());
        }

    }

    public void SpawnRings(int section)
    {

        if (section == TriggerEventSectionNumber && !hasTriggeredEvent)
        {
            currentCourintine = StartCoroutine(SpawnRingsCourintine());
            hasTriggeredEvent = true;




        }

    }

    private IEnumerator SpawnRingsCourintine()
    {
        yield return new WaitForSeconds(1.5f);
        bool recalc = false;
        while (player.isAlive)
        {
            if (lvlID.PauseSpawning) recalc = true;
            yield return new WaitUntil(() => lvlID.PauseSpawning == false);
            if (recalc)
            {
                yield return new WaitForSeconds(waitTime);
                recalc = false;
            }
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
            yield return new WaitUntil(() => lvlID.PauseSpawning == false);
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

            yield return new WaitUntil(() => lvlID.PauseSpawning == false);
            if (enemySpawner != null)
            {
                enemySpawner.StopSpawningForTime(delayToSpawnRandomEnemies);
                yield return new WaitForSeconds(enemySpawner.TimeForWaveToReachTarget(4.5f));
            }
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
                if (pigsCannotSpawnEverAtRangeBool)
                {
                    if (position.y > pigsWillNotSpawnEverRange.x && position.y < pigsWillNotSpawnEverRange.y)
                    {
                        isValidPosition = false;
                        // Do not increment attempts since this range should be avoided by default
                        continue;
                    }
                }

                attempts++;
                if (attempts >= maxAttempts)
                {

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

    public void SpawnRingsAndPigs() //(int section)
    {
        // if (section == TriggerEventSectionNumber && !hasTriggeredEvent)


        hasTriggeredEvent = true;


        pigPool = PoolKit.GetPool("PigPool");
        pigCannotSpawnRanges = new Queue<Vector2>();
        ringCannotSpawnRanges = new Queue<Vector2>();

        StartCoroutine(SpawnRingsWithPigsCoroutine(RingIndex));
        StartCoroutine(SpawnPigsCoroutine());




    }

    private void CheckRingsPassed(int amount)
    {
        if (setPigCanNeverSpawnAfterThisManyObjectives == amount)
        {
            pigsCannotSpawnEverAtRangeBool = true;
            enemySpawner.CreateCustomBoundingBox(pigsWillNotSpawnEverRange, Vector2.zero);
            basePigSpawnDelay *= 2;
        }
        else if (disablePigCanNeverSpawnAfterThisManyObjectives == amount)
        {
            pigsCannotSpawnEverAtRangeBool = false;
            enemySpawner.RemoveCustomBoundingBox();
            basePigSpawnDelay *= .5f;
        }
    }

    private void OnEnable()
    {
        // if (listenForNextSectionEventRingsPigs)
        //     lvlID.outputEvent.nextSection += SpawnRingsAndPigs;

        // if (listenForNextSectionEventRingsOnly)
        //     lvlID.outputEvent.nextSection += SpawnRings;

        lvlID.outputEvent.GetSpecialRing += GetSpecialRingSetup;
        lvlID.outputEvent.StartSpawner += StartSpawning;

        if (setPigCanNeverSpawnAfterThisManyObjectives > 0)
        {
            lvlID.outputEvent.RingParentPass += CheckRingsPassed;
        }

    }

    private void OnDisable()
    {
        lvlID.outputEvent.GetSpecialRing -= GetSpecialRingSetup;
        lvlID.outputEvent.StartSpawner -= StartSpawning;

        if (setPigCanNeverSpawnAfterThisManyObjectives > 0)
        {
            lvlID.outputEvent.RingParentPass -= CheckRingsPassed;
        }


        // if (listenForNextSectionEventRingsPigs)
        //     lvlID.outputEvent.nextSection -= SpawnRingsAndPigs;

        // if (listenForNextSectionEventRingsOnly)
        //     lvlID.outputEvent.nextSection -= SpawnRings;



    }

}
