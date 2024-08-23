using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewRandomIntensity", menuName = "Randomspawning/Intensity", order = 1)]
public class RandomSpawnIntensity : ScriptableObject
{
    [Header("State Switch Logic, 0 is null, index greater is subrtracted")]

    [SerializeField] private int overrideStateTransiton;

    public int OverrideStateTransiton
    {
        get => overrideStateTransiton;
        private set => overrideStateTransiton = value;
    }
    [Header("Pure Setup Logic")]
    [SerializeField] private float[] ringTypeWeights;


    [Header("Random Setup Rings Logic")]
    [SerializeField] private float[] randomRingSetupDifficultyWeights;
    [SerializeField] private float[] randomRingSetupAmountToSpawnWeights;
    public float[] RandomRingSetupAmountToSpawnWeights
    {
        get => randomRingSetupAmountToSpawnWeights;
        private set => randomRingSetupAmountToSpawnWeights = value;
    }
    [SerializeField] private float[] randomRingTypeWeights;
    public float[] RandomRingTypeWeights
    {
        get => randomRingTypeWeights;
        private set => randomRingTypeWeights = value;
    }
    [Header("Random Setup Enemy Logic")]
    [SerializeField] private float[] randomEnemySetupDifficultyWeights;
    public float[] RandomEnemySetupDifficultyWeights
    {
        get => randomEnemySetupDifficultyWeights;
        private set => randomEnemySetupDifficultyWeights = value;
    }
    [SerializeField] private float[] randomEnemySetupAmountToSpawnWeights;
    public float[] RandomEnemySetupAmountToSpawnWeights
    {
        get => randomEnemySetupAmountToSpawnWeights;
        private set => randomEnemySetupAmountToSpawnWeights = value;
    }

    [Header("Pure Random Logic")]
    [Header("Triggering Next Spawn")]
    [SerializeField] private float xTargetToStartSpawn;
    public float XTargetToStartSpawn
    {
        get => xTargetToStartSpawn;
        private set => xTargetToStartSpawn = value;
    }

    [SerializeField] private Vector2 spawnRandomRangeAfterTarget;
    public Vector2 SpawnRandomRangeAfterTarget
    {
        get => spawnRandomRangeAfterTarget;
        private set => spawnRandomRangeAfterTarget = value;
    }

    [Header("Wave Spawn Logic")]
    [SerializeField] private float[] amountOfWaveWeights;
    [SerializeField] private float[] waveSizeWeights;
    public float[] WaveSizeWeights
    {
        get => waveSizeWeights;
        private set => waveSizeWeights = value;
    }

    [SerializeField] private Vector2 xSpawnRange;
    public Vector2 XSpawnRange
    {
        get => xSpawnRange;
        private set => xSpawnRange = value;
    }

    [Header("MissilePigSpawning")]

    public Vector2 xSpawnRangeMissilePig;
    public float shootingRangeAdjustment;
    public float minMissilePigDelay;
    public float missileBasePigChance;
    public float missilePigFlipPChance;
    public float missilePigFlipWChance;

    [Header("Bounding Box Adjustment")]
    [SerializeField] private Vector2 allowedOverlap;
    public Vector2 AllowedOverlap
    {
        get => allowedOverlap;
        private set => allowedOverlap = value;
    }

    [SerializeField] private float activeBoundingBoxTimeAdjustment;
    public float ActiveBoundingBoxTimeAdjustment
    {
        get => activeBoundingBoxTimeAdjustment;
        private set => activeBoundingBoxTimeAdjustment = value;
    }

    [Header("Enemy Type Weights")]
    [SerializeField] private float normalPig_W;
    public float NormalPig_W
    {
        get => normalPig_W;
        private set => normalPig_W = value;
    }

    [SerializeField] private float jetpackPig_W;
    public float JetpackPig_W
    {
        get => jetpackPig_W;
        private set => jetpackPig_W = value;
    }

    [SerializeField] private float bigPig_W;
    public float BigPig_W
    {
        get => bigPig_W;
        private set => bigPig_W = value;
    }

    [SerializeField] private float tenderizerPig_W;
    public float TenderizerPig_W
    {
        get => tenderizerPig_W;
        private set => tenderizerPig_W = value;
    }

    [Header("Enemy Bounding Boxes")]
    [ExposedScriptableObject]
    public EnemyBoundingBox NormalPig_BB;
    [ExposedScriptableObject]
    public EnemyBoundingBox JetpackPig_BB;
    [ExposedScriptableObject]
    public EnemyBoundingBox BigPig_BB;
    [ExposedScriptableObject]
    public EnemyBoundingBox TenderizerPig_BB;

