using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;


public class ChallengesUIManager : MonoBehaviour
{

    private bool doBadgeSeq = false;
    [SerializeField] private RectTransform challengeCardParent;
    [SerializeField] private float moveCardDur;
    [SerializeField] private float spacingBetweenCards;
    [SerializeField] private float cardScale;

    private Vector2 startBadgePos;
    [SerializeField] private Image certifiedBadge;
    [SerializeField] private GameObject certifiedBadgePrev;
    [SerializeField] private SceneManagerSO sceneSO;
    [SerializeField] private GameObject challengeUIPrefab;
    private Sequence badgeSeq;
    private Sequence starSeq;

    private CanvasGroup group;



    [SerializeField] private float scaleForLevelStart;

    [SerializeField] private GameObject NoChallenges;

    [SerializeField] private float fadeInFinishedDur;
    [SerializeField] private float betweenStarTime;
    [SerializeField] private Vector2 starStartPos;
    [SerializeField] private float starStartScale;
    [SerializeField] private float starTweenDur;

    [SerializeField] private int shownType;

    private Sequence levelStartSeq;

    [SerializeField] private GameObject StarSprite;

    public static Action<int, Transform> OnGetCompletedStar;

    public static Action<int> OnFailedChallenge;

    private int currentCheckedChallengeIndex;

    private bool challengesAvailable = true;




    private UIHeightAdjuster[] challengeCards;

    [SerializeField] private float startCardOffset;
    [SerializeField] private float initalMoveDelay;
    [SerializeField] private float betweenMoveDelay;
    [SerializeField] private float moveDuration;



    [SerializeField] private float padding;

    private LevelChallenges currentLevelChallenge;
    private bool hasInitialized = false;

    private Coroutine MoveCardsRoutine;
    [SerializeField] private bool skipStart = false;

    private bool doMoveCardSeq = false;


    // Start is called before the first frame update


    void Start()
    {
        if (skipStart)
        {
            hasInitialized = true;
            return;
        }

        hasInitialized = true;
        group = GetComponent<CanvasGroup>();

        if (shownType == 1)
        {
            certifiedBadge.gameObject.SetActive(false);
            certifiedBadgePrev.gameObject.SetActive(false);

        }

        currentLevelChallenge = sceneSO.ReturnLevelChallenges();
        if (currentLevelChallenge == null)
        {
            // NoChallenges.SetActive(true);

            if (shownType == 1)
            {
                bool[] data = new bool[2];
                if (!FrameRateManager.under085)
                    data[0] = true;
                data[1] = false;
                SaveManager.instance.UpdateLevelData(sceneSO.ReturnLevelNumber(), data);
            }
            challengesAvailable = false;
            gameObject.SetActive(false);
            return;
        }
        else
        {
            NoChallenges.SetActive(false);
        }


        bool[] savedData = SaveManager.instance.GetSavedLevelData(sceneSO.ReturnLevelNumber());



        if (shownType == 1)
        {
            if (savedData[1]) certifiedBadgePrev.SetActive(true);
            else certifiedBadgePrev.SetActive(false);

        }



        // for (int b = 0; b < savedData.Length; b++)
        // {
        //     Debug.LogError("Saved Index: " + b + " is: " + savedData[b]);
        // }


        if (shownType == 2)
        {

            padding *= .38f;

        }



        float currentYPosition = 0f; // Starting Y position for the first challenge
        challengeCards = new UIHeightAdjuster[currentLevelChallenge.challengeTexts.Length];

        for (int i = 0; i < currentLevelChallenge.challengeTexts.Length; i++)
        {
            // Instantiate the challenge UI prefab and get its UIHeightAdjuster component
            var challengeCard = Instantiate(challengeUIPrefab, transform).GetComponent<UIHeightAdjuster>();

            // Set the text for the challenge and difficulty
            challengeCard.SetChallenge(currentLevelChallenge, i, currentLevelChallenge.challengeTexts[i], currentLevelChallenge.challengeDifficulties[i], savedData[i + 2]);

            challengeCards[i] = challengeCard;

            // Adjust the position of the newly instantiated challenge card
            RectTransform cardRect = challengeCard.GetComponent<RectTransform>();

            // Position the card at the current Y position, moving it downward based on the previous height and padding
            cardRect.anchoredPosition = new Vector2(0, currentYPosition);

            // Calculate the new Y position for the next card
            float cardHeight = cardRect.sizeDelta.y;
            Debug.Log("CARD HEIGHT" + cardHeight);
            currentYPosition -= (cardHeight + padding); // Move down by height of card + padding


        }

        // CardActionsBasedOnShownType();

        if (FrameRateManager.under1) TurnChallengesRed();

    }

