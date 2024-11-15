using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyFrenzySpawner : MonoBehaviour
{
    [Header("Flappy Pig")]
    [SerializeField] private float[] spawnFlappyPigDelays;
    [SerializeField] private SpecialRandomEnemyLogic[] flappyPigData;
    [SerializeField] private int[] nextFlappyPigDataValues;

    private int currentFlappyPigDataIndex;
    private int currentFlappyPigIndex;

    [Header("Eggs")]
    [SerializeField] private float[] spawnEggDelays;
    [SerializeField] private int[] eggTypes;
    private int currentEggIndex;






    private SpawnStateManager spawner;

    private BarnAndEggSpawner eggSpawner;
    // Start is called before the first frame update
    void Start()
    {
        currentFlappyPigIndex = 0;
        currentFlappyPigDataIndex = 0;
        spawner = GetComponent<SpawnStateManager>();

        StartCoroutine(SpawnFlappyPigsCoroutine());
        StartCoroutine(SpawnEggs());

    }

    // Update is called once per frame
    private IEnumerator SpawnFlappyPigsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnFlappyPigDelays[currentFlappyPigIndex]);
            flappyPigData[currentFlappyPigDataIndex].SpawnWithoutTimer(spawner);

            currentFlappyPigIndex++;

            if (currentFlappyPigIndex >= nextFlappyPigDataValues[currentFlappyPigDataIndex])
                currentFlappyPigDataIndex++;

        }

    }

    private IEnumerator SpawnEggs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnEggDelays[currentEggIndex]);
            Vector2 pos = new Vector2(Random.Range(13.8f, 16f), Random.Range(-2.8f, 4.1f));
            spawner.GetEggByType(pos, eggTypes[currentEggIndex], 0);
            currentEggIndex++;
        }

    }
}
