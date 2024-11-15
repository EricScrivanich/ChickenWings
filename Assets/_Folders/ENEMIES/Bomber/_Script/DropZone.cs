using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropZone : MonoBehaviour
{
    private Sequence flashSeq;
    private Sequence arrowsSeq;
    private SpriteRenderer sr;
    [SerializeField] private SpriteRenderer arrows;
    private int flipDirectionInt;
    [SerializeField] private Transform warningTransform;
    [SerializeField] private SpriteRenderer[] warningSprites;

    private Vector2 normalDropZoneScale = new Vector2(3.2f, 2.8f);

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private Color arrowColor1;
    [SerializeField] private Color arrowColor2;
    private Color arrowColorStart = new Color(1, .9f, .9f, 0);

    [SerializeField] private GameObject dropZone;

    private void Awake()
    {
        sr = dropZone.GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {



    }

    public void Initilaize(float x, float scaleMultiplier, float planeX, int flipInt)
    {

        sr.color = color2;
        transform.position = new Vector2(x, 0);
        flipDirectionInt = flipInt;
        arrows.color = arrowColorStart;

        if (flipInt >= 0) arrows.flipX = false;
        else arrows.flipX = true;




        dropZone.transform.localScale = normalDropZoneScale * scaleMultiplier;
        gameObject.SetActive(true);
        if (Mathf.Abs(planeX) < 11.3f)
        {
            warningTransform.gameObject.SetActive(true);

            warningTransform.position = new Vector2(planeX, BoundariesManager.TopViewBoundary - 1.8f);
            if (flipInt == -1) warningSprites[2].flipX = true;
            else warningSprites[2].flipX = false;

            // warningSprites[2].transform.lo

            foreach (var s in warningSprites)
            {
                s.DOFade(1, .6f).From(0);
            }


            WarningTween();

        }
        else
        {
            warningTransform.gameObject.SetActive(false);
        }




        // DoArrowSeq();
        sr.DOFade(color2.a, .75f).From(0).OnComplete(FlashSeqence).SetEase(Ease.InSine);
        arrows.DOColor(arrowColor1, .73f).SetEase(Ease.OutSine);
    }

    public void FadeOut()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();
        if (arrowsSeq != null && arrowsSeq.IsPlaying())
            arrowsSeq.Kill();

        sr.DOFade(0, .7f).SetEase(Ease.OutSine).OnComplete(() => gameObject.SetActive(false));

        arrows.DOFade(0, .6f);

    }

    private void DoArrowSeq()
    {

        arrowsSeq = DOTween.Sequence();

        arrowsSeq.Append(arrows.DOColor(arrowColor1, .7f).SetEase(Ease.OutSine));
        arrowsSeq.AppendInterval(.3f);


        arrowsSeq.Play();

    }

    private void WarningTween()
    {
        var warningSeq = DOTween.Sequence();

        warningSeq.Append(warningSprites[1].transform.DOLocalMoveY(1.15f, .4f).From(.95f));
        warningSeq.Join(warningSprites[2].transform.DOLocalMoveX(-.15f * flipDirectionInt, .4f).From(0));
        warningSeq.Append(warningSprites[1].transform.DOLocalMoveY(.95f, .3f));
        warningSeq.Join(warningSprites[2].transform.DOLocalMoveX(0, .3f));

        warningSeq.Play().SetLoops(3).OnComplete(FadeOutWarning);

    }

    private void FadeOutWarning()
    {
        foreach (var s in warningSprites)
        {
            s.DOFade(0, .35f);
        }

    }
    private void FlashSeqence()
    {
        flashSeq = DOTween.Sequence();

        flashSeq.Append(sr.DOColor(color1, .8f).From(color2).SetEase(Ease.InOutSine));
        flashSeq.Join(arrows.DOColor(arrowColor2, .8f).From(arrowColor1));
        flashSeq.Append(sr.DOColor(color2, .8f).SetEase(Ease.InOutSine));
        flashSeq.Join(arrows.DOColor(arrowColor1, .8f));

        flashSeq.Play().SetLoops(-1);

    }


}
