using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SliderHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum HandleType { Main, Min, Max, AbsoluteMax, Object, Time, StartTime, StartTween, EndTween, TweenDisplay, MultipleObject, MultSelectCenter, MultSelectLeft, MultSelectRight, MultSelectMove }

    [SerializeField] private bool isStartTween;
    [SerializeField] private TextMeshProUGUI text;
    private int tweenIndex;
    public HandleType type;
    public System.Action<HandleType, float> OnDragged; // float = delta x in pixels
    public System.Action<HandleType, bool> OnPressed; // float = delta x in pixels
    [SerializeField] private Color normalColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private float pressedScale;
    [SerializeField] private float smallScale;


    [SerializeField] private LevelCreatorColors colorSO;

    [SerializeField] private Image[] outline;

    private Image image;
    private Vector2 lastPointerPos;

    private bool clicked;
    private bool hasClicked;

    private int currentTimeStep;


    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {

        if (type == HandleType.Min || type == HandleType.Max)
        {
            image.color = normalColor;
            image.rectTransform.localScale = Vector3.one;
        }
        else if (type == HandleType.Object || type == HandleType.MultipleObject)
        {
            CustomTimeSlider.instance.OnEditObjectTime += EditObjectTime;
            foreach (var i in outline)
            {
                i.color = colorSO.iconOutlineColor;
            }
            pressedColor = colorSO.iconFillColor;
            normalColor = Color.clear;

            image.color = normalColor;

        }
        else if (type == HandleType.Time)
        {
            CustomTimeSlider.instance.OnEditObjectTime += EditObjectTime;
            pressedColor = colorSO.SelctedUIColor;
            normalColor = colorSO.MainUIColor;
            pressedScale = 1;
            image.color = normalColor;
        }
        else if (type == HandleType.StartTween || type == HandleType.EndTween)
        {

            normalColor = Color.clear;


        }

    }

    private void OnDestroy()
    {
        if (type == HandleType.Object || type == HandleType.Time)
        {
            CustomTimeSlider.instance.OnEditObjectTime -= EditObjectTime;
        }
    }

    public void SetTweenIndex(int index)
    {
        tweenIndex = index;
        text.text = tweenIndex.ToString();

    }

    public void SetTweenStep(int step)
    {
        currentTimeStep = step;


    }

    public int GetStep()
    {
        return currentTimeStep;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        if (type == HandleType.Min || type == HandleType.Max)
        {
            image.color = pressedColor;
            image.rectTransform.localScale = Vector3.one * pressedScale;
        }

        lastPointerPos = eventData.position;


        if (type == HandleType.StartTween || type == HandleType.EndTween)
        {
            // image.color = pressedColor;
            // image.rectTransform.localScale = Vector3.one * pressedScale;
            DynamicValueAdder.instance.StartTimeStepEdit(isStartTween, true);
            return;
        }
        else if (type == HandleType.TweenDisplay)
        {
            image.color = colorSO.SelctedUIColor;
            DynamicValueAdder.instance.StartTimeStepEdit(isStartTween, false);
            return;

        }




        OnPressed?.Invoke(type, true);
        Debug.Log("Pressed");

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!clicked) return;



        float deltaX = eventData.position.x - lastPointerPos.x;


        if (type == HandleType.StartTween || type == HandleType.EndTween || type == HandleType.TweenDisplay)
        {
            // image.color = pressedColor;
            // image.rectTransform.localScale = Vector3.one * pressedScale;
            CustomTimeSlider.instance.OnDragTweenTime(deltaX, type == HandleType.TweenDisplay);

        }
        else
            OnDragged?.Invoke(type, deltaX);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
        if (type == HandleType.Min || type == HandleType.Max)
        {
            image.color = normalColor;
            image.rectTransform.localScale = Vector3.one;
        }

        if (type == HandleType.StartTween || type == HandleType.EndTween)
        {
            CustomTimeSlider.instance.OnPressTweenTime(false, true, 0);

            return;
        }
        else if (type == HandleType.TweenDisplay)
        {
            image.color = colorSO.MainUIColor;
            CustomTimeSlider.instance.OnPressTweenTime(false, false, 0);

            return;

        }

        OnPressed?.Invoke(type, false);
    }

    public void SetTweenHandle(bool isSelected)
    {
        if (isSelected)
        {
            image.enabled = true;
            text.color = Color.white;
            image.rectTransform.localScale = Vector3.one;
        }
        else
        {
            image.enabled = false;
            text.color = Color.gray;
            image.rectTransform.localScale = Vector3.one * smallScale;
        }
    }

    private void EditObjectTime(bool pressed)
    {
        if (pressed)
        {
            image.color = pressedColor;
            image.rectTransform.localScale = Vector3.one * pressedScale;
        }
        else
        {
            image.color = normalColor;
            image.rectTransform.localScale = Vector3.one;
        }
    }
}