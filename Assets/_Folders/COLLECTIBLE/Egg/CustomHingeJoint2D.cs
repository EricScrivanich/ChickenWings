using System;
using UnityEngine;
using System.Collections;

public class CustomHingeJoint2D : MonoBehaviour, ICollectible
{
    public Transform playerPos; // Reference to the playerPos (to attach the object to)
    public Transform chicPos;
    public PlayerID player;
    private SpriteRenderer sr;

    [SerializeField] private ParticleSystem cageParticles;

    [SerializeField] private PlayerParts deathParticles;

    [SerializeField] private AnimationDataSO animDataSO;

    [SerializeField] private bool attatchedToObject;
    [SerializeField] private CageAttatchment attatchedObject;
    private float rotationSpeedAttached = 90f; // Speed at which the object rotates to the target angle when attached


    public BoxCollider2D collectableColl;
    private ForceBoundaries boundaries;

    private static bool hasAttachment;

    [SerializeField] private Rigidbody2D chicRb;
    private bool isAttached = false; // Determines if the object is attached to the playerPos
    private float targetAngle = 0f; // Target angle for predefined rotation
    private readonly float rotationSpeed = 300f; // Speed at which the object rotates to the target angle
    private float rotationSpeedFree = 120f; // Speed at which the object rotates to the target angle

    private readonly float freeRotationBaseSpeed = 90f; // Base speed for free rotation
    private readonly float freeRotationMaxSpeed = 200f; // Maximum speed for free rotation
    private readonly float freeRotationAccelerationStep = 10f; //



    public float forceFromPlayerRatio;
    public float addedYForceOnRelease;
    public float releaseCooldown;
    public float releaseCooldownVar;

    private Vector2 addedForceOnRelease;

    private bool justReleased;



    private bool setPositionManually = false;



    private Rigidbody2D rb;

    private Rigidbody2D playerRb;
    private bool rotatingToTarget = false;

    private Vector2 lastPosition;
    private bool justReleasedFromObject = false;
    private LevelChallenges levelChallenges;



    // private void Awake()
    // {
    //     chicRb.transform.parent = null;
    // }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (playerPos == null)
        {
            playerPos = player._transform;
        }

