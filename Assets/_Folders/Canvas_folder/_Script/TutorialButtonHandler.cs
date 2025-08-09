using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialButtonHandler : MonoBehaviour
{

    private Vector2 orignalPosition1;
    private Vector2 orignalPosition2;
    private float hiddenAmmount = 500;
    private float showDuration = 2f;
    [SerializeField] private Image handImage;

    [SerializeField] private GameObject[] dropOutlines;
    [SerializeField] private GameObject[] dashOutlines;
    [SerializeField] private GameObject[] flipButtons;
    [SerializeField] private GameObject[] eggButtons;
    [SerializeField] private GameObject dropButton;
    [SerializeField] private GameObject dashButton;
    private Sequence showButtonSeq;
    private Sequence handSlideSeq;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void InitializeTutorialButtons(int levelType, bool doSeq)
    {
        Debug.Log("InitializeTutorialButtons called with levelType: " + levelType + " doSeq: " + doSeq);
        if (doSeq)
            showButtonSeq = DOTween.Sequence();
        if (levelType == 0)
        {

            dropButton.SetActive(false);
            dashButton.SetActive(false);
            for (int i = 0; i < eggButtons.Length; i++)
            {
                eggButtons[i].SetActive(false);
            }
            foreach (var outline in dropOutlines)
            {
                outline.SetActive(false);
            }
            foreach (var outline in dashOutlines)
            {
                outline.SetActive(false);
            }


            if (doSeq)
            {
                var rect1 = flipButtons[0].GetComponent<RectTransform>();
                var rect2 = flipButtons[1].GetComponent<RectTransform>();
                orignalPosition1 = rect1.anchoredPosition;
                orignalPosition2 = rect2.anchoredPosition;
                rect1.anchoredPosition = new Vector2(orignalPosition1.x - hiddenAmmount, orignalPosition1.y);
                rect2.anchoredPosition = new Vector2(orignalPosition2.x + hiddenAmmount, orignalPosition2.y);
                showButtonSeq.Append(rect1.DOAnchorPosX(orignalPosition1.x, showDuration));
                showButtonSeq.Join(rect2.DOAnchorPosX(orignalPosition2.x, showDuration));
                showButtonSeq.Pause();
            }




        }
        else if (levelType == 1)
        {
            dropButton.SetActive(false);

            for (int i = 0; i < eggButtons.Length; i++)
            {
                eggButtons[i].SetActive(false);
            }
            foreach (var outline in dropOutlines)
            {
                outline.SetActive(false);
            }


            if (doSeq)
            {
                var rect = dashButton.GetComponent<RectTransform>();
                orignalPosition1 = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(orignalPosition1.x + hiddenAmmount, orignalPosition1.y);
                showButtonSeq.Append(rect.DOAnchorPosX(orignalPosition1.x, showDuration));
                showButtonSeq.Pause();
            }


        }
        else if (levelType == 2)
        {


            for (int i = 0; i < eggButtons.Length; i++)
            {
                eggButtons[i].SetActive(true);
            }



            if (doSeq)
            {
                var rect = dropButton.GetComponent<RectTransform>();
                orignalPosition1 = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(orignalPosition1.x - hiddenAmmount, orignalPosition1.y);
                showButtonSeq.Append(rect.DOAnchorPosX(orignalPosition1.x, showDuration));
                showButtonSeq.Pause();
            }


        }
        else if (levelType == 3)
        {


            if (doSeq)
            {
                var rect = eggButtons[0].GetComponent<RectTransform>();
                orignalPosition1 = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(orignalPosition1.x, orignalPosition1.y - hiddenAmmount);

                showButtonSeq.Append(rect.DOAnchorPosY(orignalPosition1.y, showDuration));
                showButtonSeq.Pause();
            }

        }


    }

    public void DoHandSlide(bool kill)
    {
        Debug.Log("DoHandSlide called with kill: " + kill);
        if (handSlideSeq != null && handSlideSeq.IsPlaying())
        {
            handSlideSeq.Kill();
        }
        handSlideSeq = DOTween.Sequence();
        if (kill)
        {
            handSlideSeq.Join(handImage.DOFade(0, 0.4f));
            handSlideSeq.Join(handImage.rectTransform.DOScale(1.4f, 0.4f));
            handSlideSeq.Play().SetUpdate(true).OnComplete(() =>
            {
                handImage.gameObject.SetActive(false);
            });
            return;

        }
        handImage.gameObject.SetActive(true);

        handSlideSeq.Append(handImage.rectTransform.DOAnchorPosY(160, 0));
        handSlideSeq.Join(handImage.DOFade(1, 0.8f).From(0).SetEase(Ease.InSine));
        handSlideSeq.Join(handImage.rectTransform.DOScale(.9f, 0.8f).From(1.4f).SetEase(Ease.InSine));
        handSlideSeq.Append(handImage.rectTransform.DOAnchorPosY(-70, 1f).SetEase(Ease.InOutSine));
        handSlideSeq.Append(handImage.DOFade(0, 0.2f));
        handSlideSeq.Join(handImage.rectTransform.DOScale(1.4f, 0.2f));

        handSlideSeq.Play().SetLoops(-1).SetUpdate(true);

    }

    public void FinishTween()
    {
        if (showButtonSeq != null)
        {
            showButtonSeq.Play().SetUpdate(true);
        }
    }

    // Update is called once per frame

}
