using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool canSpawn = true;
    public bool canSpawnCrop = true;
    public bool canSpawnCargo = true;
    public bool canSpawnJet = true;

    public void SetCanSpawn(bool value)
    {
        canSpawn = value;
    }

    public void SetCanSpawnForCrop(bool value)
    {
        canSpawnCrop = value;
    }

    public void SetCanSpawnForCargo(bool value)
    {
        canSpawnCargo = value;
    }
    public void SetCanSpawnForJet(bool value)
    {
        canSpawnJet = value;
    }

    // Add similar methods for other plane types if needed
}