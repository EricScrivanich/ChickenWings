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
    private int minRange = 0;
    private int maxRange = 80;
    private int currentIndex = 0;
    private int currentMinIndex = 0;
    private int currentMaxIndex = 80;
    private int currentObjectIndex = 0;
    private int currentMulipleObjectIndex = 0;


    private int currentMultSelectLeftIndex;
    private int currentMultSelectRightIndex;
    private int currentMultSelectCenterIndex;

    private int lastMultSelectLeftIndex;
    private int lastMultSelectRightIndex;
    private int lastMultSelectCenterIndex;

    public bool canMultSelect = false;







    private int currentTweenStep;

    [Header("UI References")]
    [SerializeField] private GameObject mainHandleArrow;
    [SerializeField] private RectTransform multSelectHighlight;
    [SerializeField] private float mainHandleLowY;
    public RectTransform mainHandle;
    public RectTransform minHandle;
    public RectTransform maxHandle;
    public RectTransform objectHandle;
    // public RectTransform startTimeHandle;
    public RectTransform absMaxHandle;
    public RectTransform multipleObjectHandle;


    [SerializeField] private RectTransform[] multSelecthandles;
    // public GameObject levelLengthObject;

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

    [Header("Tween Objects")]
    [SerializeField] private RectTransform selectedTweenParent;
    [SerializeField] private RectTransform nonSelectedTweenParent;
    [SerializeField] private GameObject startTweenObject;
    [SerializeField] private GameObject endTweenObject;

    private List<SliderHandle> startTweenHandles = new List<SliderHandle>();
    private List<SliderHandle> endTweenHandles = new List<SliderHandle>();



    private float trackWidth;

    private bool isTimeEditorView = false;
    private bool isTempTimeEditorView = false;
    public bool isPlayView { get; private set; } = false;
    private Vector2Int multipleSelectMinMaxChange;
    private int lastSavedAverageMultipleStep;
    private int lastSavedMainValue;
    private int savedMainValuePrePlayView;
    private int lastSavedMinValue;
    private int lastSavedMaxValue;
    private int lastSavedObjectValue;


    private int lastSavedTweenValue;

    public static CustomTimeSlider instance;
    [SerializeField] private Image objectSliderImage;

    public Action<bool> OnEditObjectTime;

    public int currentTweenIndex = 0;

    public bool usingListView = false;

    private float pixelDragFactor;
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
        canvas = GetComponentInParent<Canvas>();

        objectHandle.gameObject.SetActive(false);
        // startTimeHandle.gameObject.SetActive(false);
        mainHandle.localPosition = new Vector2(mainHandle.localPosition.x, 0);
        mainHandleArrow.gameObject.SetActive(false);
        multSelectHighlight.gameObject.SetActive(false);



        // shownLength = normalLength;
        // sliderTrack.sizeDelta = new Vector2(shownLength, sliderTrack.sizeDelta.y);
        // pixelsPerStep = (shownLength / (maxRange - minRange) * canvas.scaleFactor* 1.6f);


        // minHandle.gameObject.SetActive(false);
        // maxHandle.gameObject.SetActive(false);
        // levelLengthObject.SetActive(false);

        AssignHandlers();
        LevelRecordManager.OnShowObjectTime += ShowSelectedObject;


    }




    public void SetVariables(int main, int min, int max)
    {
        currentIndex = main;
        currentMinIndex = min;
        currentMaxIndex = max;
        // minRange = min;
        // maxRange = max;

        UpdateHandlePositions();
        UpdateTexts();



    }

    public void SetMaxTime(int max)
    {
        if (currentMaxIndex >= absoluteMax) currentMaxIndex = max;
        absoluteMax = max;



        if (isPlayView)
        {
            maxRange = absoluteMax;

            pixelsPerStep = (fullLength / (maxRange - minRange) * pixelDragFactor);

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
        multipleObjectHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        multipleObjectHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
        objectTimeDisplay.OnDragged += OnHandleDragged;
        objectTimeDisplay.OnPressed += OnHandlePressed;
        // startTimeHandle.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
        // startTimeHandle.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;

        foreach (var i in multSelecthandles)
        {
            i.GetComponent<SliderHandle>().OnDragged += OnHandleDragged;
            i.GetComponent<SliderHandle>().OnPressed += OnHandlePressed;
            i.gameObject.SetActive(false);
        }



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
        multipleObjectHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        multipleObjectHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        LevelRecordManager.OnShowObjectTime -= ShowSelectedObject;
        objectTimeDisplay.OnDragged -= OnHandleDragged;
        objectTimeDisplay.OnPressed -= OnHandlePressed;
        // startTimeHandle.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
        // startTimeHandle.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        foreach (var i in multSelecthandles)
        {
            i.GetComponent<SliderHandle>().OnDragged -= OnHandleDragged;
            i.GetComponent<SliderHandle>().OnPressed -= OnHandlePressed;
        }
    }

    public void TempTimeEditorView(bool show)
    {
        if (isTimeEditorView) return;
        isTempTimeEditorView = show;
        if (show)
        {
            mainHandle.localScale = Vector3.one * .85f;
            minRange = 0;
            maxRange = absoluteMax;

            sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
            shownLength = fullLength;

            pixelsPerStep = (fullLength / (maxRange - minRange) * pixelDragFactor);
            minHandle.gameObject.SetActive(true);
            maxHandle.gameObject.SetActive(true);
            if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
            {
                multipleObjectHandle.gameObject.SetActive(true);
                SendMultipleObjects();
            }
            else
                objectHandle.gameObject.SetActive(true);

            OrganizeTimeSteps();
            SetAllHandles();
            SetMainHandlePosition(currentIndex);

        }
        else
        {
            mainHandle.localScale = Vector3.one;


            minRange = currentMinIndex;
            maxRange = currentMaxIndex;

            sliderTrack.sizeDelta = new Vector2(normalLength, sliderTrack.sizeDelta.y);
            shownLength = normalLength;

            pixelsPerStep = (shownLength / (maxRange - minRange) * pixelDragFactor);
            minHandle.gameObject.SetActive(false);
            maxHandle.gameObject.SetActive(false);
            objectHandle.gameObject.SetActive(false);
            multipleObjectHandle.gameObject.SetActive(false);
            OrganizeTimeSteps();
            SetShownHandle();
            SetTweenHandlePosition();
            SetMainHandlePosition(currentIndex);



        }


    }
    public void TimeEditorView()
    {

        isTimeEditorView = true;
        // levelLengthObject.SetActive(true);
        // SetObjects();
        DOTween.Kill(this);
        Camera.main.transform.DOMoveY(4f, .8f).SetEase(Ease.OutCubic).SetUpdate(true);
        mainHandle.localScale = Vector3.one * .85f;
        minRange = 0;
        maxRange = absoluteMax;

        sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
        shownLength = fullLength;

        pixelsPerStep = (fullLength / (maxRange - minRange) * pixelDragFactor);
        minHandle.gameObject.SetActive(true);
        maxHandle.gameObject.SetActive(true);
        OrganizeTimeSteps();
        SetMainHandlePosition(currentIndex);

        if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
        {
            multipleObjectHandle.gameObject.SetActive(true);
            SendMultipleObjects();
        }

        else if (LevelRecordManager.instance.currentSelectedObject != null)
            objectHandle.gameObject.SetActive(true);

        SetAllHandles();


    }
    public void SetPixelDragFactor()
    {
        pixelDragFactor = GetComponentInParent<Canvas>().scaleFactor * 1.35f;
    }

    public void NormalView()
    {
        isTimeEditorView = false;

        DOTween.Kill(this);
        // levelLengthObject.SetActive(false);
        Camera.main.transform.DOMoveY(-1.6f, .8f).SetEase(Ease.OutCubic).SetUpdate(true);
        mainHandle.localScale = Vector3.one;


        minRange = currentMinIndex;
        maxRange = currentMaxIndex;

        sliderTrack.sizeDelta = new Vector2(normalLength, sliderTrack.sizeDelta.y);
        shownLength = normalLength;

        pixelsPerStep = (shownLength / (maxRange - minRange) * pixelDragFactor);
        minHandle.gameObject.SetActive(false);
        maxHandle.gameObject.SetActive(false);

        objectHandle.gameObject.SetActive(false);
        multipleObjectHandle.gameObject.SetActive(false);
        OrganizeTimeSteps();
        SetShownHandle();
        SetMainHandlePosition(currentIndex);
        SetTweenHandlePosition();

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
        mainHandle.localPosition = new Vector2(mainHandle.localPosition.x, mainHandleLowY);
        mainHandle.localScale = Vector3.one * .75f;
        lastMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
        mainHandleArrow.gameObject.SetActive(true);
        savedMainValuePrePlayView = currentIndex;
        currentText.gameObject.SetActive(false);
        minRange = 0;
        maxRange = absoluteMax;
        // levelLengthObject.SetActive(false);
        minHandle.gameObject.SetActive(false);
        maxHandle.gameObject.SetActive(false);
        multipleObjectHandle.gameObject.SetActive(false);
        LevelRecordManager.instance.SetUsingMultipleSelect(false);
        LevelRecordManager.instance.SetUsingMultipleSelect(true);

        objectHandle.gameObject.SetActive(false);
        // mainHandle.gameObject.SetActive(false);
        // startTimeHandle.gameObject.SetActive(true);
        shownLength = fullLength;
        int c = currentIndex;
        currentMultSelectLeftIndex = c;
        currentMultSelectRightIndex = c;
        currentMultSelectCenterIndex = c;

        foreach (var r in multSelecthandles)
        {
            r.gameObject.SetActive(true);
        }
        canMultSelect = false;


        pixelsPerStep = (shownLength / (maxRange - minRange) * pixelDragFactor);
        sliderTrack.sizeDelta = new Vector2(fullLength, sliderTrack.sizeDelta.y);
        OrganizeTimeSteps();
        // SetMainHandlePosition(currentIndex);
        SetAllHandles();
        SetMultSelectHighlightLength();
        multSelectHighlight.gameObject.SetActive(true);



    }

    public void ExitPlayView()
    {
        isPlayView = false;
        mainHandle.localPosition = new Vector2(mainHandle.localPosition.x, 0);
        mainHandle.localScale = Vector3.one;
        multSelectHighlight.gameObject.SetActive(false);

        mainHandleArrow.gameObject.SetActive(false);
        currentText.gameObject.SetActive(true);
        foreach (var r in multSelecthandles)
        {
            r.gameObject.SetActive(false);
        }
        LevelRecordManager.instance.SetUsingMultipleSelect(false);


        foreach (var i in objectsUnactiveForPlayView)
        {
            i.SetActive(true);
        }

        // startTimeHandle.gameObject.SetActive(false);
        // mainHandle.gameObject.SetActive(true);
        currentIndex = savedMainValuePrePlayView;
        LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

        if (isTimeEditorView) TimeEditorView();
        else NormalView();




    }


    private bool didDragOnObjectTimeDisplay;

    public bool ChangingObjectTime { get; private set; } = false;
    public bool OverObjectTime { get; private set; } = false;


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

    public void OnSelectTweenIndex(int index)
    {
        for (int i = 1; i < startTweenHandles.Count; i++)
        {
            if (i == index)
            {
                startTweenHandles[i].SetTweenHandle(true);
                endTweenHandles[i].SetTweenHandle(true);
                startTweenHandles[i].GetComponent<RectTransform>().SetParent(selectedTweenParent);
                endTweenHandles[i].GetComponent<RectTransform>().SetParent(selectedTweenParent);

            }
            else
            {
                startTweenHandles[i].SetTweenHandle(false);
                endTweenHandles[i].SetTweenHandle(false);
                startTweenHandles[i].GetComponent<RectTransform>().SetParent(nonSelectedTweenParent);
                endTweenHandles[i].GetComponent<RectTransform>().SetParent(nonSelectedTweenParent);
            }

        }



    }

    public void OnPressTweenTime(bool pressed, bool isHandle, int value)
    {
        lastSavedTweenValue = value;
        lastSavedMainValue = currentIndex;
        if (pressed && !isHandle)
        {
            didDragOnObjectTimeDisplay = false;
        }
        else if (!isHandle)
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



    }
    private bool hasCheckedOtherSteps = false;
    public void OnDragTweenTime(float deltaX, bool isNotHandle)
    {
        int stepChange = Mathf.RoundToInt(deltaX / pixelsPerStep);
        if (stepChange == lastStepChange) return;




        int tweenIndex = lastSavedTweenValue + stepChange;

        if (isNotHandle && !didDragOnObjectTimeDisplay)
        {
            didDragOnObjectTimeDisplay = true;
            TempTimeEditorView(true);
        }





        if (DynamicValueAdder.instance.SetNewTimeStep((ushort)tweenIndex))
        {
            currentTweenStep = tweenIndex;
            currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);
            lastStepChange = stepChange;
            hasCheckedOtherSteps = false;

        }
        else if (!hasCheckedOtherSteps)
        {
            int added = 1;
            bool doReturn = true;

            if (stepChange > 0) added = -1;

            for (int i = 1; i < 50; i++)
            {
                tweenIndex += added;

                if (DynamicValueAdder.instance.SetNewTimeStep((ushort)tweenIndex))
                {
                    stepChange += (i * added);
                    lastStepChange = stepChange;
                    currentTweenStep = tweenIndex;
                    currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);

                    hasCheckedOtherSteps = true;
                    doReturn = false;
                    break;
                }

            }
            if (doReturn) return;


        }

        else return;


        GameObject r = null;
        if (currentTweenIndex > 0)
        {

            r = startTweenHandles[currentTweenIndex].gameObject;
            startTweenHandles[currentTweenIndex].SetTweenStep(currentTweenStep);
        }

        else
        {
            r = endTweenHandles[Mathf.Abs(currentTweenIndex)].gameObject;
            endTweenHandles[Mathf.Abs(currentTweenIndex)].SetTweenStep(currentTweenStep);

        }

        LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

        SetHandlePos(r, currentTweenStep);
        UpdateHandlePositions();

        UpdateTexts();




    }
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

                if (!pressed) LevelRecordManager.instance.PreloadPlayScene();
                break;

            case SliderHandle.HandleType.Min:
                lastSavedMinValue = currentMinIndex;
                if (pressed)
                {
                    minText.gameObject.SetActive(true);
                    LevelRecordManager.instance.FadeTitleText(true);
                }
                else
                {
                    minText.gameObject.SetActive(false);
                    LevelRecordManager.instance.FadeTitleText(false);
                }

                break;

            case SliderHandle.HandleType.Max:
                lastSavedMaxValue = currentMaxIndex;
                if (pressed)
                {
                    maxText.gameObject.SetActive(true);
                    LevelRecordManager.instance.FadeTitleText(true);
                }

                else
                {
                    maxText.gameObject.SetActive(false);
                    LevelRecordManager.instance.FadeTitleText(false);
                }


                break;

            case SliderHandle.HandleType.AbsoluteMax:
                break;
            case SliderHandle.HandleType.Object:
            case SliderHandle.HandleType.Time:
            case SliderHandle.HandleType.MultipleObject:
                LevelRecordManager.instance.DoingTimeEdit(pressed);
                ChangingObjectTime = pressed;
                OverObjectTime = false;
                lastSavedObjectValue = currentObjectIndex;
                lastSavedMainValue = currentIndex;
                objSpawnText.gameObject.SetActive(pressed);
                OnEditObjectTime?.Invoke(pressed);

                if (pressed)
                {
                    if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
                    {
                        LevelRecordManager.instance.SetChangeMultipleObjectSpawnStep(true);
                        SendMultipleObjects();
                    }


                    LevelRecordManager.instance.FadeTitleText(true);
                }

                else
                {
                    if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
                        LevelRecordManager.instance.SetChangeMultipleObjectSpawnStep(false);
                    LevelRecordManager.instance.FadeTitleText(false);
                }
                break;

            case SliderHandle.HandleType.MultSelectCenter:
                if (!pressed)
                {
                    if (canMultSelect)
                        LevelRecordManager.instance.SetMultipleSelected(currentMultSelectLeftIndex, currentMultSelectRightIndex);
                }
                else
                {

                    lastMultSelectCenterIndex = GetAverageStepForMultipleObjects();
                    currentIndex = lastMultSelectCenterIndex;
                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);


                }
                break;
            case SliderHandle.HandleType.MultSelectLeft:
                if (!pressed)
                {
                    currentMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
                    SetAllHandles();

                    if (canMultSelect)
                        LevelRecordManager.instance.SetMultipleSelected(currentMultSelectLeftIndex, currentMultSelectRightIndex);
                }
                else
                {
                    lastMultSelectLeftIndex = currentMultSelectLeftIndex;
                    Debug.Log("Setting Last Left Index to: " + lastMultSelectLeftIndex);
                    currentIndex = lastMultSelectLeftIndex;
                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);



                }

                break;
            case SliderHandle.HandleType.MultSelectRight:
                if (!pressed)
                {
                    currentMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
                    SetAllHandles();
                    if (canMultSelect)
                        LevelRecordManager.instance.SetMultipleSelected(currentMultSelectLeftIndex, currentMultSelectRightIndex);
                }
                else
                {
                    lastMultSelectRightIndex = currentMultSelectRightIndex;
                    Debug.Log("Setting Last Right Index to: " + lastMultSelectRightIndex);
                    currentIndex = lastMultSelectRightIndex;
                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);




                }

                break;

            case SliderHandle.HandleType.MultSelectMove:

                LevelRecordManager.instance.DoingTimeEdit(pressed);
                if (pressed)
                {

                    lastMultSelectRightIndex = currentMultSelectRightIndex;
                    lastMultSelectLeftIndex = currentMultSelectLeftIndex;
                    lastMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
                    LevelRecordManager.instance.SetChangeMultipleObjectSpawnStep(true);
                    currentIndex = lastMultSelectCenterIndex;
                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                    LevelRecordManager.instance.FadeTitleText(true);
                }

                else
                {
                    lastMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);

                    LevelRecordManager.instance.SetChangeMultipleObjectSpawnStep(false);
                    LevelRecordManager.instance.FadeTitleText(false);
                }

                break;

        }



        UpdateTexts();
    }

    private void OnHandleDragged(SliderHandle.HandleType type, float deltaX)
    {



        int stepChange = Mathf.RoundToInt(deltaX / pixelsPerStep);
        if (stepChange == lastStepChange) return;
        lastStepChange = stepChange;

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
            case SliderHandle.HandleType.MultipleObject:

                currentMulipleObjectIndex = lastSavedAverageMultipleStep + stepChange;
                if (currentMulipleObjectIndex < multipleSelectMinMaxChange.x)
                {
                    currentMulipleObjectIndex = multipleSelectMinMaxChange.x;
                    return;


                }
                else if (currentMulipleObjectIndex > multipleSelectMinMaxChange.y)
                {
                    currentMulipleObjectIndex = multipleSelectMinMaxChange.y;
                    return;
                }
                LevelRecordManager.instance.UpdateMultipleObjectsTime((lastSavedAverageMultipleStep - currentMulipleObjectIndex) * -1);


                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);

                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                break;
            case SliderHandle.HandleType.Time:
                if (!didDragOnObjectTimeDisplay)
                {
                    didDragOnObjectTimeDisplay = true;
                    TempTimeEditorView(true);
                }

                if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
                {
                    currentMulipleObjectIndex = lastSavedAverageMultipleStep + stepChange;
                    if (currentMulipleObjectIndex < multipleSelectMinMaxChange.x)
                    {
                        currentMulipleObjectIndex = multipleSelectMinMaxChange.x;
                        return;


                    }
                    else if (currentMulipleObjectIndex > multipleSelectMinMaxChange.y)
                    {
                        currentMulipleObjectIndex = multipleSelectMinMaxChange.y;
                        return;
                    }
                    LevelRecordManager.instance.UpdateMultipleObjectsTime((lastSavedAverageMultipleStep - currentMulipleObjectIndex) * -1);


                    currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);

                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                }
                else
                {
                    currentObjectIndex = Mathf.Clamp(lastSavedObjectValue + stepChange, minRange, maxRange);

                    currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);


                    LevelRecordManager.instance.UpdateObjectTime(currentObjectIndex);
                    LevelRecordManager.instance.UpdateTime((ushort)currentIndex);
                }




                break;

            case SliderHandle.HandleType.StartTime:

                currentIndex = Mathf.Clamp(lastSavedMainValue + stepChange, minRange, maxRange);
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex, true);
                SetAllHandles();
                return;

                break;

            case SliderHandle.HandleType.MultSelectCenter:
                currentMultSelectCenterIndex = lastMultSelectCenterIndex + stepChange;
                if (currentMultSelectCenterIndex < multipleSelectMinMaxChange.x)
                {
                    currentMultSelectCenterIndex = multipleSelectMinMaxChange.x;



                }
                else if (currentMultSelectCenterIndex > multipleSelectMinMaxChange.y)
                {
                    currentMultSelectCenterIndex = multipleSelectMinMaxChange.y;


                }
                else
                {
                    currentMultSelectLeftIndex = lastMultSelectLeftIndex + stepChange;
                    currentMultSelectRightIndex = lastMultSelectRightIndex + stepChange;
                }


                currentIndex = currentMultSelectCenterIndex;
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                break;
            case SliderHandle.HandleType.MultSelectLeft:

                currentMultSelectLeftIndex = Mathf.Clamp(lastMultSelectLeftIndex + stepChange, 0, currentMultSelectRightIndex);
                Debug.Log("Setting left handle, last val: " + lastMultSelectLeftIndex + " - Current Value: " + currentMultSelectLeftIndex + " - Step Change: " + stepChange);
                currentIndex = currentMultSelectLeftIndex;
                currentMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
                if (!canMultSelect && currentMultSelectLeftIndex != currentMultSelectRightIndex) canMultSelect = true;

                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                break;
            case SliderHandle.HandleType.MultSelectRight:

                currentMultSelectRightIndex = Mathf.Clamp(lastMultSelectRightIndex + stepChange, currentMultSelectLeftIndex, LevelRecordManager.instance.finalSpawnStep);
                Debug.Log("Setting right handle, last val: " + lastMultSelectRightIndex + " - Current Value: " + currentMultSelectRightIndex + " - Step Change: " + stepChange);

                currentIndex = currentMultSelectRightIndex;
                currentMultSelectCenterIndex = GetAverageStepForMultipleObjects(true);
                if (!canMultSelect && currentMultSelectLeftIndex != currentMultSelectRightIndex) canMultSelect = true;
                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);

                break;

            case SliderHandle.HandleType.MultSelectMove:

                currentMultSelectCenterIndex = lastMultSelectCenterIndex + stepChange;
                if (currentMultSelectCenterIndex < multipleSelectMinMaxChange.x)
                {
                    currentMultSelectCenterIndex = multipleSelectMinMaxChange.x;



                }
                else if (currentMultSelectCenterIndex > multipleSelectMinMaxChange.y)
                {
                    currentMultSelectCenterIndex = multipleSelectMinMaxChange.y;


                }
                else
                {
                    currentMultSelectLeftIndex = lastMultSelectLeftIndex + stepChange;
                    currentMultSelectRightIndex = lastMultSelectRightIndex + stepChange;
                }


                currentIndex = currentMultSelectCenterIndex;
                LevelRecordManager.instance.UpdateMultipleObjectsTime((lastMultSelectCenterIndex - currentMultSelectCenterIndex) * -1);

                LevelRecordManager.instance.UpdateTime((ushort)currentIndex);



                break;



        }


        UpdateHandlePositions();
        SetMultSelectHighlightLength();
        UpdateTexts();
    }

    public void SendMultipleObjects()
    {
        if (!multipleObjectHandle.gameObject.activeInHierarchy && LevelRecordManager.ShowTime)
        {
            multipleObjectHandle.gameObject.SetActive(true);

        }
        lastSavedAverageMultipleStep = GetAverageStepForMultipleObjects();
        currentMulipleObjectIndex = lastSavedAverageMultipleStep;

        multipleObjectHandle.localPosition = new Vector2(GetSliderX(lastSavedAverageMultipleStep), multipleObjectHandle.localPosition.y);

        if (objectHandle.gameObject.activeInHierarchy)
        {
            objectHandle.gameObject.SetActive(false);
        }

    }
    public void ExitMultipleObjectSelection()
    {
        if (multipleObjectHandle != null)
        {
            multipleObjectHandle.gameObject.SetActive(false);
        }

    }

    private int GetAverageStepForMultipleObjects(bool isMultSelect = false)
    {
        int stepTracker = 0;

        int minS = LevelRecordManager.instance.finalSpawnStep;
        int maxS = 0;


        foreach (var obj in LevelRecordManager.instance.MultipleSelectedObjects)
        {
            int val = (int)obj.Data.spawnedStep;
            if (val < minS) minS = val;
            if (val > maxS) maxS = val;
            stepTracker += val;

        }
        int averageStep = 0;

        if (LevelRecordManager.instance.MultipleSelectedObjects.Count == 0 && isMultSelect)
        {
            averageStep = currentIndex;
        }
        if (isMultSelect)
        {
            averageStep = (int)((currentMultSelectLeftIndex + currentMultSelectRightIndex) * .5f);
            multipleSelectMinMaxChange.x = averageStep - currentMultSelectLeftIndex;
            multipleSelectMinMaxChange.y = LevelRecordManager.instance.finalSpawnStep - (currentMultSelectRightIndex - averageStep);
        }
        else
        {
            averageStep = stepTracker / LevelRecordManager.instance.MultipleSelectedObjects.Count;
            multipleSelectMinMaxChange.x = averageStep - minS;
            multipleSelectMinMaxChange.y = LevelRecordManager.instance.finalSpawnStep - (maxS - averageStep);
        }










        Debug.Log("Average Step: " + averageStep + " Min: " + multipleSelectMinMaxChange.x + " Max: " + multipleSelectMinMaxChange.y);



        return averageStep;
    }

    private void ShowSelectedObject(int step)
    {
        if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
        {
            SendMultipleObjects();
            return;
        }
        if (!objectHandle.gameObject.activeInHierarchy && LevelRecordManager.ShowTime)
        {
            objectHandle.gameObject.SetActive(true);

        }

        if (multipleObjectHandle.gameObject.activeInHierarchy) multipleObjectHandle.gameObject.SetActive(false);
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

        // mainHandle.localPosition = new Vector2(GetSliderX(currentIndex), mainHandle.localPosition.y);
        objectHandle.localPosition = new Vector2(GetSliderX(currentObjectIndex), objectHandle.localPosition.y);
        multipleObjectHandle.localPosition = new Vector2(GetSliderX(currentMulipleObjectIndex), multipleObjectHandle.localPosition.y);

        // float x = offset
    }

    public void UpdateMainHandlePosition(int i)
    {
        currentIndex = i;
        UpdateHandlePositions();
        // SetTweenHandlePosition();


    }
    public void SetMainHandlePosition(int i)
    {
        currentIndex = i;
        mainHandle.localPosition = new Vector2(GetSliderX(currentIndex), mainHandle.localPosition.y);
        UpdateTexts();
    }
    private void SetAllHandles()
    {
        // mainHandle.localPosition = new Vector2(GetSliderX(currentIndex), mainHandle.localPosition.y);
        // startTimeHandle.localPosition = new Vector2(GetSliderX(currentIndex), startTimeHandle.localPosition.y);
        minHandle.localPosition = new Vector2(GetSliderX(currentMinIndex), minHandle.localPosition.y);
        maxHandle.localPosition = new Vector2(GetSliderX(currentMaxIndex), maxHandle.localPosition.y);
        objectHandle.localPosition = new Vector2(GetSliderX(currentObjectIndex), objectHandle.localPosition.y);
        multipleObjectHandle.localPosition = new Vector2(GetSliderX(currentMulipleObjectIndex), multipleObjectHandle.localPosition.y);
        multSelecthandles[0].localPosition = new Vector2(GetSliderX(currentMultSelectCenterIndex), multSelecthandles[0].localPosition.y);
        multSelecthandles[1].localPosition = new Vector2(GetSliderX(currentMultSelectCenterIndex), multSelecthandles[1].localPosition.y);
        multSelecthandles[2].localPosition = new Vector2(GetSliderX(currentMultSelectLeftIndex), multSelecthandles[2].localPosition.y);
        multSelecthandles[3].localPosition = new Vector2(GetSliderX(currentMultSelectRightIndex), multSelecthandles[3].localPosition.y);

        SetTweenHandlePosition();




    }



    private float GetSliderX(int index)
    {
        float l = Mathf.InverseLerp(minRange, maxRange, index);
        float offset = l * (shownLength - 40);
        return ((-shownLength + 40) * .5f) + offset;



    }

    public void CreateTweenHandles(RecordedDataStructTweensDynamic data)
    {
        // startTweenHandles.Clear();
        // endTweenHandles.Clear();

        for (int i = 0; i < data.startingSteps.Count; i++)
        {
            SliderHandle handleStart;
            SliderHandle handleEnd;

            if (i < startTweenHandles.Count)
            {
                handleStart = startTweenHandles[i];

                handleEnd = endTweenHandles[i];

            }
            else
            {
                var obj = Instantiate(startTweenObject, transform);
                obj.GetComponent<RectTransform>().localPosition = new Vector2(0, -92);

                handleStart = obj.GetComponent<SliderHandle>();
                startTweenHandles.Add(handleStart);

                var obj2 = Instantiate(endTweenObject, transform);
                obj2.GetComponent<RectTransform>().localPosition = new Vector2(0, -92);
                handleEnd = obj2.GetComponent<SliderHandle>();
                endTweenHandles.Add(handleEnd);


            }

            handleStart.SetTweenIndex(i);
            handleEnd.SetTweenIndex(i);
            handleStart.SetTweenStep(data.startingSteps[i]);
            handleEnd.SetTweenStep(data.endingSteps[i]);

            handleStart.gameObject.SetActive(false);
            handleEnd.gameObject.SetActive(false);

            // SetHandlePos(handleStart.gameObject, data.startingSteps[i]);
            // SetHandlePos(handleEnd.gameObject, data.endingSteps[i]);

        }
        if (startTweenHandles.Count > data.startingSteps.Count)
        {
            for (int i = startTweenHandles.Count - 1; i >= data.startingSteps.Count; i--)
            {
                Destroy(startTweenHandles[i].gameObject);
                startTweenHandles.RemoveAt(i);
            }
            for (int i = endTweenHandles.Count - 1; i >= data.endingSteps.Count; i--)
            {
                Destroy(endTweenHandles[i].gameObject);
                endTweenHandles.RemoveAt(i);
            }
        }

        SetTweenHandlePosition();

    }

    public void DeactivateTweenHandles()
    {
        usingListView = false;
        foreach (var i in startTweenHandles)
        {
            i.gameObject.SetActive(false);
        }
        foreach (var i in endTweenHandles)
        {
            i.gameObject.SetActive(false);
        }
    }


    public void SetTweenHandlePosition(int change = 0)
    {
        if (startTweenHandles.Count <= 1) return;
        if ((isTimeEditorView || isTempTimeEditorView) && usingListView)
        {
            foreach (var i in startTweenHandles)
            {
                SetHandlePos(i.gameObject, i.GetStep());
                i.gameObject.SetActive(true);
            }

            foreach (var i in endTweenHandles)
            {
                SetHandlePos(i.gameObject, i.GetStep());
                i.gameObject.SetActive(true);
            }
        }
        else if (startTweenHandles[startTweenHandles.Count - 1].gameObject.activeInHierarchy)
        {
            foreach (var i in startTweenHandles)
            {
                i.gameObject.SetActive(false);
            }

            foreach (var i in endTweenHandles)
            {
                i.gameObject.SetActive(false);
            }
        }
        startTweenHandles[0].gameObject.SetActive(false);
        endTweenHandles[0].gameObject.SetActive(false);


    }

    private void SetMultSelectHighlightLength()
    {
        if (!isPlayView) return;
        multSelectHighlight.position = new Vector2(multSelecthandles[2].position.x - 15, multSelecthandles[2].position.y);
        multSelectHighlight.sizeDelta = new Vector2(multSelecthandles[3].position.x - multSelecthandles[2].position.x + 30, multSelectHighlight.sizeDelta.y);
    }

    private void SetHandlePos(GameObject obj, int index)
    {

        float x = GetSliderX(index);

        obj.transform.localPosition = new Vector2(x, obj.transform.localPosition.y);
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