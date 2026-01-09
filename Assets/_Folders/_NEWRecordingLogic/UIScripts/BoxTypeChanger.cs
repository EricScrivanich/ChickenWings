using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoxTypeChanger : MonoBehaviour, IPointerClickHandler, ISelectableUI
{

    [SerializeField] private int boxType;
    [SerializeField] private string[] typeNames;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image highlightImage;
    private IBoxSliderListener listener;
    private int currentTypeIndex = 0;
    private bool isSelected = false;

    public int minTypeIndex = 0;
    public int maxTypeIndex = 0;
    [SerializeField] private bool wrap;

    public void Initialize(IBoxSliderListener listener)
    {
        this.listener = listener;

    }
    private bool useNumbers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(string title, int boxType, int currentIndex, string[] names = null, bool useNumbers = false)
    {
        this.useNumbers = useNumbers;
        if (names != null || useNumbers)
        {
            this.typeNames = names;
            titleText.text = title;
        }
        else
        {
            titleText.text = title + " RNG ID: ";
        }
        this.currentTypeIndex = currentIndex;

        if (!useNumbers)
        {
            typeText.text = typeNames[currentIndex];
            minTypeIndex = 0;
            maxTypeIndex = typeNames.Length - 1;
        }

        else
            typeText.text = (currentIndex).ToString();


        this.boxType = boxType;


    }



    public void ClickArrow(int direction)
    {
        currentTypeIndex += direction;
        if (currentTypeIndex < minTypeIndex)
        {
            if (!wrap) currentTypeIndex = minTypeIndex;
            else
                currentTypeIndex = maxTypeIndex;
        }

        if (currentTypeIndex > maxTypeIndex)
        {
            if (!wrap) currentTypeIndex = maxTypeIndex;
            else
                currentTypeIndex = minTypeIndex;
        }
        if (useNumbers)
            typeText.text = (currentTypeIndex).ToString();
        else
            typeText.text = typeNames[currentTypeIndex];

        listener.OnBoxTypeChanged(boxType, currentTypeIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Use NodeTreeCanvas selection manager if available
        if (NodeTreeCanvas.Instance != null)
        {
            NodeTreeCanvas.Instance.SelectSelectableUI(this);
        }
        else
        {
            SetSelected(true);
        }
    }

    // ISelectableUI implementation
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (highlightImage != null)
            highlightImage.enabled = selected;
    }

    public bool IsSelected => isSelected;
}
