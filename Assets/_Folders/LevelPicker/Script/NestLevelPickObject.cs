using UnityEngine;

public class NestLevelPickObject : MonoBehaviour, ILevelPickerPathObject
{
    [SerializeField] private int type;
    [SerializeField] private int pathIndex;
    [SerializeField] private int order;

    [SerializeField] private Transform linePosition;

    [SerializeField] private Vector3 level_World_Number_Special;

    public Vector2 ReturnLinePostion()
    {
        if (linePosition != null)
        {
            return linePosition.position;
        }
        else
        {
            return transform.position;
        }
    }

    public Vector3Int Return_Type_PathIndex_Order()
    {
        return new Vector3Int(type, pathIndex, order);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
}
