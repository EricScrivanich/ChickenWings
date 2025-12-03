using UnityEngine;



[CreateAssetMenu(fileName = "RecordableObjectPool", menuName = "ScriptableObjects/RecordableObjectPool")]
public class RecordableObjectPool : ScriptableObject
{
    [SerializeField] protected GameObject prefab;

    [SerializeField] protected bool instantiateOnly = false;

    protected SpawnedObject[] pool;
    protected ObjectPositioner[] positionerPool;
    protected LevelData levelData;
    protected int currentIndex;
    public int poolSize;
    protected ushort spawnNumber;

    public void CreatePool(int size, LevelData levelData)
    {
        this.levelData = levelData;
        currentIndex = 0;
        if (instantiateOnly)
        {

            return;
        }
        else if (size <= 0)
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

    // public void GenericObjectLogic(GameObject obj, short trigger)
    // {
    //     if (trigger != 0) 
    //     {
    //         switch (trigger)
    //         {
    //             case 1:
    //                obj.GetComponent<SpawnedPigBossObject>()?.SetTriggerWhenDead();
    //                 break;
    //             default:
    //                 Debug.LogWarning("Unhandled generic object logic trigger: " + trigger);
    //                 break;
    //         }
    //     }
    // }

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

    public void SpawnOverride(Vector2 pos, ushort type, float[] floatData)
    {
        int l;
        if (floatData == null)
            l = 0;
        else
            l = floatData.Length;

        switch (floatData.Length)
        {
            case 0:
                Spawn(new DataStructSimple(0, type, pos));
                break;
            case 1:
                Spawn(new DataStructFloatOne(0, type, pos, floatData[0]));
                break;
            case 2:
                Spawn(new DataStructFloatTwo(0, type, pos, floatData[0], floatData[1]));
                break;
            case 3:
                Spawn(new DataStructFloatThree(0, type, pos, floatData[0], floatData[1], floatData[2]));
                break;
            case 4:
                Spawn(new DataStructFloatFour(0, type, pos, floatData[0], floatData[1], floatData[2], floatData[3]));
                break;
            case 5:
                Spawn(new DataStructFloatFive(0, type, pos, floatData[0], floatData[1], floatData[2], floatData[3], floatData[4]));
                break;
            default:
                Debug.LogError("Invalid float data length: " + floatData.Length);
                break;
        }
    }
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
    // public void SpawnSimpleObject(DataStructSimple data)
    // {
    //     var obj = Instantiate(prefab, data.startPos, Quaternion.identity);
    //     obj.GetComponent<SpawnedObject>()?.ApplyTypeData(data.type);


    // }

    public virtual void Spawn<T>(T data) where T : ISpawnData
    {

        SpawnedObject obj;
        if (instantiateOnly)
        {
            Debug.Log("Instantiating object at position: " + data.GetStartPos());
            obj = Instantiate(prefab, data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();


        }
        else if ((pool != null || pool.Length > 0) && !pool[currentIndex].gameObject.activeInHierarchy)
        {
            obj = pool[currentIndex];
            currentIndex = (currentIndex + 1) % pool.Length;

        }

        else
        {
            Debug.LogError("Pool is null or empty");
            obj = Instantiate(prefab, data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();
            // make pool a duplicatge of itself then add object
            if (pool != null && pool.Length > 0)
            {
                SpawnedObject[] newPool = new SpawnedObject[poolSize + 1];
                for (int i = 0; i < poolSize; i++)
                {
                    newPool[i] = pool[i];
                }
                newPool[poolSize] = obj;
                pool = newPool;
                poolSize++;

            }
            else
            {
                pool = new SpawnedObject[1];
                pool[0] = obj;
                poolSize = 1;
            }




        }



        obj.InitialSpawnCheck(spawnNumber, instantiateOnly);

        data.ApplyTo(obj);


        spawnNumber++;
    }

    public virtual void SpawnBoss<T>(T data, bool hasTrigger) where T : ISpawnData
    {

        SpawnedObject obj;
        if (instantiateOnly)
        {
            Debug.Log("Instantiating object at position: " + data.GetStartPos());
            obj = Instantiate(prefab, data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();


        }
        else if (pool != null || pool.Length > 0)
        {
            obj = pool[currentIndex];
            currentIndex = (currentIndex + 1) % pool.Length;

        }

        else
            return;



        obj.InitialSpawnCheck(spawnNumber, instantiateOnly);
        var pigBoss = obj.GetComponent<SpawnedPigBossObject>();
        if (hasTrigger)
            pigBoss?.SetTriggerWhenDead(levelData.levelDataBossAndRandomLogic);
        pigBoss?.SetHealth(data.GetID());

        data.ApplyTo(obj);


        spawnNumber++;
    }
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
        if (instantiateOnly)
        {
            var o = Instantiate(prefab, data.startPos, Quaternion.identity);
            o.GetComponent<SpawnedObject>().ApplyFloatTwoData(data);
        }
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
