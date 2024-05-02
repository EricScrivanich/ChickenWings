using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timerText;// Assign this in the inspector with your UI Text
    private float startTime;
    private bool timerActive = false;

    void Start()
    {
        // Optionally start the timer immediately, or start it from a method call
        StartTimer();
    }

    public void StartTimer()
    {
      
    }

    public void StopTimer()
    {
        
    }

    void Update()
    {
      
            
            
            timerText.text = "Time - " + Time.time.ToString("00.00");
        
    }
}
