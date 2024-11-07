using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnSetupParent", menuName = "Setups/SetupParent")]


public class SetupParent : ScriptableObject
{
    [SerializeField] private Vector3[] eventCallBack_Trigger_ID_Delay;



    [Header("Egg stuff, 0 and 1 normal, 2 and 3 shotgun")]
    [SerializeField] private Vector3Int[] eggTriggerAndIndexAndType;
    [SerializeField] private Vector2[] eggPosition;
    [SerializeField] private float[] eggSpeed;


    [Header("Ring Stuff")]
    public bool mustCompleteRingSequence;
    private Queue<int> lastPickedTriggers = new Queue<int>();
    private Queue<int> recentSpawnedRandomRingTriggers;

    public int[] enemyTriggerForEachRingIndex;
    public int[] enemyTriggerForEachRingIndexWeights;

    public List<EnemyDataArray> enemySetup = new List<EnemyDataArray>();
    // public SpawnSetupEnemy[] enemySetup;
    public List<CollectableDataArray> collectableSetup = new List<CollectableDataArray>();

    public int collectableTriggerCount => collectableSetup.Count;

    public int enemyTriggerCount => enemySetup.Count;
    public bool isRandomSetup;
    [Header("Testing Data")]
    public bool testFromTrigger;
    public int startTestTrigger;

    [Header("Recording Data")]
    public bool ignoreXTriggetTime;
    public float XTriggerForRecording;






    // public void SpawnCollectablesOnly(CollectablePoolManager collectableManager, int currentTrigger, bool isFinal)
    // {
    //     foreach (var coll in collectableSetup[currentTrigger].dataArray)
    //     {
    //         coll.InitializeCollectable(collectableManager, collectableSetup.Count == currentTrigger + 1);
    //     }
    //     if (isFinal == null)
    //         collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(null, collectableSetup[currentTrigger].dataArray)));

    // }

    public void SpawnRandomSetWithRings(SpawnStateManager manager, bool finalRing)
    {
        List<float> tempList = new List<float>();

        int randomRingTrigger = -1;
        bool ringFound = false;

        if (recentSpawnedRandomRingTriggers == null)
            recentSpawnedRandomRingTriggers = new Queue<int>();

        // Try to get a unique random ring index up to 3 times
        for (int i = 0; i < 3; i++)
        {
            randomRingTrigger = GetRandomRingIndexByWeight();

            if (GetRingNotFromQueue(randomRingTrigger))
            {
                ringFound = true;
                break;
            }
        }

        // If no unique ring index was found after 3 attempts, use the least recent trigger from the queue
        if (!ringFound)
        {
            // Check if the queue does not cover all possible ring triggers
            if (recentSpawnedRandomRingTriggers.Count < collectableSetup.Count)
            {
                // Find a random ring trigger not currently in the queue
                for (int i = 0; i < collectableSetup.Count; i++)
                {
                    if (GetRingNotFromQueue(i))
                    {
                        randomRingTrigger = i;
                        ringFound = true;
                        break;
                    }
                }
            }

            // If still no unique index found, fall back to the least recent trigger
            else
            {
                randomRingTrigger = recentSpawnedRandomRingTriggers.Peek();
            }
        }


        AddToQueue(randomRingTrigger);


        int randomRingSetupInTrigger = Random.Range(0, collectableSetup[randomRingTrigger].dataArray.Length);


        // Debug.LogError("Null stuff in case, trying to spawn trigger: " + randomRingTrigger + " Set of: " + randomRingSetupInTrigger);
        var pickedRingSet = collectableSetup[randomRingTrigger].dataArray[randomRingSetupInTrigger];
        int randomEnemyTrigger;

        if (enemyTriggerForEachRingIndex == null || enemyTriggerForEachRingIndex.Length < 1)
        {
            randomEnemyTrigger = randomRingTrigger;
        }
        else if (randomRingTrigger == 0)
        {
            randomEnemyTrigger = Random.Range(0, enemyTriggerForEachRingIndex[0]);

        }
        else if (randomRingTrigger >= enemyTriggerForEachRingIndex.Length)
        {
            randomEnemyTrigger = -1;
        }
        else
        {
            randomEnemyTrigger = Random.Range(enemyTriggerForEachRingIndex[randomRingTrigger - 1], enemyTriggerForEachRingIndex[randomRingTrigger]);

        }

        pickedRingSet.InitializeCollectable(manager, finalRing);
        tempList.Add(pickedRingSet.TimeToTrigger);

        if (randomEnemyTrigger >= 0)
        {

            if (eggTriggerAndIndexAndType != null && eggTriggerAndIndexAndType.Length > 0)
            {
                foreach (var item in eggTriggerAndIndexAndType)
                {
                    if (item.x == randomEnemyTrigger)
                    {
                        SpawnEgg(manager, item.y, item.z);

                    }
                }
            }
            foreach (var set in enemySetup[randomEnemyTrigger].dataArray)
            {
                set.InitializeEnemy(manager);
                tempList.Add(set.TimeToTrigger);
            }
            Debug.LogError("Ring Trigger: " + randomRingTrigger + "Ring Index in trigger: " + randomRingSetupInTrigger + "Enemy Trigger: " + randomEnemyTrigger);

        }


        // manager.StartCoroutine(manager.SetupDuration(Mathf.Max(tempList.ToArray())));
        manager.TimerForNextWave(Mathf.Max(tempList.ToArray()));

    }

