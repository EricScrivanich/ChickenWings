using UnityEngine;

public class ExplodingBullet : MonoBehaviour
{
    [SerializeField] private GameObject shardPrefab;
    private Rigidbody2D rb;
    private SpriteRenderer childSpriteRenderer;
    [SerializeField] private float initialSpeed = 10f;
    [SerializeField] private float slowdownRate = 0.5f; // Adjust this value to control the rate of slowdown
    [SerializeField] private float minOpacity = 0f;
    [SerializeField] private float maxOpacity = 100f;
    [SerializeField] private int shardCount = 6;

void Update()
{
    

}
void SlowDown()
{
 
}








    // private IEnumerator Start()
    // {
    //     rb = GetComponent<Rigidbody2D>();
    //     rb.velocity = transform.right * initialSpeed;

    //     childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    //     childSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

    //     yield return StartCoroutine(SlowDownBullet());
    // }

    // private void Update()
    // {
    //     UpdateOpacity();
    // }

    // private IEnumerator SlowDownBullet()
    // {
    //     while (rb.velocity.magnitude > 0.1f) // Adjust the threshold as needed
    //     {
    //         rb.velocity -= rb.velocity.normalized * slowdownRate * Time.deltaTime;
    //         yield return null;
    //     }

    //     // Start the explosion coroutine
    //     yield return StartCoroutine(Explode());
    // }

    // private void UpdateOpacity()
    // {
    //     float normalizedVelocity = rb.velocity.magnitude / initialSpeed;
    //     float targetOpacity = Mathf.Lerp(minOpacity, maxOpacity, 1f - normalizedVelocity); // Invert the normalizedVelocity since we want higher opacity for slower bullets
    //     childSpriteRenderer.color = new Color(1f, 1f, 1f, targetOpacity / 100f);
    // }

    // private IEnumerator Explode()
    // {
    //     // Instantiate the shards evenly spaced around the bullet
    //     for (int i = 0; i < shardCount; i++)
    //     {
    //         float angle = i * (360f / shardCount);
    //         Vector3 shardDirection = Quaternion.Euler(0f, 0f, angle) * Vector3.right;
    //         GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
    //         Rigidbody2D shardRb = shard.GetComponent<Rigidbody2D>();
    //         shardRb.velocity = shardDirection * initialSpeed;
    //     }

    //     // Destroy the bullet and its child objects after a short delay to let the shards move away from the bullet
    //     yield return new WaitForSeconds(0.1f);
    //     Destroy(gameObject);
    // }
}
