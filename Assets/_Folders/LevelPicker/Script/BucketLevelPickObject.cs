using UnityEngine;
using DG.Tweening;

public class BucketLevelPickObject : MonoBehaviour, ILevelPickerPathObject
{
    [SerializeField] private SpriteRenderer blur;
    [SerializeField] private Transform selectedArrowTransform;
    [field: SerializeField] public Vector3Int WorldNumber { get; private set; }
    [SerializeField] private Transform linePos;
    [SerializeField] private int type;
    [SerializeField] private int pathIndex;
    [SerializeField] private int order;
    [SerializeField] private Color unbeatenColor;
    [SerializeField] private Color beatenColor;

    [SerializeField] private Vector2 backHillPos;
    [SerializeField] private float backHillScale;
    [SerializeField] private Vector2 frontHillPos;
    [SerializeField] private float frontHillScale;
    [SerializeField] private string AnimTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (!string.IsNullOrEmpty(AnimTrigger))
        // {
        //     GetComponent<Animator>().SetTrigger(AnimTrigger);
        // }

    }
    void Awake()
    {
        blur.enabled = false;
        selectedArrowTransform.gameObject.SetActive(false);
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
        if (LevelDataConverter.instance.CheckIfCompletedLevel(WorldNumber))
        {
            blur.color = beatenColor;
            selectedArrowTransform.GetComponent<SpriteRenderer>().color = beatenColor;

        }
        else
        {
            blur.color = unbeatenColor;
            selectedArrowTransform.GetComponent<SpriteRenderer>().color = unbeatenColor;
        }

    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            blur.enabled = true;
            selectedArrowTransform.gameObject.SetActive(true);
        }
        else
        {
            blur.enabled = false;



        }

        DoSequence(selected);
    }
    private Sequence arrowSequence;

    private void DoSequence(bool doSeq)
    {
        if (arrowSequence != null && arrowSequence.IsActive())
        {
            arrowSequence.Kill();
        }
        if (!doSeq)
        {
            selectedArrowTransform.gameObject.SetActive(false);
            return;
        }

        arrowSequence = DOTween.Sequence();
        arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.7f, .3f).SetEase(Ease.OutQuad).From(2.5f));
        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 180, 0), .3f).SetEase(Ease.InSine).From(Vector3.zero));
        arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.5f, .3f).SetEase(Ease.InQuad));
        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 360, 0), seqDur).SetEase(Ease.OutSine));
        arrowSequence.SetLoops(-1);
    }

    public void DoStarSeq(int index, bool enterTween)
    {

    }
}
