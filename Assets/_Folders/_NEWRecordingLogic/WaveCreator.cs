using UnityEngine;

public class WaveCreator : MonoBehaviour
{

    public static WaveCreator instance;

    public int numberOfSubWaves;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Open(int waveIndex)
    {
        LevelRecordManager.instance.OpenWaveEditor(true, waveIndex);
        this.gameObject.SetActive(true);
    }
    public void Close()
    {
        LevelRecordManager.instance.OpenWaveEditor(false, -1);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame

}
