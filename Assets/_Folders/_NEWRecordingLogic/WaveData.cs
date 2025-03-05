using UnityEngine;

public class WaveData : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RecordedDataStruct[] Data;

    public void SpawnWave(SpawnStateManager spawner)
    {
        for (int i = 0; i < Data.Length; i++)
        {
            if (Data[i].ID == 0) spawner.GetPigNew(Data[i]);
        }

    }


}
