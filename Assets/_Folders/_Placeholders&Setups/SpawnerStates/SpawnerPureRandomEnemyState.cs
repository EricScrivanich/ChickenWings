using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerPureRandomEnemyState : SpawnBaseState
{
    private EnemyBoundingBox normalPigBoundingBox;
    private EnemyBoundingBox jetPackPigBoundingBox;
    private EnemyBoundingBox bigPigBoundingBox;
    private EnemyBoundingBox tenderizerPigBoundingBox;

    private Vector2 flipXSpawnVector = new Vector2(-1, 1);

    private float flipChance;

    private float minYAdj;

    private int amountOfWavesToSpawn;
    private int currentAmountOfWavesSpawned;
    private bool useWaveAmount;

    private float bigPigYRangeCenter;
    private float bigPigYRangeHalf;
    // private Queue<Rect> customBoundingBoxes = new Queue<Rect>();
    private List<int> pigTypeList = new List<int>();
    private bool isSpawning;

    private RandomSpawnIntensity currentIntensityData;
    private float spawnInterval;
    private float timeSinceLastWave;
    private class ActiveBoundingBox
    {
        public Rect box;
        public float expirationTime;
    }
    private List<ActiveBoundingBox> activeBoundingBoxes = new List<ActiveBoundingBox>();

    private SpawnStateManager spawnCache;

    private float spawnInteval;


    // Start is called before the first frame update
    public override void EnterState(SpawnStateManager spawner)
    {
        Debug.LogError("Entered Random State");
        minYAdj = 0;


        currentAmountOfWavesSpawned = 0;


        spawnCache = spawner;

        currentIntensityData = spawner.currentRandomSpawnIntensityData;
        normalPigBoundingBox = currentIntensityData.NormalPig_BB ?? spawner.normalPigBoundingBoxBase;
        jetPackPigBoundingBox = currentIntensityData.JetpackPig_BB ?? spawner.jetPackPigBoundingBoxBase;
        bigPigBoundingBox = currentIntensityData.BigPig_BB ?? spawner.bigPigBoundingBoxBase;
        tenderizerPigBoundingBox = currentIntensityData.TenderizerPig_BB ?? spawner.tenderizerPigBoundingBoxBase;

        amountOfWavesToSpawn = currentIntensityData.GetRandomAmountOfWaves();
        Debug.LogError("Amount of waves is: " + amountOfWavesToSpawn);
        flipChance = currentIntensityData.flipRandomEnemySpawnXChance;

        if (amountOfWavesToSpawn < 0) useWaveAmount = false;

        else useWaveAmount = true;

        // Calculate and cache the center and half range for Big Pig's ySpawnRange
        bigPigYRangeCenter = (bigPigBoundingBox.ySpawnRange.x + bigPigBoundingBox.ySpawnRange.y) / 2f;
        bigPigYRangeHalf = Mathf.Abs(bigPigBoundingBox.ySpawnRange.y - bigPigBoundingBox.ySpawnRange.x) / 2f;

        if (amountOfWavesToSpawn < 0)
        {
            Debug.LogError("Random waves is 0");
            useWaveAmount = false;

        }
        else if (amountOfWavesToSpawn == 0)
        {


            spawner.TimerForNextWave(.4f);
            return;
        }

        if (!spawner.stopRandomSpawning)
        {
            timeSinceLastWave = 0;
            int waveSize = currentIntensityData.GetRandomWaveSize();
            Debug.LogError("Wave size is: " + waveSize);
            if (spawner.GetMissilePigIfReady(currentIntensityData.minMissilePigDelay, currentIntensityData.missileBasePigChance))
            {

                minYAdj = .8f;
                waveSize--;
                float rFlipP = Random.Range(0f, 1f);
                float rFlipW = Random.Range(0f, 1f);
                int type = 0;

                if (rFlipP < currentIntensityData.missilePigFlipPChance)
                {
                    type += 1;

                }
                if (rFlipW < currentIntensityData.missilePigFlipWChance)
                {
                    type += 2;

                }

                spawner.GetMissilePig(Random.Range(currentIntensityData.xSpawnRangeMissilePig.x, currentIntensityData.xSpawnRangeMissilePig.y), type, 0);
            }
            spawner.recentlySpanwnedPositions = new List<Vector2>();
            pigTypeList = DeterminePigTypesForWave(waveSize);

            spawner.SpawnWithDelayRoutine = spawner.StartCoroutine(SpawnWaveWithDelay(spawner));


        }







    }

    public override void ExitState(SpawnStateManager spawner)
    {
        useWaveAmount = false;
    }

    public override void SetSpeedAndPos(SpawnStateManager spawner)
    {

    }

    public void Initialize()
    {

    }

    public override void SetNewIntensity(SpawnStateManager spawner, RandomSpawnIntensity spawnIntensity)
    {
        // Debug.LogError("Set new intensity in script");

        if (useWaveAmount)
        {
            Debug.LogError("Using Wave Amount");
            return;
        }
        spawnCache = spawner;
        currentIntensityData = spawnIntensity;
        flipChance = currentIntensityData.flipRandomEnemySpawnXChance;
        normalPigBoundingBox = currentIntensityData.NormalPig_BB ?? spawner.normalPigBoundingBoxBase;
        jetPackPigBoundingBox = currentIntensityData.JetpackPig_BB ?? spawner.jetPackPigBoundingBoxBase;
        bigPigBoundingBox = currentIntensityData.BigPig_BB ?? spawner.bigPigBoundingBoxBase;
        tenderizerPigBoundingBox = currentIntensityData.TenderizerPig_BB ?? spawner.tenderizerPigBoundingBoxBase;

        amountOfWavesToSpawn = currentIntensityData.GetRandomAmountOfWaves();
        // Debug.LogError("Random amount of waves to spawn is: " + amountOfWavesToSpawn);
        if (amountOfWavesToSpawn < 0) useWaveAmount = false;
        // else if (amountOfWavesToSpawn == 0)
        // {
        //     spawner.TimerForNextWave(.4f);
        //     return;
        // }
        else useWaveAmount = true;

        // Calculate and cache the center and half range for Big Pig's ySpawnRange
        bigPigYRangeCenter = (bigPigBoundingBox.ySpawnRange.x + bigPigBoundingBox.ySpawnRange.y) / 2f;
        bigPigYRangeHalf = Mathf.Abs(bigPigBoundingBox.ySpawnRange.y - bigPigBoundingBox.ySpawnRange.x) / 2f;

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
        foreach (var customBox in spawnCache.customBoundingBoxes)
        {
            if (newBox.Overlaps(customBox, true))
            {
                return true; // Overlap detected
            }
        }

        return false; // No overlap detected
    }

    private List<int> DeterminePigTypesForWave(int waveSize)
    {
        List<int> pigTypeList = new List<int>();

        // Calculate the total weight
        float totalWeight = currentIntensityData.NormalPig_W + currentIntensityData.JetpackPig_W +
                            currentIntensityData.BigPig_W + currentIntensityData.TenderizerPig_W;

        // Calculate the probabilities for each pig type
        float normalPigProb = currentIntensityData.NormalPig_W / totalWeight;
        float jetPackPigProb = currentIntensityData.JetpackPig_W / totalWeight;
        float bigPigProb = currentIntensityData.BigPig_W / totalWeight;
        float tenderizerPigProb = currentIntensityData.TenderizerPig_W / totalWeight;

        // Debug.Log($"Probabilities - Normal: {normalPigProb}, JetPack: {jetPackPigProb}, Big: {bigPigProb}, Tenderizer: {tenderizerPigProb}");

        // Randomly determine the pig types for the wave based on the probabilities
        for (int i = 0; i < waveSize; i++)
        {
            float randomValue = Random.value;  // Random value between 0 and 1

            if (randomValue <= tenderizerPigProb)
            {
                pigTypeList.Add(3);  // Tenderizer Pig
            }
            else if (randomValue <= tenderizerPigProb + bigPigProb)
            {
                pigTypeList.Add(2);  // Big Pig
            }
            else if (randomValue <= tenderizerPigProb + bigPigProb + jetPackPigProb)
            {
                pigTypeList.Add(1);  // JetPack Pig
            }
            else
            {
                pigTypeList.Add(0);  // Normal Pig
            }
        }

        Debug.Log($"Pig Type List Finalized: {string.Join(", ", pigTypeList)}");

        return pigTypeList;
    }

    private IEnumerator SpawnWaveWithDelay(SpawnStateManager manager)
    {
        if (useWaveAmount)
        {
            currentAmountOfWavesSpawned++;
            // Debug.LogError("Amount of waves spawned is: " + currentAmountOfWavesSpawned);
        }


        yield return new WaitForSeconds(.15f);

        // Spawn pigs according to the sorted order
        for (int i = 0; i < pigTypeList.Count; i++)
        {



            activeBoundingBoxes.RemoveAll(box => Time.time > box.expirationTime);

            SpawnPig(pigTypeList[i]);

            yield return new WaitForSeconds(0.22f);




        }

        // manager.WaitForWaveFinishRoutine = manager.StartCoroutine(manager.SetupDuration(manager.TimeForWaveToReachTarget(currentIntensityData.XTargetToStartSpawn) + Random.Range(currentIntensityData.SpawnRandomRangeAfterTarget.x, currentIntensityData.SpawnRandomRangeAfterTarget.y)));

        manager.TimerForNextWave(manager.TimeForWaveToReachTarget(currentIntensityData.XTargetToStartSpawn, true) + Random.Range(currentIntensityData.SpawnRandomRangeAfterTarget.x, currentIntensityData.SpawnRandomRangeAfterTarget.y));



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
                Random.Range(ySpawnRange.x + minYAdj, ySpawnRange.y)
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
                spawnCache.recentlySpanwnedPositions.Add(new Vector2(speed, spawnPos.x));

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
                spawnCache.recentlySpanwnedPositions.Add(new Vector2(speed, spawnPos.x));

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

    private void SpawnSelectedPig(int pigType, Vector2 spawnPos, Vector3 scale, float speed, float yForce = 0f, float distanceToFlap = 0f)
    {

        if (flipChance > 0)
        {
            if (Random.Range(0f, 1f) <= flipChance)
            {
                spawnPos *= flipXSpawnVector;
                speed -= 1.5f;
                speed *= -1;
            }

        }

        switch (pigType)
        {
            case 0:
                spawnCache.GetNormalPig(spawnPos, scale, speed);
                break;
            case 1:
                spawnCache.GetJetPackPig(spawnPos, scale, speed);
                break;
            case 2:
                spawnCache.GetBigPig(spawnPos, scale, speed, yForce, distanceToFlap, 0);
                break;
            case 3:

                spawnCache.GetTenderizerPig(spawnPos, scale, speed, true);
                break;
        }
    }

    public override void SetupHitTarget(SpawnStateManager spawner)
    {
        timeSinceLastWave = 0;
        minYAdj = 0;

        if (useWaveAmount && currentAmountOfWavesSpawned >= amountOfWavesToSpawn)
        {
            Debug.LogError("swithcing to next state from random");
            spawner.SwitchStateWithLogic();
            return;

        }
        int waveSize = currentIntensityData.GetRandomWaveSize();
        if (spawner.GetMissilePigIfReady(currentIntensityData.minMissilePigDelay, currentIntensityData.missileBasePigChance))
        {

            minYAdj = .8f;

            waveSize--;
            float rFlipP = Random.Range(0f, 1f);
            float rFlipW = Random.Range(0f, 1f);

            int type = 0;

            if (rFlipP < currentIntensityData.missilePigFlipPChance)
            {
                type += 1;

            }
            if (rFlipW < currentIntensityData.missilePigFlipWChance)
            {
                type += 2;

            }

            spawner.GetMissilePig(Random.Range(currentIntensityData.xSpawnRangeMissilePig.x, currentIntensityData.xSpawnRangeMissilePig.y), type, 0);
        }
        spawner.recentlySpanwnedPositions = new List<Vector2>();

        pigTypeList = DeterminePigTypesForWave(waveSize);

        spawner.SpawnWithDelayRoutine = spawner.StartCoroutine(SpawnWaveWithDelay(spawner));

    }

    public override void RingsFinished(SpawnStateManager spawner, int n, bool isCorrect)
    {

    }
}
