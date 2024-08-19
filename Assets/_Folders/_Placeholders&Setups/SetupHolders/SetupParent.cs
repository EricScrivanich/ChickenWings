using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnSetupParent", menuName = "Setups/SetupParent")]


public class SetupParent : ScriptableObject
{




    public bool mustCompleteRingSequence;
    private Queue<int> lastPickedTriggers = new Queue<int>();

    public int[] enemyTriggerForEachRingIndex;

    public List<EnemyDataArray> enemySetup = new List<EnemyDataArray>();
    // public SpawnSetupEnemy[] enemySetup;
    public List<CollectableDataArray> collectableSetup = new List<CollectableDataArray>();

    public int collectableTriggerCount => collectableSetup.Count;

    public int enemyTriggerCount => enemySetup.Count;
    public bool isRandomSetup;






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

        int randomRingTrigger = Random.Range(0, collectableSetup.Count);

        int randomRingSetupInTrigger = Random.Range(0, collectableSetup[randomRingTrigger].dataArray.Length);
        var pickedRingSet = collectableSetup[randomRingTrigger].dataArray[randomRingSetupInTrigger];
        int randomEnemyTrigger;

        if (randomRingTrigger == 0)
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
            foreach (var set in enemySetup[randomEnemyTrigger].dataArray)
            {
                set.InitializeEnemy(manager);
                tempList.Add(set.TimeToTrigger);
            }

        }


        // manager.StartCoroutine(manager.SetupDuration(Mathf.Max(tempList.ToArray())));
        manager.TimerForNextWave(Mathf.Max(tempList.ToArray()));

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

        Debug.Log("Trigger Time for next Enemy setup is: " + maxTimeToTrigger + " seconds with a count of: " + count);
        return maxTimeToTrigger;
    }




    public void RecordForEnemyTrigger(List<EnemyData> dataList, int trigger)
    {
        EnemyData[] data = new EnemyData[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            data[i] = dataList[i];
        }

        var indexData = new EnemyDataArray(data);
        Debug.Log("Length is: " + indexData.dataArray.Length);
        enemySetup[trigger] = indexData;

    }

    public void DuplicateOrRemoveEnemy(int trigger, int indexOfDuplicate, bool duplicate, EnemyData dataType)
    {
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







    // Start is called before the first frame update

}
