using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject cropPlanePrefab;
    [SerializeField] private GameObject cargoPlanePrefab;
    [SerializeField] private GameObject jetPlanePrefab;

    public PlaneData CropData;
    public PlaneData CargoData;
    public PlaneData JetData;

    private List<GameObject> cropPlanePool = new List<GameObject>();
    private List<GameObject> cargoPlanePool = new List<GameObject>();
    private List<GameObject> jetPlanePool = new List<GameObject>();
    

    // Define initial pool sizes for each plane type
    private int cropPoolSize = 5;
    private int cargoPoolSize = 3;
    private int jetPoolSize = 5;

    // Variables used to create a space within a plane's spawn
    private bool createSpace = false;
    private float upperRegion;
    private float lowerRegion;

    private void Awake()
    {
        upperRegion = 3f;
        lowerRegion = -2f;
        createSpace = true;

        InitializePool(cropPlanePool, cropPlanePrefab, cropPoolSize);
        InitializePool(cargoPlanePool, cargoPlanePrefab, cargoPoolSize);
        InitializePool(jetPlanePool, jetPlanePrefab, jetPoolSize);

        StatsManager.OnScoreChanged += CheckScore;
    }

    void Start()
    {
        StartCoroutine(SpawnRandomCoroutine());
    }

     private IEnumerator SpawnCropCoroutine()
    {
        yield return new WaitForSeconds(3f); // Initial delay

        while (true)
        {
            GetCrop();
            GetCrop();
            yield return new WaitForSeconds(1.5f); // Spawn interval
        }
    }

    private IEnumerator SpawnRandomCoroutine()
{
    yield return new WaitForSeconds(3f); // Initial delay

    while (true)
    {
        float randomValue = UnityEngine.Random.value; // Generates a random number between 0.0 and 1.0

        // Decide which method to call based on randomValue
        if (randomValue < 0.50f) // 50% chance
        {
            GetCrop();
        }
        else if (randomValue < 0.85f) // 35% chance (50% to 85%)
        {
            GetJet();
        }
        else // 15% chance (85% to 100%)
        {
            GetCargo();
        }

        yield return new WaitForSeconds(1.5f); // Spawn interval
    }
}

    void CheckScore(int newScore)
    {
        

    }

    void Spawner()
    {

    }

    void InitializePool(List<GameObject> pool, GameObject prefab, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledPlane(List<GameObject> pool)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

   

    private void GetCrop()
    {
        GameObject crop = GetPooledPlane(cropPlanePool);
        if (crop == null)
        {
            Debug.LogError("No crop plane available in the pool.");
            return;
        }

        
        float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnCrop);
        crop.transform.position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
        crop.SetActive(true);
    }

   private void GetCargo()
{
    GameObject cargo = GetPooledPlane(cargoPlanePool);
    if (cargo == null)
    {
        Debug.LogError("No cargo plane available in the pool.");
        return;
    }

    float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnCargo);
    cargo.transform.position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
    cargo.SetActive(true);
}

    private void GetJet()
    {
        GameObject jet = GetPooledPlane(jetPlanePool);
        if (jet == null)
        {
            Debug.LogError("No jet plane available in the pool.");
            return;
        }


        
        float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnJet);
        jet.transform.position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
        jet.SetActive(true);
    }

//     private void SpawnCargoPlane()
// {
//     List<SpawnPoint> validSpawnPoints = new List<SpawnPoint>();

//     for (int i = 0; i < SpawnPointManager.SpawnPoints.Count; i++)
//     {
//         if (SpawnPointManager.SpawnPoints[i].canSpawn && SpawnPointManager.SpawnPoints[i].canSpawnCargo)
//         {
//             validSpawnPoints.Add(SpawnPointManager.SpawnPoints[i]);
//         }
//     }

//     if (validSpawnPoints.Count == 0)
//     {
//         Debug.LogError("No valid spawn points for cargo planes.");
//         return;
//     }

//     int randomIndex = Random.Range(0, validSpawnPoints.Count);
//     float spawnPositionY = validSpawnPoints[randomIndex].transform.position.y;
//     return spawnPositionY;

//     // Instantiate or activate your cargo plane at spawnPosition
// }


    // private float CalculateSpawnPosition(PlaneData planeData, float upperBound, float lowerBound, bool createSpace)
    // {
    //     if (!createSpace)
    //     {
    //         return Random.Range(planeData.minSpawn, planeData.maxSpawn);
    //     }
    //     else
    //     {
    //         float upperRegionSize = planeData.maxSpawn - upperBound;
    //         float lowerRegionSize = lowerBound - planeData.minSpawn;
    //         float totalSpawnableArea = upperRegionSize + lowerRegionSize;

    //         float upperRegionWeight = upperRegionSize / totalSpawnableArea;

    //         if (Random.value < upperRegionWeight)
    //         {
    //             return Random.Range(upperBound, planeData.maxSpawn);
    //         }
    //         else
    //         {
    //             return Random.Range(planeData.minSpawn, lowerBound);
    //         }
    //     }
    // }

}
