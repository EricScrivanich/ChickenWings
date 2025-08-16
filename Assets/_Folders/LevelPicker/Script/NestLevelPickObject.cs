using UnityEngine;
using DG.Tweening;
public class NestLevelPickObject : MonoBehaviour, ILevelPickerPathObject
{
    [field: SerializeField] public Vector3Int WorldNumber { get; private set; }
    [SerializeField] private int type;
    [SerializeField] private int pathIndex;
    [SerializeField] private int order;
    // [SerializeField] private Animator pigAnim;
    [SerializeField] private GameObject sleepingPig;
    [SerializeField] private GameObject flappingPig;
    [SerializeField] private Transform stars;
    [SerializeField] private SpriteRenderer badge;
    private bool doBadgeSeq = false;





    [SerializeField] private Transform linePosition;
    private Sequence arrowSequence;
    [SerializeField] private Vector3 level_World_Number_Special;
    [SerializeField] private SpriteRenderer nestBlur;
    [SerializeField] private SpriteRenderer pigBlur;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Transform selectedArrowTransform;
    [SerializeField] private float seqDur;
    private bool isSelected = false;

    [SerializeField] private Color unbeatenBlurColor;
    [SerializeField] private Color beatenBlurColor;

    [SerializeField] private SpriteRenderer[] starSprites;
    [SerializeField] private SpriteRenderer[] starBlurSprites;



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
    public Vector3Int ReturnWorldNumber()
    {
        return WorldNumber;
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

    public void SetLastSelectable(Vector3Int num)
    {
        if (WorldNumber.x > num.x)
        {
            gameObject.SetActive(false);
            return;

        }

        if (WorldNumber.y == num.y)
        {
            sleepingPig.SetActive(false);
            flappingPig.SetActive(true);
            stars.gameObject.SetActive(false);

        }
        else if (WorldNumber.y < num.y)
        {
            sleepingPig.SetActive(false);
            flappingPig.SetActive(false);
            nestBlur.color = beatenBlurColor;
            selectedArrowTransform.GetComponent<SpriteRenderer>().color = beatenBlurColor;

            if (transform.localScale.x < 0) stars.localScale = new Vector3(-1, 1, 1);
            ReadyStarSeq();
        }
        else
        {
            sleepingPig.SetActive(true);
            flappingPig.SetActive(false);
            stars.gameObject.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created

    }
    private float blurInTime = .9f;
    private float blurOutTime = .8f;
    private float blurInterval = .2f;
    private bool[] challengeCompletion;
    private bool doStarSeq = false;
    public void ReadyStarSeq()
    {
        var data = LevelDataConverter.instance.ReturnCompletedChallengesForLevel(WorldNumber);
        if (data == null)
        {
            doBadgeSeq = false;
            doStarSeq = false;
            for (int i = 0; i < 3; i++)
            {
                starSprites[i].color = Color.black;
                starBlurSprites[i].enabled = false;
            }
            return;
        }

        if (data.MasteredLevel)
        {
            doBadgeSeq = true;
            stars.gameObject.SetActive(false);
            badge.gameObject.SetActive(true);
            return;
        }
        doBadgeSeq = false;
        challengeCompletion = data.ChallengeCompletion;

        if (challengeCompletion == null || challengeCompletion.Length == 0)
        {
            Debug.LogWarning("No challenge completion data found for level: " + WorldNumber);
            return;
        }


        for (int i = 0; i < challengeCompletion.Length; i++)
        {

            if (challengeCompletion[i])
            {

                starSprites[i].color = Color.white;
                starBlurSprites[i].enabled = true;
                if (!doStarSeq) doStarSeq = true;


            }
            else
            {
                // starSequence.AppendInterval(blurInTime + blurInterval);

                starSprites[i].color = Color.black;
                starBlurSprites[i].enabled = false;
                starBlurSprites[i].color = beatenBlurColor;
            }
        }




    }
    private bool badgeIn = false;
    public void DoStarSeq(int index, bool enter)
    {
        if (doBadgeSeq)
        {
            if (index == 0 && enter)
            {
                if (badgeIn)
                {
                    badgeIn = false;
                    badge.DOFade(1, blurInTime * 2).SetUpdate(true).SetEase(Ease.InSine).From(0);
                }
                else
                {
                    badgeIn = true;
                    badge.DOFade(0, blurOutTime * 2).SetUpdate(true);
                }

            }

            return;
        }
        if (!doStarSeq)
            return;

        if (challengeCompletion[index])
        {
            if (enter)
            {
                starBlurSprites[index].DOFade(1, blurInTime).SetUpdate(true).SetEase(Ease.InSine);
            }
            else
            {
                starBlurSprites[index].DOFade(0, blurOutTime).SetUpdate(true);

            }

        }

    }
}