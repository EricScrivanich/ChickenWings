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
    private bool isUnknown = false;




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
    private bool specialType = false;
    public void SetDataAgain(int index)
    {
        currentType = (ushort)index;
        text.text = typesByIndex[currentType];
        specialType = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(string type, string[] s, int setIndex = -1, bool unkown = false)
    {
        Type = type;

        if (unkown)
        {
            isUnknown = true;
            if (type == "Health") text.text = "Health: ?";
            else text.text = "?";
            fillImage.color = LevelRecordManager.instance.colorSO.UnSelctableUIColor;
            fillImage.enabled = true;
            return;
        }
        else if (isUnknown)
        {
            fillImage.enabled = false;
            fillImage.color = LevelRecordManager.instance.colorSO.SelctedUIColor;
            isUnknown = false;
        }


        if ((s == null || s.Length <= 0) && type != "Health")
        {
            openListButton = true;


            return;
        }
        openListButton = false;

        typesByIndex = s;
        if (setIndex == -1)
        {
            if (type != "Trigger" && type != "Health")
                currentType = LevelRecordManager.instance.currentSelectedObject.Data.type;
            isListPanel = false;
        }
        else
        {
            currentType = (ushort)setIndex;
            isListPanel = true;
        }
        if (type == "Health")
        {
            currentType = (ushort)LevelRecordManager.instance.currentSelectedObject.Data.health;
            text.text = "Health: " + currentType.ToString();


        }
        else
            text.text = typesByIndex[currentType];
    }

    public void ClickArrow(bool right)
    {
        if (isUnknown) return;
        if (openListButton) return;
        if (!isSelected) OnPress();
        int addedIndex = 1;

        if (!right) addedIndex = -1;

        int nextIndex = currentType + addedIndex;
        if (Type == "Health")
        {
            if (nextIndex < 1) nextIndex = 1;
            else if (nextIndex > 99) nextIndex = 99;

        }
        else
        {
            if (nextIndex < 0) nextIndex = typesByIndex.Length - 1;
            else if (nextIndex >= typesByIndex.Length) nextIndex = 0;

        }




        ushort val = (ushort)nextIndex;
        currentType = val;


        if (isListPanel)
        {
            DynamicValueAdder.instance.EditTypeValue(Type, currentType);

        }
        else if (specialType)
        {
            Debug.LogError("trying to set trigger type to: " + (short)val);
            LevelRecordManager.instance.currentSelectedObject.Data.triggerType = (short)val;
        }
        else if (Type == "Health")
        {
            LevelRecordManager.instance.currentSelectedObject.SetHealth((int)val);
            text.text = "Health: " + val.ToString();
            return;
        }

        else
        {
            if (LevelRecordManager.instance.multipleObjectsSelected && LevelRecordManager.instance.MultipleSelectedObjects.Count > 1)
            {
                foreach (var obj in LevelRecordManager.instance.MultipleSelectedObjects)
                {
                    obj.Data.type = val;
                    obj.UpdateObjectData();
                }
            }
            else
            {
                LevelRecordManager.instance.currentSelectedObject.Data.type = val;
                LevelRecordManager.instance.currentSelectedObject.UpdateObjectData();
            }


        }


        text.text = typesByIndex[val];
    }

    public void SetIfSelected(string type)
    {

        if (isUnknown) return;

        if (isSelected && Type != type)
        {
            isSelected = false;
            fillImage.enabled = false;


        }



    }
    public void OnPress()
    {
        if (isUnknown) return;
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
