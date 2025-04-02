using UnityEngine;



[CreateAssetMenu(fileName = "RecordableObjectPool", menuName = "RecordableObjectPool")]
public class RecordableObjectPool : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    private IRecordableObject[] pool;
    private int currentIndex;
    public int poolSize;

    public void CreatePool(int size)
    {
        currentIndex = 0;
        pool = new IRecordableObject[size];
        poolSize = size;
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(prefab);
            Debug.Log("Creating pool object: " + go.name);


            pool[i] = go.GetComponent<IRecordableObject>();
            go.SetActive(false);
        }
    }

    // public void SpawnItem(RecordedDataStruct data)
    // {
    //     if (pool == null || pool.Length == 0)
    //     {

    //         return;
    //     }
    //     pool[currentIndex].ApplyRecordedData(data);
    //     currentIndex++;
    //     if (currentIndex >= pool.Length)
    //     {
    //         currentIndex = 0;
    //     }
    // }

    public void SpawnFloatOne(DataStructFloatOne data)
    {
        if (pool == null || pool.Length == 0)
        {

            return;
        }
        if (pool[currentIndex] == null)
        {
            Debug.LogError("Pool is empty");
            return;
        }
        pool[currentIndex].ApplyFloatOneData(data);
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }
    public void SpawnFloatTwo(DataStructFloatTwo data)
    {
        if (pool == null || pool.Length == 0)
        {

            return;
        }
        pool[currentIndex].ApplyFloatTwoData(data);
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }
    public void SpawnFloatThree(DataStructFloatThree data)
    {
        if (pool == null || pool.Length == 0)
        {

            return;
        }
        pool[currentIndex].ApplyFloatThreeData(data);
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }
    public void SpawnFloatFour(DataStructFloatFour data)
    {
        if (pool == null || pool.Length == 0)
        {

            return;
        }
        pool[currentIndex].ApplyFloatFourData(data);
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }
    public void SpawnFloatFive(DataStructFloatFive data)
    {
        if (pool == null || pool.Length == 0)
        {

            return;
        }
        pool[currentIndex].ApplyFloatFiveData(data);
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }

}