    public void TurnChallengesRed()
    {
        if (challengeCards == null || challengeCards.Length <= 0)
            return;
        foreach (var item in challengeCards)
        {
            item.TurnRed();
        }
    }

    public void FadeOut(float dur)
    {
        group.DOFade(0, dur).SetUpdate(true).SetEase(Ease.InSine);
    }

    public void ShowChallengesForLevelPicker(LevelChallenges challenges, LevelSavedData savedLevelData, bool finisheLevel = false)
    {
        Debug.Log("Showing Challenges for Level Picker");
        currentLevelChallenge = challenges;
        float currentYPosition = 0f; // Starting Y position for the first challenge
        doMoveCardSeq = finisheLevel;

        // challengeCards = null;
        if (challengeCards == null)
        {
            NoChallenges.SetActive(false);
            if (challengeCardParent.GetComponent<VerticalLayoutGroup>() != null)
                challengeCardParent.GetComponent<VerticalLayoutGroup>().spacing = spacingBetweenCards;
            challengeCardParent.localScale = BoundariesManager.vectorThree1 * cardScale;
            challengeCards = new UIHeightAdjuster[currentLevelChallenge.challengeTexts.Length];
            for (int i = 0; i < currentLevelChallenge.challengeTexts.Length; i++)
            {
                // Instantiate the challenge UI prefab and get its UIHeightAdjuster component
                var challengeCard = Instantiate(challengeUIPrefab, challengeCardParent).GetComponent<UIHeightAdjuster>();

                // Set the text for the challenge and difficulty

                // setting completed to false since we are testing, (Last Bool parameter)
                Debug.Log("Setting Challenge Card: " + i + " with text: " + currentLevelChallenge.challengeTexts[i] + " and difficulty: " + currentLevelChallenge.challengeDifficulties[i]);
                challengeCard.SetChallenge(currentLevelChallenge, i, currentLevelChallenge.challengeTexts[i], currentLevelChallenge.challengeDifficulties[i], savedLevelData.ChallengeCompletion[i]);
                // log saved level data name and challenge completion
                Debug.Log("Saved Level Data: " + savedLevelData.LevelName + " Challenge Completion: " + savedLevelData.ChallengeCompletion[i]);

                challengeCards[i] = challengeCard;

                // Adjust the position of the newly instantiated challenge card
                // RectTransform cardRect = challengeCard.GetComponent<RectTransform>();

                // // Position the card at the current Y position, moving it downward based on the previous height and padding
                // cardRect.anchoredPosition = new Vector2(0, currentYPosition);

                // // Calculate the new Y position for the next card
                // float cardHeight = cardRect.sizeDelta.y;
                // Debug.Log("CARD HEIGHT" + cardHeight);
                // currentYPosition -= (cardHeight + padding); 


            }


        }
        else
        {
            for (int i = 0; i < challengeCards.Length; i++)
            {
                challengeCards[i].SetChallenge(currentLevelChallenge, i, currentLevelChallenge.challengeTexts[i], currentLevelChallenge.challengeDifficulties[i], savedLevelData.ChallengeCompletion[i]);
            }

        }





        Debug.Log("Current Level Challenge: " + currentLevelChallenge.challengeTexts.Length + " " + challengeCards.Length);
        if (challengeCards.Length != currentLevelChallenge.challengeTexts.Length)
        {
            Debug.Log("Challenge Cards Length Changed: " + challengeCards.Length + " != " + currentLevelChallenge.challengeTexts.Length);
            // Clear existing challenge cards if the length has changed
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            challengeCards = new UIHeightAdjuster[currentLevelChallenge.challengeTexts.Length];


        }


        if (finisheLevel)
        {
            group = GetComponent<CanvasGroup>();
            Debug.LogError("Doing finsih level Challenge Tween");
            group.DOFade(1, fadeInFinishedDur).SetUpdate(true).SetEase(Ease.InSine).OnComplete(() =>
            {
                StartCoroutine(LevelFinishedRoutine());
            });

        }




    }

