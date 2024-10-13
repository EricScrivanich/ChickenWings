using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class TimerScript : MonoBehaviour
{
    [SerializeField] private RectTransform GoodJob;
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI FinalTime;
    public TextMeshProUGUI timerText;// Assign this in the inspector with your UI Text
    private float startTime;
    private bool timerActive = false;
    public CameraID cam;
    private float currentTime;

    void Start()
    {
        // Optionally start the timer immediately, or start it from a method call
        StartTimer();
        currentTime = 0;
        GoodJob.anchoredPosition = new Vector2(0, 1000);
        GoodJob.gameObject.SetActive(false);

    }

    public void StartTimer()
    {
        timerActive = true;

    }

    public void StopTimer(bool finished)
    {
        timerActive = false;

        if (finished)
        {
            player.SetActive(false);
            GoodJob.gameObject.SetActive(true);
            FinalTime.text = "Your Time: " + currentTime.ToString("00.00");
            GoodJob.DOAnchorPos(Vector2.zero, 2.3f);
        }

    }

    void Update()
    {


        if (timerActive)
        {
            currentTime += Time.deltaTime;
            timerText.text = "Time: " + currentTime.ToString("00.00");

        }

    }

    private void OnEnable()
    {
        cam.events.OnStopTimer += StopTimer;

    }

    private void OnDisable()
    {
        cam.events.OnStopTimer -= StopTimer;

    }
}
