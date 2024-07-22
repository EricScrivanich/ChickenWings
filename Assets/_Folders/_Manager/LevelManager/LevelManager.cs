using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;
    public PlayerID player;
    [SerializeField] private GameObject finishedLevelUIPrefab;
    [SerializeField] private bool activateRingsAfterDelay;
    private Canvas canvas;
    private Button pauseButton;
    private int currentSection;
    [SerializeField] private bool showSectionAtStart;
    [Header("RingPass")]
    [SerializeField] private bool areRingsRequired;
    private bool finishedRings;
    [SerializeField] private int ringsNeeded;



    [Header("Barns")]

    [SerializeField] private bool areBarnsRequired;
    [SerializeField] private int barnsNeeded;
    private bool finishedBarns;
    private int currentBarnsPassed;


    [Header("Other")]
    [SerializeField] private int currentRingsPassed;
    [SerializeField] private List<GameObject> initalGameObjectsToDeactivate;
    [SerializeField] private List<GameObject> sections;
    [SerializeField] private List<int> playSections;
    [SerializeField] private List<float> playSectionDelayToUI;

    private bool hasStartedPlayTimeDelayedPause;

    private void Awake()
    {
        currentSection = 0;
        hasStartedPlayTimeDelayedPause = false;
        LvlID.ResetLevel(areRingsRequired, ringsNeeded, barnsNeeded);
        finishedRings = !areRingsRequired;
        finishedBarns = !areBarnsRequired;

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
    }
    void Start()
    {
        pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        foreach (var obj in sections)
        {
            obj.SetActive(false);
        }
        foreach (var obj in initalGameObjectsToDeactivate)
        {
            obj.SetActive(false);
        }


        if (showSectionAtStart)
        {
            ShowSection(true, 0);
        }



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

                    if (n < playSectionDelayToUI.Count)
                    {
                        ShowNextUISectionFromPlaytime(playSectionDelayToUI[n], currentSection);
                    }

                    return; // Exit the method immediately
                }
                else if (currentSection == playSections[n] - 1)
                {

                    SetButtonsActive buttonScriptvar = GameObject.Find("Buttons").GetComponent<SetButtonsActive>();
                    buttonScriptvar.SetActive(true);
                    buttonScriptvar.SetPlayerButtons(true);
                    StartCoroutine(ShowNextUISection(1.2f, currentSection));

                    return; // Exit the method immediately
                }
            }
            SetButtonsActive buttonScript = GameObject.Find("Buttons").GetComponent<SetButtonsActive>();
            buttonScript.SetActive(false);
            buttonScript.SetPlayerButtons(false);
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
        GetComponent<RingParentSpawner>().SpawnRingsAndPigs(3);
    }

    public void ShowNextUISectionFromPlaytime(float delay, int section)
    {
        currentSection++;
        hasStartedPlayTimeDelayedPause = true;

        if (activateRingsAfterDelay)
        {
            Invoke("SetRingsActiveAfterDelay", delay);
        }

        StartCoroutine(SmoothTimeScaleTransition(0, .5f, delay, true, currentSection));
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
                    SetButtonsActive buttonScript = GameObject.Find("Buttons").GetComponent<SetButtonsActive>();
                    buttonScript.SetActive(true);
                    buttonScript.SetPlayerButtons(true);
                }
                else if (currentSection != playSections[n])
                {
                    SetButtonsActive buttonScript = GameObject.Find("Buttons").GetComponent<SetButtonsActive>();
                    buttonScript.SetActive(false);
                    buttonScript.SetPlayerButtons(false);
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
        currentRingsPassed++;
        LvlID.outputEvent.RingParentPass(currentRingsPassed);

        if (currentRingsPassed == LvlID.ringsNeeded)
        {
            finishedRings = true;
            CheckGoals();
        }

    }

    void HitBarn(int inARow)
    {
        currentBarnsPassed++;
        Debug.Log("HitBarn");


        if (currentBarnsPassed >= LvlID.barnsNeeded)
        {
            finishedBarns = true;
            Debug.Log("SHould Check Goals");
            CheckGoals();
        }

    }

    private void OnDestroy()
    {
        if (areRingsRequired)
        {
            LvlID.inputEvent.RingParentPass -= PassRing;
        }
        if (areBarnsRequired)
        {
            player.globalEvents.OnAddScore -= HitBarn;
        }

    }



    private void CheckGoals()
    {
        currentSection++;
        Debug.Log("Goals Checked: FinishedRings = " + finishedRings + ": FinishedBarns = " + finishedBarns);

        if (currentSection + 1 <= sections.Count)
        {
            ShowSection(true, currentSection);
        }
        if (finishedRings && finishedBarns)
        {
            CreateFinish();
        }
    }


    private void CreateFinish()
    {
        StartCoroutine(SmoothTimeScaleTransition(0, .5f, .4f, false, 0));
        GameObject finishedLevelUI = Instantiate(finishedLevelUIPrefab);

        // Set the parent to the canvas and maintain world position
        finishedLevelUI.transform.SetParent(canvas.transform, false);

        // Get the RectTransform component
        RectTransform rectTransform = finishedLevelUI.GetComponent<RectTransform>();

        // Set the anchored position
        rectTransform.anchoredPosition = new Vector2(0, 645);
    }
}
