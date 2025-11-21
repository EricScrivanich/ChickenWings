using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : UIButton
{

    private ILevelPickerPathObject level;
    private LevelPickerManager manager;
    private RectTransform buttonRect;

    // public void SetAction(LevelPickerManager man)
    // {
    //     if (manager != null)
    //     {
    //         manager = man;
    //         man.OnUpdateUIPostions += UpdateRectPosition;
    //        
    //     }


    // }


    // void OnDisable()
    // {
    //     if (manager != null)
    //     {
    //         manager.OnUpdateUIPostions -= UpdateRectPosition;
    //     }
    // }

    public void SetLevelPickerObject(ILevelPickerPathObject levelPickerObject)
    {
        level = levelPickerObject;
        buttonRect = GetComponent<RectTransform>();

    }

    public void UpdateRectPosition()
    {
        Vector2 canvasPoint = Camera.main.WorldToScreenPoint(level.transform.position);

        buttonRect.position = canvasPoint;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnPress(bool fromCursor = false)
    {
        level.OnPress(fromCursor);
    }
    public override void OnHighlight(bool highlight, bool fromCursor = false)
    {
        level.OnHighlight(highlight);
    }
}
