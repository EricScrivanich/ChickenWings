using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string LoadSceneWithTransition;
    [SerializeField] private bool DoFeedback = false;
    public void LoadCreatorScene(bool test)
    {
        if (test)
        {

            StartCoroutine(AwaitLoadScene(true));
        }
        else
        {
            SceneManager.LoadScene("LevelCreator");
        }
    }

    public IEnumerator AwaitLoadScene(bool test)
    {


        LevelRecordManager.instance.UpdateGameTimeBeforePlaymode();
        yield return null;
        LevelRecordManager.instance.SaveAsset();
        yield return null;
        yield return null;


        if (test)
        {
            PlayerPrefs.SetString("LevelType", "Custom");
            PlayerPrefs.Save();
            // yield return new WaitUntil(() => LevelRecordManager.PreloadedSceneReady);
            LevelRecordManager.instance.RestoreStaticParameters();
            SceneManager.LoadScene("LevelPlayer");
            // LevelRecordManager.PlayPreloadedScene = true;
        }
        else
        {
            PlayerPrefs.SetString("LevelType", "Menu");
            PlayerPrefs.Save();
            LevelRecordManager.ResetStaticParameters();
            SceneManager.LoadScene("MainMenu");
        }


    }

    public void PressForSceneTransition()
    {

        // TransitionDirector.instance.GoTo(LoadSceneWithTransition);
        if (DoFeedback)
            HapticFeedbackManager.instance.PressUIButton();
        SceneManagerScript.instance.LoadScene(LoadSceneWithTransition);
    }

    public void ShowPlayView(bool show)
    {
        LevelRecordManager.instance.EnterPlayTime(show);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void LoadMainMenu()
    {
        StartCoroutine(AwaitLoadScene(false));

    }

}
