using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawnManager : MonoBehaviour
{
    [ExposedScriptableObject]
    public PlaneData CropData;
    public PlayerID player;

    [ExposedScriptableObject]
    public PlaneData CargoData;

    [ExposedScriptableObject]
    public PlaneData JetData;

    [SerializeField] private GameObject cropPlanePrefab;
    [SerializeField] private GameObject cargoPlanePrefab;
    [SerializeField] private GameObject jetPlanePrefab;
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

    private int cropPlanesAvailable = 1;
    private int jetPlanesAvailable = 1;
    private int cargoPlanesAvailable = 1;

    [SerializeField] private float minCropOffset = 1;
    [SerializeField] private float maxCropOffset = 3;
    [SerializeField] private float minCropIterationDelay = 4;
    [SerializeField] private float maxCropIterationDelay = 6;

    private float totalSpawnDelay = 0f;
    private float totalIterationSpawnDelay = 0f;

    [SerializeField] private float minJetOffset = 0;
    [SerializeField] private float maxJetOffset = 2;
    [SerializeField] private float minJetIterationDelay = .5f;
    [SerializeField] private float maxJetIterationDelay = 3;

    [SerializeField] private float minCargoOffset = 2;
    [SerializeField] private float maxCargoOffset = 4;
    [SerializeField] private float minCargoIterationDelay = 4;
    [SerializeField] private float maxCargoIterationDelay = 5.5f;

    [SerializeField] private PlaneAreaSpawn spawnArea;

    private void Awake()
    {
        upperRegion = 3f;
        player.globalEvents.OnUpdateScore += CheckScore;

        lowerRegion = -2f;
        createSpace = true;

        // InitializePool(cropPlanePool, cropPlanePrefab, cropPoolSize);
        // InitializePool(cargoPlanePool, cargoPlanePrefab, cargoPoolSize);
        // InitializePool(jetPlanePool, jetPlanePrefab, jetPoolSize);

        // SetSpawnRatio(.85f, .15f);



    }

    void Start()
    {
        CropData.SpawnPlanePool();
        JetData.SpawnPlanePool();
        CargoData.SpawnPlanePool();

        Invoke("SpawnPlaneInArea", 1f);




        // StartCoroutine(SpawnRandomCoroutine());
    }



    void SpawnPlaneInArea()
    {
        if (!spawnArea.gameObject.activeInHierarchy)
        {
            return;
        }
        int planesToSpawn = Random.Range(spawnArea.minPlanes, spawnArea.maxPlanes + 1); // +1 because Random.Range is exclusive for the max value

        for (int i = 0; i < planesToSpawn; i++)
        {
            // Generate a random position within the spawn area
            float randomX = Random.Range(spawnArea.GetMinX(), spawnArea.GetMaxX());
            float randomY = Random.Range(spawnArea.GetMinY(), spawnArea.GetMaxY());
            Vector2 spawnPosition = new Vector2(randomX, randomY); // Assuming a 2D game

            // Determine the type of plane to spawn based on chances
            PlaneData planeType = DeterminePlaneType(spawnArea);

            // Log the choices for now
            planeType.GetPlane(0, spawnPosition);
        }
    }

    PlaneData DeterminePlaneType(PlaneAreaSpawn area)
    {
        float totalChance = area.cropChance + area.jetChance + area.cargoChance;
        float randomPoint = Random.Range(0, totalChance);

        if (randomPoint <= area.cropChance)
        {
            return CropData;
        }
        else if (randomPoint <= area.cropChance + area.jetChance)
        {
            return JetData;
        }
        else
        {
            return CargoData;
        }
    }





    /// Add another timer if plane count is above a certain number to avoid long lines of planes

    IEnumerator SpawnCropPlanes()
    {
        while (true) // Main game loop
        {
            for (int i = 0; i < cropPlanesAvailable; i++)
            {
                float cropSpawnOffsetVar = Random.Range(minCropOffset, maxCropOffset);
                yield return new WaitForSeconds(cropSpawnOffsetVar + totalSpawnDelay);
                GetCrop(); // Spawn a plane
                // Wait for offset
            }
            float cropIterationEndDelay = Random.Range(minCropIterationDelay, maxCropIterationDelay);
            yield return new WaitForSeconds(cropIterationEndDelay + totalIterationSpawnDelay); // End-of-iteration delay
                                                                                               // Optional: Adjust cropPlanesAvailable and delays based on game conditions
        }
    }
    IEnumerator SpawnJetPlanes()
    {
        while (true) // Main game loop
        {
            for (int i = 0; i < jetPlanesAvailable; i++)
            {
                float jetSpawnOffsetVar = Random.Range(minJetOffset, maxJetOffset);
                yield return new WaitForSeconds(jetSpawnOffsetVar + totalSpawnDelay);
                GetJet(); // Spawn a plane
                // Wait for offset
            }
            float jetIterationEndDelay = Random.Range(minJetIterationDelay, maxJetIterationDelay);
            yield return new WaitForSeconds(jetIterationEndDelay + totalIterationSpawnDelay); // End-of-iteration delay
                                                                                              // Optional: Adjust cropPlanesAvailable and delays based on game conditions
        }
    }
    IEnumerator SpawnCargoPlanes()
    {
        while (true) // Main game loop
        {
            for (int i = 0; i < cargoPlanesAvailable; i++)
            {
                float cargoSpawnOffsetVar = Random.Range(minCargoOffset, maxCargoOffset);
                yield return new WaitForSeconds(cargoSpawnOffsetVar + totalSpawnDelay);
                GetCargo(); // Spawn a plane
                // Wait for offset
            }
            float cargoIterationEndDelay = Random.Range(minCargoIterationDelay, maxCargoIterationDelay);
            yield return new WaitForSeconds(cargoIterationEndDelay + totalIterationSpawnDelay); // End-of-iteration delay
                                                                                                // Optional: Adjust cropPlanesAvailable and delays based on game conditions
        }
    }

    void CheckScore(int newScore)
    {
        switch (newScore)
        {
            case 1:
                StartCoroutine(SpawnCropPlanes());
                break;
            case 3:
                cropPlanesAvailable = 2;
                break;
            case 4:
                StartCoroutine(SpawnJetPlanes());
                
                break;

            case 6:
                jetPlanesAvailable = 2;
                break;
            case 7:
                cropPlanesAvailable = 3;
                break;
            case 8:
                StartCoroutine(SpawnCargoPlanes());

                break;
            case 10:
               
                break;
            case 12:
                cargoPlanesAvailable = 2;
                cropPlanesAvailable = 4;

                break;
            case 14:
                cargoPlanesAvailable = 3;
                break;
            case 16:
                
                jetPlanesAvailable = 4;

                break;
            case 19:
                cargoPlanesAvailable = 3;

                break;
            default:
                break;
        }
    }

    // void CheckScore(int newScore)
    // {
    //     if (newScore == 1)
    //     {
    //         StartCoroutine(SpawnCropPlanes());

    //     }

    //     else if (newScore > 2 && newScore < 4)
    //     {
    //         cropPlanesAvailable = 3;
    //     }
    //     else if (newScore > 3 && newScore < 5)
    //     {
    //         cropPlanesAvailable = 5;
    //     }
    // }

    #region Pools
    // void InitializePool(List<GameObject> pool, GameObject prefab, int size)
    // {
    //     for (int i = 0; i < size; i++)
    //     {
    //         GameObject obj = Instantiate(prefab);
    //         obj.SetActive(false);
    //         pool.Add(obj);
    //     }
    // }

    // public GameObject GetPooledPlane(List<GameObject> pool)
    // {
    //     foreach (var obj in pool)
    //     {
    //         if (!obj.activeInHierarchy)
    //         {
    //             return obj;
    //         }
    //     }
    //     return null;
    // }
    private void GetCrop()
    {
        // GameObject crop = GetPooledPlane(cropPlanePool);
        // if (crop == null)
        // {
        //     Debug.LogError("No crop plane available in the pool.");
        //     return;
        // }

        float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnCrop);
        Vector2 position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
        CropData.GetPlane(0, position);
    }
    private void GetCargo()
    {
        // GameObject cargo = GetPooledPlane(cargoPlanePool);
        // if (cargo == null)
        // {
        //     Debug.LogError("No cargo plane available in the pool.");
        //     return;
        // }

        float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnCargo);
        Vector2 position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
        CargoData.GetPlane(0, position);
    }

    private void GetJet()
    {
        // GameObject jet = GetPooledPlane(jetPlanePool);
        // if (jet == null)
        // {
        //     Debug.LogError("No jet plane available in the pool.");
        //     return;
        // }
        float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnJet);
        Vector2 position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
        JetData.GetPlane(0, position);
    }
    #endregion


    private void OnDisable()
    {
        player.globalEvents.OnUpdateScore -= CheckScore;

    }
}

