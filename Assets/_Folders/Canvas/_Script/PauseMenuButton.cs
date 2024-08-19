using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenuPrefab;

    private RectTransform rectTransform;
    private bool isPaused = false;
    private PauseMenuManager pmm;
    private GameObject PauseMenu;
    private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu = Instantiate(PauseMenuPrefab);


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
        if (Time.timeScale == 0) return;
        PauseMenu.SetActive(true);
        pmm.InstantPause();

        Time.timeScale = 0;
        isPaused = true;
    }


    public void NormalPause()
    {
        if (!isPaused)
        {
            isPaused = true;
            PauseMenu.SetActive(true);
            StartCoroutine(SmoothTimeScaleTransition(0, .15f, 0));
            pmm.DropSignTween();
            Debug.Log("yuh");



        }
        else
        {
            Debug.Log("Huh");
            isPaused = false;
            StartCoroutine(SmoothTimeScaleTransition(1, .25f, .9f));
            pmm.RetractOnly();
        }
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
