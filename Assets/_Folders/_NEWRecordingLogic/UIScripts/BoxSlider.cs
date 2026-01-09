using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BoxSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, ISelectableUI
{
    [SerializeField] private int Type;

    [SerializeField] private IBoxSliderListener listener;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string Title;
    [SerializeField] private Image fillImage;
    private bool isSelected = false;
    private Vector2 pointerStartPos;
    private float currentValue;

    private ushort currentTimeValue;


    private float startValue;
    private ushort startTimeValue;
    private float dragRange = 300;
    private float valueStep;
    private bool useDragValue = false;

    public enum BoxSliderType
    {
        Byte,
        Other,
        Time,
        ByteRNG

    }
    [SerializeField] private BoxSliderType boxSliderType;



    // Start is called once before the first execution of Update after the MonoBehaviour is created



    [SerializeField] private float minVal;
    [SerializeField] private float maxVal;
    [SerializeField] private bool isUnkownValue = false;

    // For Byte type (0-100 as percentage)
    private int minByteVal = 0;
    private int maxByteVal = 100;
    private int currentByteValue;
    private int startByteValue;

    // For Time type (ushort)
    private ushort minTimeVal = 0;
    private ushort maxTimeVal = 500;

    private bool customRange = false;



    void Awake()
    {
        fillImage = GetComponent<Image>();
        fillImage.enabled = false;
    }

    /// <summary>
    /// Sets custom range for Byte type (0-100)
    /// </summary>
    public void SetByteRange(byte min, byte max)
    {
        minByteVal = min;
        maxByteVal = max;
        customRange = true;
    }

    /// <summary>
    /// Sets custom range for Time type (ushort)
    /// </summary>
    public void SetTimeRange(ushort min, ushort max)
    {
        minTimeVal = min;
        maxTimeVal = max;

        customRange = true;
    }

    public void Initialize(IBoxSliderListener listener, int type, BoxSliderType sliderType, string title)
    {
        this.listener = listener;
        this.Type = type;
        this.boxSliderType = sliderType;
        this.Title = title;

        if (boxSliderType == BoxSliderType.Byte)
        {
            valueStep = 1; // Step by 1 for byte values
        }
        else
        {
            valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));
        }
    }

    // Keep old Initialize for backwards compatibility
    public void Initialize(IBoxSliderListener listener)
    {
        this.listener = listener;
        valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));
    }

    public void OnPress()
    {
        if (isUnkownValue) return;

        // Use NodeTreeCanvas selection manager if available
        if (NodeTreeCanvas.Instance != null)
        {
            NodeTreeCanvas.Instance.SelectSelectableUI(this);
        }
        else
        {
            SetSelected(true);
        }

        if (boxSliderType == BoxSliderType.ByteRNG || boxSliderType == BoxSliderType.Other)
        {
            if (ValueEditorManager.instance != null)
                ValueEditorManager.instance.OnSetSelectedType?.Invoke(Title);
            if (LevelRecordManager.UsingWaveCreator)
                WaveCreator.instance.ChangeBoxTypeData("Spawn Chance", 0);
        }
    }

    private void OnEnable()
    {
        if (ValueEditorManager.instance != null && boxSliderType == BoxSliderType.ByteRNG)

            ValueEditorManager.instance.OnSetSelectedType += SetIfSelected;
    }

    public void SetIfSelected(string type)
    {

        if (isUnkownValue) return;


        if (isSelected && Title != type)
        {
            isSelected = false;
            fillImage.enabled = false;


        }



    }



    private void OnDisable()
    {

        if (ValueEditorManager.instance != null && boxSliderType == BoxSliderType.ByteRNG)


            ValueEditorManager.instance.OnSetSelectedType -= SetIfSelected;


    }


    /// <summary>
    /// Sets value for Byte type (0-100 displayed as percentage)
    /// </summary>
    public void SetByteValue(byte val)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        currentByteValue = val;
        valueText.text = Title + ": " + val.ToString() + "%";
    }

    /// <summary>
    /// Sets value for Time type (ushort)
    /// </summary>
    public void SetTimeValue(ushort val)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        currentTimeValue = val;
        float displayedTimeValue = ((float)val * (LevelRecordManager.TimePerStep / FrameRateManager.BaseTimeScale));
        valueText.text = Title + ": " + displayedTimeValue.ToString("F2") + " Sec(s)";
    }

    // Keep old SetValue for backwards compatibility
    public void SetValue(float val)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        if (boxSliderType == BoxSliderType.Byte || boxSliderType == BoxSliderType.ByteRNG)
        {
            valueText.text = Title + ": " + Mathf.RoundToInt(val * 100).ToString() + "%";
        }
        else
        {
            valueText.text = Title + ": " + val.ToString("F2");
        }
        currentValue = val;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress();

        useDragValue = false;
        startValue = currentValue;
        startTimeValue = currentTimeValue;
        startByteValue = currentByteValue;
        pointerStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - pointerStartPos.x;
        int stepCount = Mathf.FloorToInt(deltaX / 5f);

        if (boxSliderType == BoxSliderType.Time)
        {
            // Calculate the new time value based on steps
            ushort newTimeValue = (ushort)Mathf.Clamp(startTimeValue + stepCount, minTimeVal, maxTimeVal);

            if (newTimeValue != currentTimeValue)
            {
                currentTimeValue = newTimeValue;
                listener.OnBoxSliderTimeValueChanged(Type, currentTimeValue);

                float displayedTimeValue = ((float)currentTimeValue * (LevelRecordManager.TimePerStep / FrameRateManager.BaseTimeScale));
                valueText.text = Title + ": " + displayedTimeValue.ToString("F2") + " Sec(s)";
            }
            return;
        }

        if (boxSliderType == BoxSliderType.Byte)
        {
            // stepCount = Mathf.FloorToInt(deltaX / .01f);
            // Calculate the new byte value based on steps
            int newByteValue = (int)Mathf.Clamp((int)startByteValue + stepCount, minByteVal, maxByteVal);

            if (newByteValue != currentByteValue)
            {
                currentByteValue = newByteValue;
                listener.OnBoxSliderByteValueChanged(Type, (byte)currentByteValue);

                valueText.text = Title + ": " + currentByteValue.ToString() + "%";
            }
            return;
        }

        // Default/Other type - use float
        currentValue = Mathf.Clamp(startValue + (stepCount * valueStep), minVal, maxVal);
        // valueText.text = Title + ": " + currentValue.ToString() + "%";
        listener.OnBoxSliderValueChanged(Type, currentValue);
    }

    // ISelectableUI implementation
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (fillImage != null)
            fillImage.enabled = selected;
    }

    public bool IsSelected => isSelected;
}
