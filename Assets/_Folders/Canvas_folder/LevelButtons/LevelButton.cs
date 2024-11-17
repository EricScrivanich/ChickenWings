using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private SceneManagerSO sceneSO;

    [SerializeField] private Image[] stars;
    [SerializeField] private GameObject badge;
    [SerializeField] private SceneManagerSO sceneLoader;
    public int levelNum;
    [SerializeField] private bool isUnlocked;
   
    [SerializeField] private TextMeshProUGUI[] Text;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private RectTransform Lock;
    private Sequence lockMoveTween;


    void Start()
    {
        Text[0].text = sceneSO.ReturnLevelName(levelNum);
        Text[1].text = ("Level " + levelNum.ToString());

        // if (levelNum == 1 || SaveManager.instance.GetSavedLevelData(levelNum - 1)[0])
        //     isUnlocked = true;
        // else isUnlocked = false;


        isUnlocked = SaveManager.instance.HasCompletedLevel(levelNum - 1);

        Lock.gameObject.SetActive(!isUnlocked);

        foreach (var item in stars)
        {
            item.enabled = false;
        }

        if (isUnlocked)
        {
            foreach (var text in Text) text.color = Color.white;
            ButtonImage.color = colorSO.NormalSignButtonColor;
            UnlockedAnimation();

            bool[] data = SaveManager.instance.GetSavedLevelData(levelNum);
            if (data != null && data.Length > 2)
            {
                bool b = data[1];
                badge.SetActive(b);
                if (!b)
                {
                    for (int i = 0; i < data.Length - 2; i++)
                    {
                        stars[i].enabled = true;

                        if (data[i + 2]) stars[i].color = colorSO.StarNormalColor;
                        else stars[i].color = colorSO.StarNoneColor;
                    }
                }

            }
        }

        else
        {
            foreach (var text in Text) text.color = colorSO.disabledSignTextColor;
            ButtonImage.color = colorSO.disabledSignButtonColor;
        }
    }

    public void UnlockedAnimation()
    {

        RectTransform rt = ButtonImage.gameObject.GetComponent<RectTransform>();
        float startY = rt.anchoredPosition.y;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rt.DORotate(new Vector3(0, 0, .5f), .8f).SetEase(Ease.OutSine));
        // sequence.Join(rt.DOAnchorPosY(startY + 5, 1.6f).SetEase(Ease.InOutSine));
        sequence.Append(rt.DORotate(Vector3.zero, .8f).SetEase(Ease.InSine));
        sequence.Append(rt.DORotate(new Vector3(0, 0, -.5f), .8f).SetEase(Ease.OutSine));
        sequence.Append(rt.DORotate(Vector3.zero, .8f).SetEase(Ease.InSine));

        sequence.SetLoops(-1);
        sequence.Play();
    }

    public void PressButton()
    {
        if (isUnlocked)
        {
            HapticFeedbackManager.instance.PressUIButton();
            sceneLoader.LoadLevel(levelNum);
        }

        else
        {
            // GameObject.Find("MenuButtons").GetComponent<LevelLockedManager>().CheckLockedLevel(levelNum);
            LevelLockedManager.OnShowLevelLocked?.Invoke(levelNum - 1, true);

            if (lockMoveTween != null && lockMoveTween.IsPlaying())
                lockMoveTween.Kill();
            lockMoveTween = DOTween.Sequence();

            lockMoveTween.Append(Lock.DOAnchorPosY(10, .2f).SetEase(Ease.OutSine));
            lockMoveTween.Join(Lock.DORotate(new Vector3(0, 0, 15), .3f).SetEase(Ease.InOutSine));
            lockMoveTween.Append(Lock.DORotate(new Vector3(0, 0, -15), .2f).SetEase(Ease.InOutSine));
            lockMoveTween.Append(Lock.DORotate(new Vector3(0, 0, 10), .2f).SetEase(Ease.InOutSine));
            lockMoveTween.Append(Lock.DORotate(new Vector3(0, 0, -10), .2f).SetEase(Ease.InOutSine));
            lockMoveTween.Append(Lock.DORotate(Vector3.zero, .3f).SetEase(Ease.OutSine));
            lockMoveTween.Join(Lock.DOAnchorPosY(0, .3f).SetEase(Ease.OutSine));
            // lockMoveTween.OnComplete(() => but.interactable = true);


            lockMoveTween.Play();
        }



    }

    public string LevelName()
    {

        string s = Text[0].text;
        return s;



    }
}