    private void SpawnEgg(SpawnStateManager manager, int index, int type)
    {
        manager.GetEggByType(eggPosition[index], type, eggSpeed[index]);

    }

    private bool GetRingNotFromQueue(int tryVal)
    {
        if (recentSpawnedRandomRingTriggers.Contains(tryVal)) return false;

        else return true;
    }

    private void AddToQueue(int val)
    {
        recentSpawnedRandomRingTriggers.Enqueue(val);

        if (recentSpawnedRandomRingTriggers.Count > 4)
            recentSpawnedRandomRingTriggers.Dequeue();
    }

    private int GetRandomRingIndexByWeight()
    {
        if (enemyTriggerForEachRingIndexWeights == null || enemyTriggerForEachRingIndexWeights.Length == 0)
        {
            // Debug.LogError("REturning lenght");
            return Random.Range(0, collectableSetup.Count); // Fallback to random if weights are not set
        }

        int totalWeight = 0;
        foreach (var weight in enemyTriggerForEachRingIndexWeights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < enemyTriggerForEachRingIndexWeights.Length; i++)
        {
            cumulativeWeight += enemyTriggerForEachRingIndexWeights[i];
            if (randomValue < cumulativeWeight)
            {
                return i; // Return the ring index based on weight
            }
        }

        return enemyTriggerForEachRingIndexWeights.Length - 1; // Return the last index if something goes wrong
    }



    public void SpawnRandomEnemies(SpawnStateManager manager)
    {
        List<float> tempList = new List<float>();
        int attempts = 0;
        int random = -1;

        while (attempts < 5)
        {
            random = Random.Range(0, enemySetup.Count);
            if (!lastPickedTriggers.Contains(random))
            {
                break;
            }
            attempts++;
        }

        if (attempts == 5)
        {
            random = lastPickedTriggers.Dequeue(); // Get the least recent trigger
        }

        lastPickedTriggers.Enqueue(random);
        if (lastPickedTriggers.Count > 3)
        {
            lastPickedTriggers.Dequeue(); // Ensure only the last 3 triggers are kept
        }

        var pickedSet = enemySetup[random].dataArray;

        if (eggTriggerAndIndexAndType != null && eggTriggerAndIndexAndType.Length > 0)
        {
            foreach (var item in eggTriggerAndIndexAndType)
            {
                if (item.x == random)
                {
                    SpawnEgg(manager, item.y, item.z);

                }
            }
        }

        for (int i = 0; i < pickedSet.Length; i++)
        {
            tempList.Add(pickedSet[i].TimeToTrigger);
            pickedSet[i].InitializeEnemy(manager);
        }

        manager.TimerForNextWave(Mathf.Max(tempList.ToArray()));

    }


