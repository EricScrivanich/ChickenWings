using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float initialY;
    private float initialX;

    public bool startAtTop;
    public bool startAtBottom;

    public float startingFallSpot; // -1 for below flap position, 1 for above, 0 for initial position

    private Vector2 addForce;
    private bool addedForce = false;
    public float speed;
    public float yForce;
    public float distanceToFlap;

    // The height the pig would reach after applying yForce

    private static readonly int FlapTriggerHash = Animator.StringToHash("Flap");

    [Header("Positions")]
    [SerializeField] private Transform wingPosition;
    [SerializeField] private Transform legPosition;
    [SerializeField] private Transform tailPosition;

    [Header("Objects")]
    [SerializeField] private Transform Body;
    [SerializeField] private Transform wingObject;
    [SerializeField] private Transform legObject;
    [SerializeField] private Transform tailObject;


    [Header("NEW Variables")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private int predictionSteps;

    [SerializeField] float _sineFrequency = 5.0f;
    [SerializeField] float _sineMagnitude = 2.5f;

    Vector3 _axis;
    Vector3 _direction;
    Vector3 _position;

    [SerializeField] private float predictionTime;
    [SerializeField] private float gameTime;

    private float currentYVelocity = 0f;  // Tracks the current Y velocity

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }

        // Calculate the maximum height the pig would reach after applying yForce

    }

    private void OnValidate()
    {
        // Time.timeScale = gameTime;
        // DrawTrajectory();
    }

    public void InitializeObject()
    {

        // Any additional initialization logic can go here
    }
    private float gravity;

    void Start()
    {
        // Set initial position
        initialY = transform.position.y;
        initialX = transform.position.x;
        // DrawTrajectory();

        // Define the initial velocity
        // addForce = new Vector2(-speed, yForce);
        // gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale); // Ensure gravity is positive

        // DrawTrajectory();
        _position = transform.position;
        _axis = transform.up;
        _direction = -transform.right;

    }

    private void DrawTrajectory()
    {
        Vector3[] positions = new Vector3[predictionSteps];

        for (int i = 0; i < predictionSteps; i++)
        {
            float t = (i / (float)predictionSteps) * predictionTime; // Simulated future time

            // Simulate future movement using the same logic
            Vector3 futurePosition = transform.position + _direction * (t * speed);
            futurePosition += _axis * Mathf.Abs(Mathf.Sin(t * _sineFrequency)) * _sineMagnitude;

            positions[i] = futurePosition;
        }

        // Apply positions to LineRenderer
        line.positionCount = predictionSteps;
        line.SetPositions(positions);
    }


    private void OnEnable()
    {
        Body.localScale = new Vector3(1 + ((.9f - Mathf.Abs(transform.localScale.x)) * .7f), 1, 1);
        wingObject.position = wingPosition.position;
        legObject.position = legPosition.position;
        tailObject.position = tailPosition.position;

        initialY = transform.position.y;
        initialX = transform.position.x;

        // Determine initial position based on startingFallSpot
        if (startingFallSpot > 0)
        {
            float maxHeightFromYForce = ((yForce * yForce) / (2 * Mathf.Abs(Physics2D.gravity.y * rb.gravityScale))) * startingFallSpot;
            transform.position = new Vector3(transform.position.x, initialY + maxHeightFromYForce - distanceToFlap, transform.position.z);
        }
        else if (startingFallSpot < 0)
        {
            transform.position = new Vector3(transform.position.x, initialY - (distanceToFlap * -startingFallSpot), transform.position.z);
        }

        rb.linearVelocity = new Vector2(-speed, 0);
        addForce = new Vector2(-speed, yForce);
    }

    public float GetInitialY()
    {
        return initialY;
    }

    void Update()
    {
        // return;
        // if (speed > 0)
        // {
        //     if (transform.position.x < BoundariesManager.leftBoundary)
        //     {
        //         gameObject.SetActive(false);
        //     }
        // }
        // else
        // {
        //     if (transform.position.x > BoundariesManager.rightBoundary)
        //     {
        //         gameObject.SetActive(false);
        //     }
        // }

        // if (transform.position.y < (initialY - distanceToFlap))
        if (transform.position.y < (initialY))
        {
            rb.linearVelocity = addForce;

            if (!addedForce)
            {
                anim.SetTrigger(FlapTriggerHash);
                addedForce = true;
            }
        }
        else if (transform.position.y > (initialY - distanceToFlap) && addedForce)
        {
            addedForce = false;
        }
    }

    // private void FixedUpdate()
    // {
    //     // DrawTrajectory();
    //     _position += _direction * Time.deltaTime * speed;
    //     // Time.time is the time since the start of the game
    //     transform.position = _position + _axis * Mathf.Abs(Mathf.Sin(Time.time * _sineFrequency)) * _sineMagnitude;
    //     // if (useNewVaraibles)
    //     // {
    //     //     CustomMovement();
    //     //     DrawPrediction();
    //     // }

    // }


}

// private void DefaultMovement()
// {
//     float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
//     rb.velocity = new Vector2(-speed, y - transform.position.y);
// }



