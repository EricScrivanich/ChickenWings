using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LevelLockedManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private RectTransform TextBox;
    [SerializeField] private List<LevelButton> LevelButtons;
    [SerializeField] private CanvasGroup Group;
    // Start is called before the first frame update
    void Start()
    {
        Group.alpha = 0;
        Group.gameObject.SetActive(false);
    }

    public void CheckLockedLevel(int levelNum)
    {
        foreach (var script in LevelButtons)
        {
            if (script.levelNum == levelNum - 1)
            {
                Text.text = ("You must beat level " + script.levelNum.ToString() + " (" + script.LevelName() + ") to unlock this level.");
                break;
            }
        }

        Group.alpha = 0;
        Group.gameObject.SetActive(true);
        TextBox.anchoredPosition = Vector2.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(Group.DOFade(1, .5f).SetEase(Ease.OutSine));
        sequence.Join(TextBox.DOAnchorPosY(15, 1.8f).SetEase(Ease.InOutSine));

        sequence.Append(TextBox.DOAnchorPosY(0, .75f).SetEase(Ease.InSine));
        sequence.Append(TextBox.DOAnchorPosY(-30, .45f));
        sequence.Join(Group.DOFade(0, .7f).SetEase(Ease.InOutSine));


        sequence.OnComplete(() => Group.gameObject.SetActive(false));

    }

    // Update is called once per frame

}
