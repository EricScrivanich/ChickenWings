using UnityEngine;

public class bulletMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    Vector2 direction = transform.right;

        // Apply speed to the direction
        Vector2 velocity = direction * speed;

        // Apply the velocity to the Rigidbody2D
        rb.velocity = velocity;
    }
}
