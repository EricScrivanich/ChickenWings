using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameModeButton : MonoBehaviour
{

    private bool isUnlocked;
    [SerializeField] private int neededUnlockIndex;
    [SerializeField] private int gameModeIndex;
    [SerializeField] private SceneManagerSO sceneSO;


    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image ButtonImage;
    [SerializeField] private RectTransform Lock;
    private Sequence lockMoveTween;


    // Start is called before the first frame update
    void Start()
    {
        if (neededUnlockIndex == -1) isUnlocked = false;

        else isUnlocked = (SaveManager.instance.HasCompletedLevel(neededUnlockIndex));

        if (isUnlocked)
        {
            Lock.gameObject.SetActive(false);
            text.color = Color.white;
            ButtonImage.color = colorSO.NormalSignButtonColor;
        }
        else
        {
            if (Lock != null)
                Lock.gameObject.SetActive(true);
            text.color = colorSO.disabledSignTextColor;
            ButtonImage.color = colorSO.disabledSignButtonColor;

        }

    }

    public void PressButton()
    {
        if (isUnlocked)
        {
            HapticFeedbackManager.instance.PressUIButton();
            sceneSO.LoadGamemode(gameModeIndex);
        }

        else
        {
            // GameObject.Find("MenuButtons").GetComponent<LevelLockedManager>().CheckLockedLevel(levelNum);
            LevelLockedManager.OnShowLevelLocked?.Invoke(neededUnlockIndex, false,false);

            if (Lock != null)
            {
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
                lockMoveTween.Play();

            }


        }



    }
}
