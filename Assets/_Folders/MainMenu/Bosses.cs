using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LevelGroups : MonoBehaviour
{
    [SerializeField] private Color normalRedColor;
    [SerializeField] private Color normalWhiteColor;
    [SerializeField] private Color disabledRedColor;
    [SerializeField] private Color disabledWhiteColor;
    [SerializeField] private Image[] leftAndRightButtonArrows;
    [SerializeField] private Image[] leftAndRightButtonFills;
    [SerializeField] private GameObject intro;
    [SerializeField] private List<GameObject> Tank;
    [SerializeField] private List<GameObject> Pig;
    [SerializeField] private List<GameObject> DeviledEgg;
    [SerializeField] private List<GameObject> Heli;
    private List<GameObject> activeList;

    private Sequence seq;

    [SerializeField] private List<RectTransform> levelGroups;
    private int currentLevelGroupIndex = 0;

    public void LeftButtonClick()
    {
        if (currentLevelGroupIndex > 0)
        {
            HapticFeedbackManager.instance.PressUIButton();
            int c = currentLevelGroupIndex;
            seq = DOTween.Sequence();
            seq.Append(levelGroups[currentLevelGroupIndex].DOLocalMoveX(2400, .45f).OnComplete(() => levelGroups[c].gameObject.SetActive(false)));
            levelGroups[currentLevelGroupIndex - 1].gameObject.SetActive(true);
            seq.Append(levelGroups[currentLevelGroupIndex - 1].DOLocalMoveX(0, .5f).SetEase(Ease.OutBack).From(-2400));
            currentLevelGroupIndex--;
            CheckButtonsToShow();
            seq.Play();
        }

    }
    public void RightButtonClick()
    {
        HapticFeedbackManager.instance.PressUIButton();

        if (currentLevelGroupIndex < levelGroups.Count - 1)
        {
            seq = DOTween.Sequence();
            int c = currentLevelGroupIndex;
            seq.Append(levelGroups[currentLevelGroupIndex].DOLocalMoveX(-2400, .55f).SetEase(Ease.InBack).OnComplete(() => levelGroups[c].gameObject.SetActive(false)));
            levelGroups[currentLevelGroupIndex + 1].gameObject.SetActive(true);
            seq.Append(levelGroups[currentLevelGroupIndex + 1].DOLocalMoveX(0, .75f).SetEase(Ease.OutBack).From(2400));
            currentLevelGroupIndex++;
            CheckButtonsToShow();

            seq.Play();
        }

    }

    private void SetColor(int i, bool disable)
    {
        if (disable)
        {
            leftAndRightButtonArrows[i].color = disabledWhiteColor;
            leftAndRightButtonFills[i].color = disabledRedColor;
        }
        else
        {
            leftAndRightButtonArrows[i].color = normalWhiteColor;
            leftAndRightButtonFills[i].color = normalRedColor;
        }

    }

    public void CheckButtonsToShow()
    {
        if (currentLevelGroupIndex == 0)
            SetColor(0, true);

        else if (currentLevelGroupIndex >= levelGroups.Count - 1)
            SetColor(1, true);


        else
        {
            SetColor(0, false);
            SetColor(1, false);
        }



    }


    // Start is called before the first frame update
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

    }

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
