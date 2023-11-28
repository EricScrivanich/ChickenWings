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

    private float spawnInterval;

    // Variables used to create a space within a plane's spawn
    private bool createSpace = false;
    private float upperRegion;
    private float lowerRegion;

    private float cropSpawnRatio;
    private float jetSpawnRatio;

    private void Awake()
    {
        upperRegion = 3f;
        
        lowerRegion = -2f;
        createSpace = true;

        InitializePool(cropPlanePool, cropPlanePrefab, cropPoolSize);
        InitializePool(cargoPlanePool, cargoPlanePrefab, cargoPoolSize);
        InitializePool(jetPlanePool, jetPlanePrefab, jetPoolSize);
        
        SetSpawnRatio(.85f,.15f);


       
    }

    void Start()
    {
        StatsManager.OnScoreChanged += CheckScore;
        // StartCoroutine(SpawnRandomCoroutine());
    }
    private void OnDestroy() {
        StatsManager.OnScoreChanged -= CheckScore;
    }

    //  private IEnumerator SpawnCropCoroutine()
    // {
    //     yield return new WaitForSeconds(3f); // Initial delay

    //     while (true)
    //     {
    //         GetCrop();
    //         GetCrop();
    //         yield return new WaitForSeconds(1.5f); // Spawn interval
    //     }
    // }

    private IEnumerator SpawnRandomCoroutine()
{
     // Initial delay

    while (true)
    {
        float randomValue = UnityEngine.Random.value; // Generates a random number between 0.0 and 1.0

        // Decide which method to call based on randomValue
        if (randomValue < cropSpawnRatio) // 50% chance
        {
            GetCrop();
        }
        else if (randomValue < jetSpawnRatio) // 35% chance (50% to 85%)
        {
            GetJet();
        }
        else // 15% chance (85% to 100%)
        {
            GetCargo();
        }

        yield return new WaitForSeconds(spawnInterval); // Spawn interval
    }
}

void SetSpawnRatio(float cropRat, float jetRat)
{
    cropSpawnRatio = cropRat;
    jetSpawnRatio = cropRat + jetRat;
    
}
    void CheckScore(int newScore)
    {
        if (newScore == 1)
        {
            StartCoroutine(SpawnRandomCoroutine());
            spawnInterval = 7f;
        }
    
        else if (newScore > 3 && newScore < 6)
        {
            spawnInterval = 4f;
            SetSpawnRatio(.70f, .30f);
        }
        else if (newScore > 5 && newScore < 9)
        {
            spawnInterval = 2f;
            SetSpawnRatio(.60f, .30f);
        }
        else if (newScore > 8 && newScore < 14)
        {
            spawnInterval = 1f;
            SetSpawnRatio(.40f, .40f);
        }
        
        

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