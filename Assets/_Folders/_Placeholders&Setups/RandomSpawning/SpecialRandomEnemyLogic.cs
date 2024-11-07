using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSpecialRandomEnemyLogic", menuName = "Randomspawning/SpecialEnemy")]
public class SpecialRandomEnemyLogic : ScriptableObject
{
    [SerializeField] private int ID;




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

        float[] val = new float[rangesFloat.Length];

        if (flipSpawnChance >= Random.Range(0, 1f))
            flippedSideInt = -1;
        else flippedSideInt = -1;


        for (int i = 0; i < rangesFloat.Length; i++)
        {
            val[i] = Random.Range(rangesFloat[i].x, rangesFloat[i].y);
        }

        switch (ID)
        {
            case (0):

                spawner.StartCoroutine(SpawnAfterDelay(spawner, val));




                break;
        }

        spawner.NextLogicTriggerAfterDelay(Random.Range(spawnCooldownTimeRange.x, spawnCooldownTimeRange.y));

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
        }
    }


}
