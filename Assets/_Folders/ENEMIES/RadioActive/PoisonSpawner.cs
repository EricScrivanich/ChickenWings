using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpawner : MonoBehaviour
{
    private Transform spawnPoint;
    [SerializeField] private GameObject poisonPrefab;
    [SerializeField] private float spawnInterval;

    private GameObject spawnedObject;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.Find("PoisonSpawn").GetComponent<Transform>();

        // Spawn a poisonPrefab at the spawnPoint
        StartCoroutine(SpawnCoroutine());
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator SpawnCoroutine()
    
    {
        while (true)
        {
            spawnedObject = Instantiate(poisonPrefab, transform);
            
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

}
