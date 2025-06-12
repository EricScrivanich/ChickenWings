using System;
using UnityEngine;
using System.Collections;

public class CustomHingeJoint2D : MonoBehaviour, ICollectible
{
    public Transform playerPos; // Reference to the playerPos (to attach the object to)
    public Transform chicPos;
    public PlayerID player;

    [SerializeField] private bool attatchedToObject;
    [SerializeField] private CageAttatchment attatchedObject;
    public float rotationSpeedAttached = 300f; // Speed at which the object rotates to the target angle when attached


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
    private int fixedFramesForRotation = 1;
    private float currentTimeForFixedFrames = 0;
    private bool setPositionManually = false;



    private Rigidbody2D rb;

    private Rigidbody2D playerRb;
    private bool rotatingToTarget = false;

    private Vector2 lastPosition;
    private bool justReleasedFromObject = false;

    // private void Awake()
    // {
    //     chicRb.transform.parent = null;
    // }
    void Start()
    {
        if (ignoreStart) return;

        if (attatchedToObject)
        {
            attatchedObject.SetCageParent(this.transform, out setPositionManually);

            if (setPositionManually)
            {
                lastPosition = attatchedObject.ReturnPosition();
                rb.MovePosition(lastPosition);
                chicRb.MovePosition(chicPos.position);
            }
            else
            {
                lastPosition = transform.position;
                // chicRb.MovePosition(chicPos.position);
            }


        }

        justSpawned = true;

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

        rb.bodyType = isAttached || attatchedToObject ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }
    private bool ignoreStart = false;
    public void SetAttatchedObject(CageAttatchment attatchedObject)
    {
        this.attatchedObject = attatchedObject;
        ignoreStart = true;

        if (playerPos == null)
        {
            playerPos = GameObject.Find("Player").GetComponent<Transform>();
        }
        justSpawned = true;

        rb = GetComponent<Rigidbody2D>();
        playerRb = playerPos.gameObject.GetComponent<Rigidbody2D>();
        boundaries = GetComponent<ForceBoundaries>();
        boundaries.Activate(false);

        rb.bodyType = isAttached || attatchedToObject ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

        attatchedObject.SetCageParent(this.transform, out setPositionManually);

        if (setPositionManually)
        {
            lastPosition = attatchedObject.ReturnPosition();
            // transform.position = lastPosition;
            rb.MovePosition(lastPosition);
            chicRb.MovePosition(chicPos.position);
        }
        else
        {
            lastPosition = transform.position;
            // chicRb.MovePosition(chicPos.position);
        }

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


        // Ensure Rigidbody2D is set up correctly



        // Debug.LogError("Set Attatched Object: " + attatchedObject.name + " to: " + gameObject.name);

    }
    private float attachedToEnnemyTime;
    private float checkAttachedToEnnemyTime = 0.1f;

    [SerializeField] private float sineFreq;
    [SerializeField] private float sineAmp;

    private void AngleBasedOnAttatchedEnemyVelocity()
    {
        // fixedFramesForRotation++;
        // currentTimeForFixedFrames += Time.fixedDeltaTime;
        // if (currentTimeForFixedFrames <= 1)
        // {
        //     return;
        // }

        Vector2 newPos;

        if (setPositionManually)
        {
            newPos = attatchedObject.ReturnPosition();
            rb.MovePosition(newPos);
        }
        else
            newPos = rb.position;

        // Get the current position of the attached object
        // Vector2 newPos = attatchedObject.ReturnPosition();

        // Calculate manual velocity (based on change in position)
        Vector2 velocity = (newPos - lastPosition) / Time.fixedDeltaTime;
        lastPosition = newPos; // Update last position for next frame
        // Vector2 velocity = (newPos - lastPosition) / currentTimeForFixedFrames;

        // Store current position for next frame


        // Move this object to the new position

        // Debug.Log("Cage moved");
        // return;
        // if (attatchedObject.useRotation)
        // {
        //     rb.MoveRotation(attatchedObject.ReturnRotation());
        //     if (justSpawned)
        //     {
        //         justSpawned = false;

        //         chicRb.MovePosition(chicPos.position);
        //     }
        //     return;

        // }


        // Early exit if there's no movement
        if (velocity == Vector2.zero)
            return;
        float p = Mathf.InverseLerp(0, 9, Mathf.Abs(velocity.x));
        // Get angle based on velocity direction
        float targetAngle = Mathf.Atan2(-velocity.x, velocity.y) * Mathf.Rad2Deg * p;

        if (justSpawned)
        {
            justSpawned = false;
            rb.MoveRotation(targetAngle);
            chicRb.MovePosition(chicPos.position);
            return;
        }
        // if (velocity.y == 0)
        // {
        //     float added = Mathf.Sin(Time.time * sineFreq) * sineAmp; // Adjust the frequency and amplitude as needed
        //     targetAngle += added;
        //     Debug.LogError("Adding sine to angle: " + added);
        // }
        // Smooth rotation toward target

        float step = rotationSpeedAttached * Time.fixedDeltaTime; // you should define this like in ApplyFreeRotation()
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, step);
        if (velocity.y == 0)
        {
            float added = Mathf.Sin(Time.time * sineFreq) * sineAmp; // Adjust the frequency and amplitude as needed
            angle += added;
            // Debug.LogError("Adding sine to angle: " + added);
        }




