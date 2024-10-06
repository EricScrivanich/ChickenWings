[System.Serializable]
public class FlappyPigSO : EnemyData
{
    public FlappyPigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetFlappyPig(data[i].position, data[i].scaleFactor);
        }
    }
}