using UnityEngine;

[System.Serializable]
public class EnemyDataArray
{
    [SerializeReference] public EnemyData[] dataArray;

    public EnemyDataArray(EnemyData[] dataArray)
    {
        this.dataArray = dataArray;
    }

}

[System.Serializable]
public class CollectableDataArray
{
    [SerializeReference] public CollectableData[] dataArray;
    public CollectableDataArray(CollectableData[] dataArray)
    {
        this.dataArray = dataArray;
    }

}