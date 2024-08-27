using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateTransitionLogic", menuName = "Setups/StateTransitionLogic")]
public class SpawnStateTransitionLogic : ScriptableObject
{
    public bool loopStates;

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


    public int[] ringSpawnSetTypeOrder;

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
