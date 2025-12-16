using UnityEngine;

public class Bullet : SpawnedQueuedObject
{

    private float lifeTime = 4.5f;
    private float time = 0;
    private TrailRenderer trail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }

    void OnDisable()
    {
        trail.emitting = false;

        ReturnToPool();
    }

    public override void OnSpawnLogic(ushort id = 0)
    {
        trail.emitting = true;
        time = 0;
    }

    public void Fire(Vector2 pos, float z, float speed, int flip)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, 0, z);
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(true);
        trail.emitting = true;
        rb.AddForce(-transform.right * speed * flip, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > lifeTime)
        {
            trail.emitting = false;
            trail.Clear();
            gameObject.SetActive(false);
            time = 0;
        }

    }
}
