using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg_BulletMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float force;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    void OnEnable()
    {
        

    }
    private void FixedUpdate() 
    {
        rb.AddRelativeForce(Vector2.down * force,ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
