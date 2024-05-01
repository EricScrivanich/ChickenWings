using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private Rigidbody2D rb;

    public float force;

void Awake()
{
rb = GetComponent<Rigidbody2D>();
}
    
    void OnEnable()
    {
        Vector2 forward = transform.up;  // Assuming the missile sprite points 'up' along the local y-axis
        rb.velocity = forward * force;

    }
}