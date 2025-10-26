using UnityEngine;

public class LevelDataRandomSpawnData
{

    private ISpawnData[] spawnDatas;
    private RecordableObjectPool pool;

    private ushort dataType;

    public void Initialize(ISpawnData[] spawnDatas, RecordableObjectPool pool, ushort dataType)
    {
        this.spawnDatas = spawnDatas;
        this.pool = pool;
        this.dataType = dataType;

        Debug.Log("Initialized Random Spawn Data with " + spawnDatas.Length + " spawn datas and pool " + pool.name);
    }

    public void GenerateRandomEnemySpawn(float r)
    {
        ushort t = spawnDatas[0].GetType();
        Vector2 pos = new Vector2(Random.Range(spawnDatas[0].GetStartPos().x, spawnDatas[1].GetStartPos().x), Random.Range(spawnDatas[0].GetStartPos().y, spawnDatas[1].GetStartPos().y));
        if (dataType > 0)
        {
            float[] floatDataSpawn1 = spawnDatas[0].GetFloatData();
            float[] floatDataSpawn2 = spawnDatas[1].GetFloatData();
            float[] values = new float[dataType];

            Debug.Log("Random Pos " + pos);

            for (int i = 0; i < dataType; i++)
            {
                values[i] = Random.Range(floatDataSpawn1[i], floatDataSpawn2[i]);
                Debug.Log("Value " + i + ": " + values[i]);
            }
            pool.SpawnOverride(pos, t, values);

        }
        else
        {
            pool.SpawnOverride(pos, t, null);

        }



    }


}
