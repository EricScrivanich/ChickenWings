using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "StateTransitionLogic", menuName = "Setups/StateTransitionLogic")]
public class SpawnStateTransitionLogic : ScriptableObject
{
    public bool loopStates;

    public bool SpecialGoldRings;

    [SerializeField] private float[] delayStateTimes;
    private int currentDelayStateIndex;
    [SerializeField] private int maxSpecialSpawnsPerCycle;
    private bool hasPopulatedSpecialSpawns;

    [SerializeField] private bool spawnEachSpecialOnceMax;
    [SerializeField] private bool useMax;
    [SerializeField] private bool useWeights;

    [SerializeField] private float[] specialEnemySpawnWeights;
    private float[] specialEnemySpawnWeightsVar;
    [SerializeField] private SpecialRandomEnemyLogic[] specialEnemySpawns;

    private int SelectedSpecialEnemySpawnIndeces;

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

    private int[] specialSpawnLocations;
    public int[] ringSpawnSetTypeOrder;

    private int[] pickedSpecialEnemyIndexes;

    private int[] NewSpecialSpawnLocations;

    private int currentStateIndex;
    private int currentRingIndex;

    public void Reset()
    {
        currentStateIndex = 0;
        currentRingIndex = 0;
    }

    public int ReturnStateType()
    {
        if (OrderedSequnece == null || OrderedSequnece.Length < 1) return 0;
        if (currentStateIndex >= OrderedSequnece.Length)
        {
            if (loopStates)
            {
                ResetTransitionLogic();
                currentStateIndex = 0;

            }
            else return -3;
        }

        int val = orderedSequence[currentStateIndex];
        currentStateIndex++;
        return val;

    }

    public int ReturnRingType()
    {
        if (ringSpawnSetTypeOrder == null || ringSpawnSetTypeOrder.Length < 1)
        {
            Debug.LogError("RETURNING 0 cuss we nulllll");
            return 0;
        }
        if (currentRingIndex >= ringSpawnSetTypeOrder.Length)
            currentRingIndex = 0;

        int val = ringSpawnSetTypeOrder[currentRingIndex];

        currentRingIndex++;

        Debug.LogError("RETURNING RING TYPE FROM TRAN LOGIC: " + val);
        return val;

    }

    public void EnterTransitionLogic()
    {
        if (continueFromPrevOverriden)
            return;
        else
        {
            Reset();
        }
    }

    public bool CheckForNewIntensity()
    {
        Debug.LogWarning("CHECKING NEXT TTT");
        if (loopStates) return false;

        else
        {
            if (currentStateIndex >= OrderedSequnece.Length)
            {
                Debug.LogWarning("UMMM it should be true");
                return true;
            }

            else
            {
                Debug.LogWarning("UMMM it is not true");
                return false;
            }
        }
    }
    public void ResetTransitionLogic()
    {
        specialSpawnLocations = null;
        hasPopulatedSpecialSpawns = false;
        currentDelayStateIndex = 0;
        if (useWeights && specialEnemySpawnWeights != null)
        {
            specialEnemySpawnWeightsVar = (float[])specialEnemySpawnWeights.Clone();
        }
    }

    public int RetrunCurrentIndex()
    {
        return currentStateIndex;
    }

