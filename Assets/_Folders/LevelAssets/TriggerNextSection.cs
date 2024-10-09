using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerNextSection : MonoBehaviour
{
    [SerializeField] private int setCheckPoint;

    [SerializeField] private bool continueLockedInputs;
    [SerializeField] private int activateSection;
    [SerializeField] private bool useContactPoint;
    [SerializeField] private Transform setPlayerParentTransform;
    public GameEvent triggerEventOnEnter;
    public GameEvent triggerEventOnEnterSuction;
    [SerializeField] private bool WaitForAlowedExit;



    public bool isCheckPoint { get; private set; }


    private bool hasTriggeredEnterEvent = false;




    [SerializeField] private bool startSpawningAfterComplete;
    [SerializeField] private bool isSuction;
    [SerializeField] private bool tweenPlayerBool;
    public Transform setPlayerPositionTransform;
    [SerializeField] private Transform doSpecialRingFadeTransform;
    [SerializeField] private int doSpecialRingFadeIndex;

    [SerializeField] private GameObject suctionObject;

    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private PlayerID player;
    public float duration;
    [SerializeField] private bool clockwise;
    private Vector3 initialScale = new Vector3(1, 1, 1);


    [Header("Trigger Objects Logic")]
    [SerializeField] private GameObject[] setActiveAfterDelayObjects;
    [SerializeField] private float[] setActiveAfterDelayTimes;
    [SerializeField] private GameObject[] setActiveAfterEventObject;
    private bool activateObjectsFromEventBool;
    [SerializeField] private bool stopSpawningFromEvent;

    public enum EventTypeTracked
    {
        None,
        Mana,
        ObjectiveCount,

        Jump
    }
    public EventTypeTracked currentEventTracked;


    public int objectivesNeeded;
    private Collider2D col;

    private Transform playerTransform;
    private Sequence playerTweenSequence;


    [Header("Start Spawner Logic")]
    [SerializeField] private bool triggerSpawnerFromDelayBool;
    [SerializeField] private bool triggerSpawnerFromEventBool;
    [SerializeField] private int triggerSpawnerType;
    [SerializeField] private float triggerSpawnerDelay;




    [SerializeField] private float mustHoldDuration;
    [SerializeField] private float delayInputsDuration;
    [SerializeField] private bool checkAnyInput;
    [SerializeField] private string[] inputsChecked;
    private string[] noneInp = new string[1];
    [SerializeField] private bool flashCheckedInput;
    [SerializeField] private string pressButtonText;

    // Start is called before the first frame update
    void Start()
    {
        suctionObject.SetActive(isSuction);
        noneInp[0] = "none";


        GetComponent<SpriteRenderer>().enabled = false;
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();


    }
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = !isSuction;
        isCheckPoint = false;



    }

    private void OnDestroy()
    {


    }

    public void TriggerEventOnEnterSuction()
    {
        if (triggerEventOnEnterSuction != null)
            triggerEventOnEnterSuction.TriggerEvent();
    }

    private void HandleSectionActivication(bool show)
    {
        lvlID.outputEvent.ShowSection?.Invoke(activateSection, show);
    }



    private IEnumerator ShowPressButtons()
    {
        yield return new WaitForSecondsRealtime(delayInputsDuration);
        if (checkAnyInput) lvlID.outputEvent.SetPressButtonText?.Invoke(true, 0, "");
        else if (mustHoldDuration == 0) lvlID.outputEvent.SetPressButtonText?.Invoke(true, 1, pressButtonText);
        else lvlID.outputEvent.SetPressButtonText?.Invoke(true, 2, pressButtonText);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useContactPoint)
            setPlayerPositionTransform = other.transform;

        // Alternatively, if you need to set it to a specific point on the player's transform:
        // setPlayerPositionTransform = player.transform.position;
        if (tweenPlayerBool)
            player.globalEvents.OnEnterNextSectionTrigger?.Invoke(0, duration, clockwise, transform, setPlayerPositionTransform.position, tweenPlayerBool);

        else Time.timeScale = 0;
        EnterSection();


    }

    private void HandleOnCheckpoint(int id)
    {
        Debug.Log("Check point is: " + id + " comparing to sections check of: " + setCheckPoint);
        if (id == setCheckPoint)
        {
            isCheckPoint = true;
            col.enabled = false;
            Time.timeScale = 0;
            gameObject.SetActive(true);
            StartCoroutine(DelayToActivateSectionsAndPlayer(.2f));


        }
        else
        {
            gameObject.SetActive(false);
            Debug.Log("Not correct checkpoint");
        }



    }

    private IEnumerator DelayToActivateSectionsAndPlayer(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
       
        player.globalEvents.OnEnterNextSectionTrigger?.Invoke(0, 0, clockwise, transform, setPlayerPositionTransform.position, tweenPlayerBool);
        lvlID.outputEvent.ShowSection?.Invoke(setCheckPoint - 1, true);
        ReadyForExitAndPressButtons(true);

    }


    public void EnterSection()
    {
        Debug.Log("entered");
        col.enabled = false;
        HandleSectionActivication(true);

        if (setCheckPoint != 0)
        {

            lvlID.inputEvent.SetCheckPoint?.Invoke(setCheckPoint);
        }



        // bool checkAny, string[] inp, float duration, float delayForInputs




        if (doSpecialRingFadeTransform != null)
        {
            lvlID.outputEvent.GetSpecialRing?.Invoke(doSpecialRingFadeIndex, doSpecialRingFadeTransform.position, delayInputsDuration);

        }
        else
        {
            Debug.Log("nar");
        }

        Time.timeScale = 0;

        if (!WaitForAlowedExit)
        {
            ReadyForExitAndPressButtons(true);
        }
        else
        {
            activateSection++;
        }

        // TweenPlayer(true);

    }

    private void ReadyForExitAndPressButtons(bool isReady)
    {
        if (isReady)
        {
            player.globalEvents.OnSetInputs?.Invoke(checkAnyInput, inputsChecked, mustHoldDuration, delayInputsDuration + .4f, flashCheckedInput, continueLockedInputs);
            StartCoroutine(ShowPressButtons());
            if (triggerEventOnEnter != null && !hasTriggeredEnterEvent)
            {
                triggerEventOnEnter.TriggerEvent();
                hasTriggeredEnterEvent = true;
            }

        }
        else
        {
            StopAllCoroutines();
            player.globalEvents.OnSetInputs?.Invoke(checkAnyInput, noneInp, 0, 0, false, continueLockedInputs);

            lvlID.outputEvent.SetPressButtonText?.Invoke(false, 0, "");

        }


    }

    private void UnreadyForExitAndPressButtons()
    {

    }

    private void ExitSection()
    {
        if (isCheckPoint)
            lvlID.outputEvent.ShowSection?.Invoke(setCheckPoint - 1, false);
        else
            HandleSectionActivication(false);
        // TweenPlayer(false);

        if (startSpawningAfterComplete)
        {
            lvlID.PauseSpawning = false;
        }
        lvlID.outputEvent.SetPressButtonText?.Invoke(false, 0, "");
        Time.timeScale = 1;
        this.gameObject.SetActive(false);

        if (setActiveAfterDelayObjects.Length > 0)
        {
            for (int i = 0; i < setActiveAfterDelayObjects.Length; i++)
            {
                lvlID.outputEvent.SetObjectActiveWithDelay?.Invoke(setActiveAfterDelayObjects[i], setActiveAfterDelayTimes[i]);
            }
        }

        if (setActiveAfterEventObject.Length > 0)
        {
            lvlID.inputEvent.ActivateObjFromEvent?.Invoke(this, stopSpawningFromEvent, setActiveAfterEventObject[0]);

        }

        if (triggerSpawnerFromDelayBool)
        {
            lvlID.inputEvent.StartSpawnerInput?.Invoke(triggerSpawnerType, triggerSpawnerDelay);
        }

    }

    private void TriggerObjectFromEvent()
    {

    }






    // private void OnTriggerExit2D(Collider2D other)
    // {



    // }


    private void OnEnable()
    {
        lvlID.outputEvent.SetCheckPoint += HandleOnCheckpoint;
        player.globalEvents.ExitSectionTrigger += ExitSection;
        lvlID.outputEvent.setButtonsReadyToPress += ReadyForExitAndPressButtons;
        // player.globalEvents.OnFinishSectionTrigger += SetSectionActive;






    }

    private void OnDisable()
    {
        lvlID.outputEvent.SetCheckPoint -= HandleOnCheckpoint;

        player.globalEvents.ExitSectionTrigger -= ExitSection;
        lvlID.outputEvent.setButtonsReadyToPress -= ReadyForExitAndPressButtons;


        if (playerTweenSequence != null && playerTweenSequence.IsPlaying())
            playerTweenSequence.Kill();

        // player.globalEvents.OnFinishSectionTrigger -= SetSectionActive;


    }

    // Update is called once per frame

}