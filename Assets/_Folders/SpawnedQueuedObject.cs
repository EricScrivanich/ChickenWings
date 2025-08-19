using UnityEngine;

public class SpawnedQueuedObject : MonoBehaviour
{
    protected QPool pool;
    protected Rigidbody2D rb;

    public void Initialize(QPool p)
    {
        pool = p;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Spawn(Vector2 pos)
    {
        transform.position = pos;


        gameObject.SetActive(true);
    }
    public void ResetRB()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

    }
    // public virtual void InitializeObject
    public void SpawnWithVelocity(Vector2 pos, Vector2 velocity)
    {
        Spawn(pos);
        rb.linearVelocity = velocity;


    }
    public void SpawnWithVelocityAndRotation(Vector2 pos, Vector2 velocity, float rotation)
    {
        Spawn(pos);
        rb.linearVelocity = velocity;
        rb.SetRotation(rotation);

    }
    public virtual void SpawnWithVelocityAndRotationAndScale(Vector2 pos, Vector2 velocity, float rotation, float scale)
    {
        Spawn(pos);
        rb.linearVelocity = velocity;
        rb.rotation = rotation;
        transform.localScale = Vector3.one * scale;


    }

    public void ReturnToPool()
    {
        pool.ReturnObject(this);

    }




}
