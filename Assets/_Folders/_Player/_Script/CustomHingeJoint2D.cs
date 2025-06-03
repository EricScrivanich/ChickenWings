using System;
using UnityEngine;

public class CustomHingeJoint2D : MonoBehaviour, ICollectible
{
    public Transform playerPos; // Reference to the playerPos (to attach the object to)
    public Transform chicPos;
    public PlayerID player;


    public BoxCollider2D collectableColl;
    private ForceBoundaries boundaries;

    private static bool hasAttachment;

    public Rigidbody2D chicRb;
    public bool isAttached = false; // Determines if the object is attached to the playerPos
    public float targetAngle = 0f; // Target angle for predefined rotation
    public float rotationSpeed = 300f; // Speed at which the object rotates to the target angle
    public float rotationSpeedFree = 100f; // Speed at which the object rotates to the target angle

    public float freeRotationBaseSpeed = 50f; // Base speed for free rotation
    public float freeRotationMaxSpeed = 200f; // Maximum speed for free rotation
    public float freeRotationAccelerationStep = 10f; //

    public bool enableFreeRotation = true; // Allows free rotation after reaching the target angle
    public float maxDownSpeed;
    public float maxUpSpeed;
    public float maxLeftSpeed;
    public float maxRightSpeed;
    public float forceFromPlayerRatio;
    public float addedYForceOnRelease;
    public float releaseCooldown;
    public float releaseCooldownVar;

    private Vector2 addedForceOnRelease;

    private bool justReleased;

    private bool resetRotation;
    private bool resetChic;



    private Rigidbody2D rb;

    private Rigidbody2D playerRb;
    private bool rotatingToTarget = false;

    void Start()
    {
        hasAttachment = false;
        if (isAttached)
        {
            collectableColl.enabled = false;
            hasAttachment = true;
        }
        else
        {
            collectableColl.enabled = true;
        }

        rotationSpeedFree = freeRotationBaseSpeed;

        releaseCooldownVar = releaseCooldown;
        addedForceOnRelease = new Vector2(0, addedYForceOnRelease);
        rb = GetComponent<Rigidbody2D>();
        playerRb = playerPos.gameObject.GetComponent<Rigidbody2D>();
        boundaries = GetComponent<ForceBoundaries>();
        boundaries.Activate(false);

        // Ensure Rigidbody2D is set up correctly
        if (rb == null)
        {
            Debug.LogError("CustomHingeJoint2D requires a Rigidbody2D component.");
            return;
        }

        rb.bodyType = isAttached ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }



    private void FixedUpdate()
    {



        if (isAttached)
        {


            if (rotatingToTarget)
            {
                RotateToTargetAngle();
            }
            else if (enableFreeRotation)
            {
                ApplyFreeRotation();
            }
        }

    }

    private void LateUpdate()
    {
        if (isAttached)
            transform.position = playerPos.position;


    }

    void Update()
    {
        if (isAttached)
        {
            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     DetachFromPlayer();
            // }
            // Maintain the attachment



            // if (rotatingToTarget)
            // {
            //     RotateToTargetAngle();
            // }
            // else if (enableFreeRotation)
            // {
            //     ApplyFreeRotation();
            // }
        }

        if (justReleased)
        {
            releaseCooldownVar -= Time.deltaTime;

            if (releaseCooldownVar <= 0)
            {
                releaseCooldownVar = releaseCooldown;
                justReleased = false;
            }
        }


    }

