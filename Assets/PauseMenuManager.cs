using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject UI;


    // This method toggles the pause state of the game.
    public void TogglePause()
    {
        if (Time.timeScale != 0)
        {
            Pause(true);
        }
        else
        {
            Pause(false);
        }
    }

    // This method sets the game into a paused or unpaused state.
    private void Pause(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            UI.SetActive(true);
        }
        else
        {
            UI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ResetGame()
    {
        Pause(false);

        GameObject.Find("GameManager").GetComponent<ResetManager>().ResetGame();

    }

    // This method is called to return to the main menu.
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;  // Ensure game time is running normally in the main menu.
    }

    // Called when the application is paused or resumed.
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {

            // When the game is paused by the system (going to background).
            Pause(true);
        }
        else
        {

            // When the game returns from background, keep it paused.
            // Pause(true);
        }
    }
}

