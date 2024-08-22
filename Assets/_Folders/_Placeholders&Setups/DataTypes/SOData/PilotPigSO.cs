
[System.Serializable]
public class PilotPigSO : EnemyData
{
    public PilotPigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {

        for (int i = 0; i < data.Length; i++)
        {
            manager.GetPilotPig(data[i].position, data[i].scale, data[i].speed, data[i].flightMode, data[i].minY, data[i].maxY, data[i].yForce, data[i].maxYSpeed, data[i].xTrigger);
        }

    }

}