using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropZone : MonoBehaviour
{
    private Sequence flashSeq;
    private SpriteRenderer sr;

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {



    }

    public void Initilaize(float x)
    {
        sr.color = color2;
        transform.position = new Vector2(x, 0);

        gameObject.SetActive(true);

        sr.DOFade(color2.a, .75f).From(0).OnComplete(FlashSeqence).SetEase(Ease.InSine);
    }

    public void FadeOut()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();

        sr.DOFade(0, .7f).SetEase(Ease.OutSine).OnComplete(() => gameObject.SetActive(false));

    }


    private void FlashSeqence()
    {
        flashSeq = DOTween.Sequence();

        flashSeq.Append(sr.DOColor(color1, .8f).From(color2).SetEase(Ease.InOutSine));
        flashSeq.Append(sr.DOColor(color2, .8f).SetEase(Ease.InOutSine));

        flashSeq.Play().SetLoops(-1);

    }


}