    private WaitForEndOfFrame lineCountDelay = new WaitForEndOfFrame();
    private IEnumerator DelayToLineCount()
    {
        yield return lineCountDelay;

        for (int i = 0; i < challengeCards.Length; i++)
        {
            challengeCards[i].AdjustHeight();
        }

        if (doMoveCardSeq)
        {
            SetCardPositionsForFinsihLevel();
        }

    }

    private void HandleBadge(bool doTween)
    {
        if (doTween)
        {
            startBadgePos = certifiedBadge.rectTransform.localPosition;

            certifiedBadge.rectTransform.localPosition = new Vector2(startBadgePos.x + 350, startBadgePos.y + 1200);

            certifiedBadge.rectTransform.localScale = BoundariesManager.vectorThree1 * 6.5f;
            certifiedBadge.rectTransform.eulerAngles = Vector3.zero;
            certifiedBadge.gameObject.SetActive(true);
            doBadgeSeq = true;


        }
        else
        {
            doBadgeSeq = false;
            certifiedBadge.gameObject.SetActive(false);
        }
    }

    private void TweenBadge()
    {
        badgeSeq = DOTween.Sequence();
        badgeSeq.Append(certifiedBadge.rectTransform.DOLocalMove(startBadgePos, .7f));
        badgeSeq.Join(certifiedBadge.rectTransform.DOScale(1, .7f).SetEase(Ease.InBack));
        badgeSeq.Join(certifiedBadge.rectTransform.DORotate(Vector3.forward * -12.5f, .7f));

        badgeSeq.Play().SetUpdate(true).SetEase(Ease.InSine).OnComplete(() => AudioManager.instance.PlayLevelFinishSounds(3));

    }




    private IEnumerator LevelFinishedRoutine()
    {
        for (int i = 0; i < challengeCards.Length; i++)
        {

            var rect = challengeCards[i].GetComponent<RectTransform>();

            rect.DOLocalMoveX(245, moveCardDur).SetEase(Ease.OutBack).SetUpdate(true);
            yield return new WaitForSecondsRealtime(moveCardDur - .1f);
            Debug.LogError("Moving Card: " + i + " to position: " + rect.localPosition.x + " with size: " + rect.sizeDelta.y);

            int c = currentLevelChallenge.ReturnChallengeCompletion(i, true);
            if (c <= 0)
            {
                challengeCards[i].TurnRed();


            }
            else if (c == 1)
            {
                GetCompletedStar(challengeCards[i].ReturnStarTransform(), i);
            }
            yield return new WaitForSecondsRealtime(betweenStarTime);

        }



    }
    private void SetCardPositionsForFinsihLevel()
    {
        Debug.Log("Setting Card Positions for Finish Level");
        float baseX = 1300f;
        float baseY = 27f;

        for (int i = 0; i < challengeCards.Length; i++)
        {
            RectTransform rect = challengeCards[i].GetComponent<RectTransform>();
            rect.localPosition = new Vector2(baseX, baseY);
            baseY -= (rect.sizeDelta.y + spacingBetweenCards);
            Debug.Log("Setting Card Position: " + i + "With size: " + rect.sizeDelta.y + " and spacing: " + spacingBetweenCards);


        }


    }

