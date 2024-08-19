using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomEnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector2 position1 = new Vector2(13, 5);
    [SerializeField] private Vector2 position2 = new Vector2(13, 1);
    [SerializeField] private Vector2 position3 = new Vector2(15, -1);
    [SerializeField] private Vector2 position4 = new Vector2(19, 0);

    private EnemyPoolManager enemyPoolManager;
    [SerializeField] private LevelManagerID lvlID;
    public RandomSpawnIntensity[] intensityDataSet;

    [SerializeField] private float initialDelayToSpawn;

    private RandomSpawnIntensity currentIntensityData;

    private bool initalIntensityHasBeenSet = false;

    [Header("Base Bouding Boxes")]
    [SerializeField] private EnemyBoundingBox normalPigBoundingBoxBase;
    [SerializeField] private EnemyBoundingBox jetPackPigBoundingBoxBase;
    [SerializeField] private EnemyBoundingBox bigPigBoundingBoxBase;
    [SerializeField] private EnemyBoundingBox tenderizerPigBoundingBoxBase;
    private EnemyBoundingBox normalPigBoundingBox;
    private EnemyBoundingBox jetPackPigBoundingBox;
    private EnemyBoundingBox bigPigBoundingBox;
    private EnemyBoundingBox tenderizerPigBoundingBox;

    [Header("Target Settings")]

    [SerializeField] private float xTargetToStartSpawn;
    [SerializeField] private Vector2 spawnIntervalRangeAfterTarget;
    private List<Vector2> recentlySpanwnedPositions; // 2D array to store times [index, 0: time, 1: spawn time]
    private int currentGroupSize = 0; // Number of enemies in the current group
    private float timeSinceSpawn; // Time when the current group was spawned


    [Header("Spawn Settings")]
    // Time between waves
    private float spawnInterval;

    // Adjustment to bounding box active time
    [SerializeField] private Vector2Int waveSizeRange = new Vector2Int(1, 5); // Range for number of pigs in a wave

    [Header("Spawn Weights")]
    [SerializeField] private int normalPigWeight = 1;
    [SerializeField] private int jetPackPigWeight = 1;
    [SerializeField] private int bigPigWeight = 1;
    [SerializeField] private int tenderizerPigWeight = 1;



    [Header("Spawn Settings for Testing")]
    [SerializeField] private bool useSceneGizmos;
    [SerializeField] private float normalPigScaleMultiplier = 1f;
    [SerializeField] private float normalPigSpeed = 6.4f;

    [SerializeField] private float jetPackPigScaleMultiplier = 1f;
    [SerializeField] private float jetPackPigSpeed = 9.5f;

    [SerializeField] private float bigPigScaleMultiplier = 1f;
    [SerializeField] private float bigPigSpeed = 5f;

    [SerializeField] private float tenderizerPigScaleMultiplier = 1f;
    [SerializeField] private float tenderizerPigSpeed = 4.5f;


    private bool isSpawning = false;
    private bool PauseRandomSpawning = false;

    private float timeSinceLastWave;

    private float bigPigYRangeCenter;
    private float bigPigYRangeHalf;

    private class ActiveBoundingBox
    {
        public Rect box;
        public float expirationTime;
    }
    private List<ActiveBoundingBox> activeBoundingBoxes = new List<ActiveBoundingBox>(); // Store active bounding boxes with expiration times
    private Queue<Rect> customBoundingBoxes = new Queue<Rect>();


    // private void OnDrawGizmos()
    // {
    //     // Ensure that bounding boxes are assigned
    //     if (normalPigBoundingBoxBase == null || jetPackPigBoundingBoxBase == null ||
    //         bigPigBoundingBoxBase == null || tenderizerPigBoundingBoxBase == null)
    //     {
    //         return; // Exit if any bounding box is not assigned
    //     }

    //     // Draw the bounding boxes at the specified positions with the current properties
    //     DrawBoundingBox(normalPigBoundingBoxBase, position1, normalPigScaleMultiplier, normalPigSpeed);
    //     DrawBoundingBox(jetPackPigBoundingBoxBase, position2, jetPackPigScaleMultiplier, jetPackPigSpeed);
    //     DrawBoundingBox(bigPigBoundingBoxBase, position3, bigPigScaleMultiplier, bigPigSpeed);
    //     DrawBoundingBox(tenderizerPigBoundingBoxBase, position4, tenderizerPigScaleMultiplier, tenderizerPigSpeed);
    // }

    private void DrawBoundingBox(EnemyBoundingBox boundingBox, Vector3 position, float scaleMultiplier, float speed)
    {
        // Calculate the actual scale based on the multiplier
        Vector3 actualScale = boundingBox.baseScale * scaleMultiplier;

        // Get the bounding box rect with the given scale and speed
        Rect boundingBoxRect = boundingBox.GetBoundingBox(actualScale, speed, currentIntensityData.AllowedOverlap);

        // Draw the bounding box as a wireframe
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(position.x + boundingBoxRect.center.x, position.y + boundingBoxRect.center.y, position.z),
                            new Vector3(boundingBoxRect.width, boundingBoxRect.height, 1));
    }

    private void OnValidate()
    {
        // Ensure default values if not set
        if (normalPigSpeed == 0) normalPigSpeed = normalPigBoundingBoxBase.baseSpeed;
        if (jetPackPigSpeed == 0) jetPackPigSpeed = jetPackPigBoundingBoxBase.baseSpeed;
        if (bigPigSpeed == 0) bigPigSpeed = bigPigBoundingBoxBase.baseSpeed;
        if (tenderizerPigSpeed == 0) tenderizerPigSpeed = tenderizerPigBoundingBoxBase.baseSpeed;
    }


    private void Start()
    {

        PauseRandomSpawning = true;
        Invoke("StartSpawning", initialDelayToSpawn);
        if (!initalIntensityHasBeenSet)
            UpdateIntensity(lvlID.LevelIntensity);
        enemyPoolManager = GetComponent<EnemyPoolManager>();
        timeSinceLastWave = 0f;
    }

    private void StartSpawning()
    {
        PauseRandomSpawning = false;
    }

    private void Update()
    {




        if (!PauseRandomSpawning)
        {
            if (!isSpawning)
                timeSinceLastWave += Time.deltaTime;

            if (timeSinceLastWave >= spawnInterval && !isSpawning)
            {
                isSpawning = true;
                int waveSize = currentIntensityData.GetRandomWaveSize();
                recentlySpanwnedPositions = new List<Vector2>();

                StartCoroutine(SpawnWaveWithDelay(waveSize));

            }

            // Clean up the active bounding boxes list by removing boxes that have expired
        }
    }
    private List<int> DeterminePigTypesForWave(int waveSize)
    {
        List<int> pigTypeList = new List<int>();

        float totalWeight = currentIntensityData.NormalPig_W + currentIntensityData.JetpackPig_W +
                            currentIntensityData.BigPig_W + currentIntensityData.TenderizerPig_W;

        int tenderizerCount = Mathf.RoundToInt((currentIntensityData.TenderizerPig_W / totalWeight) * waveSize);
        int bigPigCount = Mathf.RoundToInt((currentIntensityData.BigPig_W / totalWeight) * waveSize);
        int normalPigCount = Mathf.RoundToInt((currentIntensityData.NormalPig_W / totalWeight) * waveSize);
        int jetPackPigCount = Mathf.RoundToInt((currentIntensityData.JetpackPig_W / totalWeight) * waveSize);

        // Ensure the total count matches the wave size
        int totalCount = tenderizerCount + bigPigCount + normalPigCount + jetPackPigCount;
        while (totalCount < waveSize)
        {
            jetPackPigCount++;  // Add the remaining pigs to the least weighted category
            totalCount++;
        }
        while (totalCount > waveSize)
        {
            if (jetPackPigCount > 0) jetPackPigCount--;
            else if (normalPigCount > 0) normalPigCount--;
            else if (bigPigCount > 0) bigPigCount--;
            else tenderizerCount--;

            totalCount--;
        }

        // Add pigs to the list in order: Tenderizer, Big, Normal, JetPack
        pigTypeList.AddRange(Enumerable.Repeat(3, tenderizerCount)); // Tenderizer Pigs
        pigTypeList.AddRange(Enumerable.Repeat(2, bigPigCount));     // Big Pigs
        pigTypeList.AddRange(Enumerable.Repeat(0, normalPigCount));  // Normal Pigs
        pigTypeList.AddRange(Enumerable.Repeat(1, jetPackPigCount)); // JetPack Pigs

        return pigTypeList;
    }

    private IEnumerator SpawnWaveWithDelay(int waveSize)
    {
        List<int> pigTypeList = DeterminePigTypesForWave(waveSize);

        // Spawn pigs according to the sorted order
        for (int i = 0; i < pigTypeList.Count; i++)
        {
            if (!PauseRandomSpawning)
            {
                activeBoundingBoxes.RemoveAll(box => Time.time > box.expirationTime);
                SpawnPig(pigTypeList[i]);
                timeSinceLastWave = 0f;
                yield return new WaitForSeconds(0.22f);
            }
            else
            {
                isSpawning = false;
                yield break;
            }
        }

        spawnInterval = TimeForWaveToReachTarget(currentIntensityData.XTargetToStartSpawn) + Random.Range(spawnIntervalRangeAfterTarget.x, spawnIntervalRangeAfterTarget.y);
        isSpawning = false;
    }
    private int GetRandomPigType()
    {
        // Sum the weights directly from the intensityData array
        float totalWeight = currentIntensityData.NormalPig_W
                            + currentIntensityData.JetpackPig_W
                            + currentIntensityData.BigPig_W
                            + currentIntensityData.TenderizerPig_W;

        // Generate a random number between 0 and the total weight
        float randomValue = Random.Range(0, totalWeight);

        // Determine which pig type corresponds to the random value
        if (randomValue < currentIntensityData.NormalPig_W)
        {
            return 0; // Normal Pig
        }
        else if (randomValue < currentIntensityData.NormalPig_W + currentIntensityData.JetpackPig_W)
        {
            return 1; // JetPack Pig
        }
        else if (randomValue < currentIntensityData.NormalPig_W + currentIntensityData.JetpackPig_W + currentIntensityData.BigPig_W)
        {
            return 2; // Big Pig
        }
        else
        {
            return 3; // Tenderizer Pig
        }
    }

    private void SpawnPig(int pigType)
    {
        // Generate random scale and speed based on the enemy type
        Vector3 scale;
        float speed;
        Rect boundingBox;
        float activeDuration;
        Vector2 ySpawnRange;

        switch (pigType)
        {
            case 0:
                scale = normalPigBoundingBox.baseScale * Random.Range(normalPigBoundingBox.scaleMinMax.x, normalPigBoundingBox.scaleMinMax.y);
                speed = Random.Range(normalPigBoundingBox.speedMinMax.x, normalPigBoundingBox.speedMinMax.y);
                ySpawnRange = normalPigBoundingBox.ySpawnRange;
                boundingBox = normalPigBoundingBox.GetBoundingBox(scale, speed, currentIntensityData.AllowedOverlap);
                activeDuration = Mathf.Max(0, normalPigBoundingBox.activeDuration + currentIntensityData.ActiveBoundingBoxTimeAdjustment);
                break;
            case 1:
                scale = jetPackPigBoundingBox.baseScale * Random.Range(jetPackPigBoundingBox.scaleMinMax.x, jetPackPigBoundingBox.scaleMinMax.y);
                speed = Random.Range(jetPackPigBoundingBox.speedMinMax.x, jetPackPigBoundingBox.speedMinMax.y);
                ySpawnRange = jetPackPigBoundingBox.ySpawnRange;
                boundingBox = jetPackPigBoundingBox.GetBoundingBox(scale, speed, currentIntensityData.AllowedOverlap);
                activeDuration = Mathf.Max(0, jetPackPigBoundingBox.activeDuration + currentIntensityData.ActiveBoundingBoxTimeAdjustment);
                break;
            case 2: // Big Pig
                scale = bigPigBoundingBox.baseScale * Random.Range(bigPigBoundingBox.scaleMinMax.x, bigPigBoundingBox.scaleMinMax.y);
                speed = Random.Range(bigPigBoundingBox.speedMinMax.x, bigPigBoundingBox.speedMinMax.y);
                ySpawnRange = bigPigBoundingBox.ySpawnRange;
                boundingBox = bigPigBoundingBox.GetBoundingBox(scale, speed, currentIntensityData.AllowedOverlap);
                activeDuration = Mathf.Max(0, bigPigBoundingBox.activeDuration + currentIntensityData.ActiveBoundingBoxTimeAdjustment);
                break;
            case 3:
                scale = tenderizerPigBoundingBox.baseScale * Random.Range(tenderizerPigBoundingBox.scaleMinMax.x, tenderizerPigBoundingBox.scaleMinMax.y);
                speed = Random.Range(tenderizerPigBoundingBox.speedMinMax.x, tenderizerPigBoundingBox.speedMinMax.y);
                ySpawnRange = tenderizerPigBoundingBox.ySpawnRange;
                boundingBox = tenderizerPigBoundingBox.GetBoundingBox(scale, speed, currentIntensityData.AllowedOverlap);
                activeDuration = Mathf.Max(0, tenderizerPigBoundingBox.activeDuration + currentIntensityData.ActiveBoundingBoxTimeAdjustment);
                break;
            default:
                return; // This should never happen
        }

        Vector2 spawnPos;
        int attempts = 0;
        bool validSpawn = false;

        // Try to find a valid spawn position within the original range
        while (attempts < 8 && !validSpawn)
        {
            attempts++;
            spawnPos = new Vector2(
                Random.Range(currentIntensityData.XSpawnRange.x, currentIntensityData.XSpawnRange.y),
                Random.Range(ySpawnRange.x, ySpawnRange.y)
            );

            Rect adjustedBoundingBox = new Rect(
                spawnPos.x + boundingBox.x,
                spawnPos.y + boundingBox.y,
                boundingBox.width,
                boundingBox.height
            );

            validSpawn = !CheckOverlap(adjustedBoundingBox);

            if (validSpawn)
            {
                recentlySpanwnedPositions.Add(new Vector2(speed, spawnPos.x));

                if (pigType == 2) // If Big Pig, determine yForce based on spawn position
                {
                    float yDistanceFromCenter = Mathf.Abs(spawnPos.y - bigPigYRangeCenter);
                    float yForceRangeFactor = 1f - (yDistanceFromCenter / bigPigYRangeHalf);
                    float yForce = Mathf.Lerp(bigPigBoundingBox.yForceRange.x, bigPigBoundingBox.yForceRange.y, yForceRangeFactor);
                    float distanceToFlap = bigPigBoundingBox.CalculateDistanceToFlap(yForce);
                    SpawnSelectedPig(pigType, spawnPos, scale, speed, yForce, distanceToFlap);
                }
                else
                {
                    SpawnSelectedPig(pigType, spawnPos, scale, speed);
                }

                activeBoundingBoxes.Add(new ActiveBoundingBox { box = adjustedBoundingBox, expirationTime = Time.time + activeDuration });
                return;
            }
        }

        // If no valid position found, extend the range and try again
        float extendedXMin = currentIntensityData.XSpawnRange.y + 1.2f;
        float extendedXMax = currentIntensityData.XSpawnRange.y + 7.5f;

        for (int i = 0; i < 4; i++)
        {
            spawnPos = new Vector2(
                Random.Range(extendedXMin, extendedXMax),
                Random.Range(ySpawnRange.x, ySpawnRange.y)
            );

            Rect adjustedBoundingBox = new Rect(
                spawnPos.x + boundingBox.x,
                spawnPos.y + boundingBox.y,
                boundingBox.width,
                boundingBox.height
            );

            validSpawn = !CheckOverlap(adjustedBoundingBox);

            if (validSpawn)
            {
                recentlySpanwnedPositions.Add(new Vector2(speed, spawnPos.x));

                if (pigType == 2) // If Big Pig, determine yForce based on spawn position
                {
                    float yDistanceFromCenter = Mathf.Abs(spawnPos.y - bigPigYRangeCenter);
                    float yForceRangeFactor = 1f - (yDistanceFromCenter / bigPigYRangeHalf);
                    float yForce = Mathf.Lerp(bigPigBoundingBox.yForceRange.x, bigPigBoundingBox.yForceRange.y, yForceRangeFactor);
                    float distanceToFlap = bigPigBoundingBox.CalculateDistanceToFlap(yForce);
                    SpawnSelectedPig(pigType, spawnPos, scale, speed, yForce, distanceToFlap);
                }
                else
                {
                    SpawnSelectedPig(pigType, spawnPos, scale, speed);
                }

                activeBoundingBoxes.Add(new ActiveBoundingBox { box = adjustedBoundingBox, expirationTime = Time.time + activeDuration });
                Debug.Log($"Extended spawn area used for {pigType} at position {spawnPos}");
                return;
            }
        }

        // If all attempts fail, log a warning
        Debug.LogWarning("Failed to find a valid spawn position after maximum attempts.");
    }
    private void OnDrawGizmos()
    {
        if (useSceneGizmos)
        {
            if (normalPigBoundingBoxBase == null || jetPackPigBoundingBoxBase == null || bigPigBoundingBoxBase == null || tenderizerPigBoundingBoxBase == null)
            {
                return; // Exit if any bounding box is not assigned
            }

            // Draw the bounding boxes at the specified positions with the current properties
            DrawBoundingBox(normalPigBoundingBoxBase, position1, normalPigScaleMultiplier, normalPigSpeed);
            DrawBoundingBox(jetPackPigBoundingBoxBase, position2, jetPackPigScaleMultiplier, jetPackPigSpeed);
            DrawBoundingBox(bigPigBoundingBoxBase, position3, bigPigScaleMultiplier, bigPigSpeed);
            DrawBoundingBox(tenderizerPigBoundingBoxBase, position4, tenderizerPigScaleMultiplier, tenderizerPigSpeed);

        }

        else
        {
            // Set the color for active bounding boxes
            Gizmos.color = Color.red;

            // Draw all active bounding boxes
            foreach (var activeBox in activeBoundingBoxes)
            {
                Gizmos.DrawWireCube(new Vector3(activeBox.box.center.x, activeBox.box.center.y, 0), new Vector3(activeBox.box.width, activeBox.box.height, 1));
            }

            // Optional: Set a different color for custom bounding boxes if you are using them
            Gizmos.color = Color.green;

            foreach (var customBox in customBoundingBoxes)
            {
                Gizmos.DrawWireCube(new Vector3(customBox.center.x, customBox.center.y, 0), new Vector3(customBox.width, customBox.height, 1));
            }
        }
    }
    private void SpawnSelectedPig(int pigType, Vector2 spawnPos, Vector3 scale, float speed, float yForce = 0f, float distanceToFlap = 0f)
    {
        switch (pigType)
        {
            case 0:
                enemyPoolManager.GetNormalPig(spawnPos, scale, speed);
                break;
            case 1:
                enemyPoolManager.GetJetPackPig(spawnPos, scale, speed);
                break;
            case 2:
                enemyPoolManager.GetBigPig(spawnPos, scale, speed, yForce, distanceToFlap);
                break;
            case 3:

                enemyPoolManager.GetTenderizerPig(spawnPos, scale, speed, true);
                break;
        }
    }



    public float TimeForWaveToReachTarget(float xCordTarget)
    {
        float returnedTime = 0;

        if (recentlySpanwnedPositions.Count == 0)
            return 2f;
        foreach (var vect in recentlySpanwnedPositions)
        {
            float adjustedSpeed = Mathf.Abs(vect.x);

            // Calculate time = distance / speed
            float distance = Mathf.Abs(vect.y - xCordTarget);
            float calculatedTime = distance / adjustedSpeed;

            if (calculatedTime > returnedTime) returnedTime = calculatedTime;

        }

        return returnedTime - timeSinceLastWave;
    }

    public void ResumeRandomSpawning()
    {
        PauseRandomSpawning = false;
        timeSinceLastWave = 0;


    }



    public void StopSpawing()
    {
        PauseRandomSpawning = true;
        activeBoundingBoxes.Clear();

    }

    public void StopSpawningForTime(float time)
    {
        StopSpawing();
        StartCoroutine(StopSpawningForTimeCourintine(time));

    }

    private IEnumerator StopSpawningForTimeCourintine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResumeRandomSpawning();

    }

    public float GetTimeToReachTargetAndPause(float xCordTarget)
    {
        float returnedTime = 0;
        float timeSinceLastWaveVar = timeSinceLastWave;
        StopSpawing();

        foreach (var vect in recentlySpanwnedPositions)
        {
            float adjustedSpeed = Mathf.Abs(vect.x);

            // Calculate time = distance / speed
            float distance = Mathf.Abs(vect.y - xCordTarget);
            float calculatedTime = distance / adjustedSpeed;

            if (calculatedTime > returnedTime) returnedTime = calculatedTime;

        }

        return returnedTime - timeSinceLastWaveVar;
    }

    public void CreateCustomBoundingBox(Vector2 yRange, Vector2 xRange)
    {
        // If allX is true, the bounding box will take up the entire xRange
        if (xRange == Vector2.zero)
        {
            xRange = new Vector2(currentIntensityData.XSpawnRange.x, currentIntensityData.XSpawnRange.y + 8f);
        }

        // Create the bounding box using the given dimensions
        Rect customBox = new Rect(xRange.x, yRange.x, xRange.y - xRange.x, yRange.y - yRange.x);

        // Check for overlap with existing custom bounding boxes
        // foreach (var existingBox in customBoundingBoxes)
        // {
        //     if (CalculateOverlap(customBox, existingBox) > currentIntensityData)
        //     {
        //         Debug.LogWarning("Custom bounding box overlaps with an existing custom bounding box.");
        //         return;
        //     }
        // }

        // Add the custom bounding box to the queue
        customBoundingBoxes.Enqueue(customBox);
    }

    public void RemoveCustomBoundingBox()
    {
        if (customBoundingBoxes.Count > 0)
        {
            customBoundingBoxes.Dequeue();
        }
        else
        {
            Debug.LogWarning("No custom bounding boxes to remove.");
        }
    }

    private bool CheckOverlap(Rect newBox)
    {
        // Check overlap with active bounding boxes
        foreach (ActiveBoundingBox activeBox in activeBoundingBoxes)
        {
            if (newBox.Overlaps(activeBox.box, true))
            {
                return true; // Overlap detected
            }
        }

        // Check overlap with custom bounding boxes (if any)
        foreach (var customBox in customBoundingBoxes)
        {
            if (newBox.Overlaps(customBox, true))
            {
                return true; // Overlap detected
            }
        }

        return false; // No overlap detected
    }

    // private bool IsOverlap(Rect box1, Rect box2, Vector2 allowedOverlap)
    // {
    //     float xOverlap = Mathf.Max(0, Mathf.Min(box1.xMax, box2.xMax) - Mathf.Max(box1.xMin, box2.xMin));
    //     float yOverlap = Mathf.Max(0, Mathf.Min(box1.yMax, box2.yMax) - Mathf.Max(box1.yMin, box2.yMin));

    //     // Adjust overlap comparison with allowedOverlap for x and y
    //     return (xOverlap > allowedOverlap.x && yOverlap > allowedOverlap.y);
    // }
    private void UpdateIntensity(int newIntensity)
    {
        if (newIntensity >= intensityDataSet.Length)
        {
            currentIntensityData = intensityDataSet[intensityDataSet.Length - 1];
            Debug.LogError("New Intensity Level is Too Large, ie: RandomEnemySpawner");
        }
        else
        {
            initalIntensityHasBeenSet = true;
            currentIntensityData = intensityDataSet[newIntensity];
            Debug.Log("New Intensity set");
        }

        // Check and set the bounding boxes
        normalPigBoundingBox = currentIntensityData.NormalPig_BB ?? normalPigBoundingBoxBase;
        jetPackPigBoundingBox = currentIntensityData.JetpackPig_BB ?? jetPackPigBoundingBoxBase;
        bigPigBoundingBox = currentIntensityData.BigPig_BB ?? bigPigBoundingBoxBase;
        tenderizerPigBoundingBox = currentIntensityData.TenderizerPig_BB ?? tenderizerPigBoundingBoxBase;

        // Calculate and cache the center and half range for Big Pig's ySpawnRange
        bigPigYRangeCenter = (bigPigBoundingBox.ySpawnRange.x + bigPigBoundingBox.ySpawnRange.y) / 2f;
        bigPigYRangeHalf = Mathf.Abs(bigPigBoundingBox.ySpawnRange.y - bigPigBoundingBox.ySpawnRange.x) / 2f;
    }
    // private float CalculateOverlap(Rect box1, Rect box2)
    // {
    //     float xOverlap = Mathf.Max(0, Mathf.Min(box1.xMax, box2.xMax) - Mathf.Max(box1.xMin, box2.xMin));
    //     float yOverlap = Mathf.Max(0, Mathf.Min(box1.yMax, box2.yMax) - Mathf.Max(box1.yMin, box2.yMin));
    //     float overlapArea = xOverlap * yOverlap;
    //     float box1Area = box1.width * box1.height;
    //     float box2Area = box2.width * box2.height;

    //     // Calculate the overlap percentage relative to the smaller bounding box
    //     float overlapPercentage = overlapArea / Mathf.Min(box1Area, box2Area);
    //     return overlapPercentage;
    // }

    private void OnEnable()
    {

        // lvlID.outputEvent.OnSetNewIntensity += UpdateIntensity;

    }
    private void OnDisable()
    {
        // lvlID.outputEvent.OnSetNewIntensity -= UpdateIntensity;


    }
}