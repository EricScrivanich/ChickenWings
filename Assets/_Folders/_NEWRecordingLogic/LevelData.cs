using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Setups/LevelData")]
public class LevelData : ScriptableObject
{
    public WaveData[] waves;

    public void CreateNewWave(WaveData w)
    {
        waves = new WaveData[1];
        waves[0] = w;
    }


}
