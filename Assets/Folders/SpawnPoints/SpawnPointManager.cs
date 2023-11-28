using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnPointManager : MonoBehaviour
{
    public static List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    void Awake()
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
    public void DisableSpawningAtPoint(int index)
    {
        if (index >= 0 && index < SpawnPoints.Count)
        {
            SpawnPoints[index].SetCanSpawn(false);
        }
    }

    public void EnableSpawningAtPoint(int index)
    {
        if (index >= 0 && index < SpawnPoints.Count)
        {
            SpawnPoints[index].SetCanSpawn(true);
        }
    }

     public void SetSpawningForRange(int startIndex, int endIndex, bool value)
    {
        if (startIndex < 0 || endIndex >= SpawnPoints.Count || startIndex > endIndex)
        {
            Debug.LogError("Invalid range for setting spawn points.");
            return;
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            SpawnPoints[i].SetCanSpawn(value);
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