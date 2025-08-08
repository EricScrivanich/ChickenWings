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
    [SerializeField] private GameObject cagePanel;

    [SerializeField] private int baseHeight;
    [SerializeField] private int addedHeightPerSection;

    [SerializeField] private GameObject floatDataPrefab;

    [SerializeField] private List<RectTransform> activeRects = new List<RectTransform>();

    public LevelDataEditors[] floatEditors { get; private set; }

    private int floatEditorCount = 0;

    [field: SerializeField] public Slider valueSliderHorizontal { get; private set; }
    [field: SerializeField] public TextMeshProUGUI valueSliderText { get; private set; }
    [SerializeField] private ObjectTypeEditor typeEditor;

    [SerializeField] private ObjectTypeEditor[] listEditors;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private RectTransform sliderNormalPosition;
    [SerializeField] private RectTransform sliderFolderHiddenPosition;
    private bool usingListEditors = false;
    private bool isShowingList = false;
    private bool isShowingCage = false;

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

    public void SetNewFloatSize(int change)
    {
        int newSize = floatEditorCount + change;
        lastUsedFloatIndex += change;

        if (change < 0)
        {
            for (int i = newSize; i < floatEditors.Length; i++)
            {
                floatEditors[i].gameObject.SetActive(false);

                if (activeRects.Contains(floatEditors[i].GetComponent<RectTransform>()))
                {
                    activeRects.Remove(floatEditors[i].GetComponent<RectTransform>());
                }
            }

        }
        else
        {
            for (int i = floatEditorCount; i < newSize; i++)
            {
                floatEditors[i].gameObject.SetActive(true);
                activeRects.Add(floatEditors[i].GetComponent<RectTransform>());
            }

            if (typeEditor.gameObject.activeInHierarchy)
            {
                var r = typeEditor.GetComponent<RectTransform>();
                for (int i = 0; i < activeRects.Count; i++)
                {
                    if (activeRects[i] == r)
                    {
                        activeRects.RemoveAt(i);
                        break;
                    }

                }
                activeRects.Add(r);


            }

        }

        floatEditorCount += change;

        OrderRectList();

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
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, baseHeight + (addedHeightPerSection * (activeRects.Count - 1)));
    }

    public void DeactivateFloatEditors()
    {
        for (int i = 0; i < floatEditors.Length; i++)
        {
            floatEditors[i].gameObject.SetActive(false);
        }
    }


    public void SendFloatValues(string type, int index, bool final)
    {
        if (isShowingList || isShowingCage)
        {
            ShowMainPanel();
        }

        int changeMaxLayout = 0;

        if (LevelRecordManager.instance.multipleSelectedIDs)
        {
            floatEditors[index].gameObject.SetActive(false);

        }

        else if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
        {
            var l = LevelRecordManager.instance.MultipleSelectedObjects;
            int id = l[0].ID;
            float checkVal = 0;
            switch (index)
            {
                case 0:
                    checkVal = l[0].Data.float1;
                    break;
                case 1:
                    checkVal = l[0].Data.float2;
                    break;
                case 2:
                    checkVal = l[0].Data.float3;
                    break;
                case 3:
                    checkVal = l[0].Data.float4;
                    break;
                case 4:
                    checkVal = l[0].Data.float5;
                    break;

            }
            bool allSame = true;
            foreach (var o in l)
            {
                switch (index)
                {
                    case 0:
                        if (checkVal != o.Data.float1) allSame = false;
                        break;
                    case 1:
                        if (checkVal != o.Data.float2) allSame = false;
                        break;
                    case 2:
                        if (checkVal != o.Data.float3) allSame = false;
                        break;
                    case 3:
                        if (checkVal != o.Data.float4) allSame = false;
                        break;
                    case 4:
                        if (checkVal != o.Data.float5) allSame = false;
                        break;


                }
                Debug.Log("Checking value for " + o.ID + " with type: " + type + " and index: " + index + " with value: " + checkVal);

                if (!allSame) break;


            }
            if (allSame)
            {

                floatEditors[index].SetDataForFloatSlider(type, false);


            }
            else
            {

                floatEditors[index].SetDataForFloatSlider(type, false, true);

            }

            floatEditors[index].gameObject.SetActive(true);

            activeRects.Add(floatEditors[index].GetComponent<RectTransform>());

        }
        else
        {
            floatEditors[index].gameObject.SetActive(true);
            floatEditors[index].SetDataForFloatSlider(type, false);
            activeRects.Add(floatEditors[index].GetComponent<RectTransform>());
        }


        // if (!floatEditors[index].gameObject.activeInHierarchy) floatEditors[index].gameObject.SetActive(true);


        if (final)
        {
            floatEditorCount = index + 1;

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
    private bool clippingTime = false;
    public void UpdateSpawnTime(ushort time, bool clipping = false)
    {
        if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
        {
            timeText.text = "Spawn Time: ?";

        }
        else
            timeText.text = "Spawn Time: " + LevelRecordManager.instance.FormatTimerText(time);

        if (clipping && !clippingTime)
        {
            clippingTime = true;
            timeText.color = Color.red;
        }
        else if (!clipping && clippingTime)
        {
            clippingTime = false;
            timeText.color = Color.white;
        }
    }

    public void OnPressEditor(bool pressed)
    {
        Debug.Log("OnPressEditor: " + pressed);
        LevelRecordManager.instance.currentSelectedObject.FadeIconImages(pressed);
    }

    public void SendPositionerValues(string type, int index, bool final)
    {
        if (LevelRecordManager.instance.multipleObjectsSelected)
        {
            return;
        }
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
        if (s == null || s.Length <= 0 || LevelRecordManager.instance.multipleSelectedIDs) typeEditor.gameObject.SetActive(false);
        else if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
        {
            bool allSame = true;
            ushort checkVal = LevelRecordManager.instance.MultipleSelectedObjects[0].Data.type;

            foreach (var o in LevelRecordManager.instance.MultipleSelectedObjects)
            {
                if (o.Data.type != checkVal)
                {
                    allSame = false;
                    break;
                }

            }

            if (allSame)
            {

                typeEditor.SetData(type, s);

            }
            else
            {

                typeEditor.SetData(type, s);

            }
            typeEditor.gameObject.SetActive(true);

            activeRects.Add(typeEditor.GetComponent<RectTransform>());
            lastUsedFloatIndex += 1;

        }


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
        cagePanel.SetActive(false);

        mainPanel.SetActive(true);
        if (isShowingList)
            listPanel.GetComponent<DynamicValueAdder>().Deactivate();

        isShowingList = false;
        isShowingCage = false;
    }

    public void ShowCagePanel(bool show)
    {
        if (!show)
        {
            cagePanel.SetActive(false);
            // isShowingCage = false;
            return;
        }
        if (isShowingList)
            listPanel.GetComponent<DynamicValueAdder>().Deactivate();
        else
            mainPanel.SetActive(false);

        cagePanel.SetActive(true);

        isShowingList = false;
        isShowingCage = true;

    }
    public void ShowListPanel(string type)
    {
        if (LevelRecordManager.instance.multipleObjectsSelected)
        {
            return;
        }
        mainPanel.SetActive(false);
        cagePanel.SetActive(false);
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
        isShowingCage = false;
        // listPanel.SetActive(true);
    }

    // Update is called once per frame

}
