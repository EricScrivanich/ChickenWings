using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private Rigidbody2D rb;

    public float force;
    public float accelerationTime = 0.3f; // Time to reach the desired force speed

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        StartCoroutine(LaunchMissile());
    }
   

    private IEnumerator LaunchMissile()
    {
        // Wait for 0.5 seconds before starting the launch


        Vector2 forward = transform.up; // Assuming the missile sprite points 'up' along the local y-axis
        float initialForce = 2f;
        float elapsedTime = 0f;

        // Gradually increase the force over accelerationTime
        while (elapsedTime < accelerationTime)
        {
            elapsedTime += Time.deltaTime;
            float currentForce = Mathf.Lerp(initialForce, force, elapsedTime / accelerationTime);
            rb.velocity = forward * currentForce;
            yield return null;
        }

        // Set the final velocity to the desired force speed
        rb.velocity = forward * force;
    }
}