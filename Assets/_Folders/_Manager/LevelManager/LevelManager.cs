using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public int LevelIndex;



    [SerializeField] private GameObject TriggerSectionActivateAfterEventType;
    private bool hasInvokedSpecialEvent = false;
    [SerializeField] private float activicationDelay;
    [SerializeField] private GameObject[] activateAfterDelay;

    public GameObject pressButtonPrefab;

    private FlashGroup pressButton;

    public int reachedCheckpoint;

    public bool PauseSpawning;
    [SerializeField] private LevelManagerID LvlID;
    private StateInputSystem playerInputs;
    private InputTracker inputTracker;
    public PlayerID player;
    [SerializeField] private GameObject finishedLevelUIPrefab;

    private Canvas canvas;
    private Button pauseButton;
    private int currentSection;
    [SerializeField] private bool showSectionAtStart;
    [SerializeField] private float showSectionAfterDelay;
    [Header("RingPass")]
    [SerializeField] private bool areRingsRequired;
    private bool finishedRings;
    [SerializeField] private int ringsNeeded;
    [SerializeField] private int currentRingsPassed;


    private int objectivesComplete;


    [Header("Buckets")]
    [SerializeField] private bool areBucketsRequired;
    private bool finishedBuckets;
    [SerializeField] private int bucketsNeeded;
    private int currentBucketsPassed;



    [Header("Barns")]

    [SerializeField] private bool areBarnsRequired;
    [SerializeField] private int barnsNeeded;
    private bool finishedBarns;
    private int currentBarnsPassed;

    [Header("Pigs")]

    [SerializeField] private int pigTypeTracked = -1;
    [SerializeField] private int pigsNeeded;
    private bool finishedPigs;
    private int currentPigsKilled;


    [Header("Other")]
    [SerializeField] private List<GameObject> initalGameObjectsToDeactivate;
    [SerializeField] private List<GameObject> sections;

    [SerializeField] private List<int> playSections;
    [SerializeField] private List<float> playSectionDelayToUI;

    private bool hasStartedPlayTimeDelayedPause;

    public enum EventTypeTracked
    {
        None,
        Mana,

        Jump
    }

    public EventTypeTracked currentEventTracked;


    private void Awake()
    {
        currentSection = 0;
        objectivesComplete = 0;
        hasStartedPlayTimeDelayedPause = false;
        LvlID.ResetLevel(areRingsRequired, ringsNeeded, barnsNeeded, bucketsNeeded);
        finishedRings = !areRingsRequired;
        finishedBarns = !areBarnsRequired;
        finishedBuckets = !areBucketsRequired;

        LvlID.outputEvent.ShowSection += HandleSection;
        LvlID.outputEvent.SetObjectActiveWithDelay += SetObjectActiveWithDelay;
        LvlID.inputEvent.ActivateObjFromEvent += TriggerObjectsFromEvent;
        LvlID.inputEvent.SetCheckPoint += SetNewCheckPoint;




        if (areRingsRequired)
        {
            LvlID.inputEvent.RingParentPass += PassRing;

        }

        if (areBarnsRequired)
        {
            currentBarnsPassed = 0;
            LvlID.barnsNeeded = barnsNeeded;
            player.globalEvents.OnAddScore += HitBarn;

        }

        if (areBucketsRequired)
        {
            currentBucketsPassed = 0;
            LvlID.bucketsNeeded = bucketsNeeded;
            LvlID.outputEvent.addBucketPass += HitBucket;

        }

        if (pigTypeTracked > -1)
        {
            currentPigsKilled = 0;
            finishedPigs = false;
            player.globalEvents.OnKillPig += OnKillPig;
        }
        else
            finishedPigs = true;

        switch (currentEventTracked)
        {
            case EventTypeTracked.None:
                // Implement logic for GlideUp

                break;
            case EventTypeTracked.Mana:
                player.globalEvents.SetCanDashSlash += ShowNextSectionFromEvent;

                // Implement logic for GlideUp

                break;
        }
        LvlID.inputEvent.StartSpawnerInput += TriggerSpawnerFromDelay;



    }

    private void SetNewCheckPoint(int val)
    {
        reachedCheckpoint = val;
    }

    private IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var obj in activateAfterDelay)
        {
            obj.SetActive(true);
        }
    }
    void Start()
    {
        inputTracker = GetComponent<InputTracker>();
        playerInputs = GameObject.Find("Player").GetComponent<StateInputSystem>();
        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        foreach (var obj in sections)
        {
            obj.SetActive(false);
        }





        pressButton = Instantiate(pressButtonPrefab).GetComponent<FlashGroup>();
        pressButton.transform.parent = canvas.transform;
        pressButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -355);
        pressButton.gameObject.SetActive(false);

        int checkPoint = 0;
        if (GameObject.Find("GameManager") != null)
        {
            checkPoint = GameObject.Find("GameManager").GetComponent<ResetManager>().checkPoint;

        }
        Debug.Log("Check point is: " + checkPoint);

        if (checkPoint > 0)
        {
            reachedCheckpoint = checkPoint;
            LvlID.outputEvent.SetCheckPoint?.Invoke(checkPoint);
            if (currentEventTracked == EventTypeTracked.Mana)
            {
                Debug.Log("Filling Mana from check");
                hasInvokedSpecialEvent = true;
                player.globalEvents.FillPlayerMana?.Invoke();

            }

        }
        else
        {
            foreach (var obj in initalGameObjectsToDeactivate)
            {
                obj.SetActive(false);
            }
            LvlID.outputEvent.StartCustomBoundingBoxWithDelay?.Invoke();
            StartCoroutine(ActivateAfterDelay(activicationDelay));

        }


        // if (checkPoint > 0)
        // {

        //     ShowSection(true, checkPoint);
        //     currentSection = checkPoint;
        //     playerInputs.ActivateButtons(false);
        //     inputTracker.EnableTracking(true);

        // }

        // else if (showSectionAtStart)
        // {
        //     ShowSection(true, 0);
        //     playerInputs.ActivateButtons(false);
        //     inputTracker.EnableTracking(false);

        // }
        // else if (showSectionAfterDelay > 0)
        // {
        //     ShowNextUISectionFromPlaytime(showSectionAfterDelay, 0);
        //     playerInputs.ActivateButtons(true);

        //     inputTracker.EnableTracking(false);
        // }
        // else
        // {
        //     playerInputs.ActivateButtons(true);
        //     inputTracker.EnableTracking(false);


        // }



    }

    private void TriggerObjectsFromEvent(TriggerNextSection bubbleScript, bool stopSpawning, GameObject obj)
    {
        StartCoroutine(WaitForEvent(bubbleScript, stopSpawning, obj));

    }
    public IEnumerator WaitForEvent(TriggerNextSection script, bool stopSpawn, GameObject obj)
    {
        switch (script.currentEventTracked)
        {
            case TriggerNextSection.EventTypeTracked.ObjectiveCount:
                // Implement logic for ObjectiveCount
                Debug.Log("Waiting for ObjectiveCount event");
                // Placeholder for your logic
                yield return new WaitUntil(() => objectivesComplete >= script.objectivesNeeded);
                if (stopSpawn) LvlID.PauseSpawning = true;
                obj.SetActive(true);
                break;

            case TriggerNextSection.EventTypeTracked.Mana:
                // Implement logic for Mana
                Debug.Log("Waiting for Mana event");
                // Placeholder for your logic
                // yield return new WaitUntil(() => /* your condition for Mana */);
                break;

            case TriggerNextSection.EventTypeTracked.Jump:
                // Implement logic for Jump
                Debug.Log("Waiting for Jump event");
                // Placeholder for your logic
                // yield return new WaitUntil(() => /* your condition for Jump */);
                break;

            default:
                yield break;
        }

    }


    public void SetInputs()
    {

    }

    public void TriggerSpawnerFromDelay(int type, float delay)
    {
        StartCoroutine(TriggerSpawnerFromDelayCoroutine(type, delay));

    }
    private IEnumerator TriggerSpawnerFromDelayCoroutine(int type, float delay)
    {
        yield return new WaitForSeconds(delay);
        LvlID.outputEvent.StartSpawner?.Invoke(type);
    }


    public void HandleSection(int sectionNum, bool useSection)
    {
        if (useSection)
        {
            sections[sectionNum].SetActive(true);
        }
        else
        {
            if (sections[sectionNum] != null && sections[sectionNum].activeInHierarchy)
                sections[sectionNum].GetComponent<SignMovement>().RetractToNextUILocal(true);
        }
    }

    private void SetObjectActiveWithDelay(GameObject obj, float delay)
    {
        StartCoroutine(SetObjectActiveWithDelayCoroutine(obj, delay));

    }

    private IEnumerator SetObjectActiveWithDelayCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }




    private void EnableButtonTracking()
    {

        StartCoroutine(SetButtons());


    }

    private IEnumerator SetButtons()
    {
        yield return new WaitForSecondsRealtime(1.7f);


        inputTracker.EnableTracking(true);

    }

    public void NextUIFromInput(float quickNext)
    {

        sections[currentSection].GetComponent<SignMovement>().RetractToNextUI(true);


    }

    private void ShortPlayTime(float playDuration)
    {
        Time.timeScale = 1;
        StartCoroutine(SmoothTimeScaleTransition(0, .5f, playDuration, true, currentSection));

    }




    private void ShowSection(bool show, int section)
    {
        if (show)
        {
            // sections[section].SetActive(true);
            StartCoroutine(SmoothTimeScaleTransition(0, .5f, .4f, show, section));

        }
        else
        {

            StartCoroutine(SmoothTimeScaleTransition(1, .3f, .4f, show, section));

        }

    }

    public void NextUI(bool isNext)
    {

        if (isNext)
        {
            currentSection++;
            for (int n = 0; n < playSections.Count; n++)
            {
                if (currentSection == playSections[n])
                {
                    ShowSection(false, currentSection);
                    LvlID.outputEvent.nextSection?.Invoke(currentSection);
                    Debug.Log("CUrrent Play section is: " + currentSection);

                    if (n < playSectionDelayToUI.Count)
                    {
                        ShowNextUISectionFromPlaytime(playSectionDelayToUI[n], currentSection);
                    }

                    inputTracker.EnableTracking(false);
                    playerInputs.ActivateButtons(true);






                    return; // Exit the method immediately
                }
                else if (currentSection == playSections[n] - 1)
                {

                    reachedCheckpoint = playSections[n] - 1;
                    EnableButtonTracking();
                    playerInputs.FinishCooldowns();

                    StartCoroutine(ShowNextUISection(1.2f, currentSection));

                    return; // Exit the method immediately
                }
            }
            playerInputs.ActivateButtons(false);
            StartCoroutine(ShowNextUISection(1.2f, currentSection));
        }

        else
        {
            currentSection--;

            StartCoroutine(ShowNextUISection(1.2f, currentSection));
        }
    }

    private void SetRingsActiveAfterDelay()
    {
        // GetComponent<RingParentSpawner>().SpawnRingsAndPigs(3);
    }

    public void ShowNextUISectionFromPlaytime(float delay, int section)
    {
        currentSection++;
        hasStartedPlayTimeDelayedPause = true;



        StartCoroutine(SmoothTimeScaleTransition(0, .5f, delay, true, currentSection));
    }
    public void ShowNextSectionFromEvent(bool boolVar)
    {

        switch (currentEventTracked)
        {

            case EventTypeTracked.Mana:
                Debug.Log("Will track this: " + boolVar);
                if (!boolVar || hasInvokedSpecialEvent) return;


                hasInvokedSpecialEvent = true;
                TriggerSectionActivateAfterEventType.SetActive(true);

                // Implement logic for GlideUp

                break;
        }



    }

    private IEnumerator ShowNextUISection(float delay, int section)
    {
        Debug.Log("showingSectionInFunc");
        yield return new WaitForSecondsRealtime(delay);
        sections[section].SetActive(true);
    }

    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration, float delay, bool show, int section)
    {
        float start = Time.timeScale;
        float elapsed = 0f;
        if (show)
        {
            Debug.Log("If play section is intended it should be: " + currentSection + " plus one");

            yield return new WaitForSecondsRealtime(delay);
            if (!player.isAlive)
            {
                yield break;
            }
            pauseButton.enabled = false;
            sections[section].SetActive(true);
            for (int n = 0; n < playSections.Count; n++)
            {

                if (currentSection == playSections[n] - 1)
                {
                    playerInputs.ActivateButtons(false);

                    EnableButtonTracking();
                    playerInputs.FinishCooldowns();
                    reachedCheckpoint = playSections[n] - 1;
                }
                else if (currentSection != playSections[n])
                {
                    playerInputs.ActivateButtons(false);
                    inputTracker.EnableTracking(false);
                }
            }


        }
        else
        {
            pauseButton.enabled = true;

        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTimeScale, elapsed / duration);
            yield return null;
        }



        Time.timeScale = targetTimeScale;
        hasStartedPlayTimeDelayedPause = false;
    }



    void PassRing(int inARow)
    {
        if (finishedRings)
            return;
        currentRingsPassed++;
        objectivesComplete++;
        LvlID.outputEvent.RingParentPass?.Invoke(currentRingsPassed);

        if (currentRingsPassed == LvlID.ringsNeeded)
        {
            finishedRings = true;
            CheckGoals();
        }

    }

    void HitBucket()
    {
        if (finishedBuckets)
            return;
        currentBucketsPassed++;
        objectivesComplete++;

        LvlID.outputEvent.setBucketPass?.Invoke(currentBucketsPassed);

        if (currentBucketsPassed >= LvlID.bucketsNeeded)
        {
            finishedBuckets = true;
            CheckGoals();
        }

    }
    private void OnKillPig(int type)
    {
        if (type == pigTypeTracked)
        {
            currentPigsKilled++;
            LvlID.inputEvent.OnUpdateObjective?.Invoke("Pig", 1);
            LvlID.outputEvent.killedPig?.Invoke(currentPigsKilled, pigsNeeded);
        }

        if (currentPigsKilled >= pigsNeeded)
        {
            finishedPigs = true;
            CheckGoals();
        }

    }


    void HitBarn(int inARow)
    {
        if (finishedBarns)
            return;

        currentBarnsPassed++;
        objectivesComplete++;
        LvlID.inputEvent.OnUpdateObjective?.Invoke("Barn", 1);




        if (currentBarnsPassed >= LvlID.barnsNeeded)
        {
            finishedBarns = true;

            CheckGoals();
        }

    }

    private void OnDestroy()
    {
        LvlID.outputEvent.ShowSection -= HandleSection;
        LvlID.outputEvent.SetObjectActiveWithDelay -= SetObjectActiveWithDelay;
        LvlID.inputEvent.ActivateObjFromEvent -= TriggerObjectsFromEvent;
        LvlID.inputEvent.SetCheckPoint -= SetNewCheckPoint;

        if (areRingsRequired)
        {
            LvlID.inputEvent.RingParentPass -= PassRing;
        }
        if (areBarnsRequired)
        {
            player.globalEvents.OnAddScore -= HitBarn;
        }
        if (areBucketsRequired)
        {

            LvlID.outputEvent.addBucketPass -= HitBucket;

        }

        if (pigTypeTracked > -1)
        {
            player.globalEvents.OnKillPig -= OnKillPig;
        }

        switch (currentEventTracked)
        {
            case EventTypeTracked.None:
                // Implement logic for GlideUp

                break;
            case EventTypeTracked.Mana:
                player.globalEvents.SetCanDashSlash -= ShowNextSectionFromEvent;

                // Implement logic for GlideUp

                break;
        }

        LvlID.inputEvent.StartSpawnerInput -= TriggerSpawnerFromDelay;

    }



    private void CheckGoals()
    {
        currentSection++;
        Debug.Log("Goals Checked: FinishedRings = " + finishedRings + ": FinishedBarns = " + finishedBarns);

        // if (currentSection + 1 <= sections.Count)
        // {
        //     ShowSection(true, currentSection);
        // }
        if (finishedRings && finishedBarns && finishedBuckets && finishedPigs)
        {
            CreateFinish();
        }
    }


    private void CreateFinish()
    {
        StartCoroutine(SmoothTimeScaleTransition(0, .3f, .3f, false, 0));
        GameObject finishedLevelUI = Instantiate(finishedLevelUIPrefab);
        PauseButtonActions.lockButtons = false;

        // Set the parent to the canvas and maintain world position
        finishedLevelUI.transform.SetParent(canvas.transform, false);

        // Get the RectTransform component
        RectTransform rectTransform = finishedLevelUI.GetComponent<RectTransform>();

        // Set the anchored position
        rectTransform.anchoredPosition = new Vector2(0, 645);
    }
}
