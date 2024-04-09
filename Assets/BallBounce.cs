using UnityEngine;

public class BallBounce : MonoBehaviour
{
    public float minimumHeight = 1.0f; // Minimum height you want the ball to bounce

    private Rigidbody2D rb; // Reference to the Rigidbody component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            float minVelocity = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * minimumHeight);

            // Check if the current upward velocity is less than the minimum required velocity
            if (rb.velocity.y < minVelocity)
            {
                // Adjust the velocity to ensure the ball reaches the minimum height
                rb.velocity = new Vector2(rb.velocity.x, minVelocity);
            }

        }
        // Calculate the minimum velocity needed to reach the desired height

    }
}
