using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ValueEditorManager : MonoBehaviour
{
    public static ValueEditorManager instance;
    [SerializeField] private RectTransform rect;

    [SerializeField] private DynamicValueAdder listParent;
    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject listPanel;

    [SerializeField] private int baseHeight;
    [SerializeField] private int addedHeightPerSection;

    [SerializeField] private GameObject floatDataPrefab;

    [SerializeField] private List<RectTransform> activeRects = new List<RectTransform>();

    public LevelDataEditors[] floatEditors { get; private set; }

    [field: SerializeField] public Slider valueSliderHorizontal { get; private set; }
    [field: SerializeField] public TextMeshProUGUI valueSliderText { get; private set; }
    [SerializeField] private ObjectTypeEditor typeEditor;

    [SerializeField] private ObjectTypeEditor[] listEditors;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private RectTransform sliderNormalPosition;
    [SerializeField] private RectTransform sliderFolderHiddenPosition;
    private bool usingListEditors = false;
    private bool isShowingList = false;

    private int lastUsedFloatIndex;
    public Action<string> OnSetSelectedType;
    private void Awake()
    {
        Debug.Log("ValueEditorManager Awake");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        floatEditors = new LevelDataEditors[5];
        for (int i = 0; i < 5; i++)
        {
            var o = Instantiate(floatDataPrefab, rect.transform).GetComponent<LevelDataEditors>();
            o.SetTypeIndex(i);
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 - (90 * i));
            floatEditors[i] = o;

        }

        foreach (var i in listEditors)
        {
            i.gameObject.SetActive(false);
        }
        // rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        typeEditor.gameObject.SetActive(false);
        ShowMainPanel();

    }





    // public void SetSelectedType(string type)
    // {
    //     foreach (var i in floatEditors)
    //     {
    //         if (i.gameObject.activeInHierarchy)
    //             i.SetIfSelected(type);
    //     }
    // }
    public void ResetRectList()
    {
        activeRects.Clear();
        lastUsedFloatIndex = 0;

    }

    public void OrderRectList(string type = null)
    {

        if (type == "Laser")
        {
            activeRects[1] = typeEditor.GetComponent<RectTransform>();
            activeRects[2] = floatEditors[1].GetComponent<RectTransform>();
        }
        else if (usingListEditors)
        {
            usingListEditors = false;
            foreach (var i in listEditors)
            {
                i.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < activeRects.Count; i++)
        {
            activeRects[i].anchoredPosition = new Vector2(activeRects[i].localPosition.x, (i * -addedHeightPerSection) - 60);
            Debug.Log("Position: " + ((i * -addedHeightPerSection) - addedHeightPerSection));
        }
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, baseHeight + (addedHeightPerSection * (lastUsedFloatIndex)));
    }
    public void SendFloatValues(string type, int index, bool final)
    {
        if (isShowingList)
        {
            ShowMainPanel();
        }


        // if (!floatEditors[index].gameObject.activeInHierarchy) floatEditors[index].gameObject.SetActive(true);
        floatEditors[index].gameObject.SetActive(true);
        floatEditors[index].SetDataForFloatSlider(type, false);
        activeRects.Add(floatEditors[index].GetComponent<RectTransform>());

        if (final)
        {

            for (int i = index + 1; i < floatEditors.Length; i++)
            {
                floatEditors[i].gameObject.SetActive(false);
            }

            // rect.sizeDelta = new Vector2(rect.sizeDelta.x, baseHeight + (addedHeightPerSection * (index + 1)));
        }
        else
            lastUsedFloatIndex += 1;

        ShowSlider(false);
        ValueEditorManager.instance.OnSetSelectedType?.Invoke("none");

    }

    public void ChangeAddedValueOnList(float v)
    {
        listParent.EditFloat(v);
    }

    public void ShowSlider(bool show)
    {
        valueSliderHorizontal.gameObject.SetActive(show);
    }

    public void UpdateSpawnTime(ushort time)
    {
        timeText.text = "Spawn Time: " + LevelRecordManager.instance.FormatTimerText(time);
    }

    public void OnPressEditor(bool pressed)
    {
        Debug.Log("OnPressEditor: " + pressed);
        LevelRecordManager.instance.currentSelectedObject.FadeIconImages(pressed);
    }

    public void SendPositionerValues(string type, int index, bool final)
    {
        usingListEditors = true;
        listEditors[index].SetData(type, null);
        listEditors[index].gameObject.SetActive(true);

        activeRects.Add(listEditors[index].GetComponent<RectTransform>());
        lastUsedFloatIndex += 1;

        if (final)
        {
            // lastUsedFloatIndex += index + 1;
            if (index < listEditors.Length - 1)
                for (int i = index + 1; i < listEditors.Length; i++)
                {
                    listEditors[i].gameObject.SetActive(false);
                }

            // rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + (addedHeightPerSection * index + 1));
        }





    }

    public void SendTypeValues(string type, string[] s)
    {
        if (s == null || s.Length <= 0) typeEditor.gameObject.SetActive(false);

        else
        {
            typeEditor.gameObject.SetActive(true);
            typeEditor.SetData(type, s);
            activeRects.Add(typeEditor.GetComponent<RectTransform>());
            lastUsedFloatIndex += 1;

            // typeEditor.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 - (90 * (lastUsedFloatIndex + 1)));
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void CheckStaticParameters()
    {
        if (LevelRecordManager.ShowFolders) valueSliderHorizontal.GetComponent<RectTransform>().anchoredPosition = sliderNormalPosition.anchoredPosition;
        else valueSliderHorizontal.GetComponent<RectTransform>().anchoredPosition = sliderFolderHiddenPosition.anchoredPosition;

    }
    private void OnEnable()
    {
        LevelRecordManager.CheckViewParameters += CheckStaticParameters;
    }
    private void OnDisable()
    {
        ValueEditorManager.instance.valueSliderHorizontal.onValueChanged.RemoveAllListeners();
        LevelRecordManager.CheckViewParameters -= CheckStaticParameters;



    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        listPanel.GetComponent<DynamicValueAdder>().Deactivate();
        isShowingList = false;
    }
    public void ShowListPanel(string type)
    {
        mainPanel.SetActive(false);
        RecordedDataStructTweensDynamic d = null;
        switch (type)
        {
            case "Rotations":
                d = LevelRecordManager.instance.currentSelectedObject.rotationData;
                break;
            case "Positions":
                d = LevelRecordManager.instance.currentSelectedObject.positionData;
                break;
            case "Timers":
                d = LevelRecordManager.instance.currentSelectedObject.timerData;
                break;
        }
        listPanel.GetComponent<DynamicValueAdder>().Activate(type, d);
        isShowingList = true;
        // listPanel.SetActive(true);
    }

    // Update is called once per frame

}
