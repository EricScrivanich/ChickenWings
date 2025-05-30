using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectTypeEditor : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI text;
    private string Type;

    private string[] typesByIndex;
    private ushort currentType;
    private Image fillImage;
    private bool isSelected;



    private bool openListButton = false;

    private bool isListPanel = false;

    private void Awake()
    {
        fillImage = GetComponent<Image>();

    }
    private void OnEnable()
    {
        fillImage.enabled = false;
        fillImage.color = LevelRecordManager.instance.colorSO.SelctedUIColor;
        ValueEditorManager.instance.OnSetSelectedType += SetIfSelected;

        if (openListButton)
        {
            int amount = 0;

            switch (Type)
            {
                case "Rotations":
                    amount = LevelRecordManager.instance.currentSelectedObject.rotationData.startingSteps.Count;
                    break;
                case "Positions":
                    amount = LevelRecordManager.instance.currentSelectedObject.positionData.startingSteps.Count;
                    break;
                case "Timers":
                    amount = LevelRecordManager.instance.currentSelectedObject.timerData.startingSteps.Count;
                    if (LevelRecordManager.instance.currentSelectedObject.Title == "Laser")
                    {
                        text.text = "Shooting Durations" + " (" + (amount - 1) + ")";
                        return;
                    }

                    break;
            }

            text.text = Type + " (" + (amount - 1) + ")";
        }


    }
    private void OnDisable()
    {
        ValueEditorManager.instance.OnSetSelectedType -= SetIfSelected;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(string type, string[] s, int setIndex = -1)
    {
        Type = type;


        if (s == null || s.Length <= 0)
        {
            openListButton = true;


            return;
        }

        typesByIndex = s;
        if (setIndex == -1)
        {
            currentType = LevelRecordManager.instance.currentSelectedObject.Data.type;
            isListPanel = false;
        }
        else
        {
            currentType = (ushort)setIndex;
            isListPanel = true;
        }

        text.text = typesByIndex[currentType];
    }

    public void ClickArrow(bool right)
    {
        if (openListButton) return;
        if (!isSelected) OnPress();
        int addedIndex = 1;

        if (!right) addedIndex = -1;

        int nextIndex = currentType + addedIndex;

        if (nextIndex < 0) nextIndex = typesByIndex.Length - 1;
        else if (nextIndex >= typesByIndex.Length) nextIndex = 0;



        ushort val = (ushort)nextIndex;
        currentType = val;

        if (isListPanel)
        {
            DynamicValueAdder.instance.EditTypeValue(Type, currentType);

        }
        else
        {
            LevelRecordManager.instance.currentSelectedObject.Data.type = val;
            LevelRecordManager.instance.currentSelectedObject.UpdateObjectData();
        }


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

        ValueEditorManager.instance.ShowSlider(false);

        ValueEditorManager.instance.OnSetSelectedType?.Invoke(Type);

        if (openListButton)
        {
            ValueEditorManager.instance.ShowListPanel(Type);

            return;
        }

        fillImage.enabled = true;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress();
    }
}
