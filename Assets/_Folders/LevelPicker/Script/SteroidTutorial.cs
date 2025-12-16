using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SteroidTutorial : MonoBehaviour
{
    [SerializeField] private TutorialHand[] tutorialHands;

    [SerializeField] private bool isLevelPicker;
    [SerializeField] private Animator anim;

    [SerializeField] private string[] texts;

    public static SteroidTutorial instance;

    public Action<int, bool> OnShowTutorialHand;
    [SerializeField] private TextBox textBox;
    [SerializeField] private GameObject colonelCluckCanvas;
    [SerializeField] private RectTransform colonelCluck;
    [SerializeField] private RectTransform colonelCluckStartPos;
    [SerializeField] private RectTransform colonelCluckEndPos;
    [SerializeField] private float colonelCluckTweenTime;

    public bool reset;

    void OnValidate()
    {
        if (reset)
        {
            reset = false;
            PlayerPrefs.SetInt("CompletedSteroidTutorial", 0);

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        if (isLevelPicker && PlayerPrefs.GetInt("CompletedSteroidTutorial", 0) != 0)
        {
            Destroy(colonelCluckCanvas);
            Destroy(this);

        }



    }
    void Start()
    {
        if (isLevelPicker && PlayerPrefs.GetInt("CompletedSteroidTutorial", 0) == 0)
        {
            TweenInColonelCluck();
        }




    }



    private void TweenInColonelCluck(string Message = "")
    {
        colonelCluck.position = colonelCluckStartPos.position;
        colonelCluck.gameObject.SetActive(true);
        colonelCluck.DOMove(colonelCluckEndPos.position, colonelCluckTweenTime).SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (isLevelPicker)
                ShowNextHand(0);
            else
                ShowMessage(Message);
        });
    }

    public void PlayAnim(bool play)
    {
        anim.SetBool("Talking", play);
    }

    public void ShowColonelCluck(bool show, string msg)
    {
        if (show)
        {
            TweenInColonelCluck(msg);
        }
        else
        {
            textBox.FadeOut();
            colonelCluck.DOMove(colonelCluckStartPos.position, colonelCluckTweenTime).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() =>
            {
                colonelCluck.gameObject.SetActive(false);
            });
        }

    }

    public void ShowMessage(string message)
    {
        textBox.SetText(message);
    }

    public void ShowNextHand(int index)
    {

        switch (index)
        {
            case 0:
                textBox.SetText(texts[0]);
                break;
            case 1:
                textBox.FadeOut();
                break;

            case 4:
                textBox.SetText(texts[1]);
                break;
            case 5:
                textBox.FadeOut();
                InputSystemSelectionManager.instance.tutorialType = -1;
                PlayerPrefs.SetInt("CompletedSteroidTutorial", 1);
                break;

        }



        StartCoroutine(ShowNextHandCoroutine(index));
    }

    private IEnumerator ShowNextHandCoroutine(int index)
    {
        OnShowTutorialHand?.Invoke(index, true);
        yield return new WaitForSecondsRealtime(0.5f);
        OnShowTutorialHand?.Invoke(index, false);
    }

    // Update is called once per frame

}
