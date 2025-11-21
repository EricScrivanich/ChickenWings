using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialButtonHandler : MonoBehaviour
{

    [SerializeField] private CanvasGroup handGroup;
    [SerializeField] private Image handImage;
    [SerializeField] private Image circleImage;
    [SerializeField] RectTransform handPostionsAim1;
    [SerializeField] RectTransform handPostionsAim2;

    [SerializeField] RectTransform handPostionsSwitch;
    [SerializeField] RectTransform handPostionTapCenter;
    [SerializeField] private Image centerImage;




    private Sequence handSlideSeq;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handGroup.gameObject.SetActive(false);


    }



    public void DoHandSlide(bool kill, int type, string device)
    {

        Debug.Log("DoHandSlide called with kill: " + kill);
        if (handSlideSeq != null && handSlideSeq.IsPlaying())
        {
            handSlideSeq.Kill();
        }
        handSlideSeq = DOTween.Sequence();
        bool tap = false;

        if (kill && handSlideSeq != null && handSlideSeq.IsPlaying())
        {
            // handSlideSeq.Join(handImage.DOFade(0, 0.4f));
            // handSlideSeq.Join(handImage.rectTransform.DOScale(1.4f, 0.4f));
            // handSlideSeq.Play().SetUpdate(true).OnComplete(() =>
            // {
            //     handImage.gameObject.SetActive(false);
            // });
            if (type == 0) centerImage.DOFade(0, .4f);
            handGroup.DOFade(0, .5f).SetUpdate(true).OnComplete(() =>
             {
                 handGroup.gameObject.SetActive(false);
             });
            return;

        }
        Vector2 start = handPostionsAim1.position;
        Vector2 end = handPostionsAim2.position;
        if (type == 0)
        {
            start = handPostionTapCenter.position;
            tap = true;

        }
        else if (type == 1)
        {
            circleImage.gameObject.SetActive(false);


        }
        else if (type == 2)
        {
            start = handPostionsSwitch.position;

            tap = true;

            if (device != "Touchscreen") return;
        }
        handGroup.GetComponent<RectTransform>().position = start;

        handGroup.alpha = 0;

        handGroup.gameObject.SetActive(true);
        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.AppendInterval(.4f);
        fadeSeq.Append(handGroup.DOFade(.9f, .9f).SetEase(Ease.InSine).SetUpdate(true));



        if (tap)
        {
            handSlideSeq.AppendInterval(0.4f);
            handSlideSeq.Append(handImage.rectTransform.DOScale(.7f, .45f).From(1).SetEase(Ease.InSine));
            handSlideSeq.Join(circleImage.rectTransform.DOScale(1.2f, 1.1f).From(.2f).SetEase(Ease.InSine));
            handSlideSeq.Join(circleImage.DOFade(0f, 1.4f).From(1).SetEase(Ease.InBack));

            // if (type == 0)
            // {
            //     handSlideSeq.Join(centerImage.DOFade(.4f, .6f).From(0));
            // }
            handSlideSeq.Append(handImage.rectTransform.DOScale(1, .6f).SetEase(Ease.OutSine));
            // if (type == 0)
            // {
            //     handSlideSeq.Join(centerImage.DOFade(0, .6f));
            // }

        }
        else
        {
            handSlideSeq.AppendInterval(0.4f);
            handSlideSeq.Append(handImage.rectTransform.DOAnchorPosY(160, 0));
            handSlideSeq.Join(handImage.DOFade(1, 0.8f).From(0).SetEase(Ease.InSine));
            handSlideSeq.Join(handImage.rectTransform.DOScale(.9f, 0.8f).From(1.4f).SetEase(Ease.InOutSine));
            handSlideSeq.Append(handImage.rectTransform.DOAnchorPosY(-70, 1f).SetEase(Ease.InOutSine));
            handSlideSeq.Append(handImage.DOFade(0, 0.2f));
            handSlideSeq.Join(handImage.rectTransform.DOScale(1.4f, 0.2f));

        }




        handSlideSeq.Play().SetLoops(-1).SetUpdate(true);

    }


    // Update is called once per frame

}
