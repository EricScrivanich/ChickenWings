using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "RandokmSpawnSetupParent", menuName = "Setups/RandomSetupParent")]


public class RandomSetupParent : ScriptableObject
{
    public List<EnemyDataArray> enemySetup = new List<EnemyDataArray>();
    public Vector2[] enemySetupYOffset;
    public Vector2[] enemyPercentFilled;
    
    public List<CollectableDataArray> collectableSetup = new List<CollectableDataArray>();
    // Start is called before the first frame update
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

}