    public float ReturnDelayTime()
    {
        if (delayStateTimes == null || delayStateTimes.Length == 0)
            return 5;

        float val = delayStateTimes[currentDelayStateIndex];
        currentDelayStateIndex++;

        if (currentDelayStateIndex >= delayStateTimes.Length)
            currentDelayStateIndex = 0;

        return val;

    }
    public void SpawnSpecialEnemy(SpawnStateManager spawner)
    {
        Debug.LogError("Trying to spawn special enemy");

        if (!hasPopulatedSpecialSpawns)
        {
            CreateSpecialSpawnOrder();
        }

        for (int i = 0; i < NewSpecialSpawnLocations.Length; i++)
        {
            if (NewSpecialSpawnLocations[i] == currentStateIndex - 1)
            {
                if (pickedSpecialEnemyIndexes[i] == -1)
                    spawner.NextLogicTriggerAfterDelay(.2f, -1);

                else
                    specialEnemySpawns[pickedSpecialEnemyIndexes[i]].ReturnSpecialEnemyData(spawner);

                break;
            }
        }


        // if ((specialSpawnLocations == null || specialSpawnLocations.Length == 0) && (specialEnemySpawnWeights == null || specialEnemySpawnWeights.Length == 0))

        // if (useWeights)
        // {
        //     if (specialSpawnLocations == null)
        //     {
        //         int a = orderedSequence.Count(n => n == -1);
        //         specialSpawnLocations = new int[a];

        //         // Pick a set number of special spawns based on weights
        //         List<int> selectedIndices = new List<int>();

        //         for (int i = 0; i < maxSpecialSpawnsPerCycle; i++)
        //         {
        //             int chosenIndex = GetSpecialSpawnIndexBasedOnWeights();

        //             if (spawnEachSpecialOnceMax)
        //             {
        //                 // Ensure each index is picked only once if spawnEachSpecialOnceMax is true
        //                 while (selectedIndices.Contains(chosenIndex) && selectedIndices.Count < specialEnemySpawnWeights.Length)
        //                 {
        //                     chosenIndex = GetSpecialSpawnIndexBasedOnWeights();
        //                 }
        //             }

        //             selectedIndices.Add(chosenIndex);
        //         }

        //         // Initialize specialSpawnLocations with chosen indices
        //         int n = 0;
        //         for (int i = 0; i < specialSpawnLocations.Length; i++)
        //         {
        //             if (n < selectedIndices.Count && orderedSequence[i] == -1)
        //             {
        //                 specialSpawnLocations[n] = selectedIndices[n];
        //                 n++;
        //             }
        //             else
        //             {
        //                 specialSpawnLocations[n] = -1; // Mark as unused
        //                 n++;
        //             }
        //         }

        //         for (int o = 0; o < specialSpawnLocations.Length; o++)
        //         {
        //             Debug.Log("Index of: " + o + " is: " + specialSpawnLocations[o]);
        //         }
        //     }

        // }
        // else if (!useWeights && specialSpawnLocations == null)
        // {
        //     int a = orderedSequence.Count(n => n == -1);
        //     if (a > maxSpecialSpawnsPerCycle) useMax = true;
        //     Debug.LogError("Number of special spawns is: " + a);
        //     specialSpawnLocations = new int[a];
        //     int n = 0;

        //     for (int i = 0; i < orderedSequence.Length; i++)
        //     {
        //         if (orderedSequence[i] == -1)
        //         {
        //             specialSpawnLocations[n] = i;

        //             n++;
        //         }
        //     }


        //     if (useMax)
        //     {
        //         int disableCount = a - maxSpecialSpawnsPerCycle; // Number of spawns to disable

        //         // Generate a list of unique random indices to disable
        //         HashSet<int> indicesToDisable = new HashSet<int>();
        //         while (indicesToDisable.Count < disableCount)
        //         {
        //             int randomIndex = Random.Range(0, specialSpawnLocations.Length);
        //             indicesToDisable.Add(randomIndex);
        //         }

        //         // Mark selected indices as inactive
        //         foreach (int i in indicesToDisable)
        //         {
        //             specialSpawnLocations[i] *= -1; // Set to negative to indicate no spawn
        //             Debug.LogError("Disabled special spawn location: " + specialSpawnLocations[i]);
        //         }
        //     }
        // }
        // if (!useWeights)
        // {
        //     for (int i = 0; i < specialSpawnLocations.Length; i++)
        //     {
        //         if (specialSpawnLocations[i] == index)
        //         {
        //             Debug.LogError("Spawning Special Enemy");
        //             specialEnemySpawns[i].ReturnSpecialEnemyData(spawner);
        //             break;
        //         }
        //         else if (specialSpawnLocations[i] == -index)
        //         {
        //             Debug.LogError("Not Spawning Special Enemy");

        //             spawner.NextLogicTriggerAfterDelay(.2f, -1);
        //             break;
        //         }


        //     }
        // }

        // else
        // {

        // }


    }

    // private void CreateSpecialSpawnOrder()
    // {
    //     hasPopulatedSpecialSpawns = true;
    //     int aV = orderedSequence.Count(n => n == -1);

    //     NewSpecialSpawnLocations = new int[aV];
    //     pickedSpecialEnemyIndexes = new int[aV];

    //     int nV = 0;

    //     for (int i = 0; i < orderedSequence.Length; i++)
    //     {
    //         if (orderedSequence[i] == -1)
    //         {
    //             NewSpecialSpawnLocations[nV] = i;
    //             nV++;
    //         }


    //     }

    //     if (useWeights)
    //     {
    //         for (int s = 0; s < aV; s++)
    //         {
    //             pickedSpecialEnemyIndexes[s] = PopulateSeleclctedSpecialSpawnsBasedOnWeight();

    //         }

    //     }
    //     else
    //     {
    //         for (int s = 0; s < aV; s++)
    //         {
    //             pickedSpecialEnemyIndexes[s] = s;

    //         }

    //     }


    //     if (useMax && aV > maxSpecialSpawnsPerCycle)
    //     {
    //         int dif = aV - maxSpecialSpawnsPerCycle;

    //         // Create a list of unique random indices to disable
    //         HashSet<int> indicesToDisable = new HashSet<int>();
    //         while (indicesToDisable.Count < dif)
    //         {
    //             int randomIndex = Random.Range(0, aV);
    //             indicesToDisable.Add(randomIndex);
    //         }

