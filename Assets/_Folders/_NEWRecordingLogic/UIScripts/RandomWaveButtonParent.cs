using UnityEngine;

public class RandomWaveButtonParent : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    private int currentWaveAmount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentWaveAmount = LevelRecordManager.instance.RandomWaveAmount;

        for (int i = 0; i < currentWaveAmount; i++)
        {
            var o = Instantiate(buttonPrefab, buttonParent);
            o.GetComponent<ObjectUI>().SetAsRandomWaveButton(i + 1);
            o.SetActive(true);
        }

    }

    public void AddWave()
    {
        var o = Instantiate(buttonPrefab, buttonParent);

        var s = o.GetComponent<ObjectUI>();
        s.SetAsRandomWaveButton(currentWaveAmount + 1);
        o.SetActive(true);
        s.PressWaveButton();


    }

    // Update is called once per frame

}
