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
    [SerializeField] private Color buttonColorUnlocked;
    [SerializeField] private Color buttonColorLocked;
    [SerializeField] private Color textColorUnlocked;
    [SerializeField] private Color textColorLocked;
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
        isUnlocked = true;

        Lock.gameObject.SetActive(!isUnlocked);
        foreach (var item in stars)
        {
            item.enabled = false;
        }

        bool[] data = SaveManager.instance.GetSavedLevelData(levelNum);
        if (data != null || data.Length > 0)
        {
            badge.SetActive(data[1]);
            for (int i = 0; i < data.Length - 2; i++)
            {
                stars[i].enabled = true;

                if (data[i + 2]) stars[i].color = colorSO.StarNormalColor;
                else stars[i].color = colorSO.StarNoneColor;
            }
        }






        if (isUnlocked)
        {
            foreach (var text in Text) text.color = textColorUnlocked;
            ButtonImage.color = buttonColorUnlocked;
            UnlockedAnimation();
        }

        else
        {
            foreach (var text in Text) text.color = textColorLocked;
            ButtonImage.color = buttonColorLocked;
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
            GameObject.Find("MenuButtons").GetComponent<LevelLockedManager>().CheckLockedLevel(levelNum);

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