    //         // Mark selected indices as inactive (-1) in pickedSpecialEnemyIndexes
    //         foreach (int index in indicesToDisable)
    //         {
    //             Debug.LogError("Disabling spawn at index: " + index + " with special spawn index: " + pickedSpecialEnemyIndexes[index]);
    //             pickedSpecialEnemyIndexes[index] = -1;
    //         }
    //     }
    // }

    private void CreateSpecialSpawnOrder()
    {
        hasPopulatedSpecialSpawns = true;

        // Step 1: Determine how many spawn slots we have based on orderedSequence and -1 markers
        int totalSpecialSpawnSlots = orderedSequence.Count(n => n == -1);

        // Initialize arrays for spawn locations and the selected special enemy indices
        NewSpecialSpawnLocations = new int[totalSpecialSpawnSlots];
        pickedSpecialEnemyIndexes = new int[totalSpecialSpawnSlots];

        // Fill NewSpecialSpawnLocations with positions of `-1` markers in orderedSequence
        int slotIndex = 0;
        for (int i = 0; i < orderedSequence.Length; i++)
        {
            if (orderedSequence[i] == -1)
            {
                NewSpecialSpawnLocations[slotIndex] = i;
                slotIndex++;
            }
        }
        if (!useMax) maxSpecialSpawnsPerCycle = totalSpecialSpawnSlots;

        // Step 2: Apply max spawns per cycle
        int allowedSpawns = Mathf.Min(maxSpecialSpawnsPerCycle, totalSpecialSpawnSlots);

        // Create a list to randomly select which slots to use for spawning
        HashSet<int> selectedSpawnIndices = new HashSet<int>();
        while (selectedSpawnIndices.Count < allowedSpawns)
        {
            int randomIndex = Random.Range(0, totalSpecialSpawnSlots);
            selectedSpawnIndices.Add(randomIndex);
        }

        // Mark all slots initially as inactive (-1)
        for (int i = 0; i < totalSpecialSpawnSlots; i++)
        {
            pickedSpecialEnemyIndexes[i] = -1;
        }

        // Step 3: Populate active slots with specific special enemies based on weights or order
        foreach (int spawnSlot in selectedSpawnIndices)
        {
            if (useWeights)
            {
                pickedSpecialEnemyIndexes[spawnSlot] = PopulateSeleclctedSpecialSpawnsBasedOnWeight();
            }
            else
            {
                pickedSpecialEnemyIndexes[spawnSlot] = spawnSlot % specialEnemySpawns.Length; // Just an example for non-weighted selection
            }
        }

        // Debugging output to check the result
        // for (int i = 0; i < pickedSpecialEnemyIndexes.Length; i++)
        // {
        //     if (pickedSpecialEnemyIndexes[i] == -1)
        //         Debug.LogError($"Special spawn slot {i} is inactive.");
        //     else
        //         Debug.LogError($"Special spawn slot {i} will spawn enemy index: {pickedSpecialEnemyIndexes[i]}.");
        // }
    }

    private int PopulateSeleclctedSpecialSpawnsBasedOnWeight()
    {

        if (specialEnemySpawnWeightsVar == null || specialEnemySpawnWeightsVar.Length == 0)
        {
            Debug.LogWarning("Special spawn weights are not set!");
            return -1;
        }

        float totalWeight = specialEnemySpawnWeightsVar.Sum();

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("All available enemies have already been spawned once in this cycle.");
            return -1;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < specialEnemySpawnWeights.Length; i++)
        {
            cumulativeWeight += specialEnemySpawnWeightsVar[i];
            if (randomValue < cumulativeWeight)
            {
                if (spawnEachSpecialOnceMax)
                    specialEnemySpawnWeightsVar[i] = 0;
                return i;
            }
        }

        return specialEnemySpawnWeights.Length - 1; // Fallback in case of rounding issues

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

    // public int GetRandomSequenceIndex()
    // {
    //     if (RandomSequenceWithWeights == null || RandomSequenceWithWeights.Length == 0)
    //     {
    //         Debug.LogWarning("Sequence weights are not set!");
    //         return 0; // Default to 0 if not set
    //     }

    //     float totalWeight = 0f;
    //     foreach (var weight in RandomSequenceWithWeights)
    //     {
    //         totalWeight += weight;
    //     }

    //     float randomValue = Random.Range(0f, totalWeight);
    //     float cumulativeWeight = 0f;

    //     for (int i = 0; i < RandomSequenceWithWeights.Length; i++)
    //     {
    //         cumulativeWeight += RandomSequenceWithWeights[i];
    //         if (randomValue < cumulativeWeight)
    //         {
    //             Debug.Log("Sequence Index is: " + i);
    //             return i; // Return the index
    //         }
    //     }

    //     return RandomSequenceWithWeights.Length - 1; // Return the max index if something goes wrong
    // }

}
