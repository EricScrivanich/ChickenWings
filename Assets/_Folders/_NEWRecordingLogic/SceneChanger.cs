using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string LoadSceneWithTransition;
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
        LevelRecordManager.instance.SaveAsset();
        yield return new WaitForSecondsRealtime(.1f);
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
        TransitionDirector.instance.GoTo(LoadSceneWithTransition);
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
