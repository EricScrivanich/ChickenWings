using UnityEngine;
using UnityEngine.UI;

public class SteroidButton : MonoBehaviour
{
    [SerializeField] private GameObject steroidUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPress()
    {
        InputSystemSelectionManager.instance.HandleGroup(0, false);
        Instantiate(steroidUI, GameObject.Find("Canvas").transform);
        HapticFeedbackManager.instance.PressUIButton();
    }
}
