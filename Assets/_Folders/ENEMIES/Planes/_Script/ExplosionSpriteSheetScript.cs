using HellTap.PoolKit;
using UnityEngine;

public class ExplosionSpriteSheetScript : MonoBehaviour, IPoolKitListener
{

    // Implement the IPoolKitListener interface methods
    public void OnSpawn(Pool pool)
    {
        // Play the explosion sound when the object is spawned from the pool
        AudioManager.instance.PlayBombExplosionSound();
    }

    public void OnDespawn()
    {
        // You can add any cleanup code here if needed
    }
}