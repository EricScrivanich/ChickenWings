using UnityEngine;
using UnityEngine.UI;

public class FolderButton : MonoBehaviour
{
    [field: SerializeField] public int Type { get; private set; }



    [SerializeField] private LevelCreatorColors colorSO;
    private Image img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img = GetComponent<Image>();

        if (Type == 0) OnPress();

    }

    // Update is called once per frame


    public void OnPress()
    {
        transform.localScale = Vector3.one * 1.1f;
        img.color = colorSO.SelctedUIColor;
        FolderManager.instance.SelectFolder(Type);

    }

    public void Unselect()
    {
        transform.localScale = Vector3.one;
        img.color = colorSO.MainUIColor;

    }
}
