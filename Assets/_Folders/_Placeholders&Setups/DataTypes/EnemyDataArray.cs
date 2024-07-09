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