    public void AttachToPlayer()
    {
        player.events.OnCollectCage?.Invoke();
        hasAttachment = true;
        boundaries.Activate(false);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.angularVelocity = 0;
        targetAngle = AngleBasedOnVelocity();
        rotatingToTarget = true;

        transform.position = playerPos.position;
        chicRb.position = chicPos.position;


        // Set to kinematic when attached
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !justReleased && !isAttached && !hasAttachment)
        {
            isAttached = true;
            AttachToPlayer();
            collectableColl.enabled = false;
        }
        if (other.CompareTag("Plane"))
        {
            gameObject.SetActive(false);
        }
    }

    public void DetachFromPlayer()
    {
        // chicRb.simulated = false;
        hasAttachment = false;
        boundaries.Activate(true);
        justReleased = true;
        isAttached = false;
        resetRotation = true;
        collectableColl.enabled = true;


        rb.bodyType = RigidbodyType2D.Dynamic; // Set to dynamic when detached
        rb.linearVelocity = playerRb.linearVelocity * forceFromPlayerRatio;
        rb.AddForce(addedForceOnRelease, ForceMode2D.Impulse);
    }

    public void SetTargetAngle(int angle)
    {
        if (isAttached)
        {
            targetAngle = angle;
            rotatingToTarget = true;
        }

    }

    private void RotateToTargetAngle()
    {

        if (resetRotation)
        {

            // chicRb.simulated = false;

            // transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            // rb.SetRotation(targetAngle);


            // Debug.LogError("resseting rot: " + transform.rotation + " target angle is: " + targetAngle);
            // resetRotation = false;
            // rotatingToTarget = false;

            // chicRb.simulated = true;
            // chicRb.simulated = true;
        }

        float currentAngle = rb.rotation;
        float step = rotationSpeed * Time.fixedDeltaTime;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);

        // transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.SetRotation(angle);



        // Stop rotating once the target angle is reached
        // if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.1f)
        // {
        //     rotatingToTarget = false;
        // }


        if (MathF.Abs(Mathf.Abs(currentAngle) - Mathf.Abs(targetAngle)) < 0.1f)
        {
            rotationSpeedFree = freeRotationBaseSpeed;
            rotatingToTarget = false;
        }

    }
    private float AngleBasedOnVelocity()
    {
        if (playerRb == null)
        {
            Debug.LogError("No Player Rb");
            return 0;
        }

        // Get the direction of the playerPos's velocity
        Vector2 velocityDirection = playerRb.linearVelocity.normalized;

        if (velocityDirection == Vector2.zero)
        {
            Debug.LogError("0 Vel");
            return 0;
        }  // Avoid calculations when there's no velocity

        // Calculate the angle in degrees, where 0 is upward and 180 is downward
        return Mathf.Atan2(-velocityDirection.x, velocityDirection.y) * Mathf.Rad2Deg;


    }

    private void ApplyFreeRotation()
    {

        if (rotationSpeedFree < freeRotationMaxSpeed)
        {
            rotationSpeedFree += freeRotationAccelerationStep * Time.fixedDeltaTime;
            if (rotationSpeedFree >= freeRotationMaxSpeed)
            {
                // Debug.LogError("HIT FREE ROT TARGET: " + UnityEngine.Random.Range(0f, 10f));
                rotationSpeedFree = freeRotationMaxSpeed;
            }
            // Clamp to max speed
        }

        // Get the target angle based on velocity
        float targetAngleFree = AngleBasedOnVelocity();

        // Smoothly rotate the object towards the target angle
        float currentAngle = rb.rotation;
        float step = rotationSpeedFree * Time.fixedDeltaTime;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngleFree, step);

        rb.SetRotation(angle);

        // Apply the rotation
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void OnEnable()
    {

        player.globalEvents.SetCageAngle += SetTargetAngle;
        player.globalEvents.OnReleaseCage += DetachFromPlayer;
    }
    private void OnDisable()
    {
        player.globalEvents.SetCageAngle -= SetTargetAngle;
        player.globalEvents.OnReleaseCage -= DetachFromPlayer;

    }

    public void Collected()
    {
        if (!justReleased && !isAttached && !hasAttachment)
        {

            isAttached = true;
            AttachToPlayer();
            collectableColl.enabled = false;
        }
    }

    // public void Collected()
    // {
    //     if (!justReleased && !isAttached)
    //     {
    //         isAttached = true;
    //         AttachToPlayer();
    //     }

    // }

    // void OnDrawGizmos()
    // {
    //     // Optional: Visualize the target angle in the editor
    //     if (isAttached && rotatingToTarget)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, targetAngle) * Vector3.right * 2);
    //     }
    // }

}