using UnityEngine;
using UnityEngine.UI;


public class ButtonTouchByIndex : MonoBehaviour
{

    public int index { get; private set; }
    private IButtonListener buttonListener;
    [SerializeField] private GameObject activateOnPress;
    [SerializeField] private float pressedScale;
    [SerializeField] private float unpressableScale;
    private float normalLocalY;
    [SerializeField] private float addedLocalY;
    private bool isPressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void SetData(int newIndex, IButtonListener listener, bool unpress)
    {
        index = newIndex;
        if (unpress)
        {
            transform.localScale = Vector3.one * unpressableScale;
            GetComponent<Button>().interactable = false;
            GetComponent<Image>().color = new Color(.7f, .7f, .7f, 1);

        }

        normalLocalY = transform.localPosition.y;

        buttonListener = listener;
    }

    public void PressButton()
    {
        if (buttonListener != null)
        {
            buttonListener.Press(index);
        }

    }
    public void CheckIfIndex(int check)
    {

        if (index == check)
        {
            if (!isPressed) HapticFeedbackManager.instance.PlayerButtonPress();
            isPressed = true;
            activateOnPress?.SetActive(true);
            transform.localPosition = new Vector3(transform.localPosition.x, normalLocalY + addedLocalY, transform.localPosition.z);
            transform.localScale = Vector3.one * pressedScale;
        }
        else
        {
            isPressed = false;
            activateOnPress?.SetActive(false);
            transform.localPosition = new Vector3(transform.localPosition.x, normalLocalY, transform.localPosition.z);
            transform.localScale = Vector3.one;
        }
    }

    // Update is called once per frame

}
