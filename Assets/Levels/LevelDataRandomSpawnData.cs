using System.Linq;
using UnityEngine;

public class LevelDataRandomSpawnData
{

    private ISpawnData[] spawnDatas;
    private short[] usedRNGIndices;
    public int waveIndex { get; set; }

    [SerializeField] private float[] spawnDataPercentages;

    private short[] usedRandomDataIndices;
    private RecordableObjectPool pool;

    private ushort dataType;

    public int neededRNGAmount { get; private set; }
    private byte minRngSpawnValue = 0;
    private byte maxRngSpawnValue = 100;
    private RandomWaveData parentData;





    public void Initialize(RandomWaveData data, ISpawnData[] spawnDatas, short[] randomDataIndices, RecordableObjectPool pool, ushort dataType, int waveInd, byte min = 0, byte max = 100)
    {
        this.spawnDatas = spawnDatas;
        this.parentData = data;
        this.usedRNGIndices = randomDataIndices;
        this.pool = pool;
        this.dataType = dataType;
        this.waveIndex = waveInd;
        this.minRngSpawnValue = min;
        this.maxRngSpawnValue = max;
        neededRNGAmount = usedRNGIndices.Max();

        Debug.Log("Initialized Random Spawn Data with " + spawnDatas.Length + " spawn datas and pool " + pool.name);
    }

    private float GetRNGPercent(short index)
    {
        if (index < 0)
            return (float)parentData.GetRNG() / 100f;
        else
        {
            return (float)rngData[index] / 100f;
        }

    }
    private byte[] rngData;

    public void GenerateRandomEnemySpawn(byte[] _rngData)
    {
        byte rngValue = 0;

        if (usedRNGIndices[0] == -1)
            rngValue = parentData.GetRNG();
        else
            rngValue = _rngData[usedRNGIndices[0]];

        if (rngValue > maxRngSpawnValue || rngValue < minRngSpawnValue)
        {
            Debug.LogError("RNG Data out of bounds: " + rngValue);
            return;
        }
        ushort t = spawnDatas[0].GetType();
        rngData = _rngData;

        // Mathf.Lerp(spawnDatas[0].GetStartPos().x, spawnDatas[1].GetStartPos().x, GetRNGPercent(usedRNGIndices[0]));
        // Vector2 pos = new Vector2(Random.Range(spawnDatas[0].GetStartPos().x, spawnDatas[1].GetStartPos().x), Random.Range(spawnDatas[0].GetStartPos().y, spawnDatas[1].GetStartPos().y));
        Vector2 pos = new Vector2(Mathf.Lerp(spawnDatas[0].GetStartPos().x, spawnDatas[1].GetStartPos().x, GetRNGPercent(usedRNGIndices[1])),
                                  Mathf.Lerp(spawnDatas[0].GetStartPos().y, spawnDatas[1].GetStartPos().y, GetRNGPercent(usedRNGIndices[1])));
        if (dataType > 0)
        {
            float[] floatDataSpawn1 = spawnDatas[0].GetFloatData();
            float[] floatDataSpawn2 = spawnDatas[1].GetFloatData();
            float[] values = new float[dataType];

            Debug.Log("Random Pos " + pos);

            for (int i = 0; i < dataType; i++)
            {
                Debug.Log("RNG Index for value " + i + " with total length" + usedRNGIndices.Length);


                values[i] = Mathf.Lerp(floatDataSpawn1[i], floatDataSpawn2[i], GetRNGPercent(usedRNGIndices[i + 3]));
                // values[i] = Random.Range(floatDataSpawn1[i], floatDataSpawn2[i]);
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
