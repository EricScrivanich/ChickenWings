using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonActions : MonoBehaviour
{
    [SerializeField] private SceneManagerSO sceneLoader;
    public void ResetGame()
    {
        // Pause(false);
        Time.timeScale = 1;

        GameObject.Find("GameManager").GetComponent<ResetManager>().ResetGame();

    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        GameObject.Find("GameManager").GetComponent<ResetManager>().checkPoint = 0;
        Time.timeScale = 1;  // Ensure game time is running normally in the main menu.
    }

    public void Resume()
    {
        PauseMenuButton pmb = GameObject.Find("PauseButton").GetComponent<PauseMenuButton>();
        pmb.NormalPause();

    }

    public void NextLevel()
    {
        GameObject.Find("GameManager").GetComponent<ResetManager>().checkPoint = 0;

        sceneLoader.LoadLevel(GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelIndex + 1);

    }
}
