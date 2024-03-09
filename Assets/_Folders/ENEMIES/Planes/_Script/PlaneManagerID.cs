
using UnityEngine;

using System.Collections.Generic;
[CreateAssetMenu]
public class PlaneManagerID : ScriptableObject 
{
    [SerializeField] private GameObject ExplosionPrefab;
    public List<PlaneData> PlaneDataType;
    private Queue<GameObject> explosionQueue;
    const int explosionQueueSize = 8;

    public void SpawnExplosionQueue()
    {
        explosionQueue = new Queue<GameObject>();
        if (ExplosionPrefab != null)
        {
            for (int i = 0; i < explosionQueueSize; i++)
        {
            GameObject prefab = Instantiate(ExplosionPrefab); // Instantiate from the prefab

            prefab.SetActive(false); // Start with the GameObject disabled
            explosionQueue.Enqueue(prefab);
        }
        }
        
    }

    public GameObject GetExplosion()
    {
        GameObject prefab = explosionQueue.Dequeue();
        explosionQueue.Enqueue(prefab);
        return prefab;



    }
}
