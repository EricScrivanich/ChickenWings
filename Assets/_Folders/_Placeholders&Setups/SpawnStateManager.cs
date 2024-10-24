using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStateManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;

    private BarnAndEggSpawner eggSpawner;
    public bool stopRandomSpawning { get; private set; }

    [SerializeField] private int startingState = -1;
    [SerializeField] private int startingStateDelay = 3;

    public bool startedMissileTimer = false;

    public Coroutine SpawnWithDelayRoutine;
    public Coroutine WaitForWaveFinishRoutine;
    public Coroutine MissilePigTimer;
    public bool canSpawnMissilePig = false;
    public SpawnStateTransitionLogic transitionLogic;
    private SpawnStateTransitionLogic prevTransitonLogic;


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
    [SerializeField] private int siloPoolSize;
    [SerializeField] private int windMillPoolSize;
    [SerializeField] private int gasPigPoolSize;
    [SerializeField] private int gasPigFlyingPoolSize;

    [SerializeField] private int hotAirBalloonPoolSize;
    [SerializeField] private int flappyPigPoolSize;
    [SerializeField] private int bomberPlanePoolSize;


    [Header("Enemy Pool Prefabs")]
    [SerializeField] private GameObject jetPackPigPrefab;
    [SerializeField] private GameObject tenderizerPigPrefab;
    [SerializeField] private GameObject normalPigPrefab;
    [SerializeField] private GameObject bigPigPrefab;
    [SerializeField] private GameObject missilePigPrefab;
    [SerializeField] private GameObject pilotPigPrefab;

    [SerializeField] private GameObject siloPrefab;
    [SerializeField] private GameObject windMillPrefab;
    [SerializeField] private GameObject gasPigPrefab;
    [SerializeField] private GameObject gasPigFlyingPrefab;
    [SerializeField] private GameObject hotAirBalloonPrefab;
    [SerializeField] private GameObject flappyPigPrefab;
    [SerializeField] private GameObject bomberPlanePrefab;


    [Header("Collectable Pool Prefabs")]
    public RingPool ringPool;
    public int currentRingType;


    public List<Vector2> recentlySpanwnedPositions;
    private float timeSinceLastWave;

    private Vector3 bucketDefaultScale = new Vector3(1.1f, 1.1f, 1.1f);
    private int siloIndex = 0;
    private int windMillIndex = 0;
    private int gasPigIndex = 0;
    private int hotAirBalloonIndex = 0;
    private int flappyPigIndex = 0;
    private int bomberPlaneIndex = 0;
    private int jetPackPigIndex = 0;
    private int normalPigIndex = 0;
    private int bigPigIndex = 0;
    private int tenderizerPigIndex = 0;
    private int pilotPigIndex = 0;
    private int missilePigIndex = 0;
    private int gasPigFlyingIndex;


    private JetPackPigMovement[] jetPackPig;
    private BigPigMovement[] bigPig;
    private TenderizerPig[] tenderizerPig;
    private PigMovementBasic[] normalPig;
    private MissilePigScript[] missilePig;
    private PilotPig[] pilotPig;

    private SiloMovement[] silos;
    private Windmill[] windMills;
    private GasPig[] gasPigs;
    private GasPig[] gasPigsFlying = new GasPig[2];
    private HotAirBalloon[] hotAirBalloons;
    private FlappyPigMovement[] flappyPigs;
    private DropBomb[] bomberPlanes;

    private int currentSetRingOrderIndex = 0;


    private Vector3 scaleFlip = new Vector3(-1, 1, 1);


    SpawnBaseState currentState;

    private float time = 0;

    public SpawnerPureSetupState pureSetupState = new SpawnerPureSetupState();
    public SpawnerPureRandomEnemyState pureRandomEnemyState = new SpawnerPureRandomEnemyState();
    public SpawnerRandomSetupEnemyAndRingState ringAndEnemyRandomSetupState = new SpawnerRandomSetupEnemyAndRingState();
    public SpawnerRandomSetupEnemyState enemyRandomSetupState = new SpawnerRandomSetupEnemyState();

    // Start is called before the first frame update

    private void Awake()
    {
        if (GetComponent<SmokeTrailPool>() != null)
        {
            GetComponent<SmokeTrailPool>().SetGasPigFlyingPoolSize(gasPigFlyingPoolSize);

        }
    }
    void Start()
    {
        prevTransitonLogic = transitionLogic;
        stopRandomSpawning = false;
        currentTransitionLogicIndex = 0;
        eggSpawner = GetComponent<BarnAndEggSpawner>();

        LvlID.outputEvent.OnGetLevelTime?.Invoke(TotalTime());


        SpawnPools();

#if UNITY_EDITOR
        bool isTesting = false;
        for (int i = 0; i < pureSetups.Length; i++)
        {
            if (pureSetups[i].testFromTrigger)
            {
                isTesting = true;
                Debug.Log("Set new trigger for testing, Index of: " + i + " trigger is: " + pureSetups[i].CheckIfTesting());
                currentPureSetup = i;
                pureSetupState.SetNewCurrentTrigger(pureSetups[i].CheckIfTesting());
            }
        }

        if (isTesting)
        {
            GameObject.Find("SetupRecorderParent").SetActive(false);
            startingStateDelay = 0;

        }

#endif

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

    public float TotalTime()
    {
        float totalTime = 0;

        if (pureSetups != null && pureSetups.Length > 0)
        {
            foreach (var set in pureSetups)
            {
                totalTime += set.TotalTime();
            }
        }

        totalTime += startingStateDelay;

        Debug.Log("Total level time is: " + totalTime);
        return totalTime;


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
    public int NextRingType()
    {
        if (transitionLogic.ringSpawnSetTypeOrder == null) return 0;
        if (currentSetRingOrderIndex > transitionLogic.ringSpawnSetTypeOrder.Length - 1)
            currentSetRingOrderIndex = 0;
        if (transitionLogic.ringSpawnSetTypeOrder != null && transitionLogic.ringSpawnSetTypeOrder.Length > 0)
        {
            currentRingType = transitionLogic.ringSpawnSetTypeOrder[currentSetRingOrderIndex];
            Debug.Log("Getting next ring type from order: " + transitionLogic.ringSpawnSetTypeOrder[currentSetRingOrderIndex]);


            currentSetRingOrderIndex++;
            return currentRingType;



        }
        else
        {
            currentRingType = currentRandomSpawnIntensityData.GetRandomRingTypeIndex();
            return currentRingType;

        }

    }

    public void GetEggByType(Vector2 pos, int type, float speed)
    {
        bool shotgun = false;
        bool three = false;

        if (type == 1)
        {

            three = true;
        }
        else if (type == 2)
        {
            shotgun = true;
        }
        else if (type == 3)
        {
            three = true;
            shotgun = true;
        }
        eggSpawner.GetEggCollectable(pos, shotgun, three, speed);



    }
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
        // if (!transitionLogic.loopStates)
        // {
        //     type = transitionLogic.GetRandomSequenceIndex();
        // }
        // else
        Debug.LogError("Current Tanstion logic: " + transitionLogic + " : index- " + currentTransitionLogicIndex);

        type = transitionLogic.OrderedSequnece[currentTransitionLogicIndex];


        if (SpawnWithDelayRoutine != null) StopCoroutine(SpawnWithDelayRoutine);


        if (currentState != null)
            currentState.ExitState(this);

        // currentState.ExitState(this);

        switch (type)
        {
            case (0):
                {
                    currentState = pureSetupState;

                    pureSetupState.SetRingType(NextRingType());
                    break;
                }
            case (1):
                {
                    currentState = ringAndEnemyRandomSetupState;

                    ringAndEnemyRandomSetupState.SetRingType(NextRingType());


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
        Debug.LogError("Entered New State: " + currentState);
        currentState.EnterState(this);

        currentTransitionLogicIndex++;



        if (currentTransitionLogicIndex >= transitionLogic.OrderedSequnece.Length)
        {


            if (transitionLogic.loopStates)
            {

                currentTransitionLogicIndex = 0;

            }
            else
            {

                currentRandomSpawnIntensityData.CheckForNextTranstion();

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
                    pureSetupState.SetRingType(NextRingType());

                    break;
                }
            case (1):
                {
                    currentState = ringAndEnemyRandomSetupState;
                    ringAndEnemyRandomSetupState.SetRingType(NextRingType());
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

        }

        WaitForWaveFinishRoutine = StartCoroutine(SetupDuration(duration));

    }



    public IEnumerator SetupDuration(float delay)
    {
        // Debug.LogError("waiting for " + delay + " seconds for next spawn");
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
        LvlID.outputEvent.OnSetNewTransitionLogic += SetNewTransitionLogic;
        foreach (var ringId in ringPool.RingType)
        {
            ringId.ringEvent.OnCreateNewSequence += RingSequenceFinished;
        }

    }

    private void OnDisable()
    {
        LvlID.outputEvent.OnSetNewIntensity -= SetNewIntensity;
        LvlID.outputEvent.OnSetNewTransitionLogic -= SetNewTransitionLogic;

        foreach (var ringId in ringPool.RingType)
        {
            ringId.ringEvent.OnCreateNewSequence -= RingSequenceFinished;
        }


    }


    #region EventListeners
    private void SetNewTransitionLogic(SpawnStateTransitionLogic logic, bool revert)
    {
        if (revert)
        {
            transitionLogic = prevTransitonLogic;
            currentTransitionLogicIndex = 0;
            currentSetRingOrderIndex = 0;
            return;
        }
        else if (!logic.loopStates)
        {
            if (transitionLogic.loopStates)
                prevTransitonLogic = transitionLogic;

            transitionLogic = logic;
            currentTransitionLogicIndex = 0;
            currentSetRingOrderIndex = 0;
        }


    }
    private void SetNewIntensity(RandomSpawnIntensity newIntensitySet)
    {

        currentRandomSpawnIntensityData = newIntensitySet;
        newIntensitySet.EnterIntensity();
        if (newIntensitySet.missileBasePigChance > 0 && MissilePigTimer == null && canSpawnMissilePig == false)
            MissilePigTimer = StartCoroutine(MissilePigTimerCoroutine(newIntensitySet.minMissilePigDelay));
        Debug.Log("Set Intensity: " + currentRandomSpawnIntensityData);

        // if (newIntensitySet.OverrideStateTransiton > 0)
        // {
        //     currentTransitionLogicIndex = newIntensitySet.OverrideStateTransiton - 1;
        //     transitionLogicOverriden = true;
        // }

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

        currentState.RingsFinished(this, index, correctSequence);

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

        silos = new SiloMovement[siloPoolSize];

        windMills = new Windmill[windMillPoolSize];
        gasPigs = new GasPig[gasPigPoolSize];
        hotAirBalloons = new HotAirBalloon[hotAirBalloonPoolSize];
        flappyPigs = new FlappyPigMovement[flappyPigPoolSize];
        bomberPlanes = new DropBomb[bomberPlanePoolSize];

        // Fill each pool with instances of the prefabs
        for (int i = 0; i < siloPoolSize; i++)
        {
            silos[i] = Instantiate(siloPrefab).GetComponent<SiloMovement>();
            silos[i].gameObject.SetActive(false);  // Set inactive right after instantiation
        }

        for (int i = 0; i < windMillPoolSize; i++)
        {
            windMills[i] = Instantiate(windMillPrefab).GetComponent<Windmill>();
            windMills[i].gameObject.SetActive(false);  // Ensure all windmills are initially inactive
        }

        for (int i = 0; i < gasPigPoolSize; i++)
        {
            gasPigs[i] = Instantiate(gasPigPrefab).GetComponent<GasPig>();
            gasPigs[i].gameObject.SetActive(false);  // Set inactive right after instantiation
        }

        for (int i = 0; i < hotAirBalloonPoolSize; i++)
        {
            hotAirBalloons[i] = Instantiate(hotAirBalloonPrefab).GetComponent<HotAirBalloon>();
            hotAirBalloons[i].gameObject.SetActive(false);  // Set inactive right after instantiation
        }

        for (int i = 0; i < gasPigFlyingPoolSize; i++)
        {
            gasPigsFlying[i] = Instantiate(gasPigFlyingPrefab).GetComponent<GasPig>();
            gasPigsFlying[i].id = i;
            gasPigsFlying[i].gameObject.SetActive(false);

        }

        for (int i = 0; i < flappyPigPoolSize; i++)
        {
            flappyPigs[i] = Instantiate(flappyPigPrefab).GetComponent<FlappyPigMovement>();
            flappyPigs[i].gameObject.SetActive(false);  // Set inactive right after instantiation
        }

        for (int i = 0; i < bomberPlanePoolSize; i++)
        {
            bomberPlanes[i] = Instantiate(bomberPlanePrefab).GetComponent<DropBomb>();
            bomberPlanes[i].gameObject.SetActive(false);  // Set inactive right after instantiation
        }




        jetPackPig = new JetPackPigMovement[jetPackPigPoolSize];

        for (int i = 0; i < jetPackPig.Length; i++)
        {
            // Instantiate the prefab and assign it to the pool array
            var obj = Instantiate(jetPackPigPrefab);
            jetPackPig[i] = obj.GetComponent<JetPackPigMovement>();
            jetPackPig[i].id = i;
            obj.SetActive(false);

            // Get the JetPackPigMovement component and store it in the array


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
        if (speed < 0) scale = new Vector3(scale.x * -1, scale.y, scale.z);

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
        if (speed < 0) scale = new Vector3(scale.x * -1, scale.y, scale.z);
        script.transform.localScale = scale;
        script.speed = speed;
        script.gameObject.SetActive(true);
        jetPackPigIndex++;
    }

    public void GetMissilePig(float x, int type, int wepaonType)
    {

        missilePig[missilePigIndex].Initialize(x, type, wepaonType);

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


        if (speed < 0) scale = new Vector3(scale.x * -1, scale.y, scale.z);

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


    public void GetSilo(Vector2 position, int type, float baseHeightMultiplier)
    {
        if (siloIndex >= silos.Length) siloIndex = 0;
        SiloMovement silo = silos[siloIndex];
        silo.transform.position = position;
        silo.type = type;
        silo.baseHeightMultiplier = baseHeightMultiplier;
        silo.gameObject.SetActive(true);
        siloIndex++;
    }

    public void GetWindMill(Vector2 position, int bladeAmount, float bladeScaleMultiplier, float bladeSpeed, int startRot)
    {
        if (windMillIndex >= windMills.Length) windMillIndex = 0;
        Windmill windMill = windMills[windMillIndex];
        windMill.transform.position = position;
        windMill.bladeAmount = bladeAmount;
        windMill.bladeScaleMultiplier = bladeScaleMultiplier;
        windMill.bladeSpeed = bladeSpeed;
        windMill.startRot = startRot;
        windMill.gameObject.SetActive(true);
        windMillIndex++;
    }

    public void GetGasPig(Vector2 position, float speed, float delay, float startDelay)
    {

        if (Mathf.Abs(speed) > 0)
        {
            bool flip = false;

            if (speed < BoundariesManager.GroundSpeed) flip = true;

            GasPig gasPig = gasPigs[gasPigIndex];
            gasPig.transform.position = position;



            Debug.LogError("Scale flip is: " + flip);

            gasPig.Initialize(speed, delay, flip, startDelay);
            if (flip) gasPig.gameObject.transform.localScale = scaleFlip;
            else gasPig.transform.localScale = BoundariesManager.vectorThree1;

            gasPigIndex++;
            if (gasPigIndex >= gasPigs.Length) gasPigIndex = 0;

        }
        else
        {


            GasPig gasPig = gasPigsFlying[gasPigFlyingIndex];

            gasPig.transform.position = position;
            int s = 8;
            if (position.x < 0) s = -8;
            gasPig.Initialize(s, delay, false, 0);

            gasPigFlyingIndex++;
            if (gasPigFlyingIndex >= gasPigFlyingPoolSize) gasPigFlyingIndex = 0;



        }

    }

    public void GetHotAirBalloon(Vector2 position, int type, float xTrigger, float yTarget, float speed, float delay)
    {
        if (hotAirBalloonIndex >= hotAirBalloons.Length) hotAirBalloonIndex = 0;
        HotAirBalloon hotAirBalloon = hotAirBalloons[hotAirBalloonIndex];
        hotAirBalloon.transform.position = position;
        hotAirBalloon.type = type;
        hotAirBalloon.xTrigger = xTrigger;
        hotAirBalloon.yTarget = yTarget;
        hotAirBalloon.speed = speed;
        hotAirBalloon.delay = delay;
        hotAirBalloon.gameObject.SetActive(true);
        hotAirBalloonIndex++;
    }

    public void GetFlappyPig(Vector2 position, float scaleFactor)
    {
        if (flappyPigIndex >= flappyPigs.Length) flappyPigIndex = 0;
        FlappyPigMovement flappyPig = flappyPigs[flappyPigIndex];
        flappyPig.transform.position = position;
        flappyPig.scaleFactor = scaleFactor;
        flappyPig.gameObject.SetActive(true);
        flappyPigIndex++;
    }

    public void GetBomberPlane(float xDropPosition, float dropAreaScaleMultiplier, float speedTarget)
    {

        DropBomb bomberPlane = bomberPlanes[bomberPlaneIndex];
        bomberPlane.xDropPosition = xDropPosition;
        bomberPlane.dropAreaScaleMultiplier = dropAreaScaleMultiplier;
        bomberPlane.speedTarget = speedTarget;
        bomberPlane.gameObject.SetActive(true);
        bomberPlaneIndex++;
        if (bomberPlaneIndex >= bomberPlanes.Length) bomberPlaneIndex = 0;
    }
    #endregion
}
