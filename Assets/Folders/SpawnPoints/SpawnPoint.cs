
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public bool canSpawn = true;
    public bool canSpawnCrop = true;
    public bool canSpawnCargo = true;
    public bool canSpawnJet = true;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();

    }
    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = canSpawn ? Color.green : Color.red;
        }
    }

    public void SetCanSpawn(bool value)
    {
        canSpawn = value;
        UpdateColor();

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