    public void SpawnTrigger(SpawnStateManager manager, int currentTrigger)
    {

        List<float> tempList = new List<float>();


        if (eggTriggerAndIndexAndType != null && eggTriggerAndIndexAndType.Length > 0)
        {
            foreach (var item in eggTriggerAndIndexAndType)
            {
                if (item.x == currentTrigger)
                {
                    SpawnEgg(manager, item.y, item.z);

                }
            }
        }

        if (eventCallBack_Trigger_ID_Delay != null && eventCallBack_Trigger_ID_Delay.Length > 0)
        {

            foreach (var item in eventCallBack_Trigger_ID_Delay)
            {
                if (item.x == currentTrigger)
                {
                    // Debug.LogError("EVent callback Called");
                    manager.EventCallBack(Mathf.RoundToInt(item.y), item.z);

                }

            }
        }


        if (collectableSetup.Count > currentTrigger)
        {

            foreach (var coll in collectableSetup[currentTrigger].dataArray)
            {
                if (currentTrigger == collectableSetup.Count - 1)
                {


                    coll.InitializeCollectable(manager, true);
                }
                else
                {


                    coll.InitializeCollectable(manager, collectableSetup.Count == currentTrigger + 1);
                }
                tempList.Add(coll.TimeToTrigger);


            }
        }

        if (enemySetup.Count > currentTrigger)
        {
            foreach (var enemySet in enemySetup[currentTrigger].dataArray)
            {
                enemySet.InitializeEnemy(manager);
                tempList.Add(enemySet.TimeToTrigger);
            }


        }

        manager.TimerForNextWave(Mathf.Max(tempList.ToArray()));




    }

    public float TotalTime()
    {
        float totalTime = 0;
        int length;

        if (collectableSetup.Count > enemySetup.Count)
            length = collectableSetup.Count;
        else
            length = enemySetup.Count;


        for (int i = 0; i < length; i++)
        {
            float t = 0;

            if (collectableSetup != null && collectableSetup.Count > i)
            {
                foreach (var col in collectableSetup[i].dataArray)
                {
                    if (col.TimeToTrigger > t)
                    {
                        t = col.TimeToTrigger;
                    }

                }

            }
            if (enemySetup != null && enemySetup.Count > i)
            {
                foreach (var enemy in enemySetup[i].dataArray)
                {
                    if (enemy.TimeToTrigger > t)
                    {
                        t = enemy.TimeToTrigger;
                    }

                }

            }

            totalTime += t;

        }




        return totalTime;
    }

    public float CheckTime(EnemyData[] dataEnemy, CollectableData[] dataColl)
    {
        float maxTimeToTrigger = 0f;

        int count = 0;

        if (dataEnemy != null)
        {
            foreach (var enemySetup in dataEnemy)
            {
                if (enemySetup.TimeToTrigger > maxTimeToTrigger) maxTimeToTrigger = enemySetup.TimeToTrigger;
            }
            count++;
        }
        if (dataColl != null)
        {
            foreach (var collSetup in dataColl)
            {
                if (collSetup.TimeToTrigger > maxTimeToTrigger) maxTimeToTrigger = collSetup.TimeToTrigger;
            }
            count++;
        }

        // Debug.Log("Trigger Time for next Enemy setup is: " + maxTimeToTrigger + " seconds with a count of: " + count);
        return maxTimeToTrigger;
    }



#if UNITY_EDITOR

    public int CheckIfTesting()
    {
        if (!testFromTrigger) return -1;
        else return startTestTrigger;
    }
    public void RecordForEnemyTrigger(List<EnemyData> dataList, int trigger)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        EnemyData[] data = new EnemyData[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            data[i] = dataList[i];


        }

        if (ignoreXTriggetTime && enemySetup.Count > trigger)
        {
            // Existing data at this trigger
            EnemyDataArray existingData = enemySetup[trigger];
            for (int i = 0; i < Mathf.Min(existingData.dataArray.Length, data.Length); i++)
            {
                // Preserve the old time to trigger values
                data[i].TimeToTrigger = existingData.dataArray[i].TimeToTrigger;
            }
        }

        var indexData = new EnemyDataArray(data);

