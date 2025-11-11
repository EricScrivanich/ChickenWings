using UnityEngine;
using DG.Tweening;
public class NestLevelPickObject : ILevelPickerPathObject
{

    [SerializeField] private int type;

    [SerializeField] private int pathIndex;
    [SerializeField] private int order;
    // [SerializeField] private Animator pigAnim;
    [SerializeField] private GameObject sleepingPig;
    [SerializeField] private GameObject flappingPig;
    [SerializeField] private Transform flipObjects;
    [SerializeField] private Transform stars;
    [SerializeField] private SpriteRenderer badge;
    [SerializeField] private SpriteRenderer badgeInside;
    private bool doBadgeSeq = false;





    [SerializeField] private Transform linePosition;
    // private Sequence arrowSequence;
    [SerializeField] private Vector3 level_World_Number_Special;
   

    [SerializeField] private Transform selectedArrowTransform;
    [SerializeField] private float seqDur;
    private bool isSelected = false;



    [SerializeField] private SpriteRenderer[] starSprites;
    [SerializeField] private SpriteRenderer[] starBlurSprites;








    private void Awake()
    {
        blurOther.color = unbeatenColor;
        blurMain.color = unbeatenColor;
        // selectedArrowTransform.GetComponent<SpriteRenderer>().color = unbeatenColor;
        // nestBlur.enabled = false;
        // pigBlur.enabled = false;
        // selectedArrowTransform.localPosition = new Vector3(0, 2.5f, 0);
        // selectedArrowTransform.gameObject.SetActive(false);
    }

    // private void DoSequence(bool doSeq)
    // {
    //     if (arrowSequence != null && arrowSequence.IsActive())
    //     {
    //         arrowSequence.Kill();
    //     }
    //     if (!doSeq)
    //     {
    //         selectedArrowTransform.gameObject.SetActive(false);
    //         return;
    //     }

    //     arrowSequence = DOTween.Sequence();
    //     arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.7f, seqDur).SetEase(Ease.OutQuad).From(2.5f));
    //     // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 180, 0), seqDur).SetEase(Ease.InSine).From(Vector3.zero));
    //     arrowSequence.Append(selectedArrowTransform.DOLocalMoveY(2.5f, seqDur).SetEase(Ease.InQuad));
    //     // arrowSequence.Join(selectedArrowTransform.DORotate(new Vector3(0, 360, 0), seqDur).SetEase(Ease.OutSine));
    //     arrowSequence.SetLoops(-1);
    // }
    // public override void SetSelected(bool selected)
    // {

    //     if (selected)
    //     {
    //         nestBlur.enabled = true;
    //         pigBlur.enabled = true;
    //         selectedArrowTransform.gameObject.SetActive(true);

            
    //     }
    //     else
    //     {
    //         nestBlur.enabled = false;
    //         pigBlur.enabled = false;

    //     }
    //     DoSequence(selected);
    //     // Implement selection logic here if needed
    //     // For example, change color or scale to indicate selection
    // }

    public override void SetLastSelectable(Vector3Int num)
    {
        if (WorldNumber.x > num.x)
        {
            sleepingPig.SetActive(true);
            flappingPig.SetActive(false);
            stars.gameObject.SetActive(false);
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
            blurMain.color = beatenColor;
            // selectedArrowTransform.GetComponent<SpriteRenderer>().color = beatenColor;

            if (transform.localScale.x < 0)
            {
                flipObjects.localScale = new Vector3(-1, 1, 1);
            }

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
                starBlurSprites[i].color = beatenColor;
            }
        }




    }
    private bool badgeIn = false;
    private bool fadedFromView = false;
    public override void DoStarSeq(int index, bool enter)
    {
        if (fadedFromView) return;
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
    private Sequence fadeFromView;
    public override void SetLayerFades(int layer, float dur, Ease ease)
    {

        if (!fadedFromView && layerID < layer)
        {
            if (fadeFromView != null && fadeFromView.IsPlaying())
                fadeFromView.Complete();
            fadedFromView = true;



            if (badge.gameObject.activeInHierarchy)
            {
                fadeFromView = DOTween.Sequence();
                DOTween.Kill(badge);
                fadeFromView.Append(badge.DOFade(0, dur * .5f));
                fadeFromView.Join(badgeInside.DOFade(0, dur));
                fadeFromView.Play().SetEase(ease);

            }
            else if (stars.gameObject.activeInHierarchy)
            {
                fadeFromView = DOTween.Sequence();
                fadeFromView.Append(null);
                foreach (var s in starBlurSprites)
                {
                    DOTween.Kill(s);
                    fadeFromView.Join(s.DOFade(0, dur * .5f));
                }
                foreach (var s in starSprites)
                {
                    fadeFromView.Join(s.DOFade(0, dur));
                }
                fadeFromView.Play().SetEase(ease);
            }
        }
        else if (fadedFromView && layerID >= layer)
        {
            if (fadeFromView != null && fadeFromView.IsPlaying())
                fadeFromView.Complete();
            fadedFromView = false;
            bool change = false;
            if (fadeFromView != null && fadeFromView.IsPlaying())
                fadeFromView.Kill();


            if (badge.gameObject.activeInHierarchy)
            {
                change = true;
                fadeFromView = DOTween.Sequence();
                DOTween.Kill(badge);
                ;
                fadeFromView.Append(badgeInside.DOFade(1, dur));


            }
            else if (stars.gameObject.activeInHierarchy)
            {
                change = true;
                fadeFromView = DOTween.Sequence();
                fadeFromView.Append(null);

                foreach (var s in starSprites)
                {
                    fadeFromView.Join(s.DOFade(1, dur));
                }

            }
            if (change) fadeFromView.Play().SetEase(ease).OnComplete(() => fadedFromView = false);

        }
    }

  
    [SerializeField] private Vector2 backHillPos;
    [SerializeField] private float backHillScale;
    [SerializeField] private Vector2 frontHillPos;
    [SerializeField] private float frontHillScale;








}