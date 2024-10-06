[System.Serializable]
public class BomberPlaneSO : EnemyData
{
    public BomberPlaneData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetBomberPlane(data[i].xDropPosition, data[i].dropAreaScaleMultiplier, data[i].speedTarget);
        }
    }
}