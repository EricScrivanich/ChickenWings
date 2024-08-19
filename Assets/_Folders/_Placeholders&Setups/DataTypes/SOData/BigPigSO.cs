[System.Serializable]
public class BigPigSO : EnemyData
{
    public BigPigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {

        for (int i = 0; i < data.Length; i++)
        {
            manager.GetBigPig(data[i].position, data[i].scale, data[i].speed,data[i].yForce,data[i].distanceToFlap);
        }

    }



    // Start is called before the first frame update

}