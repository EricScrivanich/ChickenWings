using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LevelLockedManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private RectTransform TextBox;
    [SerializeField] private SceneManagerSO sceneSO;
    [SerializeField] private List<LevelButton> LevelButtons;
    [SerializeField] private CanvasGroup Group;

    public static Action<int, bool> OnShowLevelLocked;
    // Start is called before the first frame update
    void Start()
    {
        Group.alpha = 0;
        Group.gameObject.SetActive(false);
    }

    public void CheckLockedLevel(int levelNum, bool isLevel)
    {
        if (levelNum == -1)
        {
            Text.text = ("This Gamemode is not yet Available");
        }
        else
        {
            string type = "";
            if (isLevel) type = "Level";
            else type = "Gamemode";
            string s = sceneSO.ReturnLevelName(levelNum);
            Text.text = ("You Must Beat Level " + levelNum.ToString() + " (" + s + ") to Unlock this " + type);
        }





        Group.alpha = 0;
        Group.gameObject.SetActive(true);
        TextBox.anchoredPosition = Vector2.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(Group.DOFade(1, .4f).SetEase(Ease.OutSine));
        sequence.Join(TextBox.DOAnchorPosY(15, 1.4f).SetEase(Ease.InOutSine));

        sequence.Append(TextBox.DOAnchorPosY(0, .75f).SetEase(Ease.InSine));
        sequence.Append(TextBox.DOAnchorPosY(-30, .45f));
        sequence.Join(Group.DOFade(0, .7f).SetEase(Ease.InOutSine));


        sequence.Play().SetUpdate(true).OnComplete(() => Group.gameObject.SetActive(false));

    }

    private void OnEnable()
    {
        LevelLockedManager.OnShowLevelLocked += CheckLockedLevel;
    }
    private void OnDisable()
    {
        LevelLockedManager.OnShowLevelLocked -= CheckLockedLevel;
    }

    // Update is called once per frame

}
