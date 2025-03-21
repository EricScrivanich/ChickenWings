using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ValueEditorManager : MonoBehaviour
{
    public static ValueEditorManager instance;
    private RectTransform rect;

    [SerializeField] private int baseHeight;
    [SerializeField] private int addedHeightPerSection;

    [SerializeField] private GameObject floatDataPrefab;

    public LevelDataEditors[] floatEditors { get; private set; }

    [field: SerializeField] public Slider valueSliderHorizontal { get; private set; }
    [field: SerializeField] public TextMeshProUGUI valueSliderText { get; private set; }
    [SerializeField] private ObjectTypeEditor typeEditor;

    private int lastUsedFloatIndex;
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
    void Start()
    {
        floatEditors = new LevelDataEditors[5];
        for (int i = 0; i < 5; i++)
        {
            var o = Instantiate(floatDataPrefab, this.transform).GetComponent<LevelDataEditors>();
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 - (90 * i));
            floatEditors[i] = o;

        }
        rect = GetComponent<RectTransform>();
        typeEditor.gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        ValueEditorManager.instance.valueSliderHorizontal.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
