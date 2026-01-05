using UnityEngine;

public class RandomWaveButtonParent : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefabGroup;
    [SerializeField] private GameObject buttonPrefabWave;
    [SerializeField] private Transform waveGroupButtonParent;
    [SerializeField] private Transform wavesInGroupButtonParent;

    [SerializeField] private GameObject waveGroupPanel;
    [SerializeField] private GameObject wavesInGroupPanel;

    // private int current
    private int currentWaveAmount = 0;
    private int currentWaveGroupAmount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWaveGroupAmount = LevelRecordManager.instance.GetWaveGroupAmount();

        for (int i = 0; i < currentWaveGroupAmount; i++)
        {
            var o = Instantiate(buttonPrefabGroup, waveGroupButtonParent);
            o.GetComponent<ObjectUI>().SetAsRandomWaveButton(i + 1, true, this);
            o.SetActive(true);
        }

    }
    public void OpenWaveGroup(int index)
    {
        LevelRecordManager.instance.OpenWaveGroup(index - 1, true);

        currentWaveAmount = LevelRecordManager.instance.RandomWaveAmount;
        Debug.Log("Current Wave Amount: " + currentWaveAmount);
        waveGroupPanel.SetActive(false);
        wavesInGroupPanel.SetActive(true);


        for (int i = 0; i < currentWaveAmount; i++)
        {
            var o = Instantiate(buttonPrefabWave, wavesInGroupButtonParent);
            o.GetComponent<ObjectUI>().SetAsRandomWaveButton(i + 1);
            o.SetActive(true);
        }


    }



    public void AddWave()
    {
        var o = Instantiate(buttonPrefabWave, wavesInGroupButtonParent);

        var s = o.GetComponent<ObjectUI>();
        s.SetAsRandomWaveButton(currentWaveAmount + 1);
        o.SetActive(true);
        s.PressWaveButton();


    }

    // Update is called once per frame

}
