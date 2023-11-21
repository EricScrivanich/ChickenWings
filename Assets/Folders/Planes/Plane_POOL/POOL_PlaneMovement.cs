using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POOL_PlaneMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    
    [SerializeField] private float planeSinFrequency;
    [SerializeField] private float planeSinAmplitude;

    private Transform _transform;
    private float planeSpeed;
    private bool planeHitBool;
    private float initialY;

    void Awake()
    {
        planeSpeed = speed;
        planeHitBool = false;
    }

    void Start()
    {
        _transform = GetComponent<Transform>();
        initialY = _transform.position.y;
    }

    void Update()
    {
        _transform.position += Vector3.left * planeSpeed * Time.deltaTime;
        float x = _transform.position.x;
        float y = Mathf.Sin(x * planeSinFrequency) * planeSinAmplitude + initialY;
        _transform.position = new Vector3(x, y, 0);
        // if (_transform.position.x < -12)
        // {
        //     SendMessageUpwards("ResetPlane", gameObject);
        // }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Fireball") || collider.gameObject.CompareTag("ResetTag"))
        {
            planeHitBool = true;

            

            // Notify the PlaneManager that this plane needs to be reset.
            gameObject.SetActive(false);
        }
    }
}
