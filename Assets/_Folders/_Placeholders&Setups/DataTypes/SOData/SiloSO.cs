[System.Serializable]
public class SiloSO : EnemyData
{
    public SiloData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetSilo(data[i].position, data[i].type, data[i].baseHeightMultiplier);
        }
    }
}