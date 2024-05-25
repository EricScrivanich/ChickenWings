using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScirpt : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private Rigidbody2D rb;
    private float currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentRotation = 0;

    }

    // Update is called once per frame
    void Update()
    {
    
        
    }

    private void FixedUpdate() {
        currentRotation += rotationSpeed * Time.fixedDeltaTime;
        Debug.Log(transform.rotation.eulerAngles.z);
        rb.MoveRotation(currentRotation);
    }
}
