using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum HandleType { Main, Min, Max, AbsoluteMax, Object, Time, StartTime }

    public HandleType type;
    public System.Action<HandleType, float> OnDragged; // float = delta x in pixels
    public System.Action<HandleType, bool> OnPressed; // float = delta x in pixels
    [SerializeField] private Color normalColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private float pressedScale;

    [SerializeField] private LevelCreatorColors colorSO;

    [SerializeField] private Image[] outline;

    private Image image;
    private Vector2 lastPointerPos;

    private bool clicked;
    private bool hasClicked;

    

    private void Start()
    {
        image = GetComponent<Image>();
        if (type == HandleType.Min || type == HandleType.Max)
        {
            image.color = normalColor;
            image.rectTransform.localScale = Vector3.one;
        }
        else if (type == HandleType.Object)
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

    }

    private void OnDestroy()
    {
        if (type == HandleType.Object || type == HandleType.Time)
        {
            CustomTimeSlider.instance.OnEditObjectTime -= EditObjectTime;
        }
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
        OnPressed?.Invoke(type, true);
        Debug.Log("Pressed");

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!clicked) return;



        float deltaX = eventData.position.x - lastPointerPos.x;



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
        OnPressed?.Invoke(type, false);
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