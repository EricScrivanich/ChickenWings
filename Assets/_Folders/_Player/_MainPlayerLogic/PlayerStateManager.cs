using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private Transform airSpawnPos;
    [SerializeField] private GameObject THETHING;
    private bool USINGTHING = false;
    [SerializeField] private float SlowFactor;
    [ExposedScriptableObject]
    public PlayerID ID;
    [SerializeField] private GameObject jumpAirPrefab;
    private Queue<GameObject> jumpAir;

    [ExposedScriptableObject]
    public PlayerMovementData MovementData;
    public CameraID Cam;
    public ParticleSystem Dust;

    [SerializeField] private float initialConstantForce;
    public float currentForce;
    [SerializeField] private bool isTutorial;

    [SerializeField] private Transform airResistance;
    private Vector2 airDashPos = new Vector2(.62f, -.43f);
    private Vector2 airDropPos = new Vector2(.15f, -.9f);
    private Vector3 airDropRot = new Vector3(0, 0, -90);



    [SerializeField] private Vector2 startPoint;

    [SerializeField] private Vector2 endPoint;
    [SerializeField] private float drawSpeed;
    private LineRenderer line;
    public PlayerParts playerParts;
    public Transform parchutePoint;
    public bool holdingFlip;

    public GameObject Sword;
    // private SpriteRenderer SwordSprite;

    [SerializeField] private List<Collider2D> ColliderType;
    public bool isDropping;

    [SerializeField] private bool isSlow;
    public bool justFlipped;
    public GameEvent DeadEvent;
    public float jumpForce;
    private bool isDamaged;
    public float flipLeftForceX;
    public float flipLeftForceY;
    public float flipRightForceX;
    public float flipRightForceY;
    private Queue<ParticleSystem> FeatherParticleQueue;
    private ParticleSystem SmokeParticle;

    private float originalTimeScale;
    public bool isDashing;
    public bool canDashSlash;
    private bool justDashedSlashed;
    public bool stillDashing;


    public Transform ImageTransform;
    private bool CanUseStamina = true;
    public GameObject ParachuteObject;
    private Coroutine fillStaminaCoroutine;
    [SerializeField] private float staminaDeplete;
    [SerializeField] private float staminaFill;
    public GameObject attackObject;
    public bool bucketIsExploded = false;
    public float maxFallSpeed;
    public float originalGravityScale { get; private set; }


    #region States
    PlayerBaseState currentState;
    PlayerBaseState previousState;
    public PlayerStartingState StartingState = new PlayerStartingState();
    public PlayerFlipRightState FlipRightState = new PlayerFlipRightState();
    public PlayerFlipLeftState FlipLeftState = new PlayerFlipLeftState();
    public PlayerJumpState JumpState = new PlayerJumpState();
    public PlayerDashState DashState = new PlayerDashState();
    public PlayerDropState DropState = new PlayerDropState();
    public PlayerBounceState BounceState = new PlayerBounceState();
    public PlayerSlashState SlashState = new PlayerSlashState();

    public PlayerDashSlashState DashSlash = new PlayerDashSlashState();

    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerFrozenState FrozenState = new PlayerFrozenState();
    public PlayerHoldJumpState HoldJumpState = new PlayerHoldJumpState();
    public BucketCollisionState BucketState = new BucketCollisionState();
    public PlayerParachuteState ParachuteState = new PlayerParachuteState();

    #endregion

    #region AnimationHashes
    public readonly int DashTrigger = Animator.StringToHash("DashTrigger");
    public readonly int DropTrigger = Animator.StringToHash("DropTrigger");
    public readonly int BounceTrigger = Animator.StringToHash("BounceTrigger");
    public readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
    public readonly int FlipTrigger = Animator.StringToHash("FlipTrigger");
    public readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
    public readonly int JumpHoldTrigger = Animator.StringToHash("JumpHoldTrigger");
    public readonly int ParachuteTrigger = Animator.StringToHash("ParachuteTrigger");
    public readonly int FrozenBool = Animator.StringToHash("FrozenBool");
    public readonly int DashSlashFinishTrigger = Animator.StringToHash("DashSlashFinishTrigger");
    public readonly int DashSlashTrigger = Animator.StringToHash("DashSlashTrigger");
    public readonly int ShootGunTrigger = Animator.StringToHash("ShootGunTrigger");
    public readonly int GetGunBool = Animator.StringToHash("GetGunBool");
    public readonly int ReloadGunTrigger = Animator.StringToHash("ReloadGunTrigger");
    public readonly int FinishDashTrigger = Animator.StringToHash("FinishDashTrigger");


    #endregion
    public Transform bucket;
    private bool isAttacking = false;

    public bool justFlippedRight;
    public bool justFlippedLeft;

    private readonly int jumpAirAmount = 7;
    private int currentJumpAirIndex;
    public int CurrentJumpAirIndex
    {
        get
        {
            currentJumpAirIndex++;

            if (currentJumpAirIndex > jumpAirAmount)
            {
                currentJumpAirIndex = 1;
            }
            Debug.Log(currentJumpAirIndex);
            return currentJumpAirIndex;

        }
        set
        {
            currentJumpAirIndex = value;
        }


    }







    public bool hasFlippedRight = false;
    public bool hasFlippedLeft = false;




    private bool canDash;
    public bool canDrop;
    private bool canSlash;
    private float dashCooldownTime = 1.5f;
    private float dropCooldownTime = 2f;
    public bool jumpHeld;
    public bool isParachuting = false;
    public bool isTryingToParachute = false;


    public int rotationLerpSpeed = 20;
    public int jumpRotSpeed = 200;
    private int frozenRotSpeed = 350;
    public int maxRotUp = 15;
    private int maxRotDown = -30;
    private float rotZ;
    private float frozenRotZ;
    private float totalRotation = 0.0f; // Total rotation done so far
    private float targetRotation = 0.0f;
    private float currentRotation = 0;
    private int remainingLeftFlips = 0;
    private int remainingRightFlips = 0;
    public float addEggVelocity { get; private set; } = 2.5f;
    public bool disableButtons;
    public bool rotateSlash;

    public bool justStartedClocker;

    private Vector2 originalPosition;




    public Rigidbody2D rb;
    public Animator anim { get; private set; }
    // 

    private void Awake()
    {
        CurrentJumpAirIndex = 0;
        ID.constantPlayerForce = initialConstantForce;
        if (initialConstantForce == 0)
        {
            ID.constantPlayerForceBool = false;
        }
        else
        {
            ID.constantPlayerForceBool = true;

        }
        ID.isTutorial = isTutorial;
        stillDashing = false;

        ID.ResetValues();
        holdingFlip = false;
        ID.UsingClocker = false;
        rotateSlash = false;
        isDropping = false;
        isDamaged = false;
        jumpHeld = false;
        canDash = true;
        canDrop = true;
        disableButtons = true;
        ID.StaminaUsed = 0;
        rb = GetComponent<Rigidbody2D>();
        justDashedSlashed = false;

        FlipRightState.CachVariables(MovementData.FlipRightInitialForceVector, MovementData.FlipRightAddForce,
        MovementData.FlipRightDownForce, MovementData.flipRightAddForceTime, MovementData.flipRightDownForceTime);

        FlipLeftState.CachVariables(MovementData.FlipLeftInitialForceVector, MovementData.FlipLeftAddForce,
        MovementData.FlipLeftDownForce, MovementData.flipLeftAddForceTime, MovementData.flipLeftDownForceTime);


    }
    // Start is called before the first frame update
    void Start()
    {
        jumpAir = new Queue<GameObject>();

        for (int i = 0; i < jumpAirAmount; i++)
        {
            var obj = Instantiate(jumpAirPrefab);
            obj.GetComponent<JumpAirBehavior>().SetIndex(CurrentJumpAirIndex);

            obj.SetActive(false);
            jumpAir.Enqueue(obj);

        }

        currentJumpAirIndex = 0;

        originalPosition = transform.position;

        originalTimeScale = Time.timeScale;


        ChangeCollider(0);

        line = GetComponent<LineRenderer>();
        // Feathers.SetActive(false);
        if (!ID.testingNewGravity)
        {
            ID.MaxFallSpeed = -9.7f;
            jumpForce = 12f;
            flipLeftForceX = -5f;
            flipLeftForceY = 10f;
            flipRightForceX = 2.5f;
            flipRightForceY = 12f;

            originalGravityScale = 2.3f;
        }
        else
        {
            ID.MaxFallSpeed = -9.7f;
            jumpForce = ID.playerJumpForce;


            originalGravityScale = rb.gravityScale;

        }
        // SwordSprite = Sword.GetComponent<SpriteRenderer>();

        maxFallSpeed = ID.MaxFallSpeed;

        FeatherParticleQueue = new Queue<ParticleSystem>();
        for (int i = 0; i < 2; i++)
        {
            var feather = Instantiate(playerParts.FeatherParticle);
            FeatherParticleQueue.Enqueue(feather);
        }
        SmokeParticle = Instantiate(playerParts.SmokeParticle);
        line = GetComponent<LineRenderer>();




        attackObject.SetActive(false);
        // originalGravityScale = rb.gravityScale;

        anim = GetComponent<Animator>();

        currentState = StartingState;

        currentState.EnterState(this);


    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     currentState.OnCollisionEnter2D(this, collision);

    // }\

    public void USETHING()
    {

        THETHING.SetActive(!USINGTHING);
        USINGTHING = !USINGTHING;


    }

    private IEnumerator LerpToOriginalPosition(float duration)
    {
        float timeElapsed = 0;
        Vector2 startPosition = transform.position;
        disableButtons = true;

        while (timeElapsed < duration)
        {
            // Use Mathf.SmoothStep for easing; it smoothly interpolates between the start and end values
            float t = timeElapsed / duration;
            t = Mathf.SmoothStep(0.0f, 1.0f, t);  // Apply smoothing to t
            transform.position = Vector2.Lerp(startPosition, originalPosition, t);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = originalPosition;
        AdjustForce(0, 0);
        disableButtons = false; // Ensure the position is set exactly at the original
    }

    void FixedUpdate()
    {
        // rb.velocity = new Vector2(currentForce, rb.velocity.y);
        MaxFallSpeed();
        if (rb.freezeRotation == false)
        {
            currentState.RotateState(this);

        }

        currentState.FixedUpdateState(this);


    }

    public void BaseRotationLogic()
    {
        // Check if the Rigidbody is moving upwards and hasn't reached the max rotation limit
        if (rb.velocity.y > 0 && rotZ < maxRotUp)
        {
            rotZ += jumpRotSpeed * Time.deltaTime;
        }
        // Check if the Rigidbody is moving downwards and hasn't reached the max rotation limit
        else if (rb.velocity.y <= 0 && rotZ > maxRotDown)
        {
            rotZ -= jumpRotSpeed * Time.deltaTime;
        }

        // Smoothly interpolate the rotation towards the target rotation
        float newRotation = Mathf.LerpAngle(rb.rotation, rotZ, Time.deltaTime * rotationLerpSpeed);

        // Apply the new rotation
        rb.MoveRotation(newRotation);
    }
    private void ChangeConstantForce(float newSpeed)
    {
        float difference = newSpeed - ID.constantPlayerForce;
        ID.constantPlayerForce = newSpeed;
        rb.velocity = new Vector2(rb.velocity.x + difference, rb.velocity.y);
    }

    public void AdjustForce(float xForce, float yForce)
    {
        // currentForce = xForce + ID.constantPlayerForce;

        rb.velocity = new Vector2(xForce + ID.constantPlayerForce, yForce);


    }

    void Update()
    {

        currentState.UpdateState(this);
        // currentState.RotateState(this);
        if (transform.position.y > 7f && !disableButtons && !ID.constantPlayerForceBool)
        {
            // ResetHoldJump();
            anim.SetBool(FrozenBool, true);

            SwitchState(FrozenState);
        }
    }

    public void MaxFallSpeed()
    {

        // If it does, limit it to the max fall speed
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, maxFallSpeed, 15f));



    }



    public void ChangeCollider(int index)
    {
        if (index < 0)
        {
            for (int i = 0; i < ColliderType.Count; i++)
            {
                // If the current index matches the parameter, enable the collider, otherwise disable it.
                ColliderType[i].enabled = false;

            }
            return;

        }
        for (int i = 0; i < ColliderType.Count; i++)
        {
            // If the current index matches the parameter, enable the collider, otherwise disable it.
            ColliderType[i].enabled = (i == index);
        }
    }



    public void SetCanRotateSlash()
    {
        rotateSlash = true;

    }



    public void SwitchState(PlayerBaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;

        if (currentState == FlipLeftState || currentState == FlipRightState)
        {
            justFlipped = true;
        }
        else
        {
            justFlipped = false;


        }
        newState.EnterState(this);



    }





    void HandleJump()
    {
        if (!disableButtons && !ID.IsTwoTouchPoints)
        {

            // ResetHoldJump();
            anim.SetTrigger(JumpTrigger);
            var obj = jumpAir.Dequeue();
            obj.transform.position = airSpawnPos.position;
            obj.transform.eulerAngles = Vector3.zero;
            obj.SetActive(true);
            jumpAir.Enqueue(obj);

            SwitchState(JumpState);
        }

    }
    void HandleHoldJump(bool isHolding)
    {


        if (isHolding == true)
        {
            // anim.SetTrigger("JumpHoldTrigger");
            anim.SetTrigger(JumpHoldTrigger);

        }
        else
        {
            if (currentState == JumpState)
            {
                ID.events.OnStopJumpAir?.Invoke(currentJumpAirIndex);

                if (ID.isHolding)
                {
                    anim.SetTrigger(IdleTrigger);
                }
                // anim.SetTrigger("IdleTrigger");

            }

        }
        ID.isHolding = isHolding;

    }





    void HandleRightFlip()
    {
        if (!disableButtons)
        {
            holdingFlip = true;
            anim.SetTrigger(FlipTrigger);
            var obj = jumpAir.Dequeue();
            obj.transform.position = airSpawnPos.position;

            obj.transform.eulerAngles = new Vector3(0, 0, 315);
            obj.SetActive(true);
            jumpAir.Enqueue(obj);

            // ResetHoldJump();
            SwitchState(FlipRightState);


        }

    }



    void HandleLeftFlip()
    {
        if (!disableButtons)
        {
            holdingFlip = true;

            // ResetHoldJump();
            anim.SetTrigger(FlipTrigger);
            var obj = jumpAir.Dequeue();
            obj.transform.position = airSpawnPos.position;

            obj.transform.eulerAngles = new Vector3(0, 0, 42);
            obj.SetActive(true);
            jumpAir.Enqueue(obj);

            SwitchState(FlipLeftState);
        }

    }

    private void HandleHoldFlip(bool isHolding)
    {
        holdingFlip = isHolding;


    }

    public void SetFlipDirection(bool isRight)
    {
        if (isRight)
        {
            justFlippedLeft = false;
            justFlippedRight = true;
        }
        else
        {
            justFlippedRight = false;
            justFlippedLeft = true;
        }

    }

    void HandleDash(bool holding)
    {
        if (stillDashing && holding && canDashSlash)
        {
            DashState.SwitchSlash();

            justDashedSlashed = true;
        }
        else if (holding && canDash && !disableButtons)
        {
            // ResetHoldJump();
            StartCoroutine(DashCooldown(dashCooldownTime));
            // airResistance.localPosition = airDashPos;
            // airResistance.localEulerAngles = Vector3.zero;
            anim.SetTrigger(DashTrigger);



            SwitchState(DashState);


        }
        if (!holding && !justDashedSlashed)
        {
            ID.globalEvents.CanDash?.Invoke(false);
            isDashing = false;
        }
        else if (!holding && justDashedSlashed)
        {
            justDashedSlashed = false;
        }

    }

    private void HandleDashSlash(bool canSlash)
    {

        canDashSlash = canSlash;
        // if (!disableButtons)
        // {
        //     ChangeCollider(-1);
        //     disableButtons = true;
        //     SwitchState(DashSlash);


        // }
    }

    private void HandleDrop()
    {
        if (!disableButtons && canDrop)
        {
            // ResetHoldJump();
            isDropping = true;
            SwitchState(DropState);
            // airResistance.localPosition = airDropPos;
            // airResistance.localEulerAngles = airDropRot;
            anim.SetTrigger(DropTrigger);

            ID.globalEvents.CanDrop?.Invoke(false);

            StartCoroutine(DropCooldown());
        }

    }




    private void HandleDamaged()
    {

        if (!isDamaged)
        {
            if (isTutorial)
            {
                // StartCoroutine(LerpToOriginalPosition(.8f));
                DamageEffects();



                return;
            }
            isDamaged = true;

            if (!ID.infiniteLives)
            {
                Debug.Log("LoseLife" + ID.infiniteLives);

                ID.Lives--;
                if (ID.Lives <= 0)
                {
                    Die();
                    return;
                }

            }

            else
            {
                ID.globalEvents.OnInfiniteLives?.Invoke();
            }
            DamageEffects();


        }
        else
        {
            return;
        }

    }

    private void HitBoss()
    {
        DashSlash.IgnoreForce(true);

    }

    private void DamageEffects()
    {
        ID.globalEvents.ShakeCamera?.Invoke(.5f, .14f);
        Cam.events.OnShakeCamera?.Invoke(.5f, .14f);

        var Feather = FeatherParticleQueue.Dequeue();
        if (Feather.isPlaying)
        {
            Feather.Stop();
        }
        Feather.transform.position = transform.position;
        Feather.Play();
        AudioManager.instance.PlayDamageSound();

        StartCoroutine(Flash());

        FeatherParticleQueue.Enqueue(Feather);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            SwitchState(IdleState);
            HandleDamaged();
            isDropping = false;
            disableButtons = false;
            foreach (ContactPoint2D pos in collision.contacts)
            {
                Debug.Log(pos.normal);

                if (pos.normal.y > .92f)
                {

                    AdjustForce(1, 9);

                }
                else if (pos.normal.x < -.7f)
                {

                    // rb.velocity = new Vector2(-6.5f, 2);
                    AdjustForce(-6.3f, 2);


                }
                else if (pos.normal.x > .6f)
                {

                    AdjustForce(3, 2);
                }
            }
        }
    }


    private void HandleGroundCollision()
    {


        if (!isDropping)
        {
            HandleDamaged();
            AdjustForce(0, 12.5f);
            SwitchState(IdleState);
            // if (isTutorial)
            // {
            //     HandleDamaged();
            //     return;
            // }
            // Die();
        }
        else
        {


            // CameraShake.instance.ShakeCamera(.2f, .08f);
            anim.SetTrigger(BounceTrigger);

            SwitchState(BounceState);
        }

        // if (isDropping)
        // {
        //     anim.SetTrigger("JumpTrigger");
        //     disableButtons = false;
        //     maxFallSpeed = ID.MaxFallSpeed;
        //     SwitchState(IdleState);

        // }

    }

    private void Die()
    {

        ID.Lives = 0;

        ID.globalEvents.ShakeCamera?.Invoke(.6f, .2f);
        Cam.events.OnShakeCamera?.Invoke(.6f, .2f);
        ID.isAlive = false;
        DeadEvent.TriggerEvent();

        AudioManager.instance.PlayDeathSound();
        for (int i = 0; i < FeatherParticleQueue.Count; i++)
        {
            var Feather = FeatherParticleQueue.Dequeue();
            if (Feather.isPlaying)
            {
                Feather.Stop();
            }
            Feather.transform.position = transform.position;
            SmokeParticle.transform.position = transform.position;
            Feather.Play();
            SmokeParticle.Play();

            gameObject.SetActive(false);


        }
    }



    private IEnumerator Flash()
    {


        for (int i = 0; i < 5; i++)
        {
            // spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Set opacity to 0
            ID.PlayerMaterial.SetFloat("_Alpha", 0);
            yield return new WaitForSeconds(.12f);
            // spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set opacity to 1
            ID.PlayerMaterial.SetFloat("_Alpha", .9f);
            yield return new WaitForSeconds(.12f);
        }
        ID.PlayerMaterial.SetFloat("_Alpha", 1);

        isDamaged = false;
    }






    private void OffScreen()
    {
        SwitchState(IdleState);
        HandleDamaged();
        float targetYVelocity;
        float currentYPosition = transform.position.y;

        if (currentYPosition < 6)
        {
            // Calculate a velocity that increases as the Y position decreases
            targetYVelocity = (6 - currentYPosition) * 2; // Multiplier adjusts the strength of the velocity
        }
        else
        {
            // If the player is at or above Y position of 6, set the velocity to zero
            targetYVelocity = 0;
        }
        if (transform.position.x < 0)
        {
            AdjustForce(5, targetYVelocity);

        }
        else
        {
            AdjustForce(-5, targetYVelocity);

        }


    }

    private IEnumerator DashCooldown(float time)
    {
        canDash = false;

        yield return new WaitForSeconds(time);
        ID.globalEvents.CanDash?.Invoke(true);
        canDash = true;
    }
    IEnumerator DropCooldown()
    {
        canDrop = false;

        yield return new WaitForSeconds(dropCooldownTime);
        ID.globalEvents.CanDrop?.Invoke(true);

        canDrop = true;
    }

    IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(.3f);
        anim.SetBool("JumpHeld", false);
    }

    private void BucketCompletion(Transform position)
    {
        bucket = position;
        // ResetHoldJump();
        // ResetParachute();
        if (isDropping)
        {
            anim.SetTrigger(BounceTrigger);
        }
        SwitchState(BucketState);

    }

    private void BucketExplosion(int index)
    {
        bucketIsExploded = true;

    }





    private void OnEnable()
    {
        ID.events.OnJump += HandleJump;
        // ID.events.OnAttack += HandleClocker;
        ID.events.OnFlipRight += HandleRightFlip;
        ID.events.OnFlipLeft += HandleLeftFlip;
        ID.events.OnDash += HandleDash;
        ID.events.OnDrop += HandleDrop;
        ID.events.OnJumpHeld += HandleHoldJump;
        // ID.events.OnParachute += HandleParachute;
        // ID.events.OnJumpReleased += HandleReleaseJump;
        ID.events.OnCompletedRingSequence += BucketCompletion;
        ID.globalEvents.OnBucketExplosion += BucketExplosion;
        ID.events.LoseLife += HandleDamaged;
        ID.events.HitGround += HandleGroundCollision;
        ID.globalEvents.SetCanDashSlash += HandleDashSlash;
        ID.events.OnHoldFlip += HandleHoldFlip;
        ID.globalEvents.OnAdjustConstantSpeed += ChangeConstantForce;
        ID.globalEvents.OnOffScreen += OffScreen;
        ID.events.HitBoss += HitBoss;

        // ID.events.OnAttack += HandleSlash;
    }
    private void OnDisable()
    {
        ID.events.OnJump -= HandleJump;
        // ID.events.OnAttack -= HandleClocker;

        ID.events.OnFlipRight -= HandleRightFlip;
        ID.events.OnFlipLeft -= HandleLeftFlip;
        ID.events.OnDash -= HandleDash;
        ID.events.OnDrop -= HandleDrop;
        ID.events.OnJumpHeld -= HandleHoldJump;
        // ID.events.OnParachute -= HandleParachute;

        // ID.events.OnJumpReleased -= HandleReleaseJump;
        ID.events.OnCompletedRingSequence -= BucketCompletion;
        ID.globalEvents.OnBucketExplosion -= BucketExplosion;
        ID.events.LoseLife -= HandleDamaged;
        ID.events.HitGround -= HandleGroundCollision;
        ID.globalEvents.SetCanDashSlash -= HandleDashSlash;

        ID.events.OnHoldFlip -= HandleHoldFlip;
        ID.globalEvents.OnAdjustConstantSpeed -= ChangeConstantForce;
        ID.globalEvents.OnOffScreen -= OffScreen;
        ID.events.HitBoss -= HitBoss;




    }

    // private void HandleClocker(bool usingClocker)
    // {
    //     if (usingClocker)
    //     {
    //         ID.UsingClocker = usingClocker;
    //         Time.timeScale = .4f;
    //         anim.SetBool("GetGunBool", true);
    //         StartCoroutine(DrawLine());
    //     }
    //     else
    //     {
    //         Time.timeScale = originalTimeScale;
    //         line.enabled = false;
    //         anim.SetBool("GetGunBool", false);

    //     }

    // }

    // IEnumerator DrawLine()
    // {
    //     line.enabled = true;
    //     float startTime = Time.time;
    //     float distance = Vector3.Distance(startPoint, endPoint);
    //     float fracJourney = 0f;

    //     line.SetPosition(0, startPoint);
    //     line.SetPosition(1, startPoint); // Start with the line collapsed at the start point

    //     while (fracJourney < 1f)
    //     {
    //         float distCovered = (Time.time - startTime) * drawSpeed;
    //         fracJourney = distCovered / distance;
    //         Vector2 currentPoint = Vector2.Lerp(startPoint, endPoint, fracJourney);

    //         line.SetPosition(1, currentPoint); // Update the end point

    //         yield return null;
    //     }

    //     line.SetPosition(1, endPoint); // Ensure the line is fully drawn to the end point
    // }





    // void HandleAttack(bool attacking)
    // {

    //     if (attacking && !disableButtons && ID.numberOfPowersThatCanBeUsed >= 1)
    //     {
    //         ID.numberOfPowersThatCanBeUsed--;
    //         ID.CurrentMana -= ID.ManaNeeded;
    //         ID.globalEvents.UsePower?.Invoke();
    //         // ResetHoldJump();
    //         isAttacking = true;
    //         SwitchState(SlashState);
    //     }
    //     else if (!attacking && isAttacking)
    //     {
    //         // invoked false by featherAnimation, called function on AddDamage()

    //         maxFallSpeed = ID.MaxFallSpeed;
    //         attackObject.SetActive(false);
    //         anim.SetBool("AttackBool", false);
    //         disableButtons = false;
    //         isAttacking = false;
    //         CheckIfIsTryingToParachute();
    //     }
    // }

    // public void CheckIfIsTryingToParachute()
    // {
    //     if (isTryingToParachute)
    //     {
    //         HandleParachute(true);
    //     }
    //     else
    //     {
    //         SwitchState(IdleState);
    //     }

    // }
    // public void HandleParachute(bool isPressing)
    // {
    //     isTryingToParachute = isPressing;

    //     if (isPressing && !disableButtons)
    //     {
    //         if (ID.StaminaUsed < ID.MaxStamina)
    //         {
    //             Debug.Log("Holding chute");

    //             SwitchState(ParachuteState);
    //             isParachuting = true;
    //             ID.globalEvents.OnUseStamina?.Invoke(true);
    //         }
    //         else
    //         {
    //             StartFillStaminaCoroutine();

    //             ID.globalEvents.OnZeroStamina?.Invoke();

    //         }

    //     }
    //     else if (!isPressing && isParachuting)
    //     {
    //         // ResetParachute();
    //         SwitchState(IdleState);
    //     }
    // }


    // public void HandleClocker(bool b)
    // {
    //     if (b)
    //     {
    //         Time.timeScale = 0.2f;
    //         justStartedClocker = true;
    //         anim.SetBool("GetGunBool", true);


    //         ID.UsingClocker = true;
    //         maxFallSpeed = -6.5f;
    //         if (justFlippedRight)
    //         {
    //             FlipRightState.ReEnterState();
    //         }
    //         else{
    //             FlipLeftState.ReEnterState();
    //         }
    //         AudioManager.instance.SlowMotionPitch(true);

    //     }
    //     else
    //     {

    //         Time.timeScale = originalTimeScale;
    //         AudioManager.instance.SlowMotionPitch(false);


    //         ID.UsingClocker = false;
    //         maxFallSpeed = ID.MaxFallSpeed;


    //     }

    // }

    // private IEnumerator FillStamina()
    // {
    //     yield return new WaitForSeconds(2.5f);

    //     while (ID.StaminaUsed > 0 && !jumpHeld)
    //     {

    //         ID.StaminaUsed -= staminaFill * Time.deltaTime;
    //         yield return null; // Wait for the next frame
    //     }

    //     if (!jumpHeld)
    //     {
    //         ID.StaminaUsed = 0;
    //         ID.globalEvents.OnUseStamina?.Invoke(false);

    //     }
    // }

    // public void UseStamina(float amount)
    // {
    //     ID.StaminaUsed += amount * Time.deltaTime;

    //     if (ID.StaminaUsed >= ID.MaxStamina)
    //     {
    //         // ResetHoldJump();
    //         // ResetParachute();

    //         ID.globalEvents.OnZeroStamina?.Invoke();
    //         SwitchState(IdleState);

    //     }


    // }

    // public void StartFillStaminaCoroutine()
    // {
    //     if (fillStaminaCoroutine != null)
    //     {
    //         StopCoroutine(fillStaminaCoroutine);
    //     }
    //     fillStaminaCoroutine = StartCoroutine(FillStamina());
    // }




    // private void ResetHoldJump()
    // {
    //     if (jumpHeld)
    //     {
    //         jumpHeld = false;
    //         anim.SetBool("JumpHeld", false);
    //         maxFallSpeed = ID.MaxFallSpeed;
    //         StartFillStaminaCoroutine();
    //     }


    // }

    // private void ResetParachute()
    // {
    //     if (isParachuting)
    //     {
    //         isParachuting = false;
    //         anim.SetBool("ParachuteBool", false);
    //         StartFillStaminaCoroutine();
    //         maxFallSpeed = ID.MaxFallSpeed;
    //         disableButtons = false;


    //     }

    // }



    // public void ShowSword(bool isShown)
    // {
    //     if (isShown)
    //     {
    //         Sword.SetActive(true);
    //     }
    //     StartCoroutine(SwordCoroutine(isShown));

    // }

    // private IEnumerator SwordCoroutine(bool isShown)
    // {

    //     float duration = 0.1f; // Duration in seconds over which the fade will occur
    //     float elapsedTime = 0;
    //     if (isShown)
    //     {


    //         while (elapsedTime < duration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
    //             SwordSprite.color = new Color(SwordSprite.color.r, SwordSprite.color.g, SwordSprite.color.b, alpha);
    //             yield return null;
    //         }

    //         // Ensure the final alpha is set to the target value
    //         SwordSprite.color = new Color(SwordSprite.color.r, SwordSprite.color.g, SwordSprite.color.b, 1);
    //     }
    //     else{
    //         while (elapsedTime < duration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
    //             SwordSprite.color = new Color(SwordSprite.color.r, SwordSprite.color.g, SwordSprite.color.b, alpha);
    //             yield return null;
    //         }

    //         // Ensure the final alpha is set to the target value
    //         SwordSprite.color = new Color(SwordSprite.color.r, SwordSprite.color.g, SwordSprite.color.b, 1);
    //         Sword.SetActive(false);

    //     }
    // }
}
