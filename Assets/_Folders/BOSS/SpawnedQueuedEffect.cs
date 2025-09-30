using UnityEngine;

public class SpawnedQueuedEffect : MonoBehaviour
{
    protected QEffectPool pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(QEffectPool p)
    {
        pool = p;
    }
    public void ReturnToPool()
    {
        pool.ReturnObject(this);
    }
    public virtual void AdditionalSpawnLogic()
    {

    }
    public void Spawn(Vector2 pos)
    {
        transform.position = pos;

        gameObject.SetActive(true);
    }
    public void Spawn(Vector2 pos, float scale)
    {
        transform.position = pos;
        transform.localScale = Vector3.one * scale;
        gameObject.SetActive(true);

    }
    public void Spawn(Vector2 pos, Vector3 scale)
    {
        transform.position = pos;
        transform.localScale = scale;
        gameObject.SetActive(true);
        AdditionalSpawnLogic();
    }
    // Update is called once per frame

}
