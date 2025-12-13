using System;
using System.Collections;
using UnityEngine;

public class SteroidTutorial : MonoBehaviour
{
    [SerializeField] private TutorialHand[] tutorialHands;

    [SerializeField] private string[] texts;

    public static SteroidTutorial instance;

    public Action<int, bool> OnShowTutorialHand;
    [SerializeField] private TextBox textBox;

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



    }
    void Start()
    {
        if (PlayerPrefs.GetInt("CompletedSteroidTutorial", 0) == 0)
            ShowNextHand(0);
        else
        {
            Destroy(textBox.gameObject);
        }

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
