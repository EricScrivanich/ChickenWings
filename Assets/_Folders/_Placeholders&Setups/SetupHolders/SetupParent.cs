using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnSetupParent", menuName = "Setups/SetupParent")]


public class SetupParent : ScriptableObject
{


    public int TriggerToEdit;

    public List<EnemyDataArray> enemySetup = new List<EnemyDataArray>();
    // public SpawnSetupEnemy[] enemySetup;
    public List<CollectableDataArray> collectableSetup = new List<CollectableDataArray>();

    public int collectableTriggerCount => collectableSetup.Count;

    public int enemyTriggerCount => enemySetup.Count;


    public void SpawnEnemiesOnly(CollectablePoolManager collectableManager, EnemyPoolManager enemyManager, int currentTrigger)
    {

        foreach (var enemySet in enemySetup[currentTrigger].dataArray)
        {
            enemySet.InitializeEnemy(enemyManager);
        }
        collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(enemySetup[currentTrigger].dataArray, null)));

    }

    public void SpawnCollectablesOnly(CollectablePoolManager collectableManager, int currentTrigger)
    {
        foreach (var coll in collectableSetup[currentTrigger].dataArray)
        {
            coll.InitializeCollectable(collectableManager, collectableSetup.Count == currentTrigger + 1);
        }
        collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(null, collectableSetup[currentTrigger].dataArray)));

    }





    public void SpawnBoth(CollectablePoolManager collectableManager, EnemyPoolManager enemyManager, int currentTrigger)
    {
        if (collectableSetup[currentTrigger].dataArray.Length > 0)
        {
            foreach (var coll in collectableSetup[currentTrigger].dataArray)
            {
                coll.InitializeCollectable(collectableManager, collectableSetup.Count == currentTrigger + 1);
            }
        }

        if (enemySetup[currentTrigger].dataArray.Length > 0)
        {
            foreach (var enemySet in enemySetup[currentTrigger].dataArray)
            {
                enemySet.InitializeEnemy(enemyManager);
            }
          

        }

        collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(enemySetup[currentTrigger].dataArray, collectableSetup[currentTrigger].dataArray)));



    }



    public void RecordForEnemyTrigger(List<EnemyData> dataList, int trigger)
    {
        EnemyData[] data = new EnemyData[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            data[i] = dataList[i];
        }

        var indexData = new EnemyDataArray(data);
        enemySetup[trigger] = indexData;

    }
    public void RecordSpecificEnemy(EnemyData data, int trigger)
    {
        if (trigger >= 0 && trigger < enemySetup.Count)
        {
            EnemyDataArray array = enemySetup[trigger];

            for (int i = 0; i < array.dataArray.Length; i++)
            {
                if (array.dataArray[i].GetType() == data.GetType())
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
    public void RecordSpecificCollectable(CollectableData data, int trigger)
    {
        if (trigger >= 0 && trigger < enemySetup.Count)
        {
            CollectableDataArray array = collectableSetup[trigger];

            for (int i = 0; i < array.dataArray.Length; i++)
            {
                if (array.dataArray[i].GetType() == data.GetType())
                {
                    array.dataArray[i] = data;
                    return; // Assuming you want to replace only one matching entry
                }
            }

        }
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

        // if (dataCollectable != null)
        // {
        //     foreach (var collSetup in dataCollectable)
        //     {
        //         if (enemySetup.TimeToTrigger > maxTimeToTrigger) maxTimeToTrigger = enemySetup.TimeToTrigger;
        //     }
        // }

        Debug.Log("Trigger Time for next Enemy setup is: " + maxTimeToTrigger + " seconds with a count of: " + count);
        return maxTimeToTrigger;
    }




    private bool CompareTriggers(Vector2 posEnemy, float speedEnemy, float xCordEnemy, Vector2 posColl, float speedColl, float xCordColl)
    {
        // Calculate the distance each object needs to travel
        float distanceEnemy = Mathf.Abs(xCordEnemy - posEnemy.x);
        float distanceColl = Mathf.Abs(xCordColl - posColl.x);

        // Calculate the time it takes for each object to reach the target x-coordinate
        float timeEnemy = distanceEnemy / speedEnemy;
        float timeColl = distanceColl / speedColl;

        // Compare the times
        if (timeEnemy > timeColl)
        {
            Debug.Log("The enemy will take longer to reach its x-coordinate.");
            return true;
            // Perform any additional logic for when the enemy takes longer
        }
        else if (timeColl > timeEnemy)
        {
            Debug.Log("The collectible will take longer to reach its x-coordinate.");
            return false;

            // Perform any additional logic for when the collectible takes longer
        }
        else
        {
            Debug.Log("Both will take the same time to reach their x-coordinates.");
            return true;

            // Perform any additional logic for when both take the same time
        }
    }

    // Start is called before the first frame update

}