        playerRb = player.playerRB;
    }
    void Start()
    {

        if (ignoreStart) return;

        if (attatchedToObject)
        {
            attatchedObject.SetCageParent(this.transform, out setPositionManually);
            float s = attatchedObject.adjustLerpSpeed;
            if (s > 0) rotationSpeedAttached = s;
            else if (s == -1) ignoreLerp = true; // -1 means no lerp, just instant rotation

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
        chicRb.transform.parent = null;
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


        boundaries = GetComponent<ForceBoundaries>();
        boundaries.Activate(false);
        player.globalEvents.OnEnterNextSectionTrigger += HandleNextSectionTrigger;

        // Ensure Rigidbody2D is set up correctly
        if (rb == null)
        {
            Debug.LogError("CustomHingeJoint2D requires a Rigidbody2D component.");
            return;
        }

        rb.bodyType = isAttached || attatchedToObject ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }
    private bool ignoreStart = false;
    private bool ignoreLerp = false;
    private bool justSpawnedInitial;

    private void HandleNextSectionTrigger(float duration, float centerDuration, bool isClockwise, Transform trans, Vector2 centPos, bool doTween)
    {
        if (!isAttached)
        {
            justReleased = false;
            if (hasAttachment) hasAttachment = false;
            Collected();
        }

    }

    public void Initialize(LevelChallenges levelChallenges, byte currentType)
    {
        this.levelChallenges = levelChallenges;

        switch (currentType)
        {
            case 1:
                // attatchedToObject = true;

                break;
            case 2:
                hasAttachment = false;
                attatchedToObject = false;

                Invoke("Collected", 0.15f);
                break;
            case 3:
                hasAttachment = false;
                attatchedToObject = false;


                HitEffect(true);
                Invoke("Collected", 0.15f);
                break;
        }


    }
    public void SetAttatchedObject(CageAttatchment attatchedObject)
    {
        this.attatchedObject = attatchedObject;
        ignoreStart = true;
        float s = attatchedObject.adjustLerpSpeed;
        if (s > 0) rotationSpeedAttached = s;
        else if (s == -1) ignoreLerp = true; // -1 means no lerp, just instant rotation

        // if (playerPos == null)
        // {
        //     playerPos = GameObject.Find("Player").GetComponent<Transform>();
        // }
        justSpawned = true;


        // playerRb = playerPos.gameObject.GetComponent<Rigidbody2D>();
        boundaries = GetComponent<ForceBoundaries>();
        boundaries.Activate(false);
        chicRb.transform.parent = null;
        justSpawnedInitial = true;
        ignoreCollisions = false;
        rb.bodyType = isAttached || attatchedToObject ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

        attatchedObject.SetCageParent(this.transform, out setPositionManually);

        if (setPositionManually)
        {
            lastPosition = attatchedObject.ReturnPosition();
            // transform.position = lastPosition;
            rb.MovePosition(lastPosition);
            chicRb.MovePosition(chicPos.position);
            chicRb.angularVelocity = 0;
            Debug.LogError("Set Attatched Object: " + attatchedObject.name + " to: " + gameObject.name + " with position: " + lastPosition);
        }
        else
        {
            lastPosition = transform.position;
            chicRb.MovePosition(chicPos.position);
            chicRb.angularVelocity = 0;
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


        Vector2 newPos;

        if (setPositionManually)
        {
            newPos = attatchedObject.ReturnPosition();
            rb.MovePosition(newPos);
        }
        else
            newPos = rb.position;


        Vector2 velocity = (newPos - lastPosition) / Time.fixedDeltaTime;

        if (attatchedObject.pigKilled)
        {
            DetachFromObject(velocity);
            return;
        }
        else if (attatchedObject.pigFinished)
        {
            Explode();
            return;
        }

        // if (fixedFramesForRotation <= 1)
        // {
        //     float s = rotationSpeedAttached * Time.fixedDeltaTime; // you should define this like in ApplyFreeRotation()
        //     float a = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, s);
        //     if (velocity.y == 0)
        //     {
        //         float added = Mathf.Sin(Time.time * sineFreq) * sineAmp; // Adjust the frequency and amplitude as needed
        //         a += added;
        //         // Debug.LogError("Adding sine to angle: " + added);
        //     }


        //     rb.MoveRotation(a);
        //     return;
        // }
        // currentTimeForFixedFrames = 0;

        // fixedFramesForRotation = 0;
        lastPosition = newPos;

        if (justSpawnedInitial)
        {
            justSpawnedInitial = false;
            return;
        }




        // Early exit if there's no movement
        if (velocity == Vector2.zero)
            return;
        float p = Mathf.InverseLerp(0, 9, Mathf.Abs(velocity.x));
        // Get angle based on velocity direction
        targetAngle = Mathf.Atan2(-velocity.x, velocity.y) * Mathf.Rad2Deg * p;



        if (justSpawned)
        {
            justSpawned = false;
            chicRb.MovePositionAndRotation(chicPos.position, targetAngle);

            rb.MoveRotation(targetAngle);
            chicRb.angularVelocity = 0;
            chicRb.linearVelocity = velocity * 1.5f;
            return;
        }

        if (ignoreLerp)
        {
            rb.MoveRotation(targetAngle);
            // chicRb.MovePosition(chicPos.position);
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

        // Debug.LogError("Chic Velocity: " + chicRb.linearVelocity);

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
    private bool ignoreCollisions = false;
    private bool ignoreEnemyCollisions = false;
    private void Explode()
    {
        ignoreCollisions = true;
        StartCoroutine(CageExplode());
        cageParticles.Play();
        AudioManager.instance.PlayCageDestroySound();

    }

    private WaitForSeconds animWait = new WaitForSeconds(0.06f);
    private IEnumerator CageExplode(bool ignore = false)
    {
        if (!hasBeenHit)
        {
            sr.sprite = animDataSO.sprites[1];
            yield return animWait;
        }

        if (!ignore)
        {
            Instantiate(deathParticles.FeatherParticle, chicRb.position, Quaternion.identity);
            Instantiate(deathParticles.SmokeParticle, chicRb.position, Quaternion.identity);
            player.KillPlayer();
            chicRb.gameObject.SetActive(false);
        }

        transform.localScale = Vector3.one * 1.2f;

        for (int i = 2; i < animDataSO.sprites.Length; i++)
        {
            sr.sprite = animDataSO.sprites[i];
            yield return animWait;

        }

        gameObject.SetActive(false);




    }
    public void AttachToPlayer()
    {
        player.globalEvents.SetCageAngle += SetTargetAngle;
        player.globalEvents.OnReleaseCage += DetachFromPlayer;
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

    private bool hasBeenHit = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoreCollisions) return;


        if (!other.CompareTag("Player"))
        {
            if (other.CompareTag("Barn"))
            {
                StartCoroutine(CageExplode(true));
                chicRb.simulated = false;
                chicRb.transform.SetParent(other.transform);
                chicRb.GetComponent<ChicTween>().enabled = true;
                levelChallenges.EditCageType(0);
                AudioManager.instance.PlayScoreSound();
                return;
            }
            else if (other.CompareTag("Floor"))
            {
                if (!isAttached && !attatchedToObject) //&& floorCollision
                {
                    if (hasBeenHit)
                    {
                        Explode();
                        return;
                    }
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x * .5f, 11f);
                    HitEffect();
                    return;


                }
            }

            if (!attatchedToObject && !justReleasedFromObject)
            {
                if (other.GetComponent<IExplodable>() != null)
                {
                    // Debug.LogError("Explodable Detected: " + other.name);
                    other.GetComponent<IExplodable>().Explode(false);
                }
                if (hasBeenHit)
                {
                    Explode();
                    return;
                }
                else
                {
                    HitEffect();
                    return;
                }
            }




            else if (other.gameObject == attatchedObject.gameObject || other.transform.parent.gameObject == attatchedObject.gameObject)
                return;

            if (other.GetComponent<IExplodable>() != null)
            {
                // Debug.LogError("Explodable Detected: " + other.name);
                other.GetComponent<IExplodable>().Explode(false);
                if (hasBeenHit)
                {
                    Explode();
                    return;
                }
                else
                {
                    HitEffect();
                    return;
                }
            }

            //  Explode(); // moved from here

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





    }
    private void HitEffect(bool ignoreSound = false)
    {
        if (ignoreEnemyCollisions) return;

        ignoreEnemyCollisions = true;
        sr.sprite = animDataSO.sprites[1];
        cageParticles.Play();
        player.UiEvents.OnCageHit?.Invoke();
        if (!ignoreSound)
        {
            StartCoroutine(ResetEnemyCollision());
            AudioManager.instance.PlayCageHitSound();
            levelChallenges.EditCageType(3);

        }
        else
        {
            hasBeenHit = true;
        }

    }
    private WaitForSeconds enemyCollisionWait = new WaitForSeconds(1f);
    private IEnumerator ResetEnemyCollision()
    {
        yield return enemyCollisionWait;
        ignoreEnemyCollisions = false;
        hasBeenHit = true;
    }
    public void DetachFromObject(Vector2 vel)
    {
        attatchedToObject = false;
        justReleasedFromObject = true;

        StartCoroutine(CheckReleaseCage(true));
        transform.SetParent(null);
        hasAttachment = false;
        boundaries.Activate(true);
        collectableColl.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = vel * forceFromPlayerRatio;
        rb.AddForce(addedForceOnRelease, ForceMode2D.Impulse);
    }

    public void DetachFromPlayer()
    {
        // chicRb.simulated = false;
        hasAttachment = false;
        boundaries.Activate(true);
        justReleased = true;
        StartCoroutine(CheckReleaseCage(false));
        isAttached = false;

        collectableColl.enabled = true;


        rb.bodyType = RigidbodyType2D.Dynamic; // Set to dynamic when detached
        rb.linearVelocity = playerRb.linearVelocity * forceFromPlayerRatio;
        rb.AddForce(addedForceOnRelease, ForceMode2D.Impulse);

        player.globalEvents.SetCageAngle -= SetTargetAngle;
        player.globalEvents.OnReleaseCage -= DetachFromPlayer;



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
    // private void OnEnable()
    // {





    // }
    private void OnDisable()
    {
        player.globalEvents.SetCageAngle -= SetTargetAngle;
        player.globalEvents.OnReleaseCage -= DetachFromPlayer;
        player.globalEvents.OnEnterNextSectionTrigger -= HandleNextSectionTrigger;

        if (attatchedToObject && !setPositionManually)
        {

            // Instantiate(deathParticles.SmokeParticle, chicRb.position, Quaternion.identity);
            player.KillPlayer();

        }

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