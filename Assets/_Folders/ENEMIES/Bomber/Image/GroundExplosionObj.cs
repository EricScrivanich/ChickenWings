using UnityEngine;

public class GroundExplosionObj : SpawnedQueuedEffect
{
    [SerializeField] private QEffectPool blemishPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void SetUnactive()
    {
        gameObject.SetActive(false);

    }
    public override void AdditionalSpawnLogic()
    {
        blemishPool.Spawn(transform.position);
    }
    void OnDisable()
    {
        ReturnToPool();
    }

    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
    }
}