        rb.MoveRotation(angle);
    }


    private void FixedUpdate()
    {



        if (isAttached)
        {
            rb.MovePosition(playerPos.position);

            if (rotatingToTarget)
            {
                RotateToTargetAngle();
            }
            else
            {
                ApplyFreeRotation();
            }
        }
        else if (attatchedToObject)
        {
            AngleBasedOnAttatchedEnemyVelocity();
        }

    }


    // private void LateUpdate()
    // {
    //     if (isAttached)



    // }

    // void Update()
    // {

    //     if (justReleased)
    //     {
    //         releaseCooldownVar -= Time.deltaTime;

    //         if (releaseCooldownVar <= 0)
    //         {
    //             releaseCooldownVar = releaseCooldown;
    //             justReleased = false;
    //         }
    //     }


    // }
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
    private IEnumerator CheckReleaseCage(bool forEnemy)
    {
        yield return waitForSeconds;

        if (!forEnemy)
        {
            justReleased = false;
        }
        else
            justReleasedFromObject = false;



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

        rb.MovePosition(playerPos.position);
        chicRb.position = chicPos.position;


        // Set to kinematic when attached
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Player"))
        {
            if (!attatchedToObject && !justReleasedFromObject)
            {
                if (other.GetComponent<IExplodable>() != null)
                {
                    // Debug.LogError("Explodable Detected: " + other.name);
                    other.GetComponent<IExplodable>().Explode(false);
                }
                gameObject.SetActive(false);
            }




            else if (other.gameObject == attatchedObject.gameObject || other.transform.parent.gameObject == attatchedObject.gameObject)
                return;

            if (other.GetComponent<IExplodable>() != null)
            {
                // Debug.LogError("Explodable Detected: " + other.name);
                other.GetComponent<IExplodable>().Explode(false);
            }

            gameObject.SetActive(false);

        }
        else if (!justReleased && !isAttached && !hasAttachment)
        {

            if (attatchedToObject)
            {
                justReleasedFromObject = true;
                attatchedToObject = false;
                StartCoroutine(CheckReleaseCage(true));
                transform.SetParent(null);
            }
            isAttached = true;
            AttachToPlayer();


            collectableColl.enabled = false;

        }

        // if (other.CompareTag("Player") && !justReleased && !isAttached && !hasAttachment)
        // {
        //     if (attatchedToObject)
        //     {
        //         justReleasedFromObject = true;
        //         attatchedToObject = false;
        //         StartCoroutine(CheckReleaseCage(true));
        //         transform.SetParent(null);
        //     }
        //     isAttached = true;
        //     AttachToPlayer();


        //     collectableColl.enabled = false;
        // }
        // else if (other.CompareTag("Plane"))

        // {
        //     if (!attatchedToObject && !justReleasedFromObject)

        //         gameObject.SetActive(false);


        //     else if (other.gameObject == attatchedObject.gameObject || other.transform.parent.gameObject == attatchedObject.gameObject)
        //         return;

        //     gameObject.SetActive(false);

        // }
    }

    public void DetachFromPlayer()
    {
        // chicRb.simulated = false;
        hasAttachment = false;
        boundaries.Activate(true);
        justReleased = true;
        StartCoroutine(CheckReleaseCage(false));
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

        // if (resetRotation)
        // {

        //     // chicRb.simulated = false;

        //     // transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        //     // rb.SetRotation(targetAngle);


        //     // Debug.LogError("resseting rot: " + transform.rotation + " target angle is: " + targetAngle);
        //     // resetRotation = false;
        //     // rotatingToTarget = false;

        //     // chicRb.simulated = true;
        //     // chicRb.simulated = true;
        // }


        float step = rotationSpeed * Time.fixedDeltaTime;
        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, step);



        rb.MoveRotation(angle);





        if (Mathf.Abs(angle - targetAngle) < 0.5f)
        {
            rotationSpeedFree = freeRotationBaseSpeed;
            rotatingToTarget = false;
        }

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
        // Debug.LogError("Free Rotating: " + angle + " current angle: " + currentAngle + " target angle: " + targetAngleFree);
        rb.MoveRotation(angle);

        // Apply the rotation
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);
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


    private bool justSpawned = true;
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