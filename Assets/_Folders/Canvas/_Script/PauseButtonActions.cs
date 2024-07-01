using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonActions : MonoBehaviour
{
    public void ResetGame()
    {
        // Pause(false);
        Time.timeScale = 1;

        GameObject.Find("GameManager").GetComponent<ResetManager>().ResetGame();

    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;  // Ensure game time is running normally in the main menu.
    }

    public void Resume()
    {
        PauseMenuButton pmb = GameObject.Find("PauseButton").GetComponent<PauseMenuButton>();
        pmb.NormalPause();

    }
}
