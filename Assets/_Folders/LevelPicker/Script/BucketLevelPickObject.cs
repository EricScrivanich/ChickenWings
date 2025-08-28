using UnityEngine;

public class BucketLevelPickObject : MonoBehaviour, ILevelPickerPathObject
{
    [field: SerializeField] public Vector3Int WorldNumber { get; private set; }
    [SerializeField] private Transform linePos;
    [SerializeField] private int type;
    [SerializeField] private int pathIndex;
    [SerializeField] private int order;

    [SerializeField] private Vector2 backHillPos;
    [SerializeField] private float backHillScale;
    [SerializeField] private Vector2 frontHillPos;
    [SerializeField] private float frontHillScale;
    [SerializeField] private string AnimTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!string.IsNullOrEmpty(AnimTrigger))
        {
            GetComponent<Animator>().SetTrigger(AnimTrigger);
        }

    }

    public Vector3 ReturnPosScaleBackHill()
    {
        return new Vector3(backHillPos.x, backHillPos.y, backHillScale);
    }

    public Vector3 ReturnPosScaleFrontHill()
    {
        return new Vector3(frontHillPos.x, frontHillPos.y, frontHillScale);
    }

    public Vector3Int Return_Type_PathIndex_Order()
    {
        return new Vector3Int(type, pathIndex, order);
    }
    public Vector3Int ReturnWorldNumber()
    {
        return WorldNumber;
    }

    public Vector2 ReturnLinePostion()
    {
        return linePos.position;
    }

    public void SetLastSelectable(Vector3Int num)
    {
        throw new System.NotImplementedException();
    }

    public void SetSelected(bool selected)
    {
        throw new System.NotImplementedException();
    }

    public void DoStarSeq(int index, bool enterTween)
    {
        throw new System.NotImplementedException();
    }
}
