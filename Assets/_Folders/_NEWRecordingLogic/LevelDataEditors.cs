using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class LevelDataEditors : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private LevelCreatorColors colorSO;
    [SerializeField] private Vector2Int dataID;

    private int typeIndex;
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
        fillImage.color = colorSO.SelctedUIColor;
    }
    private void OnEnable()
    {



        fillImage.enabled = false;
    }



    private void OnDisable()
    {


    }
    public void SetTypeIndex(int index)
    {
        typeIndex = index;
    }
    private void SetDataFromFloatSlider(float v)
    {



        if (Type == "Speed")
        {
            v = (Mathf.Round(v * 10) / 10) * recordedObj.speedChanger;
            SetValueBasedOnTypeIndex(v);
            title.text = Type + ": " + v.ToString();
        }
        else if (Type == "Blade Speed")
        {
            v = Mathf.RoundToInt(v);
            SetValueBasedOnTypeIndex(v);
            title.text = Type + ": " + v.ToString();
        }

        else if (Type == "Size")
        {

            v = Mathf.Round(v * 100) / 100;
            title.text = Type + ": " + Mathf.RoundToInt(v * 100).ToString() + "%";
            SetMainSliderText();
            SetValueBasedOnTypeIndex(v);


            recordedObj.UpdateObjectData(true);
            return;

        }

        else if (Type == "Jump Height" || Type == "Glide Height")
        {
            v = Mathf.Round(v * 100) / 100;
            SetValueBasedOnTypeIndex(v);
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

            SetValueBasedOnTypeIndex(v);
            recordedObj.UpdateObjectData();
            return;
        }

        else if (Type == "Drop Delay" || Type == "Fart Delay" || Type == "Fart Length")
        {
            v = Mathf.Round(v * 100) / 100;
            SetValueBasedOnTypeIndex(v);
            float t = v / FrameRateManager.BaseTimeScale;
            t = Mathf.Round(t * 100) / 100;

            title.text = Type + ": " + t.ToString();
            SetMainSliderText();
            recordedObj.UpdateObjectData();
            return;
        }
        else if (Type == "Rotation")
        {
            v = Mathf.RoundToInt(v);
            // float m = Mathf.Abs(0 - v);
            // v = Mathf.Abs(v);
            SetValueBasedOnTypeIndex(-v);
            title.text = Type + ": " + Mathf.Abs(-v - 90).ToString();
            SetMainSliderText();
            recordedObj.UpdateObjectData();
            return;

        }
        else if (Type == "Start Rotation")
        {
            v = Mathf.RoundToInt(v);
            title.text = Type + ": " + v.ToString();
            // float m = Mathf.Abs(0 - v);
            // v = Mathf.Abs(v);
            SetValueBasedOnTypeIndex(v);


        }

        else if (Type == "Drop Offset")
        {
            v = Mathf.Round(v * 100) / 100;
            title.text = Type + ": " + (v * 100).ToString() + "%";
            SetValueBasedOnTypeIndex(v);
            SetMainSliderText();
            recordedObj.UpdateObjectData();
            return;
        }
        else if (Type == "Frequency")
        {
            v = Mathf.Round(v * 100) / 100;
            title.text = Type + ": " + (v * 100).ToString() + "%";
            SetValueBasedOnTypeIndex(v);
            SetMainSliderText();
            recordedObj.UpdateObjectData();
            return;

        }


        // valueText.text = v.ToString();

        SetMainSliderText();
        recordedObj.UpdateObjectData();

    }

    private void SetMainSliderText()
    {
        if (isSelected)
        {
            ValueEditorManager.instance.valueSliderText.text = title.text;
        }


    }

    public void SetValueBasedOnTypeIndex(float val)
    {
        switch (typeIndex)
        {
            case 0:
                recordedObj.Data.float1 = val;
                break;
            case 1:
                recordedObj.Data.float2 = val;
                break;
            case 2:
                recordedObj.Data.float3 = val;
                break;
            case 3:
                recordedObj.Data.float4 = val;
                break;
            case 4:
                recordedObj.Data.float5 = val;
                break;

        }
    }

    public void OnPress()
    {
        isSelected = true;
        ValueEditorManager.instance.OnPressEditor(true);
        Debug.Log("OnPress sldier");


        SetDataForFloatSlider(this.Type, true);
        ValueEditorManager.instance.ShowSlider(true);

        ValueEditorManager.instance.SetSelectedType(Type);

        fillImage.enabled = true;

    }

    public void SetIfSelected(string type)
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



        ValueEditorManager.instance.valueSliderHorizontal.onValueChanged.RemoveAllListeners();
        Type = type;

        // title.text = Type + ": " + val.ToString();




        // slider.minValue = min;
        // slider.maxValue = max;


        SetSliderRange(recordedObj.FloatValues[typeIndex]);

        float v = 0;

        switch (typeIndex)
        {
            case 0:
                v = recordedObj.Data.float1;
                break;
            case 1:
                v = recordedObj.Data.float2;
                break;
            case 2:
                v = recordedObj.Data.float3;
                break;
            case 3:
                v = recordedObj.Data.float4;
                break;
            case 4:
                v = recordedObj.Data.float5;
                break;

        }

        if (Type == "Speed" || Type == "Blade Speed")
        {
            // if (setSlider)
            //     SetSliderRange(recordedObj.SpeedValues);
            v = Mathf.Abs(v);

        }
        // else if (Type == "Size")
        // {
        //     if (setSlider) SetSliderRange(recordedObj.SizeValues);

        //     v = recordedObj.Data.scale;
        // }
        // else if (Type == "Jump Height" || Type == "Glide Height")
        // {
        //     if (setSlider) SetSliderRange(recordedObj.MagnitideValues);
        //     v = recordedObj.Data.magPercent;
        // }
        // else if (Type == "Jump Offset" || Type == "Fart Offset" | Type == "Glide Offset" || Type == "Start Rotation")
        // {
        //     if (setSlider) SetSliderRange(recordedObj.DelayOrPhaseOffsetValues);
        //     v = recordedObj.Data.delayInterval;

        // }

        // else if (Type == "Drop Delay" || Type == "Fart Delay" || Type == "Fart Length" || Type == "Frequency")
        // {
        //     if (setSlider) SetSliderRange(recordedObj.TimeIntervalValues);

        //     v = recordedObj.Data.timeInterval;
        // }
        else if (Type == "Rotation")
        {
            // if (setSlider) SetSliderRange(recordedObj.TimeIntervalValues);
            v = -v;

        }
        // else if (Type == "Drop Offset")
        // {
        //     if (setSlider) SetSliderRange(recordedObj.SizeValues);

        //     v = recordedObj.Data.scale;
        // }
        SetDataFromFloatSlider(v);

        if (setSlider)
        {
            ValueEditorManager.instance.valueSliderHorizontal.value = v;
            ValueEditorManager.instance.valueSliderHorizontal.onValueChanged.AddListener(this.SetDataFromFloatSlider);

        }


    }

    private void SetSliderValues()
    {

    }

    public void SetSliderRange(Vector3 range)
    {
        ValueEditorManager.instance.valueSliderHorizontal.minValue = range.x;
        ValueEditorManager.instance.valueSliderHorizontal.maxValue = range.y;
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
            startValue = ValueEditorManager.instance.valueSliderHorizontal.value;
            minVal = ValueEditorManager.instance.valueSliderHorizontal.minValue;
            maxVal = ValueEditorManager.instance.valueSliderHorizontal.maxValue;
            valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));

        }

        if (useDragValue)
        {
            int stepCount = Mathf.FloorToInt(deltaX / 5f);

            // Calculate the new value based on steps
            float newSliderValue = Mathf.Clamp(startValue + (stepCount * valueStep), minVal, maxVal);

            // Apply the value to the slider
            ValueEditorManager.instance.valueSliderHorizontal.value = newSliderValue;
        }

        // Get the current slider reference

    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     ValueEditorManager.instance.OnPressEditor(false);

    // }
}
