using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] ringPrefabs;
    [SerializeField] private int poolSize = 5;
    private List<GameObject> ringPool;
    [SerializeField] private float spawnInterval = 2f;
    private float timer = 0f;

    void Start()
    {
        // Initialize the object pool
        ringPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            for (int j = 0; j < ringPrefabs.Length; j++)
            {
                GameObject obj = Instantiate(ringPrefabs[j]);
                obj.SetActive(false);
                ringPool.Add(obj);
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Timer-based activation
        if (timer >= spawnInterval)
        {
            print("yerrr");
            GameObject ring = GetRandomRing();
            if (ring != null)
            {
                // Position the ring
                ring.transform.position = new Vector3(BoundariesManager.rightBoundary, 0, 0);
            }
            timer = 0;
        }

        // Out-of-bounds deactivation
        foreach (GameObject ring in ringPool)
        {
            if (ring.activeInHierarchy && ring.transform.position.x < -20)
            {
               
                ReturnRing(ring);
            }
        }
    }

    public GameObject GetRingByIndex(int index)
    {
        if (index >= 0 && index < ringPool.Count)
        {
            GameObject obj = ringPool[index];
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null; // Either out-of-bounds index or the object at index is already active
    }

    public GameObject GetRandomRing()
    {
        List<GameObject> inactiveRings = ringPool.FindAll(r => !r.activeInHierarchy);

        if (inactiveRings.Count > 0)
        {
            GameObject obj = inactiveRings[Random.Range(0, inactiveRings.Count)];
            obj.SetActive(true);
            return obj;
        }
        return null; // No inactive objects to activate
    }

    public void ReturnRing(GameObject obj)
    {
        obj.SetActive(false);
    }
}
