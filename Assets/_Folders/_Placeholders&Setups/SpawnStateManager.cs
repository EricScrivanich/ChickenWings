using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStateManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;
    public bool stopRandomSpawning { get; private set; }

    [SerializeField] private int startingState = -1;
    [SerializeField] private int startingStateDelay = 3;

    public bool startedMissileTimer = false;

    public Coroutine SpawnWithDelayRoutine;
    public Coroutine WaitForWaveFinishRoutine;
    public Coroutine MissilePigTimer;
    public bool canSpawnMissilePig = false;
    public SpawnStateTransitionLogic transitionLogic;

    private bool transitionLogicOverriden = false;
    private int currentTransitionLogicIndex;



    public SetupParent[] pureSetups;

    public Queue<Rect> customBoundingBoxes = new Queue<Rect>();

    // public bool SpawnedRandomRings;
    // public bool SpawnedPureSetup;
    // public bool SpawnedPureRandom;


    private int currentPureSetup;
    public int CurrentPureSetup
    {
        get
        {
            return currentPureSetup;
        }
        set
        {
            return;
        }
    }

    public SetupParent[] randomEnemySetups;
    private int currentRandomEnemySetup;
    public SetupParent[] randomRingSetups;
    private int currentRandomRingSetup;

    private int currentRandomSpawnIndex;

    [HideInInspector]
    public RandomSpawnIntensity currentRandomSpawnIntensityData { get; private set; }
    [SerializeField] private Vector2 spawnIntervalRangeAfterTarget;
    [Header("BoundingBoxes")]
    public EnemyBoundingBox normalPigBoundingBoxBase;
    public EnemyBoundingBox jetPackPigBoundingBoxBase;
    public EnemyBoundingBox bigPigBoundingBoxBase;
    public EnemyBoundingBox tenderizerPigBoundingBoxBase;


    [Header("Enemy Pool Sizes")]
    [SerializeField] private int jetPackPigPoolSize;
    [SerializeField] private int tenderizerPigPoolSize;
    [SerializeField] private int normalPigPoolSize;
    [SerializeField] private int bigPigPoolSize;
    [SerializeField] private int missilePigPoolSize;
    [SerializeField] private int pilotPigPoolSize;

    [Header("Enemy Pool Prefabs")]
    [SerializeField] private GameObject jetPackPigPrefab;
    [SerializeField] private GameObject tenderizerPigPrefab;
    [SerializeField] private GameObject normalPigPrefab;
    [SerializeField] private GameObject bigPigPrefab;
    [SerializeField] private GameObject missilePigPrefab;
    [SerializeField] private GameObject pilotPigPrefab;


    [Header("Collectable Pool Prefabs")]
    public RingPool ringPool;
    public int currentRingType;


    public List<Vector2> recentlySpanwnedPositions;
    private float timeSinceLastWave;

    private Vector3 bucketDefaultScale = new Vector3(1.1f, 1.1f, 1.1f);
    private int jetPackPigIndex = 0;
    private int normalPigIndex = 0;
    private int bigPigIndex = 0;
    private int tenderizerPigIndex = 0;
    private int missilePigIndex = 0;
    private int pilotPigIndex = 0;
    private JetPackPigMovement[] jetPackPig;
    private BigPigMovement[] bigPig;
    private TenderizerPig[] tenderizerPig;
    private PigMovementBasic[] normalPig;
    private MissilePigScript[] missilePig;
    private PilotPig[] pilotPig;





    SpawnBaseState currentState;

    private float time = 0;

    public SpawnerPureSetupState pureSetupState = new SpawnerPureSetupState();
    public SpawnerPureRandomEnemyState pureRandomEnemyState = new SpawnerPureRandomEnemyState();
    public SpawnerRandomSetupEnemyAndRingState ringAndEnemyRandomSetupState = new SpawnerRandomSetupEnemyAndRingState();
    public SpawnerRandomSetupEnemyState enemyRandomSetupState = new SpawnerRandomSetupEnemyState();

    // Start is called before the first frame update
    void Start()
    {
        stopRandomSpawning = false;
        currentTransitionLogicIndex = 0;


        SpawnPools();

        if (startingState > -1)
        {
            StartCoroutine(SwitchToStartingStateAfterDelay(startingStateDelay));

        }

        // SwitchState();

    }

    private IEnumerator SwitchToStartingStateAfterDelay(float delay)
    {
        if (delay <= 0) delay = 1f;
        yield return new WaitForSeconds(delay);
        SwitchToSpecficState(startingState);



    }

    // private float blu()
    // {
    //     return TimeForWaveToReachTarget(currentIntensityData.XTargetToStartSpawn) + Random.Range(spawnIntervalRangeAfterTarget.x, spawnIntervalRangeAfterTarget.y);

    // }


    // void Update()
    // {
    //     currentState.UpdateState(this);
    //     // time += Time.deltaTime;

    //     // if (time > TimeForWaveToReachTarget(currentRandomSpawnData.XTargetToStartSpawn + Random.Range(spawnIntervalRangeAfterTarget.x, spawnIntervalRangeAfterTarget.y)))
    //     // {
    //     //     SwitchState(pureRandomEnemyState);
    //     //     time = 0;

    //     // }


    // }

    public void ChangePureSetupIndex(int newValue)
    {
        currentPureSetup = newValue;

    }

    // public IEnumerator SpawnWithDelay(float delayAmount, )
    // {
    //     TimeForWaveToReachTarget(currentIntensityData.XTargetToStartSpawn) + Random.Range(spawnIntervalRangeAfterTarget.x, spawnIntervalRangeAfterTarget.y);
    // }

    public void SwitchStateWithLogic()
    {
        int type = 0;
        if (!transitionLogic.loopStates)
        {
            type = transitionLogic.GetRandomSequenceIndex();
        }
        else
            type = transitionLogic.OrderedSequnece[currentTransitionLogicIndex];


        if (SpawnWithDelayRoutine != null) StopCoroutine(SpawnWithDelayRoutine);
        // currentState.ExitState(this);

        switch (type)
        {
            case (0):
                {
                    currentState = pureSetupState;
                    break;
                }
            case (1):
                {
                    currentState = ringAndEnemyRandomSetupState;
                    break;
                }
            case (2):
                {
                    currentState = enemyRandomSetupState;
                    break;
                }
            case (3):
                {
                    currentState = pureRandomEnemyState;
                    break;
                }
        }
        Debug.LogError("Type spawend is: " + currentState);
        currentState.EnterState(this);


        if (transitionLogicOverriden)
        {
            LvlID.inputEvent.finishedOverrideStateLogic?.Invoke();
            transitionLogicOverriden = false;
        }


        if (transitionLogic.loopStates)
        {
            currentTransitionLogicIndex++;
            if (currentTransitionLogicIndex >= transitionLogic.OrderedSequnece.Length)
            {
                currentTransitionLogicIndex = 0;
            }
        }

    }

    public void SwitchToSpecficState(int stateIndex)
    {
        switch (stateIndex)
        {
            case (0):
                {
                    currentState = pureSetupState;
                    break;
                }
            case (1):
                {
                    currentState = ringAndEnemyRandomSetupState;
                    break;
                }
            case (2):
                {
                    currentState = enemyRandomSetupState;
                    break;
                }
            case (3):
                {
                    currentState = pureRandomEnemyState;

                    break;
                }
        }

        currentState.EnterState(this);

    }

    public void StartLogicSpawnAfterDelay(float delay)
    {
        Invoke("SwitchStateWithLogic", delay);
    }

    public void StartSpecificSpawnAfterDelay(int type)
    {
        StartCoroutine(SwitchToSpecificStateAfterDelay(type));
    }

    private IEnumerator SwitchToSpecificStateAfterDelay(int type)
    {
        yield return new WaitForSeconds(3);
        SwitchToSpecficState(type);
    }



    public void StopSpawningForTime(float delay)
    {
        stopRandomSpawning = true;
        if (SpawnWithDelayRoutine != null)
            StopCoroutine(SpawnWithDelayRoutine);
        if (WaitForWaveFinishRoutine != null)
            StopCoroutine(WaitForWaveFinishRoutine);

        TimerForNextWave(delay);
    }

    public void CreateCustomBoundingBox(Vector2 yRange, Vector2 xRange)
    {
        Debug.Log("Creatred Custom BoudingBox");
        if (xRange == Vector2.zero)
        {
            xRange = new Vector2(currentRandomSpawnIntensityData.XSpawnRange.x, currentRandomSpawnIntensityData.XSpawnRange.y + 8f);
        }

        // Create the bounding box using the given dimensions
        Rect customBox = new Rect(xRange.x, yRange.x, xRange.y - xRange.x, yRange.y - yRange.x);
        customBoundingBoxes.Enqueue(customBox);

    }

    public void RemoveCustomBoundingBox()
    {
        if (customBoundingBoxes.Count > 0)
        {
            customBoundingBoxes.Dequeue();
        }
        else
        {
            Debug.LogWarning("No custom bounding boxes to remove.");
        }
    }


    public void TimerForNextWave(float duration)
    {
        if (WaitForWaveFinishRoutine != null)
        {
            StopCoroutine(WaitForWaveFinishRoutine);
            Debug.LogError("Overriden current timer");
        }

        WaitForWaveFinishRoutine = StartCoroutine(SetupDuration(duration));

    }



    public IEnumerator SetupDuration(float delay)
    {
        Debug.LogError("waiting for " + delay + " seconds for next spawn");
        yield return new WaitForSeconds(delay);
        currentState.SetupHitTarget(this);
        if (stopRandomSpawning) stopRandomSpawning = false;
        WaitForWaveFinishRoutine = null;

    }

    public IEnumerator MissilePigTimerCoroutine(float delay)
    {
        canSpawnMissilePig = false;
        yield return new WaitForSeconds(delay);
        canSpawnMissilePig = true;
    }

    public bool GetMissilePigIfReady(float delay, float chance)
    {
        if (!startedMissileTimer && chance > 0)
        {
            startedMissileTimer = true;
            MissilePigTimer = StartCoroutine(MissilePigTimerCoroutine(delay));
            return false;

        }
        else if (delay <= 0 || chance <= 0 || !canSpawnMissilePig || missilePig[missilePigIndex].gameObject.activeInHierarchy) return false;


        else
        {
            float ran = Random.Range(0f, 1f);

            if (ran < chance)
            {
                MissilePigTimer = StartCoroutine(MissilePigTimerCoroutine(delay));

                return true;
            }
            else
                return false;
        }


    }



    public float TimeForWaveToReachTarget(float xCordTarget)
    {
        float returnedTime = 0;

        if (recentlySpanwnedPositions.Count == 0)
            return 2f;
        foreach (var vect in recentlySpanwnedPositions)
        {
            float adjustedSpeed = Mathf.Abs(vect.x);

            // Calculate time = distance / speed
            float distance = Mathf.Abs(vect.y - xCordTarget);
            float calculatedTime = distance / adjustedSpeed;

            if (calculatedTime > returnedTime) returnedTime = calculatedTime;

        }

        return returnedTime - timeSinceLastWave;
    }

    private void OnEnable()
    {
        LvlID.outputEvent.OnSetNewIntensity += SetNewIntensity;
        foreach (var ringId in ringPool.RingType)
        {
            ringId.ringEvent.OnCreateNewSequence += RingSequenceFinished;
        }

    }

    private void OnDisable()
    {
        LvlID.outputEvent.OnSetNewIntensity -= SetNewIntensity;
        foreach (var ringId in ringPool.RingType)
        {
            ringId.ringEvent.OnCreateNewSequence -= RingSequenceFinished;
        }


    }


    #region EventListeners

    private void SetNewIntensity(RandomSpawnIntensity newIntensitySet)
    {

        currentRandomSpawnIntensityData = newIntensitySet;
        if (newIntensitySet.missileBasePigChance > 0 && MissilePigTimer == null && canSpawnMissilePig == false)
            MissilePigTimer = StartCoroutine(MissilePigTimerCoroutine(newIntensitySet.minMissilePigDelay));
        Debug.Log("Set Intensity: " + currentRandomSpawnIntensityData);

        if (newIntensitySet.OverrideStateTransiton > 0)
        {
            currentTransitionLogicIndex = newIntensitySet.OverrideStateTransiton - 1;
            transitionLogicOverriden = true;
        }

        pureRandomEnemyState.SetNewIntensity(this, currentRandomSpawnIntensityData);

    }

    #endregion
    #region RingListeners
    public void RingSequenceFinished(bool correctSequence, int index)
    {

        if (!correctSequence)
        {

            StartCoroutine(RingSequenceFinishedCourintine(0, index));


        }
        else
        {
            StartCoroutine(RingSequenceFinishedCourintine(1.5f, index));


        }

        currentState.RingsFinished(this, correctSequence);

    }

    private IEnumerator RingSequenceFinishedCourintine(float time, int index)
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
    #endregion

    #region pools

    private void SpawnPools()
    {
        ringPool.Initialize();
        jetPackPig = new JetPackPigMovement[jetPackPigPoolSize];

        for (int i = 0; i < jetPackPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(jetPackPigPrefab);
            obj.SetActive(false);

            // Get the JetPackPigMovement component and store it in the array
            jetPackPig[i] = obj.GetComponent<JetPackPigMovement>();
        }
        missilePig = new MissilePigScript[missilePigPoolSize];
        for (int i = 0; i < missilePig.Length; i++)
        {
            var obj = Instantiate(missilePigPrefab).GetComponent<MissilePigScript>();
            obj.gameObject.SetActive(false);
            missilePig[i] = obj;

        }

        pilotPig = new PilotPig[pilotPigPoolSize];
        for (int i = 0; i < pilotPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(pilotPigPrefab);
            obj.SetActive(false);

            // Get the pilotPigMovement component and store it in the array
            pilotPig[i] = obj.GetComponent<PilotPig>();
        }


        tenderizerPig = new TenderizerPig[tenderizerPigPoolSize];

        for (int i = 0; i < tenderizerPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(tenderizerPigPrefab);
            obj.SetActive(false);

            // Get the tenderizerPigMovement component and store it in the array
            tenderizerPig[i] = obj.GetComponent<TenderizerPig>();
        }

        normalPig = new PigMovementBasic[normalPigPoolSize];

        for (int i = 0; i < normalPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(normalPigPrefab);
            obj.SetActive(false);

            // Get the normalPigMovement component and store it in the array
            normalPig[i] = obj.GetComponent<PigMovementBasic>();
        }

        bigPig = new BigPigMovement[bigPigPoolSize];

        for (int i = 0; i < bigPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(bigPigPrefab);
            obj.SetActive(false);

            // Get the bigPigMovement component and store it in the array
            bigPig[i] = obj.GetComponent<BigPigMovement>();
        }

    }

    public void GetRing(Vector2 position, Vector3 scale, Quaternion rotation, float speed, bool isBucket)
    {

        if (!isBucket)
        {
            ringPool.RingType[currentRingType].GetRing((Vector2)transform.position + position, rotation, scale, speed);
        }
        else ringPool.RingType[currentRingType].GetBucket((Vector2)transform.position + position, rotation, bucketDefaultScale, speed);
    }

    public void GetNormalPig(Vector2 pos, Vector3 scale, float speed)
    {
        if (normalPigIndex >= normalPig.Length) normalPigIndex = 0;
        var script = normalPig[normalPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);

        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.xSpeed = speed;
        script.InitializePig();
        normalPigIndex++;
    }

    public void GetPilotPig(Vector2 pos, Vector3 scale, float speed, int flightMode, float minY, float maxY, float yForce, float maxYSpeed, float xTrigger)
    {
        if (pilotPigIndex >= pilotPig.Length) pilotPigIndex = 0;
        var script = pilotPig[pilotPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);

        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.initialSpeed = speed;
        script.flightMode = flightMode;
        script.minY = minY;
        script.maxY = maxY;
        script.addForceY = yForce;
        script.maxYSpeed = maxYSpeed;
        script.xTrigger = xTrigger;
        script.gameObject.SetActive(true);
        pilotPigIndex++;
    }
    public void GetJetPackPig(Vector2 pos, Vector3 scale, float speed)
    {
        if (jetPackPigIndex >= jetPackPig.Length) jetPackPigIndex = 0;
        var script = jetPackPig[jetPackPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);


        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.speed = speed;
        script.gameObject.SetActive(true);
        jetPackPigIndex++;
    }

    public void GetMissilePig(bool flippedP, bool flippedW, float rangeAdj, float x)
    {

        missilePig[missilePigIndex].Initialize(flippedP, flippedW, rangeAdj, x);

        missilePigIndex++;

        if (missilePigIndex >= missilePigPoolSize)
        {
            missilePigIndex = 0;
        }
    }

    public void GetBigPig(Vector2 pos, Vector3 scale, float speed, float yForce, float distanceToFlap, float startingFallSpot)
    {
        if (bigPigIndex >= bigPig.Length) bigPigIndex = 0;
        var script = bigPig[bigPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);
        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.speed = speed;
        script.yForce = yForce;
        script.distanceToFlap = distanceToFlap;
        script.startingFallSpot = startingFallSpot;
        script.gameObject.SetActive(true);
        bigPigIndex++;
    }

    public void GetTenderizerPig(Vector2 pos, Vector3 scale, float speed, bool hasHammer)
    {
        if (tenderizerPigIndex >= tenderizerPig.Length) tenderizerPigIndex = 0;
        var script = tenderizerPig[tenderizerPigIndex];
        if (script.gameObject.activeInHierarchy) script.gameObject.SetActive(false);

        script.transform.position = (Vector2)transform.position + pos;
        script.transform.localScale = scale;
        script.speed = speed;
        script.hasHammer = hasHammer;
        script.gameObject.SetActive(true);
        tenderizerPigIndex++;
    }

    #endregion
}
