using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{

    public static float tickTime_015 = .15f;

    public delegate void TickAction015();

    private float tickerTime015 = 0;
    public static event TickAction015 OnTickAction015;
    // Start is called before the first frame update

    // Update is called once per frame


    void Update()
    {
        tickerTime015 += Time.deltaTime;

        if (tickerTime015 >= tickTime_015)
        {
            tickerTime015 = 0;
            TicketEvent015();

        }

    }


    private void TicketEvent015()
    {
        OnTickAction015?.Invoke();
    }
}
