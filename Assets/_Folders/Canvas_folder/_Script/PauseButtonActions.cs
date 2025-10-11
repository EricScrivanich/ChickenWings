using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseButtonActions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool gameOverButton;
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private SceneManagerSO sceneLoader;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform imageRect;

    public static bool lockButtons;
    private RectTransform rect;
    [SerializeField] private Image buttonColor;
    private Vector2 originalPosition;
    private Vector2 movedPosition1;
    private Vector2 movedPosition2;
    [SerializeField] private Vector2 moveAmountOnPress;
    [SerializeField] private Vector2 moveAmountOnPress2;
    private bool ReadyToDOAction;
    [SerializeField] private bool rotateOnFinalPress;

    [SerializeField] private Vector3 rectScaleOnPress;
    [SerializeField] private Vector3 rectScaleOnFinalPress;
    [SerializeField] private Vector3 imageScaleOnPress1;
    [SerializeField] private Vector3 imageScaleOnPress2;
    private int type;
    private Vector3 normalScale = new Vector3(1, 1, 1);
    private Button button;

    private Sequence hoverSeq;
    private Sequence pressSeq;

    private Color pressColor;
    private Color unPressColor;
    [SerializeField] private GameObject levelLockedPrefab;

    private LevelLockedDisplay lockedDisplay;

    private void Awake()
    {

        if (gameOverButton)
        {
            unPressColor = colorSO.goUnPressedColor;
            pressColor = colorSO.goPressedColor;
        }
        else
        {
            unPressColor = colorSO.pmUnPressedColor;
            pressColor = colorSO.pmPressedColor;
        }
        lockButtons = false;
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();



        // if (text != null) text.color = colorSO.unPressedColor;
        // buttonColor.color = colorSO.unPressedColor;
    }

    private void OnEnable()
    {
        imageRect.localScale = normalScale;
        buttonColor.color = unPressColor;
        rect.localScale = normalScale;
        if (text != null)
            text.color = unPressColor;
        lockButtons = false;

    }
    private void Start()
    {
        originalPosition = imageRect.anchoredPosition;
        imageRect.localScale = normalScale;
        movedPosition1 = new Vector2(originalPosition.x + moveAmountOnPress.x, originalPosition.y + moveAmountOnPress.y);
        movedPosition2 = new Vector2(originalPosition.x + moveAmountOnPress2.x, originalPosition.y + moveAmountOnPress2.y);








    }

    public void ResetGame()
    {
        type = 0;
        if (lockButtons) return;
        PressInitializedTween();

    }

    public void Menu()
    {
        type = 1;

        if (lockButtons) return;
        SetNextLevel(true);
        PressInitializedTween();

    }

    public void Resume()
    {
        type = 2;
        if (lockButtons) return;
        PressInitializedTween();

    }

    public void NextLevel()
    {
        type = 3;
        SetNextLevel(false);
        if (lockButtons) return;

        PressInitializedTween();

        // if (!SaveManager.instance.HasCompletedLevel(sceneLoader.ReturnLevelNumber()))
        // {
        //     string needToCompleteAtHigherSpeed = "Complete the entire level at a speed of .85 or above for this level to count";
        //     if (lockedDisplay == null)
        //     {
        //         var parent = GameObject.Find("Canvas").GetComponent<Transform>();
        //         lockedDisplay = Instantiate(levelLockedPrefab, parent).GetComponent<LevelLockedDisplay>();
        //     }
        //     lockedDisplay.Show(needToCompleteAtHigherSpeed, true, true);
        //     return;


        // }
        // string s = SaveManager.instance.CheckAdditonalChallenges(-1);


        // Debug.Log(s);

        // if (s == null)
        //     PressInitializedTween();

        // else
        // {
        //     if (lockedDisplay == null)
        //     {
        //         var parent = GameObject.Find("Canvas").GetComponent<Transform>();
        //         lockedDisplay = Instantiate(levelLockedPrefab, parent).GetComponent<LevelLockedDisplay>();
        //     }

        //     lockedDisplay.Show(s, true, true);
        // }

    }

    // Event triggered when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {

        HoverTween(true);
        // Add your desired hover effect here

    }

    // Event triggered when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {

        HoverTween(false);

        // Reset color when the pointer exits the button

    }

    // Event triggered when the button is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        HapticFeedbackManager.instance.PressUIButton();

        PressTween(true);
        // Change the button's appearance when pressed

    }
    public void OnPointerUp(PointerEventData eventData)
    {

        PressTween(false);

    }



    private void HoverTween(bool hovering)
    {
        if (lockButtons) return;

        if (hoverSeq != null && hoverSeq.IsPlaying())
            hoverSeq.Kill();
        hoverSeq = DOTween.Sequence();
        if (hovering)
        {

            hoverSeq.Append(buttonColor.DOColor(pressColor, .23f));
            if (text != null)
                hoverSeq.Join(text.DOColor(pressColor, .23f));
            // hoverSeq.Join(imageRect.DOAnchorPos(movedPosition, .23f).SetEase(Ease.OutSine));

            hoverSeq.Play().SetUpdate(true);
        }
        else
        {
            hoverSeq.Append(buttonColor.DOColor(unPressColor, .2f));
            if (text != null)
                hoverSeq.Join(text.DOColor(unPressColor, .2f));

            // hoverSeq.Join(imageRect.DOAnchorPos(originalPosition, .2f).SetEase(Ease.OutSine));

            // hoverSeq.Join(rect.DOScale(normalScale, .2f));
            hoverSeq.Play().SetUpdate(true);
            PressTween(false);

        }

    }

    private void PressInitializedTween()
    {
        PressTween(true);
        lockButtons = true;


        Sequence seq = DOTween.Sequence();

        if (rotateOnFinalPress)
        {

            seq.Append(imageRect.DOScale(imageScaleOnPress1, .15f));
            seq.Join(imageRect.DOAnchorPos(movedPosition1, .15f).SetEase(Ease.InOutSine));
            seq.Append(imageRect.DORotate(new Vector3(0, 0, -360), .45f, RotateMode.FastBeyond360).SetEase(Ease.OutSine));
            seq.Play().SetUpdate(true).OnComplete(DoAction);

        }
        else
        {
            seq.Append(imageRect.DOAnchorPos(movedPosition1, .22f).SetEase(Ease.InOutSine));
            seq.Join(imageRect.DOScale(imageScaleOnPress1, .22f));
            seq.Append(imageRect.DOAnchorPos(movedPosition2, .17f).SetEase(Ease.InOutSine));
            seq.Join(imageRect.DOScale(imageScaleOnPress2, .17f));
            seq.Play().SetUpdate(true).OnComplete(DoAction);

        }




    }

    private IEnumerator WaitForTween()
    {
        Debug.Log("Entered COR");
        yield return new WaitUntil(() => ReadyToDOAction == true);
        Debug.Log("finish COR");

        DoAction();
    }

    private void DoAction()
    {
        switch (type)
        {
            case (0):
                Time.timeScale = FrameRateManager.TargetTimeScale;
                GameObject.Find("GameManager").GetComponent<ResetManager>().ResetGame();

                break;
            case (1):
                GameObject.Find("GameManager").GetComponent<ResetManager>().checkPoint = 0;

                SceneManager.LoadScene("MainMenu");
                Time.timeScale = FrameRateManager.TargetTimeScale;  // Ensure game time is running normally in the main menu.
                break;
            case (2):
                PauseMenuButton pmb = GameObject.Find("PauseButton").GetComponent<PauseMenuButton>();
                pmb.NormalPause();
                break;
            case (3):
                // GameObject.Find("GameManager").GetComponent<ResetManager>().checkPoint = 0;
                Time.timeScale = FrameRateManager.TargetTimeScale;
                SceneManager.LoadScene("LevelPicker");

                // sceneLoader.LoadLevel(GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelIndex + 1);
                break;
            case (4):

                break;
        }

    }


    private void SetNextLevel(bool menu)
    {
        if (menu)
        {
            PlayerPrefs.SetString("Menu", "Menu");
            PlayerPrefs.Save();
            return;
        }
        string l = PlayerPrefs.GetString("LastLevel", "1-1-0");
        string[] parts = l.Split('-');

        var next = new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]) + 1, 0);
        string n = $"{next.x}-{next.y}-{next.z}";
        Debug.Log("Next level set to: " + n);
        PlayerPrefs.SetString("NextLevel", n);
        PlayerPrefs.Save();

    }

    private void PressTween(bool isPressed)
    {
        if (lockButtons) return;
        if (pressSeq != null && pressSeq.IsPlaying())
            pressSeq.Kill();
        pressSeq = DOTween.Sequence();

        if (isPressed)
        {
            pressSeq.Append(rect.DOScale(rectScaleOnPress, .27f));

            pressSeq.Play().SetUpdate(true);
        }
        else
        {

            ReadyToDOAction = false;
            pressSeq.Append(rect.DOScale(normalScale, .2f));
            pressSeq.Join(imageRect.DOAnchorPos(originalPosition, .2f).SetEase(Ease.OutSine));

            pressSeq.Play().SetUpdate(true);

        }



    }

    private void SetReadyTrue()
    {
        Debug.Log("Ready");
        ReadyToDOAction = true;
    }

    // Event triggered when the button is released



}