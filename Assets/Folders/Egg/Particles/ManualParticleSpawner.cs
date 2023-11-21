using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualParticleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private int poolSizePerPrefab = 12;
    [SerializeField] private int minParticles = 1;
    [SerializeField] private int maxParticles = 5;
    [SerializeField] private Vector2 minSpawnForce = new Vector2(-5f, -2f);
    [SerializeField] private Vector2 maxSpawnForce = new Vector2(-2f, 2f);

    private bool spawnTriggered = false;

    private Dictionary<int, List<GameObject>> particlePools = new Dictionary<int, List<GameObject>>();

    private void Start()
    {
        InitializePool();

    }

    private void InitializePool()
    {
        for (int i = 0; i < particlePrefabs.Length; i++)
        {
            List<GameObject> pool = new List<GameObject>();
            for (int j = 0; j < poolSizePerPrefab; j++)
            {
                GameObject instance = Instantiate(particlePrefabs[i], transform);
                instance.SetActive(false);
                pool.Add(instance);
            }
            particlePools[i] = pool;
        }
    }

    public void TriggerParticles()
{
    int particleCount = Random.Range(minParticles, maxParticles + 1);
    for (int i = 0; i < particleCount; i++)
    {
        int prefabIndex = Random.Range(0, particlePrefabs.Length - 1);
        GameObject particleInstance = GetFromPool(prefabIndex);
        Vector2 spawnForce = new Vector2(Random.Range(minSpawnForce.x, maxSpawnForce.x), Random.Range(minSpawnForce.y, maxSpawnForce.y));
        Rigidbody2D rb = particleInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(spawnForce, ForceMode2D.Impulse);
        }
    }
} 

    void Update()
    {
       
        if (spawnTriggered)
        {
            TriggerParticles();
            spawnTriggered = false;
        }
    }

    private GameObject GetFromPool(int prefabIndex)
    {
        List<GameObject> pool = particlePools[prefabIndex];
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = transform.position;
                return obj;
            }
        }

        // If no inactive object is found, instantiate a new one and add it to the pool
        GameObject instance = Instantiate(particlePrefabs[prefabIndex], transform);
        pool.Add(instance);
        return instance;
    }

    public void ReturnToPool(GameObject instance)
    {
        instance.SetActive(false);
    }

    public void SetSpawnPosition(Vector3 position)
{

    transform.position = position;
    spawnTriggered = true;
}


}