    public int GetRandomAmountOfWaves()
    {
        if (amountOfWaveWeights == null || amountOfWaveWeights.Length == 0)
        {

            return 0; // Default to 1 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in amountOfWaveWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < amountOfWaveWeights.Length; i++)
        {
            cumulativeWeight += amountOfWaveWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Wave Size is: " + i);
                return i; // Return the wave size as the index
            }
        }

        return waveSizeWeights.Length - 1; // Return the max size if something goes wrong
    }

    public int GetRandomWaveSize()
    {
        if (waveSizeWeights == null || waveSizeWeights.Length == 0)
        {
            Debug.LogWarning("Wave size weights are not set!");
            return 1; // Default to 1 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in waveSizeWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < waveSizeWeights.Length; i++)
        {
            cumulativeWeight += waveSizeWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Wave Size is: " + i);
                return i; // Return the wave size as the index
            }
        }

        return waveSizeWeights.Length - 1; // Return the max size if something goes wrong
    }

    public int GetRingDifficultyIndex()
    {
        if (randomRingSetupDifficultyWeights == null || randomRingSetupDifficultyWeights.Length == 0)
        {
            Debug.LogWarning("Ring diff weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in randomRingSetupDifficultyWeights)
        {
            totalWeight += weight;
        }

        float Value = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < randomRingSetupDifficultyWeights.Length; i++)
        {
            cumulativeWeight += randomRingSetupDifficultyWeights[i];
            if (Value < cumulativeWeight)
            {
                Debug.LogError("Ring difficulty Index is: " + i);
                return i; // Return the index
            }
        }

        return randomRingSetupDifficultyWeights.Length - 1; // Return the max index if something goes wrong

    }
    public int GetRingTypeIndex()
    {
        if (ringTypeWeights == null || ringTypeWeights.Length == 0)
        {
            Debug.LogWarning("Ring type weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in ringTypeWeights)
        {
            totalWeight += weight;
        }

        float Value = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < ringTypeWeights.Length; i++)
        {
            cumulativeWeight += ringTypeWeights[i];
            if (Value < cumulativeWeight)
            {
                Debug.Log("Ring Type Index is: " + i);
                return i; // Return the index
            }
        }

        return ringTypeWeights.Length - 1; // Return the max index if something goes wrong
    }


    public int GetRandomRingSetupAmountIndex()
    {
        if (RandomRingSetupAmountToSpawnWeights == null || RandomRingSetupAmountToSpawnWeights.Length == 0)
        {
            Debug.LogWarning("Ring setup weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in RandomRingSetupAmountToSpawnWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < RandomRingSetupAmountToSpawnWeights.Length; i++)
        {
            cumulativeWeight += RandomRingSetupAmountToSpawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Ring Setup Amount Index is: " + i);
                return i; // Return the index
            }
        }

        return RandomRingSetupAmountToSpawnWeights.Length - 1; // Return the max index if something goes wrong
    }



    public int GetRandomRingTypeIndex()
    {
        if (RandomRingTypeWeights == null || RandomRingTypeWeights.Length == 0)
        {
            Debug.LogWarning("Ring type weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in RandomRingTypeWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < RandomRingTypeWeights.Length; i++)
        {
            cumulativeWeight += RandomRingTypeWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Random Ring Type Index is: " + i);
                return i; // Return the index
            }
        }

        return RandomRingTypeWeights.Length - 1; // Return the max index if something goes wrong
    }



    public int GetRandomEnemySetupAmountIndex()
    {
        if (RandomEnemySetupAmountToSpawnWeights == null || RandomEnemySetupAmountToSpawnWeights.Length == 0)
        {
            Debug.LogWarning("Enemy setup amount weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in RandomEnemySetupAmountToSpawnWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < RandomEnemySetupAmountToSpawnWeights.Length; i++)
        {
            cumulativeWeight += RandomEnemySetupAmountToSpawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Enemy Setup Amount Index is: " + i);
                return i; // Return the index
            }
        }

        return RandomEnemySetupAmountToSpawnWeights.Length - 1; // Return the max index if something goes wrong
    }

    public int GetRandomEnemySetupDifficultyIndex()
    {
        if (RandomEnemySetupDifficultyWeights == null || RandomEnemySetupDifficultyWeights.Length == 0)
        {
            Debug.LogWarning("Enemy setup difficulty weights are not set!");
            return 0; // Default to 0 if not set
        }

        float totalWeight = 0f;
        foreach (var weight in RandomEnemySetupDifficultyWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < RandomEnemySetupDifficultyWeights.Length; i++)
        {
            cumulativeWeight += RandomEnemySetupDifficultyWeights[i];
            if (randomValue < cumulativeWeight)
            {
                Debug.Log("Enemy Setup Difficulty Index is: " + i);
                return i; // Return the index
            }
        }

        return RandomEnemySetupDifficultyWeights.Length - 1; // Return the max index if something goes wrong
    }
}

