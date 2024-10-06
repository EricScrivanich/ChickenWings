using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class LevelButton : MonoBehaviour
{
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
        Lock.gameObject.SetActive(!isUnlocked);

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
