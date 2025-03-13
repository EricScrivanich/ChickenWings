using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class LevelDataEditors : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Vector2Int dataID;
    private bool isSelected;

    [SerializeField] private TextMeshProUGUI title;
    private Image fillImage;

    [SerializeField] private string Type;

    // [SerializeField] private Slider slider;
    private RecordableObjectPlacer recordedObj => LevelRecordManager.instance.currentSelectedObject;

    private float sliderSensitivity = 0.005f; // Adjust this value to control sensitivity


    private Vector2 pointerStartPos;
    private float initialSliderValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }
    private void OnEnable()
    {

        LevelRecordManager.SetSelectedDataType += SetIfSelected;
        fillImage.enabled = false;
    }



    private void OnDisable()
    {
        LevelRecordManager.SetSelectedDataType -= SetIfSelected;

    }

    private void SetDataFromFloatSlider(float v)
    {

        if (Type == "Speed")
        {
            v = Mathf.Round(v * 10) / 10;
            recordedObj.Data.speed = v * recordedObj.speedChanger;
            title.text = Type + ": " + v.ToString();
        }

        else if (Type == "Size")
        {

            v = Mathf.Round(v * 100) / 100;
            title.text = Type + ": " + Mathf.RoundToInt(v * 100).ToString() + "%";
            SetMainSliderText();
            recordedObj.Data.scale = v;


            recordedObj.UpdateObjectData(true);
            return;

        }

        else if (Type == "Jump Height" || Type == "Glide Height")
        {
            v = Mathf.Round(v * 100) / 100;
            recordedObj.Data.magPercent = v;
            title.text = Type + ": " + (v * 100).ToString() + "%";
            SetMainSliderText();

            recordedObj.UpdateObjectData();
            return;
        }

        else if (Type == "Jump Offset" || Type == "Fart Offset" || Type == "Glide Offset")
        {
            v = Mathf.Round(v * 100) / 100;
            title.text = Type + ": " + (v * 100).ToString() + "%";
            SetMainSliderText();

            recordedObj.Data.delayInterval = v;
            recordedObj.UpdateObjectData();
            return;
        }

        else if (Type == "Drop Delay" || Type == "Fart Delay")
        {
            v = Mathf.Round(v * 100) / 100;
            recordedObj.Data.timeInterval = v;
        }
        else if (Type == "Rotation")
        {
            v = Mathf.RoundToInt(v);
            // float m = Mathf.Abs(0 - v);
            // v = Mathf.Abs(v);
            recordedObj.Data.timeInterval = -v;
            title.text = Type + ": " + Mathf.Abs(-v - 90).ToString();
            SetMainSliderText();
            recordedObj.UpdateObjectData();
            return;

        }

        else if (Type == "Drop Offset")
        {
            v = Mathf.Round(v * 100) / 100;
            recordedObj.Data.scale = v;
        }


        // valueText.text = v.ToString();
        title.text = Type + ": " + v.ToString();
        SetMainSliderText();
        recordedObj.UpdateObjectData();

    }

    private void SetMainSliderText()
    {
        if (isSelected)
            LevelRecordManager.instance.valueSliderText.text = title.text;

    }

    public void OnPress()
    {
        isSelected = true;
        SetDataForFloatSlider(this.Type, true);
        LevelRecordManager.SetSelectedDataType?.Invoke(Type);
        fillImage.enabled = true;

    }

    private void SetIfSelected(string type)
    {

        if (isSelected && Type != type)
        {
            isSelected = false;
            fillImage.enabled = false;


        }



    }

    public void SetValueAndText(float val)
    {

    }



    public void SetDataForFloatSlider(string type, bool setSlider)
    {


        LevelRecordManager.instance.valueSliderHorizontal.onValueChanged.RemoveAllListeners();
        Type = type;

        // title.text = Type + ": " + val.ToString();




        // slider.minValue = min;
        // slider.maxValue = max;
        float v = 0;

        if (Type == "Speed")
        {
            if (setSlider)
                SetSliderRange(recordedObj.SpeedValues);
            v = Mathf.Abs(recordedObj.Data.speed);

        }
        else if (Type == "Size")
        {
            if (setSlider) SetSliderRange(recordedObj.SizeValues);

            v = recordedObj.Data.scale;
        }
        else if (Type == "Jump Height" || Type == "Glide Height")
        {
            if (setSlider) SetSliderRange(recordedObj.MagnitideValues);
            v = recordedObj.Data.magPercent;
        }
        else if (Type == "Jump Offset" || Type == "Fart Offset" | Type == "Glide Offset")
        {
            if (setSlider) SetSliderRange(recordedObj.DelayOrPhaseOffsetValues);
            v = recordedObj.Data.delayInterval;

        }

        else if (Type == "Drop Delay" || Type == "Fart Delay")
        {
            if (setSlider) SetSliderRange(recordedObj.TimeIntervalValues);

            v = recordedObj.Data.timeInterval;
        }
        else if (Type == "Rotation")
        {
            if (setSlider) SetSliderRange(recordedObj.TimeIntervalValues);
            v = -recordedObj.Data.timeInterval;

        }
        else if (Type == "Drop Offset")
        {
            if (setSlider) SetSliderRange(recordedObj.SizeValues);

            v = recordedObj.Data.scale;
        }
        SetDataFromFloatSlider(v);

        if (setSlider)
        {
            LevelRecordManager.instance.valueSliderHorizontal.value = v;
            LevelRecordManager.instance.valueSliderHorizontal.onValueChanged.AddListener(this.SetDataFromFloatSlider);

        }


    }

    private void SetSliderValues()
    {

    }

    public void SetSliderRange(Vector3 range)
    {
        LevelRecordManager.instance.valueSliderHorizontal.minValue = range.x;
        LevelRecordManager.instance.valueSliderHorizontal.maxValue = range.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress();
        useDragValue = false;
        pointerStartPos = eventData.position;
    }
    private bool useDragValue = false;

    private float range;
    private float minVal;
    private float maxVal;
    private float startValue;
    private float dragRange = 300;
    private float valueStep;
    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - pointerStartPos.x;

        if (!useDragValue && Mathf.Abs(deltaX) > 30)
        {
            Debug.Log("HIt drag TRhresgold");
            useDragValue = true;

            pointerStartPos = eventData.position;
            startValue = LevelRecordManager.instance.valueSliderHorizontal.value;
            minVal = LevelRecordManager.instance.valueSliderHorizontal.minValue;
            maxVal = LevelRecordManager.instance.valueSliderHorizontal.maxValue;
            valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));

        }

        if (useDragValue)
        {
            int stepCount = Mathf.FloorToInt(deltaX / 5f);

            // Calculate the new value based on steps
            float newSliderValue = Mathf.Clamp(startValue + (stepCount * valueStep), minVal, maxVal);

            // Apply the value to the slider
            LevelRecordManager.instance.valueSliderHorizontal.value = newSliderValue;
        }

        // Get the current slider reference

    }
}