    private void GetCompletedStar(Transform par, int i)
    {

        var obj = Instantiate(StarSprite, par);
        var rect = obj.GetComponent<RectTransform>();
        AudioManager.instance.PlayLevelFinishSounds(2);
        rect.localPosition = new Vector2(starStartPos.x, starStartPos.y);
        rect.localScale = BoundariesManager.vectorThree1 * starStartScale;
        challengeCards[i].StartStarHit(starTweenDur);

        // if (starSeq != null && starSeq.IsPlaying())
        //     starSeq.Kill();

        Sequence newSeq = DOTween.Sequence();
        // newSeq.AppendInterval(.2f);
        newSeq.Append(rect.DOLocalMove(Vector2.zero, starTweenDur).SetEase(Ease.InSine));
        newSeq.Join(rect.DOScale(BoundariesManager.vectorThree1, starTweenDur).SetEase(Ease.InBack));
        newSeq.Join(rect.DORotate(Vector3.zero, starTweenDur - .1f).From(Vector3.forward * 720).SetEase(Ease.InOutSine).OnComplete(() => AudioManager.instance.PlayLevelFinishSounds(1)));

        newSeq.Play().SetUpdate(true).OnComplete(() => challengeCards[i].OnCompletedStarHit());
        //  starSeq.Play().SetUpdate(true).OnComplete(() => CheckNextChallenge(n + 1));

    }

    private void StarHit(int n)
    {
        AudioManager.instance.PlayLevelFinishSounds(1);
        challengeCards[n].OnCompletedStarHit();

    }

    private void CheckNextChallenge(int i)
    {
        if (i >= challengeCards.Length)
        {
            if (doBadgeSeq) TweenBadge();
            return;
        }
        challengeCards[i].CheckIfChallengeComplete(shownType);
    }

    private void FailedChallenge(int i)
    {
        CheckNextChallenge(i + 1);
    }


    private void SaveLevelCompletion()
    {
        int challengeCount = currentLevelChallenge.GetAmountOfChallenges();
        bool[] data = new bool[challengeCount + 2];
        Debug.LogError("Checking Level Complettion Bruh");
        if (!FrameRateManager.under085)
        {
            data[0] = true;
        }



        bool completedAll = true;

        for (int i = 0; i < challengeCount; i++)
        {
            bool complete = currentLevelChallenge.CheckChallengeCompletion(i);
            if (FrameRateManager.under1) complete = false;

            if (!complete) completedAll = false;

            data[i + 2] = complete;
        }

        data[1] = completedAll;
        HandleBadge(completedAll);

        for (int b = 0; b < data.Length; b++)
        {
            Debug.LogError("Saving Data: " + b + " is: " + data[b]);
        }

        SaveManager.instance.UpdateLevelData(sceneSO.ReturnLevelNumber(), data);

    }
    private void OnEnable()
    {
        StartCoroutine(DelayToLineCount());
        // ChallengesUIManager.OnGetCompletedStar += GetCompletedStar;
        // ChallengesUIManager.OnFailedChallenge += FailedChallenge;
        // if (!hasInitialized || !challengesAvailable) return;

        // CardActionsBasedOnShownType();
        // group.alpha = 1;





    }

    private void OnDisable()
    {
        // ChallengesUIManager.OnGetCompletedStar -= GetCompletedStar;
        // ChallengesUIManager.OnFailedChallenge -= FailedChallenge;

        if (MoveCardsRoutine != null)
            StopCoroutine(MoveCardsRoutine);

        if (levelStartSeq != null && levelStartSeq.IsPlaying())
            levelStartSeq.Kill();

        if (badgeSeq != null && badgeSeq.IsPlaying())
            badgeSeq.Kill();

        if (starSeq != null && starSeq.IsPlaying())
            starSeq.Kill();
    }
    // Update is called once per frame

}
