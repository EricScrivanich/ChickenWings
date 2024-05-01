using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private bool debogLogEnabled;
    public PlayerID player;
    public static bool DebogLogEnabled;
    public float slowdownFactor;
    public float deltaScale;

    public float initialAddScoreDelay = 2f; // Initial delay
    public float minimumAddScoreDelay = 0.4f; // Minimum delay
    public float delayDecreaseDuration = 80f; // Duration over which the delay decreases

    public static float GlobalTimer;
    public static Action SpawnSetup;

    private void Awake()
    {
        DebogLogEnabled = true;
        GlobalTimer = 0;
        Debug.unityLogger.logEnabled = DebogLogEnabled;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = .02f * deltaScale;
    }

    private void Start()
    {
        GameObject.Find("Background").GetComponent<EnviromentMovement>().enabled = player.constantPlayerForceBool;

        if (player.isTutorial || player.constantPlayerForceBool)
        {
            return;
        }
        StartCoroutine(AddScore());
    }

    private void Update()
    {
        if (player.isAlive)
        {
            GlobalTimer += Time.deltaTime;
        }
    }

    public void Pause(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = slowdownFactor;
        }
    }

    private IEnumerator AddScore()
    {

        while (player.isAlive)
        {
            // Calculate the current delay based on the elapsed time
            float t = Mathf.Clamp(GlobalTimer / delayDecreaseDuration, 0, 1); // Ensure t is between 0 and 1
            float currentDelay = Mathf.Lerp(initialAddScoreDelay, minimumAddScoreDelay, t);

            yield return new WaitForSeconds(currentDelay);
            player.AddScore(1);
        }
    }
}
