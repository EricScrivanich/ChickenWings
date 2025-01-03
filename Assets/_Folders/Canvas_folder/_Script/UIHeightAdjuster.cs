using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIHeightAdjuster : MonoBehaviour
{
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private TextMeshProUGUI targetText;  // Reference to the text component
    [SerializeField] private TextMeshProUGUI difficultyText;  // Reference to the text component
    [SerializeField] private RectTransform image1;
    [SerializeField] private Image outlineImage;
    private Image fillImage;

    private int progressTextsShown = 0;

    [SerializeField] private TextMeshProUGUI[] progressTexts;

    [SerializeField] private float additionalPaddingForProgressTexts;

    [SerializeField] private float baseHeight = 100f;     // Base height for one line of text
    [SerializeField] private float extraLineHeight = 20f; // Additional height for each extra line
    [SerializeField] private float padding = 10f;

    private Sequence starHitSeq;

    private Sequence seq;



    [SerializeField] private Image StarImage;

    private Color disabledStarColor;

    private LevelChallenges currentChallengeSO;
    private int index;









    private void OnTextChanged(Object obj)
    {
        AdjustHeight();
    }

    public void SetText(string s)
    {
        targetText.text = s;

        AdjustHeight();


    }

    public void FadeOut()
    {



    }



    public void SetChallenge(LevelChallenges challengeSO, int i, string challenge, string difficulty, bool hasCompleted)
    {
        if (image1 == null) image1 = GetComponent<RectTransform>();
        if (fillImage == null) fillImage = GetComponent<Image>();
        currentChallengeSO = challengeSO;
        index = i;
        targetText.text = challenge;
        difficultyText.text = difficulty;

        progressTextsShown = currentChallengeSO.ReturnAmountOfNeededProgressTexts(i);

        if (progressTextsShown > 0)
        {
            padding += additionalPaddingForProgressTexts;
            for (int n = 0; n < progressTextsShown; n++)
            {
                progressTexts[n].gameObject.SetActive(true);
                Vector2 prog = currentChallengeSO.ReturnCurrentProgressByChallengeIndex(i, n);
                progressTexts[n].text = prog.x.ToString() + "/" + prog.y.ToString();
            }
        }



        if (hasCompleted)
        {
            disabledStarColor = colorSO.StarGottenColor;
            // outlineImage.color = colorSO.StarCardGoldOutlineColor2;
        }
        else
        {
            disabledStarColor = colorSO.StarNoneColor;
            // outlineImage.color = colorSO.StarCardDisabledOutlineColor;
        }

        AdjustHeight();
        // CheckIfChallengeComplete();


    }


    public void MoveOnEnable(int type, float dur)
    {


        transform.DOLocalMoveX(0, dur).SetEase(Ease.OutBack).SetUpdate(true);

    }

    private void OnDisable()
    {
        if (seq != null && seq.IsPlaying())
            seq.Kill();

        if (starHitSeq != null && starHitSeq.IsPlaying())
            starHitSeq.Kill();
        DOTween.Kill(this);

    }
    public void SetPosition(float xOffset)
    {
        transform.localPosition = new Vector2(xOffset, transform.localPosition.y);
    }

    private void SetCardOnPause(int type)
    {
        if (progressTextsShown > 0)
        {
            padding += additionalPaddingForProgressTexts;
            for (int n = 0; n < progressTextsShown; n++)
            {
                progressTexts[n].gameObject.SetActive(true);
                Vector2 prog = currentChallengeSO.ReturnCurrentProgressByChallengeIndex(index, n);
                progressTexts[n].text = prog.x.ToString() + "/" + prog.y.ToString();
            }
        }
        if (type == 0)
        {
            fillImage.color = colorSO.StarCardDisabledFillColor;
            outlineImage.color = colorSO.StarCardDisabledOutlineColor;
            StarImage.color = disabledStarColor;

        }
        else if (type == 1)
        {
            seq = DOTween.Sequence();

            seq.Append(StarImage.DOColor(colorSO.StarNormalColor, 1.2f).From(disabledStarColor));
            seq.Append(StarImage.DOColor(disabledStarColor, 1.2f));
            seq.Play().SetUpdate(true).SetLoops(-1);
        }
        else if (type == 2)
        {

            StarImage.color = colorSO.StarNormalColor;
        }
    }

    public void ShowChallengesAtLevelStart()
    {
        StarImage.color = disabledStarColor;
    }

    public void CheckIfChallengeComplete(int type)
    {
        switch (type)
        {
            case 0:

                SetCardOnPause(currentChallengeSO.CheckChallengeType(index));


                break;

            case 1:
                StarImage.color = disabledStarColor;
                transform.DOLocalMoveX(0, .4f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(Case1Function);

                break;
            case 2:

                break;
        }


    }

    public void OnCompletedStarHit()
    {
        starHitSeq = DOTween.Sequence();
        transform.DOShakeRotation(.3f, 30, 8, 50).SetUpdate(true);
        starHitSeq.Append(outlineImage.DOColor(colorSO.StarCardGoldOutlineColor1, .4f));
        starHitSeq.Join(fillImage.DOColor(colorSO.StarCardGoldFillColor1, .4f));
        starHitSeq.Append(outlineImage.DOColor(colorSO.StarCardGoldOutlineColor2, .4f).SetEase(Ease.OutSine));
        starHitSeq.Join(fillImage.DOColor(colorSO.StarCardGoldFillColor2, .4f).SetEase(Ease.OutSine));
        starHitSeq.Play().SetUpdate(true);

    }


    private void Case1Function()
    {
        if (currentChallengeSO.CheckChallengeCompletion(index))
            ChallengesUIManager.OnGetCompletedStar?.Invoke(index, StarImage.transform);
        else
        {

            starHitSeq = DOTween.Sequence();
            starHitSeq.AppendInterval(.45f);
            starHitSeq.AppendCallback(() => AudioManager.instance.PlayErrorSound(true));
            starHitSeq.Append(fillImage.DOColor(colorSO.StarCardDisabledFillColor, .4f));

            starHitSeq.Join(outlineImage.DOColor(colorSO.StarCardDisabledOutlineColor, .4f));
            starHitSeq.Join(transform.DOShakeRotation(.45f, 35, 8, 50));
            starHitSeq.Play().SetUpdate(true).OnComplete(() => ChallengesUIManager.OnFailedChallenge?.Invoke(index));
        }


    }

    public void TurnRed()
    {
        fillImage.DOColor(colorSO.StarCardDisabledFillColor, .3f).SetUpdate(true);

        outlineImage.DOColor(colorSO.StarCardDisabledOutlineColor, .3f).SetUpdate(true);

        if (seq != null && seq.IsPlaying())
            seq.Kill();

        StarImage.color = disabledStarColor;

    }


    private void AdjustHeight()
    {
        if (targetText == null || image1 == null) return;

        // Force TextMeshPro to update text layout to get accurate line count
        targetText.ForceMeshUpdate();

        // Get the number of lines in the current text
        int lineCount = targetText.textInfo.lineCount;

        // Calculate the new height: baseHeight for one line + extraLineHeight for each additional line
        float newHeight = baseHeight + (Mathf.Max(0, lineCount - 1) * extraLineHeight) + padding;

        // Apply the new height to the image
        image1.sizeDelta = new Vector2(image1.sizeDelta.x, newHeight);
    }
}