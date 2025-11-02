using UnityEngine;


public class ILevelPickerPathObject : MonoBehaviour
{
    [SerializeField] private Transform pathTransform;
    public int layerID = 1;
    public int challengeType = 0;
    public int layersShownFrom = 0;

    public int difficultyType;
    protected Color unbeatenColor = new Color(1f, .08f, .13f, 1f);
    protected Color beatenColor = new Color(.99f, .87f, 0, 1f);
    public Vector4 CameraPositionAndOrhtoSize;




    // Start is called once before the first exe
    // cution of Update after the MonoBehaviour is created


    public Vector3Int RootIndex_PathIndex_Order;



    public Vector3Int WorldNumber;
    public string message;


    public Vector2 ReturnLinePostion()
    {
        if (pathTransform != null)
        {
            return pathTransform.position;
        }
        return transform.position;
    }


    public Vector3 PosScaleBackHill;
    public Vector3 PosScaleFrontHill;

    public virtual void SetLastSelectable(Vector3Int num)
    {

    }
    public virtual void SetLayerFades(int layer, float dur, DG.Tweening.Ease ease)
    {

    }

    public virtual void SetSelected(bool selected)
    {

    }
    public virtual void DoStarSeq(int index, bool enterTween)
    {

    }


}
