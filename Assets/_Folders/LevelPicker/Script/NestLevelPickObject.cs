using UnityEngine;
using DG.Tweening;
public class NestLevelPickObject : MonoBehaviour, ILevelPickerPathObject
{
    [SerializeField] private int type;
    [SerializeField] private int pathIndex;
    [SerializeField] private int order;

    [SerializeField] private Transform linePosition;
    private Sequence arrowSequence;
    [SerializeField] private Vector3 level_World_Number_Special;
    [SerializeField] private SpriteRenderer nestBlur;
    [SerializeField] private SpriteRenderer pigBlur;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Transform selectedArrowTransform;
    [SerializeField] private float seqDur;
    private bool isSelected = false;

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
    private void Awake()
    {
        pigBlur.color = selectedColor;
        nestBlur.color = selectedColor;
        selectedArrowTransform.GetComponent<SpriteRenderer>().color = selectedColor;
        nestBlur.enabled = false;
        pigBlur.enabled = false;
        selectedArrowTransform.localPosition = new Vector3(0, 2.5f, 0);
        selectedArrowTransform.gameObject.SetActive(false);
    }

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
        arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.7f, seqDur).SetEase(Ease.OutQuad).From(2.5f));
        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 180, 0), seqDur).SetEase(Ease.InSine).From(Vector3.zero));
        arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.5f, seqDur).SetEase(Ease.InQuad));
        // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 360, 0), seqDur).SetEase(Ease.OutSine));
        arrowSequence.SetLoops(-1);
    }
    public void SetSelected(bool selected)
    {

        if (selected)
        {
            nestBlur.enabled = true;
            pigBlur.enabled = true;
            selectedArrowTransform.gameObject.SetActive(true);
        }
        else
        {
            nestBlur.enabled = false;
            pigBlur.enabled = false;

        }
        DoSequence(selected);
        // Implement selection logic here if needed
        // For example, change color or scale to indicate selection
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
