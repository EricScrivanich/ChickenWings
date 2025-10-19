using UnityEngine;
using System.Collections;

public class PogoPig : SpawnedPigBossObject, ISimpleRecordableObject
{
    private Animator anim;
    [SerializeField] private float groundDelay;
    [SerializeField] private float quickGroundDelay;
    [SerializeField] private float yForceSmall;
    [SerializeField] private float yForceBig;
    [SerializeField] private float angularVelocity;

    private bool isGrounded;


    [SerializeField] private float groundCheckOffset; // Distance to check for the ground

    private Vector2 groundSpeed;
    private float gravityScale;
    [SerializeField] private float gravityScaleBig;
    private float linearDamping;
    private Vector2 moveDownOnGroundSpeed;
    [SerializeField] private float yDownSpeedOnGround;

    [SerializeField] private Transform[] pupils;
    [SerializeField] private float eyeRadius = 0.1f; // Radius within which the pupil can move

    [SerializeField] private float angVelRandomMultiplier;
    [SerializeField] private float linearVelRandomMultiplier;

    [SerializeField] private Vector2 minMaxSideQuickBounceDifference; // Min and max side velocity for quick bounce

    private Transform target;

    [SerializeField] private float bigJumpFastCutoff;
    [SerializeField] private float bigJumpSlowCutoff;
    [SerializeField] private Vector2 bigJumpFaseSpeedRange;

    [SerializeField] private float bigBounceChance = .2f;

    private bool doingQuickBounce = false;

    [SerializeField] private Transform arms;
    [SerializeField] private Transform tail;
    [SerializeField] private float armsRotationSpeed; // Speed of the arms rotation
    [SerializeField] private float maxArmAngle;

    [SerializeField] private float bigJumpAimSpeed;
    private bool doingRandomBounce;
    private int quickBounceCount;

    private float armsTargetAngle = 0f;
    private bool doingBigAir = false;

