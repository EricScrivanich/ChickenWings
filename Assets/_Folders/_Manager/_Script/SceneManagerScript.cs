using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    public Action<bool, float, string> OnFadeTranstion;

    private bool loadingScene = false;

    private float fadeInDuration = .9f;
    private float fadeOutDuration = .15f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    void Start()
    {
        OnFadeTranstion?.Invoke(false, 1f, "");
    }
#endif


    public void LoadScene(string sceneName)
    {
        if (loadingScene) return;
        loadingScene = true;

        StartCoroutine(LoadSceneCoroutine(sceneName));

    }
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // OnFadeTranstion?.Invoke(true, fadeInDuration, sceneName);
        // yield return new WaitForSecondsRealtime(fadeInDuration + .2f);
        yield return null;
        SceneManager.LoadScene(sceneName);
        if (Time.timeScale == 0 && (sceneName == "MainMenu" || sceneName == "LevelPicker"))
            Time.timeScale = FrameRateManager.TargetTimeScale;
        yield return null;

        // OnFadeTranstion?.Invoke(false, fadeOutDuration, "");

        loadingScene = false;

    }

    private IEnumerator ReoadSceneCoroutine()
    {
        OnFadeTranstion?.Invoke(true, fadeInDuration * .6f, "");
        yield return new WaitForSecondsRealtime(fadeInDuration * .6f);
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return null;

        OnFadeTranstion?.Invoke(false, fadeOutDuration * .8f, "");

        loadingScene = false;

    }




    public void ReloadCurrentScene()
    {
        if (loadingScene) return;
        loadingScene = true;
        StartCoroutine(ReoadSceneCoroutine());

    }
    // Update is called once per frame

}
