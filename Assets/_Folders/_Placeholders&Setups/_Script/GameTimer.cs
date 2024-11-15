using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private PlayerID player;
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private float timeForAddObjective;

    private float originalTime;
    [SerializeField] private int objectivesAddedPerTimeCycle;
    private float time = 0;


    [SerializeField] private float timeChangePerFlappyPig;
    [SerializeField] private float minimumTime;
    [SerializeField] private bool flappyFrenzy;
    public static Action<bool> OnAddFlappyPig;


    void Update()
    {

        if (time < timeForAddObjective)
        {
            time += Time.deltaTime;
        }
        else
        {
            lvlID.inputEvent.OnUpdateObjective?.Invoke("time", objectivesAddedPerTimeCycle);

            if (flappyFrenzy)
            {
                player.globalEvents.OnAddScore?.Invoke(objectivesAddedPerTimeCycle);
            }
            time = 0;

        }


    }

    private void ChangeObjectiveTimeForFlappyFrenzy(bool isAdded)
    {
        if (isAdded)
        {
            timeForAddObjective -= timeChangePerFlappyPig;
        }
        else
        {
            timeForAddObjective += timeChangePerFlappyPig;
        }

        if (timeChangePerFlappyPig < minimumTime)
            timeChangePerFlappyPig = minimumTime;
        else if (timeChangePerFlappyPig > originalTime)
            timeChangePerFlappyPig = originalTime;
    }

    private void OnEnable()
    {

        if (flappyFrenzy)
        {
            originalTime = timeChangePerFlappyPig;
            GameTimer.OnAddFlappyPig += ChangeObjectiveTimeForFlappyFrenzy;
        }

    }

    private void OnDisable()
    {
        if (flappyFrenzy)
        {
            GameTimer.OnAddFlappyPig -= ChangeObjectiveTimeForFlappyFrenzy;
        }

    }



}
