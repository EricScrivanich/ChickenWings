using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;
    private int currentSection;
    private int amountOfSections;
    [SerializeField] private bool showSectionAtStart;
    [Header("RingPass")]
    [SerializeField] private bool areRingsRequired;
    private bool finishedRings;
    [SerializeField] private int ringsNeeded;
    [SerializeField] private int currentRingsPassed;

    [SerializeField] private List<GameObject> sections;
    // [SerializeField] private List<GameObject> Section2;
    // [SerializeField] private List<GameObject> Section3;



    // Start is called before the first frame update
    private void Awake()
    {
        currentSection = 0;


        LvlID.ResetLevel(areRingsRequired, ringsNeeded);
        finishedRings = !areRingsRequired;

        if (areRingsRequired)
        {
            LvlID.inputEvent.RingParentPass += PassRing;

        }

    }
    void Start()
    {
        foreach (var obj in sections)
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
            StartCoroutine(SmoothTimeScaleTransition(0, .5f, show, section));
        }
        else
        {
            sections[section].SetActive(false);
            Time.timeScale = 1;
        }

    }

    public void NextUI(bool isNext)
    {
        if (isNext)
        {
            currentSection++;


        }
        else
        {
            currentSection--;


        }
        StartCoroutine(ShowNextUISection(1.2f, currentSection));

    }

    private IEnumerator ShowNextUISection(float delay, int section)
    {
        yield return new WaitForSecondsRealtime(delay);
        sections[section].SetActive(true);
    }

    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration, bool show, int section)
    {
        float start = Time.timeScale;
        float elapsed = 0f;
        if (show)
        {
            yield return new WaitForSecondsRealtime(.4f);
            sections[section].SetActive(true);



        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTimeScale, elapsed / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;
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

    private void OnDestroy()
    {
        LvlID.inputEvent.RingParentPass -= PassRing;
    }



    private void CheckGoals()
    {
        currentSection++;
        if (currentSection + 1 <= sections.Count)
        {
            ShowSection(true, currentSection);
        }
        if (finishedRings == true)
        {
            LvlID.outputEvent.FinishedLevel?.Invoke();
        }
    }
}
