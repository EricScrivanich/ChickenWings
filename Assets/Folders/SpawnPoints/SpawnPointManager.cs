using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    public int numberOfPointsToDeactivate = 3;
    public static bool doRandom = true;
    private bool useSequential = true;
    private bool isActive = true;
    [SerializeField] private float interval;

    private float timer;
    private List<SpawnPoint> lastDeactivatedPoints = new List<SpawnPoint>();

    void Awake()
    {
        InitializeSpawnPoints();
    }
    private void Start() {
        int startIndex = UnityEngine.Random.Range(0, SpawnPoints.Count - numberOfPointsToDeactivate);
        SetSpawnPointsActiveInRange(startIndex, startIndex + numberOfPointsToDeactivate, false);
    }

    void Update()
    {
        if (!isActive)
        {
            SetAllSpawnPointsActive();
            return;
        }

        if (doRandom)
        {
            timer += Time.deltaTime;
        if (timer >= interval)
        {
            if (useSequential)
            {
                int startIndex = UnityEngine.Random.Range(0, (SpawnPoints.Count + 1) - numberOfPointsToDeactivate);
                Debug.Log(startIndex);
                SetSpawnPointsActiveInRange(startIndex, startIndex + numberOfPointsToDeactivate, false);
            }
            else
            {
                var randomPoints = GetRandomSpawnPoints(numberOfPointsToDeactivate);
                SetSpawnPointsActive(randomPoints, false);
            }

            timer = 0;
        }

        }

        
    }

    private void InitializeSpawnPoints()
    {
        SpawnPoints.Clear();
        foreach (Transform child in transform)
        {
            var spawnPoint = child.GetComponent<SpawnPoint>();
            if (spawnPoint != null)
            {
                SpawnPoints.Add(spawnPoint);
            }
        }
    }

    public void SetSpawnPointsActiveInRange(int startIndex, int endIndex, bool active)
    {
        ReactivateLastDeactivatedPoints();

        for (int i = startIndex; i < endIndex && i < SpawnPoints.Count; i++)
        {
            SpawnPoints[i].SetCanSpawn(active);
            if (!active) lastDeactivatedPoints.Add(SpawnPoints[i]);
        }
    }

    public List<SpawnPoint> GetRandomSpawnPoints(int count)
    {
        HashSet<int> selectedIndexes = new HashSet<int>();
        while (selectedIndexes.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);
            selectedIndexes.Add(randomIndex);
        }

        List<SpawnPoint> randomPoints = new List<SpawnPoint>();
        foreach (int index in selectedIndexes)
        {
            randomPoints.Add(SpawnPoints[index]);
        }

        return randomPoints;
    }

    public void SetSpawnPointsActive(List<SpawnPoint> points, bool active)
    {
        ReactivateLastDeactivatedPoints();

        foreach (var point in points)
        {
            point.SetCanSpawn(active);
            if (!active) lastDeactivatedPoints.Add(point);
        }
    }

    private void ReactivateLastDeactivatedPoints()
    {
        foreach (var point in lastDeactivatedPoints)
        {
            point.SetCanSpawn(true);
        }
        lastDeactivatedPoints.Clear();
    }

    private void SetAllSpawnPointsActive()
    {
        foreach (var point in SpawnPoints)
        {
            point.SetCanSpawn(true);
        }
    }



public static float GetRandomSpawnPointY(Func<SpawnPoint, bool> spawnCondition)
    {
        List<SpawnPoint> validSpawnPoints = new List<SpawnPoint>();

        foreach (var sp in SpawnPoints)
        {
            if (sp.canSpawn && spawnCondition(sp))
            {
                validSpawnPoints.Add(sp);
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogError("No valid spawn points found.");
            return 0f; // Return a default value or handle as needed
        }

        int randomIndex = UnityEngine.Random.Range(0, validSpawnPoints.Count);
        return validSpawnPoints[randomIndex].transform.position.y;
    }
}