        Debug.Log("Length is: " + indexData.dataArray.Length);
        enemySetup[trigger] = indexData;



    }

    public void DuplicateOrRemoveEnemy(int trigger, int indexOfDuplicate, bool duplicate, EnemyData dataType)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        if (duplicate)
        {
            EnemyData[] data = new EnemyData[(enemySetup[trigger].dataArray.Length + 1)];
            EnemyData[] reUse = enemySetup[trigger].dataArray;

            int tempInt = 0;

            for (int i = 0; i < enemySetup[trigger].dataArray.Length; i++)
            {
                if (i == indexOfDuplicate)
                {
                    data[i] = reUse[i];
                    data[i + 1] = dataType;
                    tempInt = 1;
                }
                else
                {
                    data[i + tempInt] = reUse[i];
                }

            }
            var indexData = new EnemyDataArray(data);
            enemySetup[trigger] = indexData;
        }

        else
        {
            EnemyData[] data = new EnemyData[(enemySetup[trigger].dataArray.Length - 1)];
            EnemyData[] reUse = enemySetup[trigger].dataArray;

            int tempInt = 0;

            for (int i = 0; i < enemySetup[trigger].dataArray.Length; i++)
            {
                if (i == indexOfDuplicate)
                {
                    tempInt = -1;
                }
                else
                {
                    data[i + tempInt] = reUse[i];
                }
            }

            var indexData = new EnemyDataArray(data);
            enemySetup[trigger] = indexData;
        }


    }

    public void DuplicateOrRemoveCollectable(int trigger, int indexOfDuplicate, bool duplicate, CollectableData dataType)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        if (duplicate)
        {
            CollectableData[] data = new CollectableData[collectableSetup[trigger].dataArray.Length + 1];
            CollectableData[] reUse = collectableSetup[trigger].dataArray;

            int tempInt = 0;

            for (int i = 0; i < reUse.Length; i++)
            {
                if (i == indexOfDuplicate)
                {
                    // Create a new instance for the duplicate to ensure unique reference
                    data[i] = reUse[i];
                    data[i + 1] = dataType;
                    tempInt = 1;
                }
                else
                {
                    data[i + tempInt] = reUse[i];
                }
            }

            var indexData = new CollectableDataArray(data);
            collectableSetup[trigger] = indexData;
        }
        else
        {
            CollectableData[] data = new CollectableData[collectableSetup[trigger].dataArray.Length - 1];
            CollectableData[] reUse = collectableSetup[trigger].dataArray;

            int tempInt = 0;

            for (int i = 0; i < reUse.Length; i++)
            {
                if (i == indexOfDuplicate)
                {
                    tempInt = -1;
                }
                else
                {
                    data[i + tempInt] = reUse[i];
                }
            }

            var indexData = new CollectableDataArray(data);
            collectableSetup[trigger] = indexData;
        }
    }
    public void RecordSpecificEnemy(EnemyData data, int trigger, int index)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        if (trigger >= 0 && trigger < enemySetup.Count)
        {
            EnemyDataArray array = enemySetup[trigger];

            for (int i = 0; i < array.dataArray.Length; i++)
            {
                if (array.dataArray[i].GetType() == data.GetType() && index == i)
                {
                    array.dataArray[i] = data;
                    return; // Assuming you want to replace only one matching entry
                }
            }

        }
    }

    public void RecordForColletableTrigger(List<CollectableData> dataList, int trigger)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        CollectableData[] data = new CollectableData[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            data[i] = dataList[i];
        }

        var indexData = new CollectableDataArray(data);
        collectableSetup[trigger] = indexData;

    }
    public void RecordSpecificCollectable(CollectableData data, int trigger, int index)
    {
        Undo.RecordObject(this, "Record Enemy Trigger");
        if (trigger >= 0 && trigger < collectableSetup.Count)
        {
            CollectableDataArray array = collectableSetup[trigger];

            for (int i = 0; i < array.dataArray.Length; i++)
            {
                if (array.dataArray[i].GetType() == data.GetType() && index == i)
                {
                    array.dataArray[i] = data;
                    return; // Assuming you want to replace only one matching entry
                }
            }

        }
    }


#endif




    // Start is called before the first frame update

}
