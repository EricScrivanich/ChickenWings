using UnityEngine;



[CreateAssetMenu(fileName = "RecordableObjectPool", menuName = "ScriptableObjects/RecordableObjectPool")]
public class RecordableObjectPool : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private bool instantiateOnly = false;


    [SerializeField] private IRecordableObject[] objectVarients;
    [SerializeField] private bool useObjectVarients = false;


    private SpawnedObject[] pool;
    private ObjectPositioner[] positionerPool;
    private int currentIndex;
    public int poolSize;
    private ushort spawnNumber;

    public void CreatePool(int size)
    {
        currentIndex = 0;
        if (instantiateOnly)
        {

            return;
        }
        spawnNumber = 0;

        pool = new SpawnedObject[size];
        poolSize = size;
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(prefab);
            Debug.Log("Creating pool object: " + go.name);


            pool[i] = go.GetComponent<SpawnedObject>();
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
    public void SpawnPositionerData(RecordedObjectPositionerDataSave data)
    {
        if (pool == null || pool.Length == 0)
        {
            return;
        }
        if (pool[currentIndex] == null)
        {
            Debug.LogError("pool is empty");
            return;
        }
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyPositionerData(data);
        currentIndex++;
        spawnNumber++;

        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }


    public CageAttatchment GetCageAttachment()
    {
        int index = currentIndex - 1;
        // Wrap around if index is negative
        if (pool != null && pool.Length > 0)
        {
            if (index < 0) index = pool.Length - 1;
            GameObject obj = (pool[index] as MonoBehaviour)?.gameObject;
            if (obj.GetComponent<CageAttatchment>() != null)
            {
                return obj.GetComponent<CageAttatchment>();
            }
            else if (obj.GetComponentInChildren<CageAttatchment>() != null)
            {
                return obj.GetComponentInChildren<CageAttatchment>();
            }
            else
            {
                Debug.LogError("CageAttatchment component not found on " + obj.name);
                return null;
            }
        }
        else if (positionerPool != null && positionerPool.Length > 0)
        {
            if (index < 0) index = positionerPool.Length - 1;
            GameObject obj = positionerPool[index].gameObject;
            if (obj.GetComponent<CageAttatchment>() != null)
            {
                return obj.GetComponent<CageAttatchment>();
            }
            else if (obj.GetComponentInChildren<CageAttatchment>() != null)
            {
                return obj.GetComponentInChildren<CageAttatchment>();
            }
            else
            {
                Debug.LogError("CageAttatchment component not found on " + obj.name);
                return null;
            }
        }

        else
        {
            Debug.LogError("positionerPool is null or empty");
            return null;
        }

    }
    public void SpawnSimpleObject(DataStructSimple data)
    {
        var obj = Instantiate(prefab, data.startPos, Quaternion.identity);
        obj.GetComponent<ISimpleRecordableObject>()?.ApplyTypeData(data.type);


    }
    public void SpawnFloatOne(DataStructFloatOne data, bool addCage = false)
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
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyFloatOneData(data);

        currentIndex++;
        spawnNumber++;
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
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyFloatTwoData(data);
        spawnNumber++;
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
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyFloatThreeData(data);
        spawnNumber++;
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
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyFloatFourData(data);
        currentIndex++;
        spawnNumber++;
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
        pool[currentIndex].InitialSpawnCheck(spawnNumber);
        pool[currentIndex].ApplyFloatFiveData(data);
        spawnNumber++;
        currentIndex++;
        if (currentIndex >= pool.Length)
        {
            currentIndex = 0;
        }
    }



}
