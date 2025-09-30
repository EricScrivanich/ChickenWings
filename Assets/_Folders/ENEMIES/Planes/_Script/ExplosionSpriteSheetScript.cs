// using HellTap.PoolKit;
using UnityEngine;

public class ExplosionSpriteSheetScript : SpawnedQueuedEffect
{

    // Implement the IPoolKitListener interface methods
    // public void OnSpawn(Pool pool)
    // {
    //     // Play the explosion sound when the object is spawned from the pool

    // }

    // public void OnDespawn()
    // {
    //     // You can add any cleanup code here if needed
    // }

    public void UnActivate()
    {

        gameObject.SetActive(false);

    }
    void OnDisable()
    {
        ReturnToPool();
    }
}