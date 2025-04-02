using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlusMinusUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int amount;

    private Image fillImage;
    public bool unselecatable;
    [SerializeField] private LevelCreatorColors colorSO;
    [SerializeField] private float pressScale = 1.1f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float smallScale = 0.8f;
    [SerializeField] private Image outline;
    [SerializeField] private TextMeshProUGUI text;

    private StartingStatEditor startingStatEditor;


    private void Awake()
    {
        fillImage = GetComponent<Image>();
        fillImage.color = colorSO.MainUIColor;
    }
    private void Start()
    {
        startingStatEditor = GetComponentInParent<StartingStatEditor>();
        if (startingStatEditor == null)
        {
            Debug.LogError("StartingStatEditor not found in parent.");
            return;
        }
    }

    public void SetData(StartingStatEditor editor)
    {


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (unselecatable) return;
        transform.localScale = Vector3.one * pressScale;
        fillImage.color = colorSO.SelctedUIColor;
        startingStatEditor.OnEdit(amount);

    }

    public void SetUnselectable(bool isUnselectable)
    {
        if (isUnselectable)
        {
            unselecatable = true;
            transform.localScale = Vector3.one * smallScale;
            fillImage.color = colorSO.MainUIColor;
            text.color = colorSO.UnSelectableColor;
            outline.color = colorSO.UnSelectableColor;
        }
        else
        {
            unselecatable = false;
            transform.localScale = Vector3.one * normalScale;
            fillImage.color = colorSO.MainUIColor;
            text.color = Color.white;
            outline.color = Color.white;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (unselecatable) return;
        transform.localScale = Vector3.one * normalScale;
        fillImage.color = colorSO.MainUIColor;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


}
