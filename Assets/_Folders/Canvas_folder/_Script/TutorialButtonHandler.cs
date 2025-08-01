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

    [SerializeField] private GameObject[] dropOutlines;
    [SerializeField] private GameObject[] dashOutlines;
    [SerializeField] private GameObject[] flipButtons;
    [SerializeField] private GameObject[] eggButtons;
    [SerializeField] private GameObject dropButton;
    [SerializeField] private GameObject dashButton;
    private Sequence showButtonSeq;

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

    public void FinishTween()
    {
        if (showButtonSeq != null)
        {
            showButtonSeq.Play().SetUpdate(true);
        }
    }

    // Update is called once per frame

}
