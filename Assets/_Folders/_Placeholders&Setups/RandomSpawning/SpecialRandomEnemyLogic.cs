using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSpecialRandomEnemyLogic", menuName = "Randomspawning/SpecialEnemy")]
public class SpecialRandomEnemyLogic : ScriptableObject
{
    [SerializeField] private int ID;

    private PlayerID player;




    [SerializeField] private float flipSpawnChance;
    private int flippedSideInt = 1;
    [SerializeField] private Vector2 SpawnXRange;
    [SerializeField] private Vector2 SpawnYRange;
    [SerializeField] private Vector2 spawnDelayTimeRange;
    [SerializeField] private Vector2 spawnCooldownTimeRange;
    [SerializeField] private Vector2[] rangesFloat;
    [SerializeField] private Vector2Int[] rangesInt;


    public void ReturnSpecialEnemyData(SpawnStateManager spawner)
    {
        if (ID == -1)
        {
            spawner.NextLogicTriggerAfterDelay(.2f, ID);
            return;
        }

        float[] val = new float[rangesFloat.Length];

        float ran = Random.Range(0, 1f);

        if (flipSpawnChance >= ran)
            flippedSideInt = -1;
        else flippedSideInt = 1;

        Debug.LogError("Random chance is: " + ran);


        for (int i = 0; i < rangesFloat.Length; i++)
        {
            val[i] = Random.Range(rangesFloat[i].x, rangesFloat[i].y);
        }


        spawner.StartCoroutine(SpawnAfterDelay(spawner, val));

        // switch (ID)
        // {
        //     case (0):

        //         spawner.StartCoroutine(SpawnAfterDelay(spawner, val));
        //         break;

        //     case (0):

        //         spawner.StartCoroutine(SpawnAfterDelay(spawner, val));
        //         break;
        // }

        spawner.NextLogicTriggerAfterDelay(Random.Range(spawnCooldownTimeRange.x, spawnCooldownTimeRange.y), ID);

    }


    private IEnumerator SpawnAfterDelay(SpawnStateManager spawner, float[] val)
    {
        yield return new WaitForSeconds(Random.Range(spawnDelayTimeRange.x, spawnDelayTimeRange.y));
        switch (ID)
        {
            case (0):
                float x = BoundariesManager.leftBoundary + .5f;

                if (val[0] >= Random.Range(0f, 1f))
                    x *= -1;

                spawner.GetFlappyPig(new Vector2(flippedSideInt * Random.Range(SpawnXRange.x, SpawnXRange.y), Random.Range(SpawnYRange.x, SpawnYRange.y)), val[0]);
                break;

            case (1):
                spawner.GetWindMill(new Vector2(Random.Range(SpawnXRange.x, SpawnXRange.y), Random.Range(SpawnYRange.x, SpawnYRange.y)), 3, val[0], flippedSideInt * val[1], 0);
                break;

            case (2):
                int s = 8;
                if (flippedSideInt < 0) s = -7;
                spawner.GetBomberPlane(val[0], val[1], s);
                break;

            case (3):

                spawner.GetHotAirBalloon(new Vector2(flippedSideInt * Random.Range(SpawnXRange.x, SpawnXRange.y), Random.Range(SpawnYRange.x, SpawnYRange.y)), 0, val[3], val[1], val[0] * flippedSideInt, val[2]);

                break;
        }
    }


}
