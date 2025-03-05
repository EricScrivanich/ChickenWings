using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPositioningLogic : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerID player;

    // [Header("Distance Settings")]
    // [SerializeField] private float forceMagnitude;
    // [SerializeField] private float preferredDistance = 5f; // Ideal distance from the player
    // [SerializeField] private float approachSpeed = 2f; // Speed at which the enemy adjusts position
    // [SerializeField] private float giveAndTakeFactor = 0.5f; // Determines the lag in maintaining distance (0 = rigid)

    // [Header("Global Boundaries")]
    // [SerializeField] private Vector2 boundaryMin; // Lower-left corner of the global boundary
    // [SerializeField] private Vector2 boundaryMax; // Upper-right corner of the global boundary

    [Header("Rigidbody Settings")]
    [SerializeField] private float forceAdded = 10f; // Force added to the Rigidbody when moving
    [SerializeField] private float maxVel = 5f; // Maximum velocity of the Rigidbody
    [SerializeField] private float lerpSpeed = 3f; // Speed for smoothly transitioning to the target position

    [Header("Prediction Settings")]
    [SerializeField] private float predictionTime = 1f; // Time ahead to predict the player's future position
    [SerializeField] private float inverseMagnitude = 2f; // Magnitude for inverted direction calculations
    [SerializeField] private float chanceOfInversion = 0.2f; // Chance to invert direction (0-1)
    [SerializeField] private float yChangeMin = -3.5f; // Minimum Y value to trigger directional changes
    [SerializeField] private float switchFromTargetToPlayerRange = 0.5f; // Threshold for switching to direct player tracking
    [SerializeField] private Vector2 addedPosition;

    [Header("Dynamic Position Adjustment")]

    [SerializeField] private float yTransitionSpeed = 1f; // Speed of transitionin

    // Internal variables
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    [SerializeField] private Rigidbody2D rbPig;
    private Vector2 targetPosition; // Current target position for the enemy
    private bool isTargetReached; // Tracks whether the target position has been reached
    [SerializeField] private float minX = 2f; // Minimum absolute value for addedPosition.x
    [SerializeField] private float xTransitionSpeed = 2f; // Speed of transitioning the X value
    private bool switchToNegativeX = false; // Controls whether to move to negative X
    private bool switchToPositiveX = false; // Co
    [SerializeField] private float playerX;
    private bool attacking = false;

    private Animator anim;

    [Header("New stuff")]

    private float randomMovementTimer;
    private float randomMovementDelay;
    [SerializeField] private Vector2 randomMovementDelayRange;
    [SerializeField] private Vector2 randomMovementMagnitudeRange;
    private bool waitingForRandomMoveDelay;

    [SerializeField] private float predictionForce;

    private Vector2 trackedPosition;

    [SerializeField] private float moveToPlayerForce;
    [SerializeField] private float moveAwayFromPlayerForce;



    [SerializeField] private Vector2 maxMagnitudes;
    [SerializeField] private float boundaryForce;

    [SerializeField] private float targetRadius;
    [SerializeField] private float playerRadius;

    [SerializeField] private Vector2 normalTargetPlayerRadius;
    [SerializeField] private Vector2 avoidTargetPlayerRadius;

    private bool insideTargetRadius;
    private bool insidePlayerRadius;

    private float trackedTimer;
    private bool trackingTarget = false;

    private bool doMovement = true;

    private Vector2 targetMovePos;
    private bool doMoveToPosition = false;
    private Vector2 playerOffsetTarget;
    private bool waitingForPlayerInput;
    private float predTimeVar;
    private float moveDur;
    private float moveTimer = 0f;

    public System.Action OnMovementTriggered;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        randomMovementDelay = Random.Range(randomMovementDelayRange.x, randomMovementDelayRange.y);
    }

    public void Attack()
    {
        // Debug.Log("CALLEFDFSD");
        attacking = true;
        rb.position = playerTransform.position;
        Invoke("StopAttack", .3f);
    }

    private Vector2 ClampForce(Vector2 force, Vector2 maxMagnitudes)
    {
        return new Vector2(
            Mathf.Clamp(force.x, -maxMagnitudes.x, maxMagnitudes.x), // Clamp X force
            Mathf.Clamp(force.y, -maxMagnitudes.y, maxMagnitudes.y)  // Clamp Y force
        );
    }

    // public void StopAttack()
    // {
    //     attacking = false;
    // }


    private void CheckAndApplyBoundaryForce()
    {
        Vector2 position = rb.position; // Current position of the object

        // Determine if the object is out of bounds
        // bool outOfBoundsX = position.x < minMaxX.x || position.x > minMaxX.y;
        // bool outOfBoundsY = position.y < minMaxY.x || position.y > minMaxY.y;

        Vector2 force = Vector2.zero; // Initialize force vector

        // if (outOfBoundsX) rb.linearVelocityX = 0;
        // if (outOfBoundsY) rb.linearVelocityY = 0;

        // If out of bounds on X, add force away from the boundary
        if (position.x < minMaxX.x)
        {
            force.x = boundaryForce;
            rb.linearVelocityX = 1.2f;
        }
        else if (position.x > minMaxX.y)
        {
            force.x = -boundaryForce; // Add force to the left
            rb.linearVelocityX = -1.2f;

        }

        // If out of bounds on Y, add force away from the boundary
        if (position.y < minMaxY.x)
        {
            rb.linearVelocityY = 1.3f;
            force.y = boundaryForce;

            // force.y = boundaryForce; // Add force upward
        }
        else if (position.y > minMaxY.y)
        {
            rb.linearVelocityY = -1f;

            force.y = -boundaryForce; // Add force downward
        }

        // Apply the calculated force to the Rigidbody
        if (force != Vector2.zero)
        {
            rb.AddForce(force, ForceMode2D.Force);
            // Debug.Log($"Out of bounds! Adding force: {force}");
        }
    }


    private void CheckPlayerRadius(Vector2 pos)
    {


        float distanceToPlayer = Vector2.Distance((Vector2)transform.position, pos);

        // Check if inside the playerRadius
        insidePlayerRadius = distanceToPlayer <= playerRadius;

        // Check if inside the targetRadius
        insideTargetRadius = distanceToPlayer <= targetRadius;

        // Debug for visualization
        if (insidePlayerRadius)
        {
            // Debug.Log("Inside player radius!");
        }

        if (insideTargetRadius)
        {
            // Debug.Log("Inside target radius!");
        }
        // OnDrawGizmos();

    }

    private void PredictPlayerMovments(Vector2 futurePos)
    {
        float distanceToPlayer = Vector2.Distance(transform.position, futurePos);
        Vector2 directionToObject = ((Vector2)transform.position - futurePos).normalized;

        if (distanceToPlayer <= playerRadius)
            rb.AddForce(directionToObject * predictionForce, ForceMode2D.Impulse);
        else if (distanceToPlayer >= targetRadius)
            rb.AddForce(-directionToObject * predictionForce, ForceMode2D.Impulse);





    }
    private void OnDrawGizmos()
    {
        // Ensure we have a playerTransform to work with
        if (playerTransform == null)
            return;

        // Draw the player radius (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(trackedPosition, playerRadius);

        // Draw the target radius (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(trackedPosition, targetRadius);
    }

    private void ApplyForceBasedOnRadius(Vector2 trackedPos)
    {
        // Calculate the direction vector from the player to this object
        Vector2 directionToObject = ((Vector2)transform.position - trackedPos).normalized;



        // Calculate the distance from this object to the player
        // float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // If inside the player radius, add force to move away from the player
        if (insidePlayerRadius)
        {
            Vector2 forceDirection = directionToObject; // Direction away from the player
            rb.AddForce(forceDirection * moveAwayFromPlayerForce, ForceMode2D.Force);
            // Debug.Log("Moving away from the player!");
        }
        // If outside the target radius, add force to move towards the player
        else if (!insideTargetRadius)
        {
            Vector2 forceDirection = -directionToObject; // Direction towards the player
            rb.AddForce(forceDirection * moveToPlayerForce, ForceMode2D.Force);
            // Debug.Log("Moving towards the player!");
        }
    }

    private void RandomIdleMovement()
    {

        float force = Random.Range(randomMovementMagnitudeRange.x, randomMovementMagnitudeRange.y);
        rb.AddForce(GenerateRandomNormalizedVector2() * force, ForceMode2D.Impulse);
        randomMovementDelay = Random.Range(randomMovementDelayRange.x, randomMovementDelayRange.y);
    }

    public void SetDoMovement(bool doMove)
    {
        doMovement = doMove;
        Debug.Log("Setting DO Movement to: " + doMove);
    }

    private Vector2 GenerateRandomNormalizedVector2()
    {
        // Generate a random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Calculate the X and Y components using cosine and sine
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);

        // Return the normalized Vector2 (it's already normalized because sine and cosine return values on the unit circle)
        return new Vector2(x, y);
    }

    [Header("new test setting")]
    [SerializeField] private Vector2 yMoveSpeed;
    [SerializeField] private Vector2 xMoveSpeed;
    [SerializeField] private Vector2 minMaxXNew;
    [SerializeField] private Vector2 minMaxYNew;
    [SerializeField] private Vector2 minMaxYSpeeds;
    [SerializeField] private Vector2 minMaxXSpeeds;

    [SerializeField] private float minDistance;
    [SerializeField] private float moveAwayForce;
    [SerializeField] private Vector2 avoidMinMaxDistance;
    [SerializeField] private Vector2 avoidMinMaxForce;
    [Header("boundary")]

    [SerializeField] private Vector2 minMaxX; // Min and Max X boundary
    [SerializeField] private Vector2 minMaxY; // Min and Max Y boundary

    [SerializeField] private float switchToNewMoveDistance;

    private float curTargetX;
    private float curTargetY;
    private bool useNewMove;
    private bool avoid;


    private void FollowTarget()
    {
        // Debug.Log("Follwing");
        if (Vector2.Distance(transform.position, playerTransform.position) < minDistance)
        {
            Vector2 directionToObject = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
            rb.AddForce(moveAwayForce * directionToObject);
        }
        else
        {
            if (Mathf.Abs(curTargetX - rb.position.x) < .2f)
            {
                rb.linearVelocityX *= .7f;
            }

            else if (rb.position.x < curTargetX)
            {
                rb.AddForce(xMoveSpeed);

            }
            else
            {
                rb.AddForce(-xMoveSpeed);
            }


            if (Mathf.Abs(curTargetY - rb.position.y) < .2f)
            {
                rb.linearVelocityY *= .5f;
            }
            else if (rb.position.y < curTargetY)
            {
                rb.AddForce(yMoveSpeed);

            }
            else
            {
                rb.AddForce(-yMoveSpeed);
            }

        }




    }

    private void AvoidTarget()
    {



        Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
        float distance = Vector2.Distance((Vector2)playerTransform.position, rb.position);

        // if (distance < avoidMinMaxDistance.x) distance = avoidMinMaxDistance.x;

        float t = Mathf.InverseLerp(avoidMinMaxDistance.y, avoidMinMaxDistance.x, distance);

        // Lerp force so that force is highest when distance is smallest
        float f = Mathf.Lerp(avoidMinMaxForce.x, avoidMinMaxForce.y, t);
        Debug.Log("Avoiding with force: " + -f + " distacne is: " + distance);


        rb.AddForce(-f * dir);




    }

    private void Update()
    {




    }

    public void SetAvoidPlayer(bool doAvoid)
    {
        Debug.Log("Set avoid to: " + doAvoid);
        avoid = doAvoid;

        if (avoid)
        {
            targetRadius = avoidTargetPlayerRadius.x;
            playerRadius = avoidTargetPlayerRadius.y;
        }
        else
        {
            targetRadius = normalTargetPlayerRadius.x;
            playerRadius = normalTargetPlayerRadius.y;
        }

    }
    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x - playerTransform.position.x) < switchToNewMoveDistance)
            useNewMove = true;

        else useNewMove = false;



        if (doMovement)
        {

            if (useNewMove)
            {
                curTargetY = Mathf.Clamp(playerTransform.position.y, minMaxYNew.x, minMaxYNew.y);
                curTargetX = Mathf.Clamp(playerTransform.position.x, minMaxXNew.x, minMaxXNew.y);
                if (avoid) AvoidTarget();

                else FollowTarget();


                CheckAndApplyBoundaryForce();
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, minMaxXSpeeds.x, minMaxXSpeeds.y), Mathf.Clamp(rb.linearVelocity.y, minMaxYSpeeds.x, minMaxYSpeeds.y));


            }
            else
            {

                trackedTimer += Time.fixedDeltaTime;
                // ApplyForceBasedOnRadius(targetPosition);
                CheckPlayerRadius(playerTransform.position);
                ApplyForceBasedOnRadius(playerTransform.position);
                CheckAndApplyBoundaryForce();
                rb.linearVelocity = ClampForce(rb.linearVelocity, maxMagnitudes);
            }


            // if (trackingTarget)
            // {

            //     ApplyForceBasedOnRadius(targetPosition);

            //     if (trackedTimer > predictionTime)
            //     {
            //         trackingTarget = false;
            //         trackedTimer = 0;
            //     }

            // }
        }
        //     else
        //     {

        //         if (trackedTimer > .1f)
        //         {
        //             CheckPlayerRadius(playerTransform.position);
        //             trackedPosition = playerTransform.position;
        //             trackedTimer = 0;
        //         }
        //         ApplyForceBasedOnRadius(playerTransform.position);


        //     }
        // }
        // else if (doMoveToPosition)
        // {
        //     moveTimer += Time.fixedDeltaTime;

        //     // Calculate the interpolation factor (clamped between 0 and 1)
        //     float t = Mathf.Clamp01(moveTimer / moveDur);

        //     // Interpolate the Rigidbody2D's position toward the target position
        //     rb.position = Vector2.Lerp(rb.position, (Vector2)playerTransform.position + playerOffsetTarget, t);

        //     // Stop moving once the target is reached
        //     if (t >= 1f)
        //     {


        //         OnMovementTriggered?.Invoke();
        //         doMoveToPosition = false; // Stop the movement
        //         moveTimer = 0f; // Reset the timer for the next move
        //         rb.position = (Vector2)playerTransform.position + playerOffsetTarget;
        //     }

        //     // move the rb position 

        // }


        // randomMovementTimer += Time.fixedDeltaTime;

        // if (randomMovementTimer >= randomMovementDelay)
        // {
        //     RandomIdleMovement();
        //     randomMovementTimer = 0;
        // }


        // if (playerTransform.position.x > playerX && addedPosition.x > 0 && !switchToNegativeX)
        // {
        //     switchToNegativeX = true;
        //     switchToPositiveX = false;
        // }
        // else if (playerTransform.position.x < -playerX && addedPosition.x < 0 && !switchToPositiveX)
        // {
        //     switchToPositiveX = true;
        //     switchToNegativeX = false;
        // }
        // if (switchToNegativeX)
        // {
        //     addedPosition.x = Mathf.MoveTowards(addedPosition.x, -minX, xTransitionSpeed * Time.fixedDeltaTime);
        // }
        // else if (switchToPositiveX)
        // {
        //     addedPosition.x = Mathf.MoveTowards(addedPosition.x, minX, xTransitionSpeed * Time.fixedDeltaTime);
        // }

        // // Transition X position

        // Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;

        // rbPig.AddForce(direction * forceMagnitude);
        // if (playerTransform == null) return;


    }

    private void Test()
    {
        if (!attacking)

            rb.position = (Vector2)playerTransform.position + addedPosition;

    }


    private void MoveTowardsTarget(Vector2 target)
    {

        Vector2 direction = (target - rb.position).normalized;
        rb.AddForce(direction * forceAdded);
        if (rb.linearVelocity.magnitude > maxVel)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVel;
        }

        // UpdateLineRenderer(target);

    }

    public void HandleMovement(bool on)
    {
        doMovement = on;

    }



    public void MoveToPlayerPosition(Vector2 playerOffset, float dur, float predTime, bool waitForPlayerInput)
    {
        waitingForPlayerInput = waitForPlayerInput;
        playerOffsetTarget = playerOffset;
        predTimeVar = predTime;
        moveDur = dur;

        if (!waitForPlayerInput) doMoveToPosition = true;


    }

    private void CheckIfTargetReached()
    {
        // Check proximity to the target to determine if it has been reached
        if ((targetPosition - rb.position).magnitude < switchFromTargetToPlayerRange)  // Threshold for reaching the target
        {
            isTargetReached = true;
        }
    }
    private void UpdateTargetPosition(Vector2 playerVelocity, float playerGravityScale)
    {
        // if (!isTargetReached) return;
        float t = predictionTime;

        if (waitingForPlayerInput) t = predTimeVar;

        // Calculate the future position of the player based on current velocity and gravity
        Vector2 futurePosition = (Vector2)playerTransform.position + playerVelocity * t;
        futurePosition += Vector2.up * Physics2D.gravity.y * playerGravityScale * Mathf.Pow(t, 2) / 2;

        if (waitingForPlayerInput)
        {

            targetMovePos = futurePosition + playerOffsetTarget;
            waitingForPlayerInput = false;
            doMoveToPosition = true;
            return;

        }
        // if (futurePosition.y < -3.4f)
        //     futurePosition = new Vector2(futurePosition.x, playerTransform.position.y + 2.5f);
        Vector2 force = Vector2.zero;
        if (Vector2.Distance(transform.position, futurePosition) < Vector2.Distance(transform.position, playerTransform.position))
        {
            if (transform.position.x < playerTransform.position.x)
                force.x = -1; // Move left
            else
                force.x = 1;  // Move right

            // Scale the X force: more force when closer to the player
            float distanceToPlayer = Mathf.Abs(transform.position.x - futurePosition.x);
            float scaledForce = Mathf.Lerp(8f, 2f, distanceToPlayer / 6.5f); // Clamp at a max distance of 5 units
            force.x *= scaledForce; // Apply the scaled force\
            Debug.Log("Distance is: " + distanceToPlayer + " x force is: " + force.x);

        }

        force.y = -1.5f * transform.position.y;

        rb.AddForce(force, ForceMode2D.Impulse);

        // targetPosition = futurePosition;
        // trackingTarget = true;
        // trackedTimer = 0;

        // isTargetReached = false;  // Reset the target reached flag
        // PredictPlayerMovments(futurePosition);
    }

    // private void OnDrawGizmosSelected()
    // {
    //     // Draw global boundaries
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(new Vector3(boundaryMin.x, boundaryMin.y, 0), new Vector3(boundaryMax.x, boundaryMin.y, 0));
    //     Gizmos.DrawLine(new Vector3(boundaryMin.x, boundaryMax.y, 0), new Vector3(boundaryMax.x, boundaryMax.y, 0));
    //     Gizmos.DrawLine(new Vector3(boundaryMin.x, boundaryMin.y, 0), new Vector3(boundaryMin.x, boundaryMax.y, 0));
    //     Gizmos.DrawLine(new Vector3(boundaryMax.x, boundaryMin.y, 0), new Vector3(boundaryMax.x, boundaryMax.y, 0));

    //     // Draw preferred distance circle
    //     if (playerTransform != null)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireSphere(playerTransform.position, preferredDistance);
    //     }
    // }

    private void OnEnable()
    {
        // player.globalEvents.OnPlayerVelocityChange += UpdateTargetPosition;
        // Ticker.OnTickAction015 += CheckPlayerRadius;
    }
    private void OnDisable()
    {
        // player.globalEvents.OnPlayerVelocityChange -= UpdateTargetPosition;
        // Ticker.OnTickAction015 -= CheckPlayerRadius;


    }
}