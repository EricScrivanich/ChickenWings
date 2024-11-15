using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuPrefab;


    private RectTransform rectTransform;

    private bool isPaused = false;
    private PauseMenuManager pmm;
    private GameObject PauseMenu;
    private Canvas canvas;
    private Image fillImage;
    [SerializeField] private ButtonColorsSO colorSO;
    public static Action<bool> OnPauseGame;



    private bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {


    }
    void Awake()
    {
        PauseMenu = Instantiate(PauseMenuPrefab);

        fillImage = GetComponent<Image>();


        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        rectTransform = PauseMenu.GetComponent<RectTransform>();
        pmm = PauseMenu.GetComponent<PauseMenuManager>();
        PauseMenu.transform.SetParent(canvas.transform, false);

        rectTransform.anchoredPosition = new Vector2(0, 645);

        // pmm.SetPauseButton(this);
        PauseMenu.SetActive(false);

    }

    // Update is called once per frame
    public void InstantPause()
    {
        if (Time.timeScale == 0 || gameOver) return;

        PauseMenuButton.OnPauseGame?.Invoke(true);


        PauseMenu.SetActive(true);
        pmm.InstantPause();

        Time.timeScale = 0;
        isPaused = true;
    }


    public void NormalPause()
    {
        // if (Time.timeScale < FrameRateManager.TargetTimeScale || gameOver) return;
        if (!isPaused && Time.timeScale != 0 && !gameOver)
        {
            isPaused = true;
            PauseMenuButton.OnPauseGame?.Invoke(true);

            HapticFeedbackManager.instance.PressUIButton();
            Time.timeScale = 0;

            Color normalColor = fillImage.color;

            var seq = DOTween.Sequence();

            seq.Append(fillImage.rectTransform.DOScale(1.2f, .25f));
            seq.Join(fillImage.DOColor(colorSO.highlightButtonColor, .25f));

            seq.Append(fillImage.rectTransform.DOScale(1, .22f));
            seq.Join(fillImage.DOColor(normalColor, .22f));
            seq.Play().SetUpdate(true);


            PauseMenu.SetActive(true);
            // StartCoroutine(SmoothTimeScaleTransition(0, .15f, 0));

            pmm.DropSignTween();
        }
        else
        {

            isPaused = false;
            pmm.RetractOnly();
            StartCoroutine(SmoothTimeScaleTransition(FrameRateManager.TargetTimeScale, .25f, .95f));
        }
    }

    private void OnEnable()
    {
        ResetManager.GameOverEvent += OnGameOveer;
    }
    private void OnDisable()
    {
        ResetManager.GameOverEvent -= OnGameOveer;

    }

    private void OnGameOveer()
    {
        gameOver = true;
    }


    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            // When the game is paused by the system (going to background).
            InstantPause();
        }
        else
        {
            // When the game returns from background, keep it paused.
            // Pause(true);
        }
    }


    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration, float delay)
    {
        float start = Time.timeScale;
        float elapsed = 0f;

        yield return new WaitForSecondsRealtime(delay);


        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTimeScale, elapsed / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;

    }
}
