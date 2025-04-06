using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;

public class CustomTimeSlider : MonoBehaviour
{


    [Header("Settings")]

    [SerializeField] private GameObject spawnedStepDisplayLine;

    [SerializeField] private GameObject[] objectsUnactiveForPlayView;

    private float pixelsPerStep; // Sensitivity: how many pixels = one time step


    [SerializeField] private int fullLength = 1600;
    [SerializeField] private int normalLength = 1200;
    private int shownLength;

    [Header("Time Control")]
    private int absoluteMax;
    [SerializeField] private int minRange = 0;
    [SerializeField] private int maxRange = 80;
    [SerializeField] private int currentIndex = 0;
    private int currentMinIndex = 0;
    private int currentMaxIndex = 80;
    private int currentObjectIndex = 0;

    [Header("UI References")]
    public RectTransform mainHandle;
    public RectTransform minHandle;
    public RectTransform maxHandle;
    public RectTransform objectHandle;
    public RectTransform startTimeHandle;
    public RectTransform absMaxHandle;
    public GameObject levelLengthObject;

    public SliderHandle objectTimeDisplay;

    [Header("Text Labels")]
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI minText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI absMaxText;
    public TextMeshProUGUI objSpawnText;

    [Header("Slider Area")]
    public RectTransform sliderTrack;
    public RectTransform lineStepsParent;

    private float trackWidth;

    private bool isTimeEditorView = false;
    private bool isPlayView = false;

    private int lastSavedMainValue;
    private int savedMainValuePrePlayView;
    private int lastSavedMinValue;
    private int lastSavedMaxValue;
    private int lastSavedObjectValue;

    public static CustomTimeSlider instance;
    [SerializeField] private Image objectSliderImage;

