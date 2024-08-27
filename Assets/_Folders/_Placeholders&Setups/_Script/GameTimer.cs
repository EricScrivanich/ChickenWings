using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private float timeForNextIntensity = 20;
    [SerializeField] private int objectivesAddedPerTimeCycle;
    private float time = 0;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

        if (time < timeForNextIntensity)
        {
            time += Time.deltaTime;
        }
        else
        {
            lvlID.inputEvent.OnUpdateObjective?.Invoke("time", objectivesAddedPerTimeCycle);
            time = 0;

        }


    }
}