#region comments
// void CheckScore(int newScore)  Ratios
// {
//     if (newScore == 1)
//     {
//         // StartCoroutine(SpawnRandomCoroutine());
//         spawnInterval = 7f;
//     }
//     else if (newScore > 3 && newScore < 6)
//     {
//         spawnInterval = 4f;
//         SetSpawnRatio(.70f, .30f);
//     }
//     else if (newScore > 5 && newScore < 9)
//     {
//         spawnInterval = 2f;
//         SetSpawnRatio(.60f, .30f);
//     }
//     else if (newScore > 8 && newScore < 14)
//     {
//         spawnInterval = 1f;
//         SetSpawnRatio(.40f, .40f);
//     }

// }


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

//     private IEnumerator SpawnRandomCoroutine()
// {
//      // Initial delay

//     while (true)
//     {
//         float randomValue = UnityEngine.Random.value; // Generates a random number between 0.0 and 1.0

//         // Decide which method to call based on randomValue
//         if (randomValue < cropSpawnRatio) // 50% chance
//         {
//             GetCrop();
//         }
//         else if (randomValue < jetSpawnRatio) // 35% chance (50% to 85%)
//         {
//             GetJet();
//         }
//         else // 15% chance (85% to 100%)
//         {
//             GetCargo();
//         }

//         yield return new WaitForSeconds(spawnInterval); // Spawn interval
//     }
// }

// void SetSpawnRatio(float cropRat, float jetRat)
// {
//     cropSpawnRatio = cropRat;
//     jetSpawnRatio = cropRat + jetRat;

// }

#endregion


