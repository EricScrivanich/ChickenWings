using UnityEngine;

public class SpawnedQueuedObject : MonoBehaviour
{
    protected QPool pool;
    protected Rigidbody2D rb;

    public void Initialize(QPool p)
    {
        pool = p;
    }
    public virtual void OnSpawnLogic()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Spawn(Vector2 pos)
    {
        transform.position = pos;


        gameObject.SetActive(true);
        OnSpawnLogic();

    }

    public void SpawnWithRotation(Vector2 pos, float r)
    {
        transform.SetPositionAndRotation(pos, Quaternion.Euler(Vector3.forward * r));
        gameObject.SetActive(true);
        OnSpawnLogic();

    }

    public void SpawnWithRotationAndSpeed(Vector2 pos, Quaternion ang, float speed, ushort side)
    {
        transform.SetPositionAndRotation(pos, ang);
        gameObject.SetActive(true);

        switch (side)
        {
            case 5:
                break;
            case 0:

                rb.linearVelocity = transform.right * speed;
                break;

            case 1:
                rb.linearVelocity = transform.up * speed;
                break;

            case 2:
                rb.linearVelocity = -transform.up * speed;
                break;

            case 3:
                rb.linearVelocity = -transform.right * speed;
                break;

        }
    }

    public virtual void SpawnSpecial(Vector2 pos, float speed, bool flipped)
    {

    }


    public void ResetRB()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.SetRotation(0);
        }

    }
    // public virtual void InitializeObject
    public void SpawnWithVelocity(Vector2 pos, Vector2 velocity)
    {
        Spawn(pos);
        rb.linearVelocity = velocity;


    }
    public void SpawnWithVelocityAndRotation(Vector2 pos, Vector2 velocity, float rotation, float angularVelocity = 0)
    {
        transform.SetPositionAndRotation(pos, Quaternion.Euler(Vector3.forward * rotation));
        gameObject.SetActive(true);
        OnSpawnLogic();
        rb.linearVelocity = velocity;
        // rb.rotation = rotation;
        if (angularVelocity != 0) rb.angularVelocity = angularVelocity;

    }
    public virtual void SpawnTransformOverride(Vector2 pos, float rotation, int ID, bool other)
    {

    }

    public virtual void SpawnWithVelocityAndRotationAndScale(Vector2 pos, float vel, float rotation, float scale, byte type = 0)
    {
        transform.localScale = Vector3.one * scale;
        SpawnWithRotation(pos, rotation);



        // rb.rotation = rotation;

        switch (type)
        {
            // case 0:
            //     // rb.linearVelocity = velocity;
            //     break;
            case 1:
                rb.linearVelocity = -transform.right * vel;
                break;
            case 2:
                rb.linearVelocity = transform.right * vel;
                break;
        }

        // rb.linearVelocity = velocity;


    }

    public void ReturnToPool()
    {
        pool.ReturnObject(this);

    }




}
