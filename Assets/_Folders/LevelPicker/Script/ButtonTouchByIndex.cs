using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ButtonTouchByIndex : MonoBehaviour
{

    public enum ButtonType
    {
        LevelStart,
        Difficulty,
        None
    }
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private bool doPosition = true;
    public int index { get; private set; }
    private IButtonListener buttonListener;
    [SerializeField] private GameObject activateOnPress;
    [SerializeField] private float pressedScale;
    [SerializeField] private float unpressableScale;
    private float normalLocalY;
    [SerializeField] private float addedLocalY;
    private bool isPressed = false;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color unselectableColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color unselectableTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Image outlineImage;

    [SerializeField] private bool doColor;
    [SerializeField] private GameObject unselectableObject;
    [SerializeField] private float normalScale = 1;
    [SerializeField] private TextMeshProUGUI buttonText;
    private Image img;



    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void SetData(int newIndex, IButtonListener listener, bool unpress, bool redo = false)
    {
        index = newIndex;

        isPressed = false;

        img = GetComponent<Image>();
        if (unpress)
        {
            transform.localScale = Vector3.one * unpressableScale;
            if (buttonText != null)
            {
                buttonText.color = unselectableTextColor;

            }
            if (outlineImage != null)
            {
                outlineImage.color = unselectableTextColor;
            }

            GetComponent<Button>().interactable = false;
            img.color = unselectableColor;

            if (unselectableObject != null)
            {
                unselectableObject.SetActive(true);
            }
        }
        else if (redo)
        {
            GetComponent<Button>().interactable = true;
            img.color = normalColor;

        }

        if (!redo)
            normalLocalY = transform.localPosition.y;

        buttonListener = listener;
    }

    public void PressButton()
    {
        if (buttonListener != null)
        {
            if (!isPressed)
            {
                HapticFeedbackManager.instance.PressUIButton();
            }
            buttonListener.Press(index, buttonType);
        }

    }
    public float GetLocalX()
    {
        return transform.localPosition.x;
    }
    public void SetText(string text)
    {
        Debug.Log($"Setting text for button with index {index} and type {buttonType}: {text}");
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }
    public void CheckIfIndex(int check)
    {
        Debug.Log($"Checking if button index {index} matches check value {check} for button type {buttonType}");

        if (index == check)
        {

            isPressed = true;
            activateOnPress?.SetActive(true);
            if (doPosition)
                transform.localPosition = new Vector3(transform.localPosition.x, normalLocalY + addedLocalY, transform.localPosition.z);
            transform.localScale = Vector3.one * pressedScale;
            if (buttonListener != null && buttonText != null)
            {
                Debug.Log($"Button Pressed: {buttonText.text}");
                buttonListener.GetText(buttonText.text, buttonType);
            }
            if (doColor)
            {
                img.color = selectedColor;
            }
        }
        else
        {

            isPressed = false;
            activateOnPress?.SetActive(false);
            if (doPosition)
                transform.localPosition = new Vector3(transform.localPosition.x, normalLocalY, transform.localPosition.z);
            transform.localScale = Vector3.one * normalScale;
            if (doColor)
            {
                img.color = normalColor;
            }
        }
    }

    // Update is called once per frame

}
