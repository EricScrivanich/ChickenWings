using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoxTypeChanger : MonoBehaviour
{

    [SerializeField] private int boxType;
    [SerializeField] private string[] typeNames;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI titleText;
    private IBoxSliderListener listener;
    private int currentTypeIndex = 0;
    [SerializeField] private bool wrap;

    public void Initialize(IBoxSliderListener listener)
    {
        this.listener = listener;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(string title, int boxType, int currentIndex)
    {
        this.currentTypeIndex = currentIndex;
        typeText.text = typeNames[currentIndex];
        titleText.text = title + " RNG ID: ";
        this.boxType = boxType;


    }

    public void ClickArrow(int direction)
    {
        currentTypeIndex += direction;
        if (currentTypeIndex < 0)
        {
            if (!wrap) currentTypeIndex = 0;
            else
                currentTypeIndex = typeNames.Length - 1;
        }

        if (currentTypeIndex >= typeNames.Length)
        {
            if (!wrap) currentTypeIndex = typeNames.Length - 1;
            else
                currentTypeIndex = 0;
        }

        typeText.text = typeNames[currentTypeIndex];
        listener.OnBoxTypeChanged(boxType, currentTypeIndex);
    }

    // public void ChangeValue
}
