using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class PooledQueue : ScriptableObject
{
    [SerializeField] private GameObject objectPrefab;
    private Queue<GameObject> pooledObject;
    [SerializeField] private int startingPoolSize;
    [SerializeField] private bool automaticallyEnqueue;

    public void CreatePool()
    {
        pooledObject = new Queue<GameObject>(startingPoolSize);
        for (int i = 0; i < startingPoolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            pooledObject.Enqueue(obj);
        }

    }

    public GameObject GetPooledObject()
    {
        // if (automaticallyEnqueue)
        // {
        //     GameObject obj = pooledObject.Dequeue();

        // }
        if (pooledObject.Count > 0)
        {
            GameObject obj = pooledObject.Dequeue();
            // obj.SetActive(true);
            return obj;

        }
        else
        {
            GameObject obj = Instantiate(objectPrefab);
            return obj;
        }
    }

    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        pooledObject.Enqueue(obj);
    }

}
