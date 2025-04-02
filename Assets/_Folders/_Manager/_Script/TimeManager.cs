using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public PlayerID player;




    public float initialAddScoreDelay = 2f; // Initial delay
    public float minimumAddScoreDelay = 0.4f; // Minimum delay
    public float delayDecreaseDuration = 80f; // Duration over which the delay decreases

    public float GlobalTimer;


    private void Awake()
    {

        GlobalTimer = 0;


    }

    private void Start()
    {
        if (GameObject.Find("Background") != null)
        {
            GameObject.Find("Background").GetComponent<EnviromentMovement>().enabled = player.constantPlayerForceBool;
        }



        // StartCoroutine(AddScore());


    }

    private void Update()
    {
        if (player.isAlive)
        {
            GlobalTimer += Time.deltaTime;
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
