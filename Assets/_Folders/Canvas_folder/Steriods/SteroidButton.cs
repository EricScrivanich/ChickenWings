using UnityEngine;
using UnityEngine.UI;

public class SteroidButton : UIButton
{
    [SerializeField] private GameObject steroidUI;

    [SerializeField] private Image highlightImage;
    [SerializeField] private float highlightScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnPress(bool fromCursor = false)
    {
        if (InputSystemSelectionManager.instance != null)
            InputSystemSelectionManager.instance.HandleGroup(0, false);
        Instantiate(steroidUI, GameObject.Find("Canvas").transform);
        // HapticFeedbackManager.instance.PressUIButton();
        OnHighlight(false);
    }

    public override void OnHighlight(bool highlight, bool fromCursor = false)
    {
        if (!highlight)
        {
            transform.localScale = Vector3.one;
            highlightImage.enabled = false;

        }
        else
        {
            transform.localScale = Vector3.one * highlightScale;
            highlightImage.enabled = true;

        }

    }
}
