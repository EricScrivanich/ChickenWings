using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIButton : MonoBehaviour, IDeselectHandler, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
{
    // private Button button;
    [SerializeField] private int buttonType = 0;
    protected bool uiSelected = false;

    private bool returnAll = false;

    void Awake()
    {
        if (GetComponent<Button>() == null)
        {
            returnAll = true;
        }
    }

    public void SetButtonOn(int type)
    {
        if (buttonType == type)
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }


    }

    // 🔹 Called when the button becomes selected (e.g., via gamepad navigation)
    public virtual void OnSelect(BaseEventData eventData)
    {
        if (returnAll) return;
        uiSelected = true;
        OnHighlight(true);
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (returnAll) return;

        uiSelected = false;
        OnHighlight(false);
    }

    // 🔹 Called when the pointer hovers over the button (useful for mouse too)
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (returnAll) return;

        uiSelected = true;
        OnHighlight(true);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (returnAll) return;

        uiSelected = false;
        OnHighlight(false);
    }

    // 🔹 Called when the button is clicked (mouse)
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (returnAll) return;

        OnPress();
    }

    // 🔹 Called when "Submit" (A button, Enter key, etc.) is triggered while selected
    public virtual void OnSubmit(BaseEventData eventData)
    {
        if (returnAll) return;

        OnPress();
    }

    public virtual void OnHighlight(bool isSelected) { }

    public virtual void OnPress() { }
}