    public Action<bool> OnEditObjectTime;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }


    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        objectHandle.gameObject.SetActive(false);
        startTimeHandle.gameObject.SetActive(false);

        // shownLength = normalLength;
        // sliderTrack.sizeDelta = new Vector2(shownLength, sliderTrack.sizeDelta.y);
        // pixelsPerStep = (shownLength / (maxRange - minRange) * canvas.scaleFactor);


        // minHandle.gameObject.SetActive(false);
        // maxHandle.gameObject.SetActive(false);
        // levelLengthObject.SetActive(false);

        AssignHandlers();
        LevelRecordManager.OnShowObjectTime += ShowSelectedObject;
        UpdateHandlePositions();
        UpdateTexts();
    }

    public void SetVariables(int main, int min, int max)
    {
        currentIndex = main;
        currentMinIndex = min;
        currentMaxIndex = max;


    }

    public void SetMaxTime(int max)
    {
        absoluteMax = max;

        if (isTimeEditorView)
        {
            maxRange = absoluteMax;

            pixelsPerStep = (fullLength / (maxRange - minRange) * canvas.scaleFactor);

            SetAllHandles();
        }
        OrganizeTimeSteps();
        absMaxText.text = "Level Length: " + LevelRecordManager.instance.FormatTimerText(absoluteMax);


    }


    private void AssignHandlers()
    {
        mainHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        mainHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
        minHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        minHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
        maxHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        maxHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
        objectHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        objectHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
        objectTimeDisplay.OnDragged += OnHandleDragged;
        objectTimeDisplay.OnPressed += OnHandlePressed;
        startTimeHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        startTimeHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
    }

    private void OnDisable()
    {
        mainHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        mainHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        minHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        minHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        maxHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        maxHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        objectHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        objectHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        LevelRecordManager.OnShowObjectTime -= ShowSelectedObject;
        objectTimeDisplay.OnDragged -= OnHandleDragged;
        objectTimeDisplay.OnPressed -= OnHandlePressed;
        startTimeHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        startTimeHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
    }

    public void TempTimeEditorView(bool show)
    {
        if (isTimeEditorView) return;
        if (show)
        {
            mainHandle.localScale = Vector3.one * .85f;
            minRange = 0;
            maxRange = absoluteMax;

            sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
            shownLength = fullLength;

            pixelsPerStep = (fullLength / (maxRange - minRange) * canvas.scaleFactor);
            minHandle.gameObject.SetActive(true);
            maxHandle.gameObject.SetActive(true);
            objectHandle.gameObject.SetActive(true);
            OrganizeTimeSteps();
            SetAllHandles();
        }
        else
        {
            mainHandle.localScale = Vector3.one;

            Debug.Log("Normal View");
            minRange = currentMinIndex;
            maxRange = currentMaxIndex;

            sliderTrack.sizeDelta = new Vector2(normalLength, sliderTrack.sizeDelta.y);
            shownLength = normalLength;

            pixelsPerStep = (shownLength / (maxRange - minRange) * canvas.scaleFactor);
            minHandle.gameObject.SetActive(false);
            maxHandle.gameObject.SetActive(false);
            objectHandle.gameObject.SetActive(false);
            OrganizeTimeSteps();
            SetShownHandle();

        }


    }
    public void TimeEditorView()
    {

        isTimeEditorView = true;
        levelLengthObject.SetActive(true);
        // SetObjects();
        DOTween.Kill(this);
        Camera.main.transform.DOMoveY(4f, .8f).SetEase(Ease.OutCubic).SetUpdate(true);
        mainHandle.localScale = Vector3.one * .85f;
        minRange = 0;
        maxRange = absoluteMax;

        sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
        shownLength = fullLength;

        pixelsPerStep = (fullLength / (maxRange - minRange) * canvas.scaleFactor);
        minHandle.gameObject.SetActive(true);
        maxHandle.gameObject.SetActive(true);
        OrganizeTimeSteps();
        if (LevelRecordManager.instance.currentSelectedObject != null)
            objectHandle.gameObject.SetActive(true);
        SetAllHandles();
    }

    public void NormalView()
    {
        isTimeEditorView = false;

        DOTween.Kill(this);
        levelLengthObject.SetActive(false);
        Camera.main.transform.DOMoveY(-2.26f, .8f).SetEase(Ease.OutCubic).SetUpdate(true);
        mainHandle.localScale = Vector3.one;

        Debug.Log("Normal View");
        minRange = currentMinIndex;
        maxRange = currentMaxIndex;

        sliderTrack.sizeDelta = new Vector2(normalLength, sliderTrack.sizeDelta.y);
        shownLength = normalLength;

        pixelsPerStep = (shownLength / (maxRange - minRange) * canvas.scaleFactor);
        minHandle.gameObject.SetActive(false);
        maxHandle.gameObject.SetActive(false);
        objectHandle.gameObject.SetActive(false);
        OrganizeTimeSteps();
        SetShownHandle();
    }

    public void PlayView()
    {
        DOTween.Kill(this);
        Camera.main.transform.DOMoveY(.8f, .8f).SetEase(Ease.OutCubic).SetUpdate(true);
        foreach (var i in objectsUnactiveForPlayView)
        {
            i.SetActive(false);
        }
        isPlayView = true;
        savedMainValuePrePlayView = currentIndex;
        currentText.gameObject.SetActive(false);
        minRange = 0;
        maxRange = absoluteMax;
        levelLengthObject.SetActive(false);
        minHandle.gameObject.SetActive(false);
        maxHandle.gameObject.SetActive(false);
        objectHandle.gameObject.SetActive(false);
        mainHandle.gameObject.SetActive(false);
        startTimeHandle.gameObject.SetActive(true);
        shownLength = fullLength;

        pixelsPerStep = (shownLength / (maxRange - minRange) * canvas.scaleFactor);
        sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
        OrganizeTimeSteps();
        SetAllHandles();
    }

    public void ExitPlayView()
    {
        isPlayView = false;
        currentText.gameObject.SetActive(true);
        foreach (var i in objectsUnactiveForPlayView)
        {
            i.SetActive(true);
        }

        startTimeHandle.gameObject.SetActive(false);
        mainHandle.gameObject.SetActive(true);
        currentIndex = savedMainValuePrePlayView;
        LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

        if (isTimeEditorView) TimeEditorView();
        else NormalView();




    }


    private bool didDragOnObjectTimeDisplay;

    public bool ChangingObjectTime { get; private set; } = false;
    public bool OverObjectTime { get; private set; } = false;
    private void OnHandlePressed(SliderHandle.HandleType type, bool pressed)
    {
        LevelRecordManager.instance.SetHoldingHandle(pressed);
        lastStepChange = 0;


        if (pressed && type == SliderHandle.HandleType.Time)
        {

            didDragOnObjectTimeDisplay = false;
        }
        else if (type == SliderHandle.HandleType.Time)
        {
            if (!didDragOnObjectTimeDisplay && !LevelRecordManager.ShowTime)
            {
                LevelRecordManager.instance.SetStaticViewParameter(HideItemUI.Type.Time, true);
            }
            else
            {
                didDragOnObjectTimeDisplay = false;
                TempTimeEditorView(false);
            }

        }
        else if (didDragOnObjectTimeDisplay)
        {
            TempTimeEditorView(false);

        }
        switch (type)
        {
            case SliderHandle.HandleType.Main:
            case SliderHandle.HandleType.StartTime:
                lastSavedMainValue = currentIndex;
                break;

            case SliderHandle.HandleType.Min:
                lastSavedMinValue = currentMinIndex;
                if (pressed) minText.gameObject.SetActive(true);
                else minText.gameObject.SetActive(false);
                break;

            case SliderHandle.HandleType.Max:
                lastSavedMaxValue = currentMaxIndex;
                if (pressed) maxText.gameObject.SetActive(true);
                else maxText.gameObject.SetActive(false);
                break;

            case SliderHandle.HandleType.AbsoluteMax:
                break;
            case SliderHandle.HandleType.Object:
            case SliderHandle.HandleType.Time:
                LevelRecordManager.instance.DoingTimeEdit(pressed);
                ChangingObjectTime = pressed;
                OverObjectTime = false;
                lastSavedObjectValue = currentObjectIndex;
                lastSavedMainValue = currentIndex;
                objSpawnText.gameObject.SetActive(pressed);
                OnEditObjectTime?.Invoke(pressed);
                break;
        }

        UpdateTexts();
    }

    // public void RetractTime(bool retract)
    // {
    //     if (retract)
    //         currentIndex = currentIndex - 1;
    //     else
    //         currentIndex = currentIndex + 1;

    //     Debug.Log("Updating Time In Retract");
    //     LevelRecordManager.instance.UpdateTime((ushort)currentIndex);
    //     // UpdateHandlePositions();
    //     // UpdateTexts();

    // }


    private float valueStep;
    private int lastStepChange;
    [SerializeField] private Canvas canvas;
    public List<int> timeStepList;
    public List<Image> timeStepImages = new List<Image>();
    public void SetSpawnedTimeSteps(List<int> steps)
    {
        timeStepList = steps;
        if (timeStepImages.Count < steps.Count)
        {
            for (int i = timeStepImages.Count; i < steps.Count; i++)
            {
                var obj = Instantiate(spawnedStepDisplayLine, lineStepsParent);
                Image img = obj.GetComponent<Image>();
                img.color = Color.red;


                timeStepImages.Add(img);
            }
        }
        else if (timeStepImages.Count > steps.Count)
        {
            for (int i = timeStepImages.Count - 1; i >= steps.Count; i--)
            {
                Destroy(timeStepImages[i].gameObject);
                timeStepImages.RemoveAt(i);
            }
        }
        OrganizeTimeSteps();
    }

    public void OrganizeTimeSteps()
    {
        for (int i = 0; i < timeStepList.Count; i++)
        {
            if (timeStepList[i] >= minRange || timeStepList[i] <= maxRange)
            {
                if (!timeStepImages[i].gameObject.activeInHierarchy)
                {
                    timeStepImages[i].gameObject.SetActive(true);
                }
                timeStepImages[i].transform.localPosition = new Vector2(GetSliderX(timeStepList[i]), 0);
            }
            else
            {
                timeStepImages[i].gameObject.SetActive(false);
            }

        }

    }

    public void NextTimeStep(bool forward)
    {
        if (timeStepList.Count > 0 && timeStepList[timeStepList.Count - 1] < currentIndex)
        {
            if (forward) return;
            else
            {
                currentIndex = timeStepList[timeStepList.Count - 1];
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex, isPlayView);
                UpdateHandlePositions();
                return;
            }
        }
        for (int i = 0; i < timeStepList.Count; i++)
        {
            if (timeStepList[i] >= currentIndex)
            {
                int s = i;


                if (forward)
                {
                    if (timeStepList[i] > currentIndex) s--;
                    if (s + 1 < timeStepList.Count)
                    {
                        currentIndex = timeStepList[s + 1];
                        LevelRecordManager.instance.UpdateTime((ushort)currentIndex, isPlayView);
                        UpdateHandlePositions();
                        return;
                    }
                    else return;
                }
                else
                {
                    if (s - 1 >= 0)
                    {
                        currentIndex = timeStepList[s - 1];
                        LevelRecordManager.instance.UpdateTime((ushort)currentIndex, isPlayView);
                        UpdateHandlePositions();
                        return;

                    }
                    else return;

                }
            }


        }

    }


    private void OnHandleDragged(SliderHandle.HandleType type, float deltaX)
    {

        int stepChange = Mathf.RoundToInt(deltaX / pixelsPerStep);



        if (stepChange == lastStepChange) return;
        // else if (stepChange - lastStepChange > 1)
        // {
        //     stepChange = lastStepChange + 1;
        // }
        // else if (stepChange - lastStepChange < -1)
        // {
        //     stepChange = lastStepChange - 1;
        // }



        lastStepChange = stepChange;



        // Debug.Log("DeltaX: " + deltaX + " StepChange: " + stepChange + "Type: " + type);
        // valueStep = Mathf.Abs(Mathf.Lerp(minRange, maxRange, .01f) - Mathf.Lerp(minRange, maxRange, 0));

        switch (type)
        {
            case SliderHandle.HandleType.Main:

                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);
                break;

            case SliderHandle.HandleType.Min:
                currentMinIndex = Mathf.Clamp(lastSavedMinValue + stepChange, 0, currentIndex);

                break;

            case SliderHandle.HandleType.Max:
                currentMaxIndex = Mathf.Clamp(lastSavedMaxValue + stepChange, currentIndex, absoluteMax);

                break;

            case SliderHandle.HandleType.AbsoluteMax:
                absoluteMax = Mathf.Max(maxRange, absoluteMax + stepChange);
                break;
            case SliderHandle.HandleType.Object:
                currentObjectIndex = Mathf.Clamp(lastSavedObjectValue + stepChange, minRange, maxRange);

                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);


                LevelRecordManager.instance.UpdateObjectTime(currentObjectIndex);
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);



                break;
            case SliderHandle.HandleType.Time:
                if (!didDragOnObjectTimeDisplay)
                {
                    didDragOnObjectTimeDisplay = true;
                    TempTimeEditorView(true);
                }
                currentObjectIndex = Mathf.Clamp(lastSavedObjectValue + stepChange, minRange, maxRange);

                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);


                LevelRecordManager.instance.UpdateObjectTime(currentObjectIndex);
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);



                break;

            case SliderHandle.HandleType.StartTime:

                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex, true);
                SetAllHandles();
                return;

                break;

        }





        UpdateHandlePositions();
        UpdateTexts();
    }

    private void ShowSelectedObject(int step)
    {
        if (!objectHandle.gameObject.activeInHierarchy && LevelRecordManager.ShowTime)
        {
            objectHandle.gameObject.SetActive(true);

        }
        currentObjectIndex = step;
        objectHandle.localPosition = new Vector2(GetSliderX(step), objectHandle.localPosition.y);
        objectSliderImage.sprite = LevelRecordManager.instance.GetIconImage();

    }

    public void UnselectObject()
    {
        if (objectHandle != null)
        {
            objectHandle.gameObject.SetActive(false);
        }

    }

    private void UpdateHandlePositions()
    {

        if (isPlayView)
        {
            SetAllHandles();
            return;
        }

        if (currentIndex > currentMaxIndex)
        {
            currentMaxIndex = currentIndex;
        }
        else if (currentIndex < currentMinIndex)
        {
            currentMinIndex = currentIndex;
        }

        LevelRecordManager.instance.SetMinAndMax(currentMinIndex, currentMaxIndex);

        // Place handles on the slider track by normalized % of absoluteMax
        // SetHandlePos(mainHandle, currentIndex);
        if (!isTimeEditorView)
            SetShownHandle();
        else
        {
            SetAllHandles();

        }
        // SetHandlePos(minHandle, minRange);
        // SetHandlePos(maxHandle, maxRange);
        // SetHandlePos(absMaxHandle, absoluteMax);
    }
    private void SetShownHandle()
    {

        mainHandle.localPosition = new Vector2(GetSliderX(currentIndex), mainHandle.localPosition.y);
        objectHandle.localPosition = new Vector2(GetSliderX(currentObjectIndex), objectHandle.localPosition.y);

        // float x = offset
    }

    public void UpdateMainHandlePosition(int i)
    {
        currentIndex = i;
        UpdateHandlePositions();

    }

    private void SetAllHandles()
    {
        mainHandle.localPosition = new Vector2(GetSliderX(currentIndex), mainHandle.localPosition.y);
        startTimeHandle.localPosition = new Vector2(GetSliderX(currentIndex), startTimeHandle.localPosition.y);
        minHandle.localPosition = new Vector2(GetSliderX(currentMinIndex), minHandle.localPosition.y);
        maxHandle.localPosition = new Vector2(GetSliderX(currentMaxIndex), maxHandle.localPosition.y);
        objectHandle.localPosition = new Vector2(GetSliderX(currentObjectIndex), objectHandle.localPosition.y);



    }

    private float GetSliderX(int index)
    {
        float l = Mathf.InverseLerp(minRange, maxRange, index);
        float offset = l * (shownLength - 40);
        return ((-shownLength + 40) * .5f) + offset;



    }


    private void SetHandlePos(RectTransform handle, int index)
    {
        float normalized = (float)index / absoluteMax;
        float x = normalized * trackWidth;

        Vector2 pos = handle.anchoredPosition;
        handle.anchoredPosition = new Vector2(x, pos.y);
    }

    private void UpdateTexts()
    {

        currentText.text = LevelRecordManager.instance.FormatTimerText(currentIndex);
        minText.text = LevelRecordManager.instance.FormatTimerText(currentMinIndex);
        maxText.text = LevelRecordManager.instance.FormatTimerText(currentMaxIndex);
        objSpawnText.text = LevelRecordManager.instance.FormatTimerText(currentObjectIndex);
        // minText.text = $"Min: {minRange} ({minRange * timeStep * scale:F2}s)";
        // maxText.text = $"Max: {maxRange} ({maxRange * timeStep * scale:F2}s)";
        // absMaxText.text = $"Abs Max: {absoluteMax} ({absoluteMax * timeStep * scale:F2}s)";
    }



    // Optional for external access:

}