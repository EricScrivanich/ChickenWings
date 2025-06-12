using UnityEngine;

public class ChicAndCageRecordable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private RecordableObjectPlacer attachedObjectData;
    [SerializeField] SpriteRenderer outlineBlur;

    private void Awake()
    {


        outlineBlur.color = LevelRecordManager.instance.colorSO.SelectedPigOutlineColor;
        outlineBlur.enabled = false;


    }

    public void SetSelected(bool isSelected)
    {

        outlineBlur.enabled = isSelected;
    }


    public void SetData(RecordableObjectPlacer data)
    {
        attachedObjectData = data;
    }

    public void RemoveSelf()
    {
        attachedObjectData.SetCageAttachment(false);
        Debug.Log("Removing ChicAndCageRecordable from " + attachedObjectData);
    }
}
