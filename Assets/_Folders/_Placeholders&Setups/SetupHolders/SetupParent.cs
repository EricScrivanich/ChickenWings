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
    public SpawnSetupCollectable[] collectableSetup;

    public int collectableTriggerCount
    {
        get
        {

            return collectableSetup.Length;

        }

        private set
        {

        }

    }
    public int enemyTriggerCount
    {
        get
        {
            return enemySetup.Count;
        }

        private set
        {

        }

    }

    public void SpawnEnemiesOnly(CollectablePoolManager collectableManager, EnemyPoolManager enemyManager, int currentTrigger)
    {

        foreach(var enemySet in enemySetup[currentTrigger].dataArray)
        {
            enemySet.InitializeEnemy(enemyManager);
        }
        collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(enemySetup[currentTrigger].dataArray, null)));
        // foreach (var setup in enemySetup[currentTrigger])
        // {
        //     setup.InitializeEnemy(enemyManager);
        // }


        // collectableManager.StartCoroutine(collectableManager.NextTriggerCourintine(CheckTime(enemySetup[currentTrigger], null)));
        // collectableManager.GetTriggerObject(enemySetup[currentTrigger].triggerObjectPosition, enemySetup[currentTrigger].triggerObjectSpeed, enemySetup[currentTrigger].triggerObjectXCordinateTrigger);
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
    
    public float CheckTime(EnemyData[] dataEnemy, CollectableData dataCollectable)
    {
        float maxTimeToTrigger = 0f;

        if (dataEnemy != null)
        {
            foreach (var enemySetup in dataEnemy)
            {
                if (enemySetup.TimeToTrigger > maxTimeToTrigger) maxTimeToTrigger = enemySetup.TimeToTrigger;
            }
        }

        // if (dataCollectable != null)
        // {
        //     foreach (var collSetup in dataCollectable)
        //     {
        //         if (enemySetup.TimeToTrigger > maxTimeToTrigger) maxTimeToTrigger = enemySetup.TimeToTrigger;
        //     }
        // }

        Debug.Log("Trigger Time for next Enemy setup is: " + maxTimeToTrigger + " seconds");
        return maxTimeToTrigger;
    }

    public void SpawnCollectablesOnly(CollectablePoolManager collectableManager, int currentTrigger)
    {
        collectableSetup[currentTrigger].SpawnCollectables(collectableManager, collectableSetup.Length == currentTrigger + 1);
        collectableManager.GetTriggerObject(collectableSetup[currentTrigger].triggerObjectPosition, collectableSetup[currentTrigger].triggerObjectSpeed, collectableSetup[currentTrigger].triggerObjectXCordinateTrigger);
    }


    public void SpawnBoth(CollectablePoolManager collectableManager, EnemyPoolManager enemyManager, int currentTrigger)
    {
        // bool hasEnemies = false;
        // bool hasCollectables = false;

        // if (enemySetup[currentTrigger] == null && collectableSetup[currentTrigger] == null)
        // {
        //     Debug.Log("NoIndex");
        //     return;
        // }

        // if (enemySetup[currentTrigger] != null)
        // {
        //     enemySetup[currentTrigger].SpawnEnemies(enemyManager);
        //     hasEnemies = true;
        // }

        // if (collectableSetup[currentTrigger] != null)
        // {
        //     collectableSetup[currentTrigger].SpawnCollectables(collectableManager, collectableSetup.Length == currentTrigger + 1);
        //     hasCollectables = true;
        // }

        // if (hasEnemies && !hasCollectables) collectableManager.GetTriggerObject(enemySetup[currentTrigger].triggerObjectPosition, enemySetup[currentTrigger].triggerObjectSpeed, enemySetup[currentTrigger].triggerObjectXCordinateTrigger);
        // else if (!hasEnemies && hasCollectables) collectableManager.GetTriggerObject(collectableSetup[currentTrigger].triggerObjectPosition, collectableSetup[currentTrigger].triggerObjectSpeed, collectableSetup[currentTrigger].triggerObjectXCordinateTrigger);
        // else if (hasEnemies && hasCollectables)
        // {



        // bool useEnemy = CompareTriggers(enemySetup[currentTrigger].triggerObjectPosition, enemySetup[currentTrigger].triggerObjectSpeed, enemySetup[currentTrigger].triggerObjectXCordinateTrigger,
        // collectableSetup[currentTrigger].triggerObjectPosition, collectableSetup[currentTrigger].triggerObjectSpeed, collectableSetup[currentTrigger].triggerObjectXCordinateTrigger);

        // if (useEnemy) collectableManager.GetTriggerObject(enemySetup[currentTrigger].triggerObjectPosition, enemySetup[currentTrigger].triggerObjectSpeed, enemySetup[currentTrigger].triggerObjectXCordinateTrigger);
        // else collectableManager.GetTriggerObject(collectableSetup[currentTrigger].triggerObjectPosition, collectableSetup[currentTrigger].triggerObjectSpeed, collectableSetup[currentTrigger].triggerObjectXCordinateTrigger);

        // enemySetup[currentTrigger].SpawnEnemies(enemyManager);
        // collectableSetup[currentTrigger].SpawnCollectables(collectableManager, collectableSetup.Length == currentTrigger + 1);


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
