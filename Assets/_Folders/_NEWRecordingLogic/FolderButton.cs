using UnityEngine;
using UnityEngine.UI;

public class FolderButton : MonoBehaviour
{
    [field: SerializeField] public int Type { get; private set; }



    [SerializeField] private LevelCreatorColors colorSO;
    private Image img;
    private bool isPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img = GetComponent<Image>();

        if (Type == 0)
        {
            isPressed = true;
            transform.localScale = Vector3.one * 1.1f;
            img.color = colorSO.SelctedUIColor;
        }

    }

    // Update is called once per frame


    public void OnPress()
    {
        if (isPressed) return;
        isPressed = true;
        transform.localScale = Vector3.one * 1.1f;
        img.color = colorSO.SelctedUIColor;
        FolderManager.instance.SelectFolder(Type);

    }

    public void Unselect()
    {
        isPressed = false;
        transform.localScale = Vector3.one;
        img.color = colorSO.MainUIColor;

    }
}
