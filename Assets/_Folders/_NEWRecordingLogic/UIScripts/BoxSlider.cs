using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BoxSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private int Type;

    [SerializeField] private IBoxSliderListener listener;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string Title;
    [SerializeField] private Image fillImage;
    public bool isSelected = false;
    private Vector2 pointerStartPos;
    private float currentValue;


    private float startValue;
    private float dragRange = 300;
    private float valueStep;
    private bool useDragValue = false;

    public enum BoxSliderType
    {
        Byte,
        Other

    }
    [SerializeField] private BoxSliderType boxSliderType;



    // Start is called once before the first execution of Update after the MonoBehaviour is created



    [SerializeField] private float minVal;
    [SerializeField] private float maxVal;
    [SerializeField] private bool isUnkownValue = false;

    void Awake()
    {
        fillImage = GetComponent<Image>();
        fillImage.enabled = false;
    }
    public void Initialize(IBoxSliderListener listener)
    {
        this.listener = listener;
        valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));
    }

    public void OnPress()
    {
        if (isUnkownValue) return;
        isSelected = true;
        // ValueEditorManager.instance.OnPressEditor(true);
        // Debug.Log("OnPress sldier");
        ValueEditorManager.instance.OnSetSelectedType?.Invoke(Title);
        if (LevelRecordManager.UsingWaveCreator)
            WaveCreator.instance.ChangeBoxTypeData("Spawn Chance", 0);



        fillImage.enabled = true;

    }

    private void OnEnable()
    {

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
        ValueEditorManager.instance.OnSetSelectedType -= SetIfSelected;


    }


    public void SetValue(float val)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        if (boxSliderType == BoxSliderType.Byte)
        {
            valueText.text = Title + ": " + Mathf.RoundToInt(val * 100).ToString();
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
        pointerStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {


        float deltaX = eventData.position.x - pointerStartPos.x;

        // if (!useDragValue && Mathf.Abs(deltaX) > 30)
        // {
        //     Debug.Log("HIt drag TRhresgold");
        //     useDragValue = true;

        //     pointerStartPos = eventData.position;
        //     startValue = ValueEditorManager.instance.valueSliderHorizontal.value;
        //     Debug.LogError("Start Value: " + startValue);

        //     valueStep = Mathf.Abs(Mathf.Lerp(minVal, maxVal, .01f) - Mathf.Lerp(minVal, maxVal, 0));

        // }

        // if (useDragValue)
        // {
        //     int stepCount = Mathf.FloorToInt(deltaX / 5f);

        //     // Calculate the new value based on steps
        //     float newSliderValue = Mathf.Clamp(startValue + (stepCount * valueStep), minVal, maxVal);
        //     listener.OnBoxSliderValueChanged(Type, newSliderValue);

        //     // Apply the value to the slider
        //     // ValueEditorManager.instance.valueSliderHorizontal.value = newSliderValue;
        // }

        int stepCount = Mathf.FloorToInt(deltaX / 5f);

        // Calculate the new value based on steps
        float newSliderValue = Mathf.Clamp(startValue + (stepCount * valueStep), minVal, maxVal);
        listener.OnBoxSliderValueChanged(Type, newSliderValue);

        // Get the current slider reference

    }

}
