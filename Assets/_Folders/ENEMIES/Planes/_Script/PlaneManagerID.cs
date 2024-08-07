
using UnityEngine;

using System.Collections.Generic;
[CreateAssetMenu]
public class PlaneManagerID : ScriptableObject
{
    [SerializeField] private GameObject ExplosionPrefab;
    public float bomberTime;
    public float missileTime;

    public bool finsihedDrop;

    public int bombsDropped;


    public int numberOfSpawnsToDeactivate;

    public Vector3 droppedBombExplosionScale;
    [SerializeField] private GameObject bombPrefab;
    public bool spawnRandomPlanesBool;
    public List<PlaneData> PlaneDataType;
    public PlaneEvents events;
    private Queue<GameObject> explosionQueue;
    private Queue<GameObject> bombQueue;
    const int explosionQueueSize = 8;
    const int bomberPlaneBombQueueSize = 10;
    private Transform parent;

    public Material planeMaterial;
    public Material defualtMaterial;

    public void SpawnExplosionQueue()
    {
        explosionQueue = new Queue<GameObject>();
        if (!parent)
        {
            parent = new GameObject(name).transform;
        }
        if (ExplosionPrefab != null)
        {
            for (int i = 0; i < explosionQueueSize; i++)
            {
                GameObject prefab = Instantiate(ExplosionPrefab,parent); // Instantiate from the prefab

                prefab.SetActive(false); // Start with the GameObject disabled
                explosionQueue.Enqueue(prefab);
            }
        }

        SpawnBombQueue();

    }

    public void SpawnBombQueue()
    {
        bombQueue = new Queue<GameObject>();
        if (!parent)
        {
            parent = new GameObject(name).transform;
        }
        if (bombPrefab != null)
        {
            for (int i = 0; i < explosionQueueSize; i++)
            {
                GameObject prefab = Instantiate(bombPrefab,parent); // Instantiate from the prefab

                prefab.SetActive(false); // Start with the GameObject disabled
                bombQueue.Enqueue(prefab);
            }
        }

    }
    public void GetBomb(Vector2 position)
    {
        GameObject prefab = bombQueue.Dequeue();
        prefab.transform.position = position;
        prefab.SetActive(true);
        bombQueue.Enqueue(prefab);
        



    }

    public void GetExplosion(Vector2 position, Vector3 scale)
    {
        GameObject prefab = explosionQueue.Dequeue();
        prefab.transform.position = position;
        prefab.transform.localScale = scale;
        prefab.SetActive(true);
        explosionQueue.Enqueue(prefab);
        



    }

    // public void SpawnInArea(int minPlanes, int maxPlanes, float minX, float maxX, float minY, float maxY, float cropChance, float jetChance, float cargoChance)
    // {
    //     int planesToSpawn = Random.Range(minPlanes, maxPlanes + 1);
    //     Debug.Log("number to spawn: " + planesToSpawn);
    //     for (int i = 0; i < planesToSpawn; i++)
    //     {
    //         float randomX = Random.Range(minX, maxX);
    //         float randomY = Random.Range(minY, maxY);
    //         Vector2 spawnPosition = new Vector2(randomX, randomY);

    //         PlaneData planeType = DeterminePlaneType(cropChance, jetChance, cargoChance);

    //         // Log the choices for now
    //         planeType.GetPlane(0, spawnPosition);

    //     }

    // }

    public void SpawnInArea(int minPlanes, int maxPlanes, float minX, float maxX, float minY, float maxY, float cropChance, float jetChance, float cargoChance)
    {
        int planesToSpawn = Random.Range(minPlanes, maxPlanes + 1);
        Debug.Log("number to spawn: " + planesToSpawn);

        // Calculate the number of rows and columns
        int rows = planesToSpawn;
        int columns = planesToSpawn;

        // Calculate the width and height of each grid cell
        float cellWidth = (maxX - minX) / columns;
        float cellHeight = (maxY - minY) / rows;

        // Track which rows and columns have been used
        HashSet<int> usedRows = new HashSet<int>();
        HashSet<int> usedColumns = new HashSet<int>();

        for (int i = 0; i < planesToSpawn; i++)
        {
            int row, column;
            do
            {
                row = Random.Range(0, rows);
                column = Random.Range(0, columns);
            }
            while (usedRows.Contains(row) || usedColumns.Contains(column));

            // Mark this row and column as used
            usedRows.Add(row);
            usedColumns.Add(column);

            // Calculate the spawn position within the selected cell
            float randomX = minX + cellWidth * column + Random.Range(0, cellWidth);
            float randomY = minY + cellHeight * row + Random.Range(0, cellHeight);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            PlaneData planeType = DeterminePlaneType(cropChance, jetChance, cargoChance);

            // Spawn the plane at the determined position
            planeType.GetPlane(0,0,0, spawnPosition);
        }
    }

    PlaneData DeterminePlaneType(float cropChance, float jetChance, float cargoChance)
    {
        float totalChance = cropChance + jetChance + cargoChance;
        float randomPoint = Random.Range(0, totalChance);

        if (randomPoint <= cropChance)
        {
            return PlaneDataType[0];
        }
        else if (randomPoint <= cropChance + jetChance)
        {
            return PlaneDataType[1];
        }
        else
        {
            return PlaneDataType[2];
        }
    }
}
