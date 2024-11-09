using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private PlayerID player;
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private float timeForAddObjective;
    [SerializeField] private int objectivesAddedPerTimeCycle;
    private float time = 0;

    [SerializeField] private bool addScore;


    void Update()
    {

        if (time < timeForAddObjective)
        {
            time += Time.deltaTime;
        }
        else
        {
            lvlID.inputEvent.OnUpdateObjective?.Invoke("time", objectivesAddedPerTimeCycle);

            if (addScore)
            {
                player.globalEvents.OnAddScore?.Invoke(objectivesAddedPerTimeCycle);
            }
            time = 0;

        }


    }



}
