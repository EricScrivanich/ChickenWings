using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class ILevelPickerPathObject : MonoBehaviour
{
    [SerializeField] private Transform pathTransform;

    [field: SerializeField] public float uiScaleChangeMult { get; protected set; } = 1f;

    [SerializeField] protected SpriteRenderer blurMain;
    [SerializeField] protected SpriteRenderer blurOther;
    [SerializeField] protected Transform arrow;
    protected Sequence arrowSequence;



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
    protected bool isSelected = false;
    public string message;

    void OnEnable()
    {
        arrow.gameObject.SetActive(false);
        blurMain.enabled = false;
        if (blurOther != null) blurOther.enabled = false;
    }
    void OnDisable()
    {
        // buttonRect = null;
        if (arrowSequence != null && arrowSequence.IsActive())
        {
            arrowSequence.Kill();
        }
    }

    public void SetButtonRect(RectTransform rect)
    {

        Vector2 canvasPoint = Camera.main.WorldToScreenPoint(transform.position);
        rect.GetComponent<LevelButtonUI>().SetLevelPickerObject(this);
        rect.localScale = Vector3.one * (uiScaleChangeMult * transform.localScale.y);
        rect.position = canvasPoint;
    }




    public virtual void OnHighlight(bool highlight)
    {
        if (isSelected) return;
        DoHighlightSequence(highlight);
    }

    public virtual void OnPress()
    {
        LevelPickerManager.OnLevelPickerObjectSelected?.Invoke(this);

    }




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
        isSelected = selected;
        if (selected)
        {
            Color col = new Color(blurMain.color.r, blurMain.color.g, blurMain.color.b, 1f);
            blurMain.color = col;
            blurMain.enabled = true;


            if (blurOther != null)
            {
                blurOther.color = col;
                blurOther.enabled = true;
            }
            arrow.gameObject.SetActive(true);


        }
        else
        {
            blurMain.enabled = false;
            if (blurOther != null) blurOther.enabled = false;

        }
        DoSequence(selected);

    }

    private void DoSequence(bool doSeq)
    {
        if (arrowSequence != null && arrowSequence.IsActive())
        {
            arrowSequence.Kill();
        }
        if (!doSeq)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrowSequence = DOTween.Sequence();
        arrowSequence.Append(arrow.DOLocalMoveY(2.7f, .3f).SetEase(Ease.OutQuad).From(2.5f));
        // arrowSequence.Join(arrow.DORotate(new Vector3(0, 180, 0), .3f).SetEase(Ease.InSine).From(Vector3.zero));
        arrowSequence.Append(arrow.DOLocalMoveY(2.5f, .3f).SetEase(Ease.InQuad));
        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 360, 0), seqDur).SetEase(Ease.OutSine));
        arrowSequence.SetLoops(-1);
    }

    private void DoHighlightSequence(bool doSeq)
    {
        if (arrowSequence != null && arrowSequence.IsActive())
        {
            arrowSequence.Kill();

        }
        if (!doSeq)
        {
            arrow.gameObject.SetActive(false);
            blurMain.enabled = false;
            if (blurOther != null) blurOther.enabled = false;
            return;
        }
        else
        {
            arrow.gameObject.SetActive(true);
            blurMain.enabled = true;
            if (blurOther != null) blurOther.enabled = true;
        }

        arrowSequence = DOTween.Sequence();
        arrowSequence.Append(arrow.DOLocalMoveY(1.3f, .3f).SetEase(Ease.OutQuad).From(1f));
        arrowSequence.Join(blurMain.DOFade(.6f, .3f).From(0f));
        // arrowSequence.Join(arrow.DORotate(new Vector3(0, 180, 0), .3f).SetEase(Ease.InSine).From(Vector3.zero));
        arrowSequence.Append(arrow.DOLocalMoveY(1f, .3f).SetEase(Ease.InQuad));
        arrowSequence.Join(blurMain.DOFade(0, .3f));

        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 360, 0), seqDur).SetEase(Ease.OutSine));
        arrowSequence.SetLoops(-1);
    }
    public virtual void DoStarSeq(int index, bool enterTween)
    {

    }


}
