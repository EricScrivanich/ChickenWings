using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SteroidTutorial : MonoBehaviour
{
    [SerializeField] private TutorialHand[] tutorialHands;

    [SerializeField] private bool isLevelPicker;
    private int index;
    [SerializeField] private Transform nameLabel;



    [SerializeField, TextArea(3, 10)] private string[] texts;

    public static SteroidTutorial instance;

    private bool isFlipped;

    public Action<int, bool> OnShowTutorialHand;
    [SerializeField] private TextBox textBox;
    [SerializeField] private TextBox textBoxFlipped;
    [SerializeField] private GameObject colonelCluckCanvas;
    [SerializeField] private RectTransform colonelCluck;
    [SerializeField] private RectTransform[] colonelCluckStartPos;
    [SerializeField] private RectTransform[] colonelCluckEndPos;
    [SerializeField] private float colonelCluckTweenTimeIn;
    [SerializeField] private float colonelCluckTweenTimeOut;

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
            index = 0;
            TweenInColonelCluck();
        }




    }



    private void TweenInColonelCluck(string Message = "", bool flipped = false, float scale = 1.16f)
    {
        colonelCluck.position = colonelCluckStartPos[index].position;
        if (flipped)
        {
            isFlipped = true;
            nameLabel.localScale = new Vector3(-1, 1, 1);
            colonelCluck.localScale = new Vector3(-scale, scale, scale);
        }
        else if (!flipped)
        {
            isFlipped = false;
            nameLabel.localScale = Vector3.one;
            colonelCluck.localScale = new Vector3(scale, scale, scale);
        }
        colonelCluck.gameObject.SetActive(true);
        colonelCluck.DOMove(colonelCluckEndPos[index].position, colonelCluckTweenTimeIn).SetUpdate(true).SetEase(Ease.OutSine).OnComplete(() =>
        {
            if (isLevelPicker)
                ShowNextHand(0);
            else
                ShowMessage(Message);
        });
    }



    public void ShowColonelCluck(bool show, int i, string msg, bool flipped = false, float scale = 1.16f)
    {
        index = i;
        if (show)
        {
            TweenInColonelCluck(msg, flipped, scale);
        }
        else
        {
            if (isFlipped)
                textBoxFlipped.FadeOut();
            else
                textBox.FadeOut();
            colonelCluck.DOMove(colonelCluckStartPos[index].position, colonelCluckTweenTimeOut).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() =>
            {
                colonelCluck.gameObject.SetActive(false);
            });
        }

    }

    public void ShowMessage(string message)
    {
        if (isFlipped)
            textBoxFlipped.SetText(message);
        else
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
