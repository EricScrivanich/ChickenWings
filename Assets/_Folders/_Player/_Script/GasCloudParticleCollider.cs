using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloudParticleCollider : MonoBehaviour
{
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveAmount;
    private Rigidbody2D rb;

    private Vector2 initialVelocity;
    private float moveStartTime;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Initialize is called to set up initial state
    public void Initialize(Vector2 pos, Vector2 vel)
    {
        // Set the position
        transform.position = pos;

        // Calculate the angle from the velocity
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;

        // Adjust rotation - Unity's rotation is counterclockwise, and 0 degrees is to the right.
        // Subtract 90 degrees if you want up to be 0 degrees
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        // Store the initial velocity and the start time
        initialVelocity = vel.normalized * moveAmount;
        moveStartTime = Time.time;

        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        // Check if the move duration has not passed
        if (Time.time < moveStartTime + moveDuration)
        {
            // Move the object downward relative to its local rotation
            // Using local up vector which will be down in world space if rotation is set correctly
            rb.MovePosition(rb.position - (Vector2)transform.up * (initialVelocity.magnitude * Time.fixedDeltaTime));
        }
        else
        {
            // Optionally, disable or destroy the object after moving
            gameObject.SetActive(false); // Disable the GameObject
            // Destroy(gameObject); // Or destroy it if it's no longer needed
        }
    }
}