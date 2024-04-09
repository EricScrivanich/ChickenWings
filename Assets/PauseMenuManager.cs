using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject UI;


    public void Pause(bool isPaused)
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
    // Start is called before the first frame update
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }



    void OnApplicationFocus(bool pauseStatus)
    {
        // if (!pauseStatus )
        // {
        //     Time.timeScale = 0;
        //     UI.SetActive(true);//you

        // }
       
     
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Time.timeScale = 0;
            UI.SetActive(true);

        }
        else
        {

        }
    }
}
