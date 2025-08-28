using UnityEngine;

public interface ILevelPickerPathObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3Int Return_Type_PathIndex_Order();

    Vector2 ReturnLinePostion();
    Vector3Int ReturnWorldNumber();
    Vector3 ReturnPosScaleBackHill();
    Vector3 ReturnPosScaleFrontHill();

    void SetLastSelectable(Vector3Int num);

    void SetSelected(bool selected);
    void DoStarSeq(int index, bool enterTween);


}
