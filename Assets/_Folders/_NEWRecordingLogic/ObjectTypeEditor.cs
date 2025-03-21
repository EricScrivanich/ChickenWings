using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectTypeEditor : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI text;
    private string Type;

    private string[] typesByIndex;
    private short currentType;
    private Image fillImage;
    private bool isSelected;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }
    private void OnEnable()
    {
        fillImage.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(string type, string[] s)
    {
        Type = type;
        typesByIndex = s;
        currentType = LevelRecordManager.instance.currentSelectedObject.Data.type;
        text.text = typesByIndex[currentType];
    }

    public void ClickArrow(bool right)
    {
        if (!isSelected) OnPress();
        int addedIndex = 1;

        if (!right) addedIndex = -1;

        int nextIndex = currentType + addedIndex;

        if (nextIndex < 0) nextIndex = typesByIndex.Length - 1;
        else if (nextIndex >= typesByIndex.Length) nextIndex = 0;



        short val = (short)nextIndex;
        currentType = val;



        LevelRecordManager.instance.currentSelectedObject.Data.type = val;
        LevelRecordManager.instance.currentSelectedObject.UpdateObjectData();
        text.text = typesByIndex[val];
    }

    public void SetIfSelected(string type)
    {

        if (isSelected && Type != type)
        {
            isSelected = false;
            fillImage.enabled = false;


        }



    }
    public void OnPress()
    {
        isSelected = true;

        ValueEditorManager.instance.SetSelectedType(Type);

        fillImage.enabled = true;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress();
    }
}
