using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneMovement : MonoBehaviour
{
    private float speed;
    
    // Start is called before the first frame update
    void Awake()
    {
        speed = 7;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
