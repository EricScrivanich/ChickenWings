using UnityEngine;

public class BounceyBall : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float playerBounceForce;
    [SerializeField] private float wallBounceForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Player"))
            rb.AddForceY(playerBounceForce, ForceMode2D.Impulse);
        if (other.gameObject.CompareTag("Boundry"))
            rb.AddForceY(wallBounceForce, ForceMode2D.Impulse);

    }
}
