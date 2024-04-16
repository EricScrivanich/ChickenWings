using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droplets : MonoBehaviour
{

    [SerializeField] private GameObject droplet;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private float waitTime;
    [SerializeField] private float dropTime;
    private float time;

    private bool ready;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;

        ready = false;
        Invoke("WaitToActivate", waitTime);

    }

    private void WaitToActivate()
    {
        ready = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            time += Time.deltaTime;

            if (time > dropTime)
            {
                Debug.Log("drop");
                Instantiate(droplet, transform);
                time = 0;
            }

        }
        
    }
}
