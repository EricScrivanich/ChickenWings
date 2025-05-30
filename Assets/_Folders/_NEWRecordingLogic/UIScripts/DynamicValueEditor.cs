using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DynamicValueEditor : MonoBehaviour, IPointerDownHandler
{
    private DynamicValueAdder parent;

    private Image imageFill;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private LevelCreatorColors colorSO;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private TextMeshProUGUI endTimeText;
    private string type;




    // private ushoty startTime;
    // private float endTime;


    private int index;

    void Awake()
    {
        imageFill = GetComponent<Image>();
        imageFill.color = colorSO.MainUIColor;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void SetParent(DynamicValueAdder parent)
    {
        this.parent = parent;
    }


    public void EditValue(float val)
    {
        LevelRecordManager.instance.currentSelectedObject.rotationData.values[index] = val;
        valueText.text = (-val).ToString();
        LevelRecordManager.instance.UpdateTime(LevelRecordManager.CurrentTimeStep);





    }

    public void EditTimeFromSpawnTime()
    {
        startTimeText.text = LevelRecordManager.instance.FormatTimerText(parent.data.startingSteps[index]);
        endTimeText.text = LevelRecordManager.instance.FormatTimerText(parent.data.endingSteps[index]);
        EditTimer();
    }

    public void EditTime(ushort time, bool start)
    {
        if (start)
        {
            startTimeText.text = LevelRecordManager.instance.FormatTimerText(time);
        }
        else
        {
            endTimeText.text = LevelRecordManager.instance.FormatTimerText(time);
        }

        if (type == "Timers") EditTimer();

    }

    public void EditPostionText(Vector2 pos)
    {
        valueText.text = LevelRecordManager.instance.FormatVectorText(pos);
    }

    public void SetValue(int val, bool isNew, string t)
    {
        numberText.text = (val).ToString() + ".";
        index = val;
        var data = parent.data;
        if (isNew)
        {
            data.AddDuplicateItems();
        }
        type = t;

        Debug.Log("Data: " + index + " has values of length: " + data.values.Count + " and positions of length: " + data.positions.Count);
        if (type == "Positions")
            valueText.text = LevelRecordManager.instance.FormatVectorText(data.positions[index]);
        else if (type == "Rotations")
            valueText.text = (-data.values[index]).ToString();
        else if (type == "Timers")
        {
            EditTimer();
        }

        startTimeText.text = LevelRecordManager.instance.FormatTimerText(data.startingSteps[index]);
        endTimeText.text = LevelRecordManager.instance.FormatTimerText(data.endingSteps[index]);


    }



    public void OnPress()
    {

        parent.SetCurrentSelectedValue(index);


    }

    public void EditTimer()
    {
        if (type == "Timers")
        {
            float val = (parent.data.endingSteps[index] - parent.data.startingSteps[index]) * LevelRecordManager.instance.ConvertLevelTimeToRealTime(LevelRecordManager.TimePerStep);
            valueText.text = val.ToString("F1") + " s";
        }

    }

    public void SetSelected()
    {
        if (index == 0) return;
        imageFill.color = colorSO.SelctedUIColor;


        LevelRecordManager.instance.UpdateTime(parent.data.startingSteps[index]);
    }

    public void SetUnselected()
    {
        imageFill.color = colorSO.MainUIColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress();

    }

    // Update is called once per frame

}
