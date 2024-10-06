[System.Serializable]
public class MissilePigSO : EnemyData
{
    public MissilePigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetMissilePig(data[i].position.x, data[i].movementType, data[i].missileType);
        }
    }
}