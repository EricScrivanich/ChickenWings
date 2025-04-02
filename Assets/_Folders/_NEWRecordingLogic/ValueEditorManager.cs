using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ValueEditorManager : MonoBehaviour
{
    public static ValueEditorManager instance;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    [SerializeField] private int baseHeight;
    [SerializeField] private int addedHeightPerSection;

    [SerializeField] private GameObject floatDataPrefab;

    public LevelDataEditors[] floatEditors { get; private set; }

    [field: SerializeField] public Slider valueSliderHorizontal { get; private set; }
    [field: SerializeField] public TextMeshProUGUI valueSliderText { get; private set; }
    [SerializeField] private ObjectTypeEditor typeEditor;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private RectTransform sliderNormalPosition;
    [SerializeField] private RectTransform sliderFolderHiddenPosition;

    private int lastUsedFloatIndex;
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
            var o = Instantiate(floatDataPrefab, this.transform).GetComponent<LevelDataEditors>();
            o.SetTypeIndex(i);
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 - (90 * i));
            floatEditors[i] = o;

        }
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        typeEditor.gameObject.SetActive(false);

    }

    public void FadeGroup(bool fade)
    {
        if (fade)
        {
            canvasGroup.alpha = .3f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

    }



    public void SetSelectedType(string type)
    {
        foreach (var i in floatEditors)
        {
            if (i.gameObject.activeInHierarchy)
                i.SetIfSelected(type);
        }
    }

    public void SendFloatValues(string type, int index, bool final)
    {


        // if (!floatEditors[index].gameObject.activeInHierarchy) floatEditors[index].gameObject.SetActive(true);
        floatEditors[index].gameObject.SetActive(true);
        floatEditors[index].SetDataForFloatSlider(type, false);

        if (final)
        {
            lastUsedFloatIndex = index;
            for (int i = index + 1; i < floatEditors.Length; i++)
            {
                floatEditors[i].gameObject.SetActive(false);
            }

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, baseHeight + (addedHeightPerSection * index));
        }

        ShowSlider(false);
        SetSelectedType("none");

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

    public void SendTypeValues(string type, string[] s)
    {
        if (s == null || s.Length <= 0) typeEditor.gameObject.SetActive(false);

        else
        {
            typeEditor.gameObject.SetActive(true);
            typeEditor.SetData(type, s);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, baseHeight + (addedHeightPerSection * (lastUsedFloatIndex + 1)));
            typeEditor.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 - (90 * (lastUsedFloatIndex + 1)));
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

    // Update is called once per frame

}
