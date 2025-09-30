using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ticker : MonoBehaviour
{

    public static float tickTime_015 = .15f;


    public delegate void TickAction015();
    



    private float tickerTime015 = 0;

    public static event TickAction015 OnTickAction015;
    public static Action OnDespawnAction;
    public static Action<Vector2> OnSendPlayerPosition;
    private Transform playerTransform;

    private void Start() {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Start is called before the first frame update

    // Update is called once per frame


    void Update()
    {
        tickerTime015 += Time.deltaTime;

        if (tickerTime015 >= tickTime_015)
        {
            tickerTime015 = 0;

            OnTickAction015?.Invoke();
            // OnSendPlayerPosition?.Invoke(playerTransform.position);
        }


    }


    private void TicketEvent015()
    {

    }
}
