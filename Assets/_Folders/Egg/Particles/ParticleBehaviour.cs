using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    [SerializeField] private float minRotationSpeed = -200f; // Minimum rotation speed
    [SerializeField] private float maxRotationSpeed = 200f; // Maximum rotation speed
    [SerializeField] private float groundSpeed = -5f; // Speed of movement when on the ground

    private float rotationSpeed; // Actual rotation speed of the particle
    private bool isGrounded = false;
    private Rigidbody2D rb;
    private ManualParticleSpawner particleSpawner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Find the particle spawner in the scene
        particleSpawner = FindObjectOfType<ManualParticleSpawner>();
    }

    private void OnEnable()
    {
        // Set a random rotation speed within the range
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        isGrounded = false;
    }

    private void Update()
    {
        // Rotate the particle if it's not grounded
        if (!isGrounded)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Set the ground movement speed
            rb.velocity = new Vector2(groundSpeed, rb.velocity.y);
        }

        // Check if the particle has gone off-screen
      
    }

    // Detect collision with the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    // Detect leaving the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        particleSpawner.ReturnToPool(this.gameObject);
    }
}

