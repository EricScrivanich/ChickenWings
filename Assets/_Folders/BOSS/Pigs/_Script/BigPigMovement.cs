using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float initialY;

    public bool startAtTop;
    public bool startAtBottom;

    public float startingFallSpot; // -1 for below flap position, 1 for above, 0 for initial position

    private Vector2 addForce;
    private bool addedForce = false;
    public float speed;
    public float yForce;
    public float distanceToFlap;

    // The height the pig would reach after applying yForce

    private static readonly int FlapTriggerHash = Animator.StringToHash("FlapTrigger");

    [Header("Positions")]
    [SerializeField] private Transform wingPosition;
    [SerializeField] private Transform legPosition;
    [SerializeField] private Transform tailPosition;

    [Header("Objects")]
    [SerializeField] private Transform Body;
    [SerializeField] private Transform wingObject;
    [SerializeField] private Transform legObject;
    [SerializeField] private Transform tailObject;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Calculate the maximum height the pig would reach after applying yForce

    }

    public void InitializeObject()
    {
        // Any additional initialization logic can go here
    }

    private void OnEnable()
    {
        Body.localScale = new Vector3(1 + ((.9f - Mathf.Abs(transform.localScale.x)) * .7f), 1, 1);
        wingObject.position = wingPosition.position;
        legObject.position = legPosition.position;
        tailObject.position = tailPosition.position;

        initialY = transform.position.y;

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

        rb.velocity = new Vector2(-speed, 0);
        addForce = new Vector2(-speed, yForce);
    }

    void Update()
    {
        if (speed > 0)
        {
            if (transform.position.x < BoundariesManager.leftBoundary)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (transform.position.x > BoundariesManager.rightBoundary)
            {
                gameObject.SetActive(false);
            }
        }

        if (transform.position.y < (initialY - distanceToFlap))
        {
            rb.velocity = addForce;

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
}