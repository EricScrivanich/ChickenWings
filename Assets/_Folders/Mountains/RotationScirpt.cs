using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScirpt : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float startRotation;
    private Rigidbody2D rb;
    private float currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentRotation = startRotation;

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void FixedUpdate()
    {
        currentRotation += rotationSpeed * Time.fixedDeltaTime;

        rb.MoveRotation(currentRotation);
    }
}
