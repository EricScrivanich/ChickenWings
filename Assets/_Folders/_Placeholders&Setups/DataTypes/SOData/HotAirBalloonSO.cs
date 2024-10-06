[System.Serializable]
public class HotAirBalloonSO : EnemyData
{
    public HotAirBalloonData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetHotAirBalloon(data[i].position, data[i].type, data[i].xTrigger, data[i].yTarget, data[i].speed, data[i].delay);
        }
    }
}