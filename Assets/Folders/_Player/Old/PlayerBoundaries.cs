using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundaries : MonoBehaviour
{
    private Transform _transform;
    private Rigidbody2D rb;
    
    private Vector2 pushForceLeft;
    private Vector2 pushForceRight;

    private void Awake() 
    {
        pushForceLeft = new Vector2(-3,1);
        pushForceRight = new Vector2(3,1);

    }

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called at fixed intervals
    private void FixedUpdate()
    {
        if (_transform.position.x > BoundariesManager.rightPlayerBoundary)
        {
            _transform.position = new Vector2(BoundariesManager.leftPlayerBoundary, _transform.position.y);
            rb.AddForce(pushForceRight, ForceMode2D.Impulse);
        }
        if (_transform.position.x < BoundariesManager.leftPlayerBoundary)
        {
            _transform.position = new Vector2(BoundariesManager.rightPlayerBoundary, _transform.position.y);
            rb.AddForce(pushForceLeft, ForceMode2D.Impulse);
        }
    }
}
