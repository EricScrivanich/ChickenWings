using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneMovement : MonoBehaviour
{
    private float speed;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        speed = 7;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("Log", 3);

    }

    // Update is called once per frame
    void Update()
    {
        // transform.rotation = Quaternion.Euler(0, 0, 0);
        // transform.Translate(Vector3.left * speed * Time.deltaTime);

    }

    private void Log()
    {

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log(rb.linearVelocity.x);

    }
}
