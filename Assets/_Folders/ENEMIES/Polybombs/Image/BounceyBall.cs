using UnityEngine;

public class BounceyBall : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float playerBounceForce;
    [SerializeField] private float wallBounceForce;
    [SerializeField] private Vector2 playerNormalForceLerp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Bouncey Ball Collided with: " + other.gameObject.name + " Tag: " + other.gameObject.tag);


        if (other.gameObject.CompareTag("Player"))
        {
            float n = other.contacts[0].normal.y;

            if (n > 0)
                rb.AddForceY(playerBounceForce * Mathf.Lerp(playerNormalForceLerp.x, playerNormalForceLerp.y, n), ForceMode2D.Impulse);
        }

        // if (other.gameObject.CompareTag("Boundry"))
        //     rb.AddForceY(wallBounceForce, ForceMode2D.Impulse);

    }
}
