using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class TransitionDirector : MonoBehaviour
{
    public static TransitionDirector instance;
    [SerializeField] private PlayerID player;

    [Header("Camera Move")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float travelUpDistance = 12f; // world units
    [SerializeField] private float upDuration = 0.6f;
    [SerializeField] private float downDuration = 0.6f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Header("Clouds")]


    [Header("Misc")]
    [SerializeField] private bool lockInputs = true;
    public bool mainMenuStarted = false;
    private string lastScene;

    public Action<bool> OnHandleGameUI;
    public Action<float, Ease, float> OnDoUITween;


    bool busy;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);


        if (mainCam == null) mainCam = Camera.main;


    }
    public void UndoDestroy()
    {
        var o = GameObject.Find("Background").transform;
        if (o != null)
            mainCam.transform.parent = o;

    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" || SceneManager.GetActiveScene().name != "LevelPicker")
            DoTransitionImmediate();
    }

    public void GoTo(string sceneName, int buttonsToShow = 8)
    {
        Debug.Log("Transitioning to " + sceneName);
        if (!busy)
        {
            lastScene = SceneManager.GetActiveScene().name;
            RectTransform objToKeepDown = null;
            switch (sceneName)
            {
                case "LevelPicker":
                    objToKeepDown = GameObject.Find("MenuButtons").GetComponent<RectTransform>();
                    float scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
                    objToKeepDown.DOAnchorPosY(WorldToScreenDeltaY(travelUpDistance, scaleFactor), upDuration).SetEase(ease);
                    Debug.Log("LevelPicker check");
                    break;
                case "MainMenu":

                    break;

            }
            Debug.Log("Object to keep down is " + objToKeepDown);
            StartCoroutine(DoTransition(sceneName, buttonsToShow));
        }
    }

    public void SetRectParent(RectTransform rect)
    {
        var c = GetComponentInChildren<Canvas>();
        rect.SetParent(c.transform);
    }
    float WorldToScreenDeltaY(float worldDeltaY, float canvasScale = 1f)
    {
        var cam = Camera.main;
        var a = cam.WorldToScreenPoint(new Vector3(0f, 0f, 0f));
        var b = cam.WorldToScreenPoint(new Vector3(0f, worldDeltaY, 0f));

        return -(b.y - a.y) / canvasScale;

    }

    public void DoTransitionImmediate(int buttonsToShow = 8)
    {
        // OnLoadNewScene?.Invoke();

        // ShowPlayerUI?.Invoke(true, buttonsToShow);

        // player.events.OnStartPlayer?.Invoke();

    }

    private System.Collections.IEnumerator DoTransition(string sceneName, int buttonsToShow)
    {
        busy = true;
        Time.timeScale = FrameRateManager.BaseTimeScale;
        if (lockInputs) SetInputEnabled(false);

        // 1) Move camera up into clouds
        Vector3 startPos = mainCam.transform.position;


        // ensure clouds render on top (sorting layer / camera stack already set in setup)

        yield return mainCam.transform.DOMove(new Vector3(0, travelUpDistance, -10), upDuration).SetEase(ease).WaitForCompletion();
        yield return new WaitForSecondsRealtime(0.2f);
        // 2) Load next scene additively while hidden

        AsyncOperation load = SceneManager.LoadSceneAsync("LoadScreen", LoadSceneMode.Additive);
        load.allowSceneActivation = false; // we want to control when it finishes

        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return null;
        Debug.Log("Unloading " + lastScene);
        SceneManager.UnloadSceneAsync(lastScene);

        // while (!unload.isDone) yield return null;
        yield return null;
        AsyncOperation newLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        newLoad.allowSceneActivation = false;
        // Resources.UnloadUnusedAssets();

        while (!newLoad.isDone)
        {
            if (newLoad.progress >= 0.9f)
            {
                newLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return null;


        if (sceneName == "MainMenu")
        {
            var objToKeepDown = GameObject.Find("MenuButtons").GetComponent<RectTransform>();

            float scaleFactor = GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
            objToKeepDown.anchoredPosition = new Vector2(objToKeepDown.anchoredPosition.x, WorldToScreenDeltaY(travelUpDistance, scaleFactor));
            objToKeepDown.DOAnchorPosY(0, upDuration).SetEase(ease);
        }
        // else if (sceneName == "LevelPicker")
        // {

        // }
        mainCam.DOOrthoSize(5.5f, downDuration - .2f).SetEase(ease);
        Debug.LogError("Doing Screen Ortho");
        // OnDoUITween?.Invoke(downDuration, ease, travelUpDistance);
        // OnLoadNewScene?.Invoke();
        yield return mainCam.transform.DOMoveY(0, downDuration).SetEase(ease).WaitForCompletion();

        SceneManager.UnloadSceneAsync("LoadScreen");



        if (lockInputs) SetInputEnabled(true);
        busy = false;
    }

    private void SetObjectsActiveInNewScene(string title)
    {
        Debug.LogError("Trying to set object active: " + title);
        GameObject.Find(title).SetActive(true);

    }

    private void SetInputEnabled(bool on)
    {
        // Hook into your input manager. For the new Input System, you can enable/disable actions.
        // Example:
        // PlayerInput.all.ForEach(pi => pi.enabled = on);
    }
}