    [SerializeField] private Vector2 startingEndingAddYForce;
    [SerializeField] private float addForceTimeBig;
    [SerializeField] private float addedXForce;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveDownOnGroundSpeed = new Vector2(BoundariesManager.GroundSpeed * .3f, yDownSpeedOnGround);
        gravityScale = rb.gravityScale;
        linearDamping = rb.linearDamping;
        if (GameObject.Find("Player") != null)
            target = GameObject.Find("Player").GetComponent<Transform>();



    }

    void Start()
    {
        bigJumpFastCutoff = BoundariesManager.TopViewBoundary - bigJumpFastCutoff;
        bigJumpSlowCutoff = BoundariesManager.TopViewBoundary - bigJumpSlowCutoff;
        groundCheckOffset = BoundariesManager.GroundPosition + groundCheckOffset;

    }
    public override void OnEnableLogic()
    {
        Ticker.OnTickAction015 += MoveEyesWithTicker;
    }
    private void OnDisable()
    {
        Ticker.OnTickAction015 -= MoveEyesWithTicker;
    }

    void Update()
    {
        arms.rotation = Quaternion.RotateTowards(arms.rotation, Quaternion.Euler(0, 0, armsTargetAngle), armsRotationSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isGrounded) return;

        int switchDir = 1;
        if (target.position.x < transform.position.x)
        {
            switchDir = -1;
        }

        rb.AddForce(Vector2.right * switchDir * addedXForce * Time.fixedDeltaTime, ForceMode2D.Force);


        if (rb.position.y < groundCheckOffset)
        {
            isGrounded = true;
            rb.gravityScale = 0; // Disable gravity to prevent further falling
            rb.linearDamping = 0; // Set linear damping to reduce speed
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0; // Reset angular velocity to prevent spinning

            if (Mathf.Abs(target.position.x - transform.position.x) < 1.5f && transform.position.y < target.position.y - 1)
            {
                // Decide whether to do a quick bounce or a big bounce
                // 30% chance for a big bounce
                doingQuickBounce = false;
            }
            else if (quickBounceCount >= (int)Random.Range(1, 4))
            {
                doingQuickBounce = false;
                doingRandomBounce = true;

            }

            else
            {
                doingQuickBounce = true;
            }
            if (doingQuickBounce)
            {
                anim.SetTrigger("QuickBounce");
                StartCoroutine(QuickBounce());
                quickBounceCount++;
            }
            else
            {
                anim.SetTrigger("Ground");


                StartCoroutine(HitGround());
                quickBounceCount = 0; // Reset quick bounce count for big bounce
            }

            // change to quick bounce 70% of times


        }

    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Floor"))
    //     {
    //         anim.SetTrigger("Ground");
    //         StartCoroutine(HitGround());
    //     }
    // }
    private WaitForFixedUpdate wait = new WaitForFixedUpdate();


    private IEnumerator HitGround()
    {
        float time = 0;

        float randomX = Random.Range(-1f, 1f);
        float startRot = rb.rotation;
        Vector2 direction = DirectionToTarget(); // Calculate the direction to the player
        float ang = GetAngleToTarget(direction, 7f); // Get the angle to the target with a clamp of 30 degrees


        while (time < groundDelay)
        {
            time += Time.fixedDeltaTime;
            rb.MovePosition(rb.position - moveDownOnGroundSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Mathf.Lerp(startRot, ang, time / groundDelay));
            yield return wait;
        }
        // rb.MoveRotation(GetAngleToTarget(15f));
        anim.SetTrigger("Up");
        AudioManager.instance.PlayBoingPogoSound(true);

        // if (Mathf.Abs(target.position.x - transform.position.x) < 2.7f && transform.position.y < target.position.y - 1)
        // {
        //     direction = DirectionToTarget();
        //     if (doingRandomBounce) doingRandomBounce = false; // Reset random bounce state
        // }


        // rb.angularVelocity = angVelRandomMultiplier * -randomX;
        // yield return wait;
        // Reset gravity scale

        // rb.linearDamping = linearDamping; // Reset linear damping
        if (doingRandomBounce)
        {
            rb.linearVelocity = transform.up * yForceBig;
            rb.AddForce(DirectionToTarget() * Vector2.right * 2, ForceMode2D.Impulse);
            doingRandomBounce = false; // Reset random bounce state
        }

        else
        {
            rb.linearVelocity = direction * yForceBig;
            rb.AddForce(DirectionToTarget() * Vector2.right * .8f, ForceMode2D.Impulse);
        }  // Apply upward force

        rb.linearDamping = linearDamping;



        // while (rb.position.y < bigJumpFastCutoff)
        // {
        //     float targetAngle = GetAngleToTarget(40f); // Get the angle to the target with a clamp of 30 degrees
        //     targetAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, bigJumpAimSpeed * Time.fixedDeltaTime);
        //     rb.MoveRotation(targetAngle);
        //     float p = Mathf.InverseLerp(groundCheckOffset, bigJumpFastCutoff, rb.position.y);
        //     float speed = Mathf.Lerp(bigJumpFaseSpeedRange.x, bigJumpFaseSpeedRange.y, p);
        //     rb.linearVelocity = transform.up * speed;

        //     yield return wait;


        // }

        // while (rb.position.y < bigJumpSlowCutoff)
        // {
        //     // Get the angle to the target with a clamp of 30 degrees

        //     rb.MoveRotation(Mathf.MoveTowardsAngle(rb.rotation, 0, bigJumpAimSpeed * Time.fixedDeltaTime));
        //     float p = Mathf.InverseLerp(bigJumpFastCutoff, bigJumpSlowCutoff, rb.position.y);
        //     float speed = Mathf.Lerp(bigJumpFaseSpeedRange.y, .1f, p);
        //     rb.linearVelocityY = speed;
        //     // int switchDir = 1;
        //     // if (target.position.x < transform.position.x)
        //     // {
        //     //     switchDir = -1;
        //     // }
        //     // rb.AddForce(Vector2.right * addedXForce * switchDir * Time.fixedDeltaTime, ForceMode2D.Force); // Apply additional force to the sides


        //     yield return wait;


        // }
        // rb.linearVelocity = Vector2.zero; // Stop the pig's movement when it reaches the top of the jump
        // Apply additional force to the sides based on the target position

        rb.gravityScale = gravityScaleBig;
        // rb.linearVelocityY = -8;
        // Reset angular velocity to prevent spinning
        yield return new WaitForSeconds(0.2f); // Small delay to allow the jump animation to play
        isGrounded = false; // Reset grounded state


    }
    [SerializeField] private Vector2 minMaxRandomYQuickBounce;
    private IEnumerator QuickBounce()
    {
        float time = 0;

        float randomX = target.position.x - transform.position.x;

        if (transform.position.x > BoundariesManager.rightViewBoundary - 1)
        {
            randomX = Mathf.Clamp(randomX, minMaxSideQuickBounceDifference.x, -3);

        }
        else if (transform.position.x < BoundariesManager.leftViewBoundary + 1)
        {
            randomX = Mathf.Clamp(randomX, 3, minMaxSideQuickBounceDifference.y);

        }
        else
        {
            randomX = Mathf.Clamp(randomX, minMaxSideQuickBounceDifference.x, minMaxSideQuickBounceDifference.y);
        }




        float startRot = rb.rotation;
        SetArmsTargetAngle(Vector2.zero); // Set arms target angle based on the random X position


        while (time < quickGroundDelay)
        {
            time += Time.fixedDeltaTime;
            rb.MovePosition(rb.position - moveDownOnGroundSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Mathf.Lerp(startRot, 0, time / quickGroundDelay));
            yield return wait;
        }

        rb.angularVelocity = angVelRandomMultiplier * -randomX;
        AudioManager.instance.PlayBoingPogoSound(false);
        yield return wait;

        rb.gravityScale = gravityScale; // Reset gravity scale

        rb.linearDamping = linearDamping; // Reset linear damping
        Vector2 force = new Vector2(linearVelRandomMultiplier * randomX, Random.Range(minMaxRandomYQuickBounce.x, minMaxRandomYQuickBounce.y));
        rb.linearVelocity = force;
        SetArmsTargetAngle(force); // Set arms target angle based on the random X position
        // Reset angular velocity to prevent spinning
        yield return new WaitForSeconds(0.2f); // Small delay to allow the jump animation to play
        isGrounded = false; // Reset grounded state
    }

    private void MoveEyesWithTicker()
    {

        if (target != null)
        {
            Vector2 direction = DirectionToTarget(); // Calculate the direction to the player
                                                     // Ensure it's 2D




            // Move the pupil within the eye's radius
            foreach (Transform pupil in pupils)
            {
                pupil.localPosition = direction * eyeRadius;
            }
        }

    }

    private Vector2 DirectionToTarget()
    {
        Vector2 direction = target.position - transform.position; // Calculate the direction to the player
                                                                  // Ensure it's 2D
        direction.Normalize(); // Normalize the direction
        return direction;

    }

    private float GetAngleToTarget(Vector2 angle, float clamp)
    {
        // Ensure the angle is normalized
        float ang = Mathf.Atan2(-angle.x, angle.y) * Mathf.Rad2Deg;
        ang = Mathf.Clamp(ang, -clamp, clamp); // Clamp the angle to the maximum arm angle
        return ang;
    }

    public void SetArmsTargetAngle(Vector2 angle)
    {
        // Set the target angle for the arms to rotate towards
        // This can be used to make the arms follow the player or a specific direction
        // The angle should be in degrees
        angle.Normalize(); // Ensure the angle is normalized
        float ang = Mathf.Atan2(-angle.x, angle.y) * Mathf.Rad2Deg;
        Debug.Log("Arms target angle: " + ang);
        armsTargetAngle = Mathf.Clamp(ang, -maxArmAngle, maxArmAngle); // Set the target angle for the arms
        tail.rotation = Quaternion.Euler(0, 0, armsTargetAngle); // Rotate the tail to match the arms angle
        // ang += 360;
        // armsTargetAngle = Mathf.Clamp(ang, )





    }

    public void ApplyTypeData(ushort type)
    {

    }

    // Update is called once per frame

}
