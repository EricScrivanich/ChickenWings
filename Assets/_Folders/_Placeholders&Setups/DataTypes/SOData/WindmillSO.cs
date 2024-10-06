[System.Serializable]
public class WindMillSO : EnemyData
{
    public WindMillData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetWindMill(data[i].position, data[i].bladeAmount, data[i].bladeScaleMultiplier, data[i].bladeSpeed, data[i].heightMultiplier);
        }
    }
}