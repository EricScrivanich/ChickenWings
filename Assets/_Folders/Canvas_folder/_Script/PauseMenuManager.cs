using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;



public class PauseMenuManager : MonoBehaviour
{


    // private PauseMenuButton pauseButton;
    [SerializeField] private SceneManagerSO sceneSO;
    [SerializeField] private TextMeshProUGUI scoreWarning;
    [SerializeField] private TextMeshProUGUI challengeWarning;
    [SerializeField] private TextMeshProUGUI levelWarning;

    [SerializeField] private CanvasGroup gameSpeedGroup;
    [SerializeField] private TextMeshProUGUI gameSpeedText;
    [SerializeField] private ChallengesUIManager challenges;

    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private float displayedGameSpeed;


    [SerializeField] private Image DarkPanel;



    [SerializeField] private RectTransform target;


    private Vector2 endPosition = new Vector2(0, -830);
    private Vector2 startPos = new Vector2(0, 80);

    private Sequence sequence;

    // public void SetPauseButton(PauseMenuButton pmb)
    // {
    //     pauseButton = pmb;

    // }
    // This method toggles the pause state of the game.

    private void Awake()
    {
        displayedGameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1);
        DarkPanel.GetComponent<RectTransform>().position = Vector2.zero;
        DarkPanel.gameObject.SetActive(false);

    }
    private void Start()
    {
        scoreWarning.enabled = false;
        challengeWarning.enabled = false;
        levelWarning.enabled = false;


        CheckWarnings();

    }


    private void CheckWarnings()
    {
        if (FrameRateManager.under1)
        {
            if (sceneSO.isLevel)
            {
                challengeWarning.enabled = true;

                if (FrameRateManager.under085)
                {
                    levelWarning.enabled = true;
                }

            }
            else
            {

                scoreWarning.enabled = true;
            }
        }

    }
    public void DropSignTween()
    {
        FadeDarkPanel(true);
        StartCoroutine(UnlockButtons());
        PauseButtonActions.lockButtons = true;

        target.anchoredPosition = startPos;

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
                .SetUpdate(true);
    }

    private void FadeDarkPanel(bool fadeIn)
    {
        if (fadeIn)
        {
            DarkPanel.gameObject.SetActive(true);
            DarkPanel.DOFade(.78f, .5f).SetEase(Ease.InOutSine).From(0).SetUpdate(true);
            gameSpeedGroup.DOFade(1, .65f).From(0).SetUpdate(true).SetEase(Ease.InSine);

        }
        else
        {
            StartCoroutine(DarkPanelFadeOut());
        }
    }


    private void OnEnable()
    {



        leftArrow.enabled = true;
        rightArrow.enabled = true;
        if (displayedGameSpeed <= .7f)
            leftArrow.enabled = false;
        else if (displayedGameSpeed >= 2)
            rightArrow.enabled = false;

        gameSpeedText.text = displayedGameSpeed.ToString("F2");

        var c = LevelDataConverter.instance.ReturnLevelData().GetLevelChallenges(false, null);
        if (c.LevelDifficulty > 0)
            challenges.ShowChallengesForLevelPicker(c, LevelDataConverter.instance.ReturnLevelSavedData());
        else
            challenges.gameObject.SetActive(false);

    }
    public void SaveOrDefault(bool saveNew)
    {
        if (saveNew)
        {
            PlayerPrefs.SetFloat("GameSpeed", displayedGameSpeed);
            PlayerPrefs.Save();

            if (displayedGameSpeed < .85f)
            {
                FrameRateManager.under085 = true;
                FrameRateManager.under1 = true;

            }
            else if (displayedGameSpeed < 1)
            {
                FrameRateManager.under1 = true;
                challenges.TurnChallengesRed();
            }
            FrameRateManager.OnChangeGameTimeScale?.Invoke(displayedGameSpeed >= 1);
            CheckWarnings();

            FrameRateManager.TargetTimeScale = FrameRateManager.BaseTimeScale * PlayerPrefs.GetFloat("GameSpeed", 1);
        }


        else
        {

            displayedGameSpeed = 1;
            gameSpeedText.text = displayedGameSpeed.ToString("F2");
        }











    }

    public void ChangeGameSpeed(bool isRight)
    {
        float x = .05f;


        if (!isRight) x = -.05f;
        displayedGameSpeed += x;
        displayedGameSpeed = Mathf.Round(displayedGameSpeed * 100f) / 100f;

        leftArrow.enabled = true;
        rightArrow.enabled = true;

        if (displayedGameSpeed <= .7f)
            leftArrow.enabled = false;
        else if (displayedGameSpeed >= 1.5f)
            rightArrow.enabled = false;

        gameSpeedText.text = displayedGameSpeed.ToString("F2");


    }

    public void RestoreToSavedValues()
    {
        displayedGameSpeed = PlayerPrefs.GetFloat("GameSpeed", 1);
        gameSpeedText.text = displayedGameSpeed.ToString("F2");

    }

    private IEnumerator DarkPanelFadeOut()
    {
        yield return new WaitForSecondsRealtime(.7f);
        DarkPanel.DOFade(0, .2f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(SetDarkPanelUnactive);
        gameSpeedGroup.DOFade(0, .2f).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    private void SetDarkPanelUnactive()
    {
        DarkPanel.gameObject.SetActive(false);
    }

    public void InstantPause()
    {
        PauseButtonActions.lockButtons = false;
        target.anchoredPosition = endPosition;

        DarkPanel.DOFade(.78f, 0);

    }

    private IEnumerator UnlockButtons()
    {
        yield return new WaitForSecondsRealtime(.6f);
        PauseButtonActions.lockButtons = false;


    }



    public void RetractOnly()
    {


        FadeDarkPanel(false);
        float currentPos = target.anchoredPosition.y - 70;


        Tweener bounceTween = target.DOAnchorPosY(currentPos, .35f).SetEase(Ease.OutSine).SetUpdate(true);





        Tweener riseTween = target.DOAnchorPosY(startPos.y, .5f)
           .SetEase(Ease.InSine)
           .SetUpdate(true);


        sequence = DOTween.Sequence();
        sequence.Append(bounceTween).SetUpdate(true);
        sequence.Append(riseTween)
                .OnComplete(SetUnactive)
                .SetUpdate(true);

    }



    private void SetUnactive()
    {
        gameObject.SetActive(false);
    }

    // This method sets the game into a paused or unpaused state.



    // This method is called to return to the main menu.


    // Called when the application is paused or resumed.






}

