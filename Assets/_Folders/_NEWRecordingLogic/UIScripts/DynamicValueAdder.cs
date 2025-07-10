using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DynamicValueAdder : MonoBehaviour
{
    public static DynamicValueAdder instance;
    [SerializeField] private DynamicValueEditor editorPrefab;
    public RecordedDataStructTweensDynamic data;

    [SerializeField] private PlusMinusUI[] plusMinusUI;
    [SerializeField] private CanvasGroup editorCanvasGroup;

    private int maxIntValueRotation = 20;

    private int currentIntValue = 0;

    [Header("Scroll Settings")]
    [SerializeField] private RectTransform upperListLayoutGroup;
    [SerializeField] private Button scrollDownButton;
    [SerializeField] private Button scrollUpButton;
    [SerializeField] private float baseValuePanelHeight = -15;
    [SerializeField] private float additionalAddedSize;
    private float heightPerValueEditor;
    private int numberOfValues = 0;
    private int amountOfValuesByType;
    private int currentScrollValue;
    private Sequence scrollSequence;


    [SerializeField] private RectTransform upperListPanel;
    [SerializeField] private RectTransform lowerListPanel;

    [SerializeField] private int laserRotationEditorSize;
    [SerializeField] private int laserPositionEditorSize;
    [SerializeField] private int laserShootingEditorSize;

    [SerializeField] private GameObject positionerObjectPrefab;

    private List<PositionerObject> positionerObjects = new List<PositionerObject>();


    [SerializeField] private Transform editorParent;

    [SerializeField] private TextMeshProUGUI titleText;

    private float currentAdditionalValue;

    [SerializeField] private string title;
    private bool allowingEdits = false;

    private List<ushort> startingSteps;
    private List<ushort> endingSteps;

    [SerializeField] private GameObject plusMinusObject;
    [SerializeField] private TextMeshProUGUI plusMinusText;
    [SerializeField] private LevelDataEditors floatEditor;

    [SerializeField] private TextMeshProUGUI startTimerText;
    [SerializeField] private TextMeshProUGUI endTimerText;

    private string[] rotationDirectionString = new string[] { "Clockwise", "Counter-Clockwise" };
    private string[] easeTypeString = new string[] { "Ease: None", "Ease: In", "Ease: Out", "Ease: In/Out" };

    private bool isPostion;
    private int minStep = 0;
    private int maxStep = 0;
    private float currentReturnedFloatValue = 0f;
    private string Type;

    private bool editingStartTime = true;



    public enum DynamicValueType
    {
        Rotations,
        Positions,
        Timers
    }

    public DynamicValueType type;

    private List<DynamicValueEditor> dynamicValueEditors = new List<DynamicValueEditor>();
    private int currentSelectedValue = -1;

    [SerializeField] private ObjectTypeEditor[] typeEditors;
    [SerializeField] private GameObject positionCordinatesObject;
    [SerializeField] private TextMeshProUGUI positionCordinatesX;
    [SerializeField] private TextMeshProUGUI positionCordinatesY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


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

        upperListLayoutGroup.anchoredPosition = new Vector2(upperListPanel.anchoredPosition.x, baseValuePanelHeight);
        heightPerValueEditor = editorPrefab.GetComponent<RectTransform>().sizeDelta.y + upperListLayoutGroup.GetComponent<VerticalLayoutGroup>().spacing;


    }

    void Start()
    {

        editorPrefab.gameObject.SetActive(false);
        currentSelectedValue = -1;
        foreach (var u in plusMinusUI)
        {
            u.ChangeValue += EditInt;
        }

        Deactivate();
    }

    public void Scroll(bool down)
    {
        if (down)
        {
            currentScrollValue++;
            scrollUpButton.interactable = true;

            if (currentScrollValue >= numberOfValues) scrollDownButton.interactable = false;

        }
        else
        {
            currentScrollValue--;
            scrollDownButton.interactable = true;

            if (currentScrollValue <= amountOfValuesByType) scrollUpButton.interactable = false;
        }

        if (scrollSequence != null && scrollSequence.IsPlaying())
        {
            scrollSequence.Kill();
        }
        scrollSequence = DOTween.Sequence();

        float y = ((currentScrollValue - amountOfValuesByType) * heightPerValueEditor) + baseValuePanelHeight;
        scrollSequence.Append(upperListLayoutGroup.DOAnchorPosY(y, 0.3f));
        scrollSequence.Play().SetUpdate(true);

    }

    private void ScrollToEnd()
    {

        currentScrollValue = numberOfValues;
        if (currentScrollValue > amountOfValuesByType)
        {
            if (scrollSequence != null && scrollSequence.IsPlaying())
            {
                scrollSequence.Kill();
            }
            scrollSequence = DOTween.Sequence();

            float y = ((currentScrollValue - amountOfValuesByType) * heightPerValueEditor) + baseValuePanelHeight;
            scrollSequence.Append(upperListLayoutGroup.DOAnchorPosY(y, 0.4f));
            scrollSequence.Play().SetUpdate(true);

            // upperListLayoutGroup.anchoredPosition = new Vector2(upperListLayoutGroup.anchoredPosition.x, y);
            scrollDownButton.interactable = false;
            scrollUpButton.interactable = true;
        }
        else
        {
            upperListLayoutGroup.anchoredPosition = new Vector2(upperListLayoutGroup.anchoredPosition.x, baseValuePanelHeight);
            scrollDownButton.interactable = false;
            scrollUpButton.interactable = false;
        }



    }

    public void CheckForScroll()
    {
        if (numberOfValues > amountOfValuesByType)
        {
            Scroll(true);

        }

    }

    private void AllowEdits(bool allow)
    {
        Debug.Log("Allowing edits: " + allow);
        allowingEdits = allow;
        if (allow)
        {
            editorCanvasGroup.alpha = 1f;
            editorCanvasGroup.interactable = true;
            editorCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            editorCanvasGroup.alpha = 0.4f;
            editorCanvasGroup.interactable = false;
            editorCanvasGroup.blocksRaycasts = false;
        }
    }


    public void SetUpperEditorSize(int size)
    {

        upperListPanel.sizeDelta = new Vector2(upperListPanel.sizeDelta.x, (size * heightPerValueEditor) + additionalAddedSize);
        // lowerListPanel.anchoredPosition = new Vector2(lowerListPanel.anchoredPosition.x, size.y);
    }


    public void Activate(string type, RecordedDataStructTweensDynamic data)
    {
        if (data == null)
        {
            Debug.LogError("Data is null");
            return;
        }
        Debug.LogError("Activating List: " + type);
        titleText.text = type;
        isPostion = false;
        this.data = data;
        dynamicValueEditors.Clear();
        dynamicValueEditors.Add(editorPrefab);
        currentSelectedValue = -1;
        Type = type;
        AllowEdits(false);
        switch (type)
        {
            case "Rotations":
                this.type = DynamicValueType.Rotations;
                plusMinusObject.SetActive(true);
                floatEditor.gameObject.SetActive(true);
                // typeEditors[0].gameObject.SetActive(true);
                // typeEditors[1].SetData("Rotations", rotationDirectionString, data.other[0]);
                typeEditors[0].gameObject.SetActive(true);
                typeEditors[0].SetData("Ease", easeTypeString, data.easeTypes[0]);
                typeEditors[1].gameObject.SetActive(false);

                plusMinusText.text = "Rotations: 0";
                Debug.LogError("Setting type to rotations");
                floatEditor.SetValueForListSlider(0, false);
                positionCordinatesObject.SetActive(false);

                amountOfValuesByType = 4;
                // SetUpperEditorSize(laserRotationEditorSize);
                break;
            case "Positions":
                this.type = DynamicValueType.Positions;
                plusMinusObject.SetActive(false);
                floatEditor.gameObject.SetActive(false);
                typeEditors[0].gameObject.SetActive(true);
                typeEditors[0].SetData("Ease", easeTypeString, data.easeTypes[0]);
                positionCordinatesObject.SetActive(true);


                typeEditors[1].gameObject.SetActive(false);
                amountOfValuesByType = 5;
                isPostion = true;

                CreatePostionerObjects();
                // SetUpperEditorSize(laserPositionEditorSize);

                LevelRecordManager.instance.editingPostionerObject = true;
                // LevelRecordManager.instance.SetUsingMultipleSelect(false);


                break;
            case "Timers":
                this.type = DynamicValueType.Timers;

                plusMinusObject.SetActive(false);
                floatEditor.gameObject.SetActive(false);
                typeEditors[0].gameObject.SetActive(false);
                positionCordinatesObject.SetActive(false);


                typeEditors[1].gameObject.SetActive(false);
                isPostion = false;
                amountOfValuesByType = 7;

                // SetUpperEditorSize(laserShootingEditorSize);

                break;

        }
        numberOfValues = data.startingSteps.Count - 1;
        SetUpperEditorSize(amountOfValuesByType);

        for (int i = 1; i < data.startingSteps.Count; i++)
        {
            AddDynamicValue(false);


        }
        ScrollToEnd();
        Debug.Log("Activaing List: " + type);
        CustomTimeSlider.instance.usingListView = true;
        CustomTimeSlider.instance.CreateTweenHandles(data);

        SetTimerTexts(0);

        gameObject.SetActive(true);
        SetCurrentSelectedValue(0);


    }

    private void CreatePostionerObjects()
    {
        if (positionerObjects.Count <= 0)
        {
            var s = Instantiate(positionerObjectPrefab).GetComponent<PositionerObject>();
            positionerObjects.Add(s);
            s.Activate(0, data.positions[0]);

        }
        else
        {
            positionerObjects[0].Activate(0, data.positions[0]);

        }

        for (int i = 1; i < data.positions.Count; i++)
        {
            if (i < positionerObjects.Count)
            {
                positionerObjects[i].Activate(i, data.positions[i]);
            }
            else
            {
                var s = Instantiate(positionerObjectPrefab).GetComponent<PositionerObject>();
                positionerObjects.Add(s);
                s.Activate(i, data.positions[i]);
            }

        }

        if (positionerObjects.Count > positionerObjects.Count)
        {
            for (int i = positionerObjects.Count; i >= positionerObjects.Count; i--)
            {
                if (i != 0)
                {
                    Destroy(positionerObjects[i].gameObject);
                    positionerObjects.RemoveAt(i);
                }

            }
        }

    }

    public void UpdateSpawnTimeData()
    {

        if (currentSelectedValue > 0)
        {
            SetTimerTexts(currentSelectedValue);
            dynamicValueEditors[currentSelectedValue].EditTimeFromSpawnTime();
        }

    }

    public void StartTimeStepEdit(bool start, bool handle)
    {
        editingStartTime = start;

        int c = currentSelectedValue;

        int currentStep = 0;

        if (!start) c *= -1;

        CustomTimeSlider.instance.currentTweenIndex = c;




        switch (type)
        {
            case DynamicValueType.Rotations:
            case DynamicValueType.Positions:
                if (start)
                {
                    minStep = data.endingSteps[currentSelectedValue - 1];
                    maxStep = data.endingSteps[currentSelectedValue] - 1;
                    currentStep = data.startingSteps[currentSelectedValue];
                }
                else
                {
                    minStep = data.startingSteps[currentSelectedValue] + 1;
                    if (currentSelectedValue + 1 >= data.startingSteps.Count)
                        maxStep = LevelRecordManager.instance.finalSpawnStep - 1;
                    else
                        maxStep = data.startingSteps[currentSelectedValue + 1];

                    currentStep = data.endingSteps[currentSelectedValue];
                }
                break;


            case DynamicValueType.Timers:


                if (start)
                {
                    minStep = data.endingSteps[currentSelectedValue - 1];
                    maxStep = data.endingSteps[currentSelectedValue] - 1 - Mathf.RoundToInt(LaserParent.cooldownDuration / LevelRecordManager.TimePerStep);
                    currentStep = data.startingSteps[currentSelectedValue];
                }
                else
                {
                    minStep = data.startingSteps[currentSelectedValue] + 1 + Mathf.RoundToInt(LaserParent.cooldownDuration / LevelRecordManager.TimePerStep);
                    if (currentSelectedValue + 1 >= data.startingSteps.Count)
                        maxStep = LevelRecordManager.instance.finalSpawnStep - 1;
                    else
                        maxStep = data.startingSteps[currentSelectedValue + 1];

                    currentStep = data.endingSteps[currentSelectedValue];
                }
                break;

        }

        CustomTimeSlider.instance.OnPressTweenTime(true, handle, currentStep);

    }


    public bool SetNewTimeStep(ushort newStep)
    {

        if (newStep < minStep || newStep > maxStep)
        {

            return false;
        }


        if (editingStartTime)
        {
            data.startingSteps[currentSelectedValue] = newStep;
        }
        else
        {

            data.endingSteps[currentSelectedValue] = newStep;
        }
        if (currentSelectedValue >= 0)
        {
            SetTimerTexts(currentSelectedValue);
            dynamicValueEditors[currentSelectedValue].EditTime(newStep, editingStartTime);
        }


        return true;



    }

    public void SetTimerTexts(int i)
    {
        startTimerText.text = "Start Time: " + LevelRecordManager.instance.FormatTimerText(data.startingSteps[i]);
        endTimerText.text = "End Time: " + LevelRecordManager.instance.FormatTimerText(data.endingSteps[i]);
    }

    private void EditInt(int change)
    {
        switch (titleText.text)
        {
            case "Rotations":

                currentIntValue += change;
                plusMinusText.text = "Rotations: " + currentIntValue.ToString();
                int value = (currentIntValue * -360) + (int)currentAdditionalValue;
                dynamicValueEditors[currentSelectedValue].EditValue(value);



                break;
            case "Positions":
                this.type = DynamicValueType.Positions;
                isPostion = true;

                break;
            case "Timers":
                this.type = DynamicValueType.Timers;
                break;

        }


    }

    public void EditPositionerObject(Vector2 pos)
    {
        Debug.Log("Trying to move index: " + currentSelectedValue);
        positionerObjects[currentSelectedValue].MovePosition(pos);

        data.positions[currentSelectedValue] = pos;
        dynamicValueEditors[currentSelectedValue].EditPostionText(pos);
        positionCordinatesX.text = pos.x.ToString("F1");
        positionCordinatesY.text = pos.y.ToString("F1");
    }


    public void EditTypeValue(string type, int index)
    {
        switch (type)
        {
            case "Ease":
                data.easeTypes[currentSelectedValue] = (ushort)index;

                break;
                // case "RotationDirection":
                //     data.other[currentSelectedValue] = (ushort)index;

                //     break;


        }

        LevelRecordManager.instance.UpdateTime(LevelRecordManager.CurrentTimeStep);
    }

    public int ReturnIntValue()
    {
        if (type == DynamicValueType.Rotations)
        {
            return Mathf.RoundToInt(data.values[currentSelectedValue]);
        }

        return 0;

    }

    public float ReturnFloatValue()
    {

        if (type == DynamicValueType.Positions)
        {
            return data.positions[currentSelectedValue].x;
        }
        else if (type == DynamicValueType.Rotations)
        {

            return currentReturnedFloatValue;

        }
        else if (type == DynamicValueType.Timers)
        {
            return data.values[currentSelectedValue];
        }
        else
            return 0;

    }

    public void EditFloat(float change)
    {

        currentAdditionalValue = change;
        currentReturnedFloatValue = change;

        int value = (currentIntValue * -360) + (int)currentAdditionalValue;
        dynamicValueEditors[currentSelectedValue].EditValue(value);

    }

    public void Deactivate()
    {
        if (scrollSequence != null && scrollSequence.IsPlaying())
        {
            scrollSequence.Kill();
        }

        for (int i = 1; i < dynamicValueEditors.Count; i++)
        {
            Destroy(dynamicValueEditors[i].gameObject);
        }
        dynamicValueEditors.Clear();

        for (int i = 0; i < positionerObjects.Count; i++)
        {
            positionerObjects[i].gameObject.SetActive(false);
        }

        currentSelectedValue = -1;

        CustomTimeSlider.instance.DeactivateTweenHandles();
        LevelRecordManager.instance.editingPostionerObject = false;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        if (scrollSequence != null && scrollSequence.IsPlaying())
        {
            scrollSequence.Kill();
        }
        foreach (var u in plusMinusUI)
        {
            u.ChangeValue -= EditInt;
        }
    }

    public void SetCurrentSelectedValue(int index, bool skipUnselect = false)
    {

        if (currentSelectedValue != index && currentSelectedValue >= 0 && !skipUnselect)
            dynamicValueEditors[currentSelectedValue].SetUnselected();
        if (currentSelectedValue == index) return;
        currentSelectedValue = index;

        if (!allowingEdits && index > 0)
        {
            AllowEdits(true);
        }

        switch (type)
        {
            case DynamicValueType.Rotations:
                isPostion = false;
                float v = LevelRecordManager.instance.currentSelectedObject.rotationData.values[index];
                int whole = 0;
                float f = 0;
                if (v > 0)
                {
                    if ((v % 360) == 0)
                    {
                        whole = (int)v / -360;
                        f = 0f;
                    }
                    else
                    {
                        int i = (int)v / -360;
                        whole = i - 1;
                        f = v + (whole * 360);
                    }
                }
                else
                {
                    whole = (int)v / -360;
                    f = v % 360;
                }
                currentIntValue = whole;
                currentReturnedFloatValue = f;
                plusMinusText.text = "Rotations: " + whole.ToString();
                floatEditor.SetValueForListSlider(f, true);
                // typeEditors[0].SetData("RotationDirection", rotationDirectionString, data.other[index]);
                typeEditors[0].SetData("Ease", easeTypeString, data.easeTypes[index]);



                break;
            case DynamicValueType.Positions:
                isPostion = true;



                foreach (var p in positionerObjects)
                {
                    p.SetSelectedIndex(index);
                }
                typeEditors[0].SetData("Ease", easeTypeString, data.easeTypes[index]);
                LevelRecordManager.instance.currentSelectedPostionerObjectIndex = index;
                LevelRecordManager.instance.currentSelectedPostionerObject = positionerObjects[index];

                positionCordinatesX.text = data.positions[currentSelectedValue].x.ToString("F1");
                positionCordinatesY.text = data.positions[currentSelectedValue].y.ToString("F1");




                break;
            case DynamicValueType.Timers:
                isPostion = false;
                break;

        }

        dynamicValueEditors[index].SetSelected();

        SetTimerTexts(currentSelectedValue);
        CustomTimeSlider.instance.OnSelectTweenIndex(index);





    }

    public void RemoveDynamicValue()
    {
        int index = currentSelectedValue;
        if (index >= dynamicValueEditors.Count || index < 0 || dynamicValueEditors.Count <= 0)
        {
            return;

        }

        var editor = dynamicValueEditors[index];
        dynamicValueEditors.Remove(editor);
        Destroy(editor.gameObject);
        data.RemoveAt(index);
        currentSelectedValue = -1;
        for (int i = 0; i < positionerObjects.Count; i++)
        {
            positionerObjects[i].gameObject.SetActive(false);
        }
        for (int i = 1; i < dynamicValueEditors.Count; i++)
        {
            Destroy(dynamicValueEditors[i].gameObject);
        }
        Activate(titleText.text, data);

    }



    public void AddDynamicValue(bool isNew)
    {

        var editor = Instantiate(editorPrefab, editorParent);
        dynamicValueEditors.Add(editor);
        editor.gameObject.SetActive(true);
        editor.SetParent(this);

        int index = dynamicValueEditors.Count - 1;


        if (isNew)
        {

            editor.SetValue(index, true, Type);
            CustomTimeSlider.instance.CreateTweenHandles(data);

            switch (type)
            {
                case DynamicValueType.Rotations:



                    break;
                case DynamicValueType.Positions:
                    var s = Instantiate(positionerObjectPrefab).GetComponent<PositionerObject>();
                    positionerObjects.Add(s);
                    s.Activate(index, data.positions[index]);



                    break;
                case DynamicValueType.Timers:

                    break;

            }
            numberOfValues++;
            editor.GetComponent<DynamicValueEditor>().OnPress();
            ScrollToEnd();
        }
        else
        {
            editor.SetValue(index, false, Type);

        }



    }
}
