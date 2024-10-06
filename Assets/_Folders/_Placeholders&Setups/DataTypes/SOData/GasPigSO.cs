[System.Serializable]
public class GasPigSO : EnemyData
{
    public GasPigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetGasPig(data[i].position, data[i].speed, data[i].delay);
        }
    }
}