using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LevelGroups : MonoBehaviour
{
    [SerializeField] private float moveArrowAmount;
    [SerializeField] private float moveArrowInDuration;
    [SerializeField] private float moveArrowOutDuration;
    private Sequence MoveArrrowSeq;
    [SerializeField] private Color normalRedColor;
    [SerializeField] private SceneManagerSO sceneSO;
    [SerializeField] private Color normalWhiteColor;
    [SerializeField] private Color disabledRedColor;
    [SerializeField] private Color disabledWhiteColor;
    [SerializeField] private CanvasGroup leftArrow;
    [SerializeField] private CanvasGroup rightArrow;

    [SerializeField] private GameObject intro;
    [SerializeField] private List<GameObject> Tank;
    [SerializeField] private List<GameObject> Pig;
    [SerializeField] private List<GameObject> DeviledEgg;
    [SerializeField] private List<GameObject> Heli;
    private List<GameObject> activeList;

    [SerializeField] private TextMeshProUGUI starCount;
    [SerializeField] private TextMeshProUGUI badgeCount;


    [SerializeField] private List<LevelButton> levelButtons;

    private Sequence seq;
    private bool isMoving = false;

    [SerializeField] private List<RectTransform> levelGroups;
    private int currentLevelGroupIndex = 0;

    public void SetLevelNumbers()
    {
        for (int i = 1; i < levelButtons.Count; i++)
        {

            levelButtons[i].levelNum = i + 1;
        }
    }
    private void Awake()
    {
        SetLevelNumbers();
    }

    void Start()
    {

        CheckButtonsToShow();

        for (int i = 0; i < levelGroups.Count; i++)
        {
            if (i == currentLevelGroupIndex)
            {
                levelGroups[i].localPosition = Vector2.zero;

            }
            else
            {
                levelGroups[i].gameObject.SetActive(false);
            }


        }

        int totalStars = 0;
        int totalBadges = 0;
        for (int i = 1; i < sceneSO.ReturnNumberOfLevels(); i++)
        {
            if (sceneSO.ReturnChallengeCountByLevel(i) > 0)
            {
                totalStars += sceneSO.ReturnChallengeCountByLevel(i);
                totalBadges++;


            }
        }
        starCount.text = SaveManager.instance.ReturnCompletedChallenges(0).ToString() + "/" + totalStars.ToString();
        badgeCount.text = SaveManager.instance.ReturnCompletedChallenges(1).ToString() + "/" + totalBadges.ToString();

    }

    public void LeftButtonClick()
    {
        if (currentLevelGroupIndex > 0 && !isMoving)
        {


            isMoving = true;
            HapticFeedbackManager.instance.PressUIButton();
            levelGroups[currentLevelGroupIndex - 1].gameObject.SetActive(true);
            int c = currentLevelGroupIndex;
            MoveArrrowSeq = DOTween.Sequence();
            MoveArrrowSeq.Append(leftArrow.transform.DOLocalMoveX(-moveArrowAmount, moveArrowInDuration));
            MoveArrrowSeq.Append(leftArrow.transform.DOLocalMoveX(moveArrowAmount, moveArrowOutDuration));
            MoveArrrowSeq.Play();

            seq = DOTween.Sequence();


            seq.Append(levelGroups[currentLevelGroupIndex].DOLocalMoveX(2400, .5f).OnComplete(() => levelGroups[c].gameObject.SetActive(false)));


            seq.Append(levelGroups[currentLevelGroupIndex - 1].DOLocalMoveX(0, .5f).SetEase(Ease.OutBack).From(-2400));

            currentLevelGroupIndex--;
            CheckButtonsToShow();
            seq.Play().OnComplete(() => isMoving = false);
        }

    }
    public void RightButtonClick()
    {
        HapticFeedbackManager.instance.PressUIButton();

        if (currentLevelGroupIndex < levelGroups.Count - 1 && !isMoving)
        {
            isMoving = true;
            MoveArrrowSeq = DOTween.Sequence();
            MoveArrrowSeq.Append(rightArrow.transform.DOLocalMoveX(moveArrowAmount, moveArrowInDuration));
            MoveArrrowSeq.Append(rightArrow.transform.DOLocalMoveX(-moveArrowAmount, moveArrowOutDuration));
            MoveArrrowSeq.Play();
            seq = DOTween.Sequence();
            int c = currentLevelGroupIndex;
            seq.Append(levelGroups[currentLevelGroupIndex].DOLocalMoveX(-2400, .5f).SetEase(Ease.InBack).OnComplete(() => levelGroups[c].gameObject.SetActive(false)));

            levelGroups[currentLevelGroupIndex + 1].gameObject.SetActive(true);
            seq.Append(levelGroups[currentLevelGroupIndex + 1].DOLocalMoveX(0, .5f).SetEase(Ease.OutBack).From(2400));

            currentLevelGroupIndex++;
            CheckButtonsToShow();

            seq.Play().OnComplete(() => isMoving = false);
        }

    }

    // private void SetColor(i, bool disable)
    // {
    //     if (disable)
    //     {

    //     }
    //     else
    //     {
    //         leftAndRightButtonArrows[i].color = normalWhiteColor;
    //         leftAndRightButtonFills[i].color = normalRedColor;
    //     }

    // }

    public void CheckButtonsToShow()
    {
        if (currentLevelGroupIndex == 0)

            leftArrow.DOFade(0, .3f);

        else if (currentLevelGroupIndex >= levelGroups.Count - 1)
            rightArrow.DOFade(0, .3f);


        else
        {
            if (leftArrow.alpha == 0)
                leftArrow.DOFade(1, .35f);
            if (rightArrow.alpha == 0)
                rightArrow.DOFade(1, .35f);

        }



    }


    // Start is called before the first frame update


    public void Return()
    {
        Invoke("Reset", .65f);

    }
    private void Reset()
    {
        if (activeList != null)
        {
            foreach (var obj in activeList)
            {
                obj.SetActive(false);
            }
            intro.SetActive(true);
        }
    }

    public void Switch(int n)
    {

        if (activeList != null)
        {
            foreach (var obj in activeList)
            {
                obj.SetActive(false);
            }
        }
        intro.SetActive(false);


        switch (n)
        {
            case 1:
                SetListActive(Tank);
                activeList = Tank;
                break;
            case 2:
                SetListActive(Pig);
                activeList = Pig;


                break;
            case 3:
                SetListActive(DeviledEgg);
                activeList = DeviledEgg;


                break;
            case 4:
                SetListActive(Heli);
                activeList = Heli;

                break;

            default:

                break;
        }


    }

    void SetListActive(List<GameObject> l)
    {
        foreach (var obj in l)
        {
            obj.SetActive(true);
        }

    }

    // Update is called once per frame

}
