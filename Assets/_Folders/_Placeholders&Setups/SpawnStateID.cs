using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateTransitionLogic", menuName = "Setups/StateTransitionLogic")]
public class SpawnStateTransitionLogic : ScriptableObject
{
    public bool loopStates;

    [SerializeField] private SpecialRandomEnemyLogic[] specialEnemySpawns;

    public bool continueFromStartIfOverriden;
    public bool continueFromPrevOverriden;
    [Header("-1 = Random;  -2 = Balloon,  -3 = Flappy,  3 = PureRandom")]

    [Header("0 = PureSetup;  1 = RandomRing,  2 = RandomEnemySetup,  3 = PureRandom")]
    [SerializeField] private int[] orderedSequence;
    public int[] OrderedSequnece
    {
        get => orderedSequence;
        private set => orderedSequence = value;
    }

    [SerializeField] private float[] randomSequenceWithWeights;
    public float[] RandomSequenceWithWeights
    {
        get => randomSequenceWithWeights;
        private set => randomSequenceWithWeights = value;
    }

    public Vector2Int[] RingAmountsByType;


    public int[] ringSpawnSetTypeOrder;

    public void SpawnSpecialEnemy(SpawnStateManager spawner)
    {
        specialEnemySpawns[0].ReturnSpecialEnemyData(spawner);
    }

    public int GetRingSizeByType(int type)
    {
        if (RingAmountsByType == null || RingAmountsByType.Length <= type)
        {
            Debug.LogError("No Ring amount by types availiable, type is: " + type);
            return -1;
        }
        int val = Random.Range(RingAmountsByType[type].x, RingAmountsByType[type].y + 1);

        Debug.LogError("Ring type is avaialibe, value is: " + val);

        if (val == 0) return -1;
        return val;





    }

    public int GetRandomSequenceIndex()
    {
        if (RandomSequenceWithWeights == null || RandomSequenceWithWeights.Length == 0)
        {
            Debug.LogWarning("Sequence weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in RandomSequenceWithWeights)
        {
            totalWeight += weight;
        }

        

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < RandomSequenceWithWeights.Length; i++)
        {
            cumulativeWeight += RandomSequenceWithWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Sequence Index is: " + i);
                return i; // Return the index
            }
        }

        return RandomSequenceWithWeights.Length - 1; // Return the max index if something goes wrong
    }

    public void NextTransitionState()
    {

    }







}
