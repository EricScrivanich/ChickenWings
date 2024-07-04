using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;



public class PauseMenuManager : MonoBehaviour
{


    // private PauseMenuButton pauseButton;

    [SerializeField] private Image DarkPanel;



    [SerializeField] private RectTransform target;

    [SerializeField] private Button[] buttons;
    private Vector2 endPosition = new Vector2(0, -800);

    private Sequence sequence;

    // public void SetPauseButton(PauseMenuButton pmb)
    // {
    //     pauseButton = pmb;

    // }
    // This method toggles the pause state of the game.

    private void Awake()
    {
        DarkPanel.GetComponent<RectTransform>().position = Vector2.zero;
        DarkPanel.gameObject.SetActive(false);

    }
    public void DropSignTween()
    {
        FadeDarkPanel(true);

        target.anchoredPosition = Vector2.zero;

        // Create the main drop tween
        Tweener mainDropTween = target.DOAnchorPos(endPosition, .6f)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        // Create the overshoot tween
        Vector3 overshootPosition = endPosition - new Vector2(0, 40);
        Tweener overshootTween = target.DOAnchorPos(overshootPosition, .9f)
            .SetEase(Ease.OutElastic)
            .SetUpdate(true);

        // Chain the tweens together
        sequence = DOTween.Sequence();
        sequence.Append(mainDropTween)
                .Append(overshootTween)
                 .OnComplete(EnableButtons)
                .SetUpdate(true);
    }

    private void FadeDarkPanel(bool fadeIn)
    {
        if (fadeIn)
        {
            DarkPanel.gameObject.SetActive(true);
            DarkPanel.DOFade(.4f, .5f).SetEase(Ease.InOutSine).From(0).SetUpdate(true);

        }
        else
        {
            StartCoroutine(DarkPanelFadeOut());
        }
    }

    private IEnumerator DarkPanelFadeOut()
    {
        yield return new WaitForSecondsRealtime(.7f);
        DarkPanel.DOFade(0, .2f).SetEase(Ease.InOutSine).From(0).SetUpdate(true).OnComplete(SetDarkPanelUnactive);
    }

    private void SetDarkPanelUnactive()
    {
        DarkPanel.gameObject.SetActive(false);
    }

    public void InstantPause()
    {
        target.anchoredPosition = endPosition;
        EnableButtons();
        DarkPanel.DOFade(.4f, 0);

    }



    public void RetractOnly()
    {
        DisableButtons();
        FadeDarkPanel(false);
        float currentPos = target.anchoredPosition.y - 70;


        Tweener bounceTween = target.DOAnchorPosY(currentPos, .35f).SetEase(Ease.OutSine).SetUpdate(true);





        Tweener riseTween = target.DOAnchorPosY(0, .5f)
           .SetEase(Ease.InSine)
           .SetUpdate(true);


        sequence = DOTween.Sequence();
        sequence.Append(bounceTween).SetUpdate(true);
        sequence.Append(riseTween)
                .OnComplete(SetUnactive)
                .SetUpdate(true);

    }

    private void DisableButtons()
    {
        foreach (var but in buttons)
        {
            but.enabled = false;
        }

    }

    private void EnableButtons()
    {
        foreach (var but in buttons)
        {
            but.enabled = true;
        }

    }

    private void SetUnactive()
    {
        gameObject.SetActive(false);
    }

    // This method sets the game into a paused or unpaused state.



    // This method is called to return to the main menu.


    // Called when the application is paused or resumed.






}
