using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private bool disableButtonsAtStart;
    public bool DisableButtonsAtStart { get { return disableButtonsAtStart; } set { return; } }
    [SerializeField] private Transform airSpawnPos;
    [SerializeField] private GameObject THETHING;

    public bool rotateWithLastState = false;



    [SerializeField] private GameObject arrowPlayerPosition;
    private bool addedArrow = false;
    private bool USINGTHING = false;
    [SerializeField] private float SlowFactor;

    [Header("Scriptable Objects")]
    [ExposedScriptableObject]
    public PlayerID ID;
    [SerializeField] private GameObject jumpAirPrefab;


    [ExposedScriptableObject]
    public PlayerMovementData MovementData;
    public CameraID Cam;
    public ParticleSystem Dust;
    private Queue<GameObject> jumpAir;
    [SerializeField] private float initialConstantForce;

    private LineRenderer line;
    public PlayerParts playerParts;
    [SerializeField] private List<Collider2D> ColliderType;
    public GameEvent DeadEvent;
    public bool isDamaged { get; private set; }
    private Queue<ParticleSystem> FeatherParticleQueue;
    private ParticleSystem SmokeParticle;

    public Transform ImageTransform;
    public GameObject attackObject;
    public bool bucketIsExploded = false;
    [HideInInspector]
    public float maxFallSpeed;
    public float originalMaxFallSpeed { get; private set; }
    public float originalGravityScale { get; private set; }

    #region States
    PlayerBaseState currentState;
    public PlayerBaseState lastState;

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
    public PlayerNextSectionState NextSectionState = new PlayerNextSectionState();

    #endregion 

    #region AnimationHashes
    public readonly int DashTrigger = Animator.StringToHash("DashTrigger");
    public readonly int DropTrigger = Animator.StringToHash("DropTrigger");
    public readonly int BounceBool = Animator.StringToHash("BounceBool");
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
    public readonly int FinishDropTrigger = Animator.StringToHash("FinishDropAir");


    #endregion

    private bool isAttacking = false;
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

            return currentJumpAirIndex;

        }
        set
        {
            currentJumpAirIndex = value;
        }


    }
    [HideInInspector]
    public bool holdingRightFlip;
    [HideInInspector]
    public bool holdingLeftFlip;
    [HideInInspector]
    public bool justFlippedRight;
    [HideInInspector]
    public bool justFlippedLeft;
    [HideInInspector]
    public bool isDashing;
    [HideInInspector]
    public bool canDashSlash;
    [HideInInspector]
    public bool canDrop;
    [HideInInspector]
    public bool stillDashing;
    [HideInInspector]
    public bool isDropping;
    [HideInInspector]
    public bool justFlipped;
    [HideInInspector]
    public bool isFrozen;
    [HideInInspector]
    public bool rotateSlash;
    [HideInInspector]
    public bool justStartedClocker;
    [HideInInspector]
    public bool jumpHeld;
    [HideInInspector]
    public bool isParachuting = false;
    [HideInInspector]
    public bool isTryingToParachute = false;
    private bool justDashedSlashed;
    private bool canDash;
    private float dropCooldownTime = 2f;
    private readonly int rotationLerpSpeed = 20;
    private readonly int jumpRotSpeed = 200;
    private int frozenRotSpeed = 350;
    private readonly int maxRotUp = 15;
    private readonly int maxRotDown = -30;
    private float rotZ;
    private PlayerAddForceBoundaries playerBoundaries;

    public bool usingConstantForce { get; private set; }


    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    // 

    private void Awake()
    {
        CurrentJumpAirIndex = 0;
        ID.constantPlayerForce = initialConstantForce;
        ID.events.EnableButtons?.Invoke(true);
        if (initialConstantForce == 0)
        {
            ID.constantPlayerForceBool = false;
            usingConstantForce = false;
        }
        else
        {
            ID.constantPlayerForceBool = true;
            usingConstantForce = true;

        }

        stillDashing = false;

        ID.ResetValues();
        // holdingFlip = false;
        ID.UsingClocker = false;
        rotateSlash = false;
        isDropping = false;
        isDamaged = false;
        jumpHeld = false;
        canDash = true;
        canDrop = true;
        isFrozen = false;

        rb = GetComponent<Rigidbody2D>();
        justDashedSlashed = false;
        originalMaxFallSpeed = MovementData.MaxFallSpeed;

        FlipRightState.CachVariables(MovementData.FlipRightInitialForceVector, MovementData.FlipRightAddForce,
        MovementData.FlipRightDownForce, MovementData.flipRightAddForceTime, MovementData.flipRightDownForceTime);

        FlipLeftState.CachVariables(MovementData.FlipLeftInitialForceVector, MovementData.FlipLeftAddForce,
        MovementData.FlipLeftDownForce, MovementData.flipLeftAddForceTime, MovementData.flipLeftDownForceTime);

        JumpState.CacheVaraibles(MovementData.JumpForce, MovementData.addJumpForce);


        // NextSectionState.CacheVaraibles(MovementData.maxAmpRadRatio, MovementData.outerAmp, MovementData.innerAmp, MovementData.innerCutoff, MovementData.drag);





    }
    // Start is called before the first frame update
    void Start()
    {
        playerBoundaries = GetComponent<PlayerAddForceBoundaries>();
        playerBoundaries.enabled = false;
        jumpAir = new Queue<GameObject>();

        for (int i = 0; i < jumpAirAmount; i++)
        {
            var obj = Instantiate(jumpAirPrefab);
            obj.GetComponent<JumpAirBehavior>().SetIndex(CurrentJumpAirIndex);

            obj.SetActive(false);
            jumpAir.Enqueue(obj);

        }



        currentJumpAirIndex = 0;






        ChangeCollider(0);

        line = GetComponent<LineRenderer>();
        // Feathers.SetActive(false);





        originalGravityScale = rb.gravityScale;



        // SwordSprite = Sword.GetComponent<SpriteRenderer>();

        maxFallSpeed = originalMaxFallSpeed;

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
        ID.events.EnableButtons(false);

        currentState = StartingState;

        currentState.EnterState(this);


    }

    public void SetAddForceAtBoundaries(bool isActive)
    {
        if (isActive)
        {
            playerBoundaries.enabled = true;
            if (!addedArrow)
            {
                playerBoundaries.CreateArrow(arrowPlayerPosition);
                addedArrow = true;
            }

        }
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

    // private IEnumerator LerpToOriginalPosition(float duration)
    // {
    //     float timeElapsed = 0;
    //     Vector2 startPosition = transform.position;
    //     disableButtons = true;

    //     while (timeElapsed < duration)
    //     {
    //         // Use Mathf.SmoothStep for easing; it smoothly interpolates between the start and end values
    //         float t = timeElapsed / duration;
    //         t = Mathf.SmoothStep(0.0f, 1.0f, t);  // Apply smoothing to t
    //         transform.position = Vector2.Lerp(startPosition, originalPosition, t);
    //         timeElapsed += Time.deltaTime;
    //         yield return null; // Wait for the next frame
    //     }

    //     transform.position = originalPosition;
    //     AdjustForce(0, 0);
    //     disableButtons = false; // Ensure the position is set exactly at the original
    // }

    void FixedUpdate()
    {

        // rb.velocity = new Vector2(currentForce, rb.velocity.y);
        MaxFallSpeed();
        if (rb.freezeRotation == false)
        {
            // if (rotateWithLastState)
            // {
            //     lastState.RotateState(this);
            //     Debug.Log("Rorerere");

            // }

            // else
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

    public void AdjustForce(Vector2 force)
    {

        // currentForce = xForce + ID.constantPlayerForce;
        if (!usingConstantForce)
            rb.velocity = force;


        else
            rb.velocity = new Vector2(force.x + ID.constantPlayerForce, force.y);


    }

    void Update()
    {

        currentState.UpdateState(this);
        // currentState.RotateState(this);
        if (transform.position.y > BoundariesManager.TopPlayerBoundary && !isFrozen && !ID.constantPlayerForceBool)
        {
            // ResetHoldJump();
            anim.SetBool(FrozenBool, true);
            isFrozen = true;

            SwitchState(FrozenState);
            ID.events.EnableButtons?.Invoke(false);
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
        // if (!disableButtons && !ID.IsTwoTouchPoints)


        // ResetHoldJump();
        anim.SetTrigger(JumpTrigger);
        var obj = jumpAir.Dequeue();
        obj.transform.position = airSpawnPos.position;
        obj.transform.eulerAngles = Vector3.zero;
        obj.SetActive(true);
        jumpAir.Enqueue(obj);

        SwitchState(JumpState);


    }
    void HandleHoldJump(bool isHolding)
    {


        if (isHolding)
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





    void HandleRightFlip(bool holding)
    {

        if (holding)
        {
            holdingRightFlip = true;
            anim.SetTrigger(FlipTrigger);
            var obj = jumpAir.Dequeue();
            obj.transform.position = airSpawnPos.position;

            obj.transform.eulerAngles = new Vector3(0, 0, 315);
            obj.SetActive(true);
            jumpAir.Enqueue(obj);

            // ResetHoldJump();
            SwitchState(FlipRightState);

        }
        else
        {
            holdingRightFlip = false;
        }


    }



    void HandleLeftFlip(bool holding)
    {

        if (holding)
        {
            holdingLeftFlip = true;

            // ResetHoldJump();
            anim.SetTrigger(FlipTrigger);
            var obj = jumpAir.Dequeue();
            obj.transform.position = airSpawnPos.position;

            obj.transform.eulerAngles = new Vector3(0, 0, 42);
            obj.SetActive(true);
            jumpAir.Enqueue(obj);

            SwitchState(FlipLeftState);
        }
        else
        {
            holdingLeftFlip = false;
        }
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
        // if (stillDashing && holding && canDashSlash)
        // {
        //     DashState.SwitchSlash();

        //     justDashedSlashed = true;
        //     ID.globalEvents.SetCanDashSlash?.Invoke(false);
        //     ID.events.EnableButtons(false);
        // }
        if (holding)
        {

            SwitchState(DashState);
            anim.SetTrigger(DashTrigger);
        }
        else
        {
            isDashing = false;
        }


    }


    private void HandleDashSlash()
    {

        if (stillDashing)
            ID.events.EnableButtons(false);
        DashState.SwitchSlash();

    }

    private void HandleNewSlash(bool none)
    {
        if (none)
        {
            Debug.Log("YDFD");
            lastState = currentState;
            rotateWithLastState = true;
            SwitchState(ParachuteState);

        }
    }



    private void HandleDrop()
    {


        // ResetHoldJump();
        isDropping = true;
        SwitchState(DropState);
        // ID.events.EnableButtons?.Invoke(false);

        // airResistance.localPosition = airDropPos;
        // airResistance.localEulerAngles = airDropRot;
        anim.SetTrigger(DropTrigger);

        // ID.globalEvents.CanDrop?.Invoke(false);

        // StartCoroutine(DropCooldown());

    }

    private void HandleNextSectionTrigger(float duration, float centerDuration, bool isClockwise, Transform trans, Vector2 centPos, bool doTween)
    {
        NextSectionState.SetValues(duration, centerDuration, isClockwise, trans, centPos, doTween);
        // currentState = NextSectionState;


        // NextSectionState.EnterState(this);

        SwitchState(NextSectionState);


    }

    private void HandleBubble(bool isInside)
    {


    }



    private void HandleDamaged()
    {

        if (!isDamaged)
        {

            isDamaged = true;

            if (!ID.infiniteLives)
            {


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
    public void EnterIdleStateWithVel(Vector2 vel)
    {
        SwitchState(IdleState);
        AdjustForce(vel);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {

            HandleDamaged();
            isDropping = false;

            foreach (ContactPoint2D pos in collision.contacts)
            {


                if (pos.normal.y > .92f)
                {


                    EnterIdleStateWithVel(new Vector2(1, 9));

                }
                else if (pos.normal.x < -.7f)
                {

                    // rb.velocity = new Vector2(-6.5f, 2);

                    EnterIdleStateWithVel(new Vector2(-6.3f, 2));


                }
                else if (pos.normal.x > .6f)
                {
                    EnterIdleStateWithVel(new Vector2(3, 2));


                }
            }
        }
    }


    private void HandleGroundCollision()
    {


        if (!isDropping)
        {
            HandleDamaged();
            AdjustForce(new Vector2(0, 12.5f));
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

            ID.events.EnableButtons(false);
            DropState.SwitchToBounce();
            // CameraShake.instance.ShakeCamera(.2f, .08f)
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
            AdjustForce(new Vector2(5, targetYVelocity));
        }
        else
        {
            AdjustForce(new Vector2(-5, targetYVelocity));
        }
    }
    public IEnumerator SetUndamagableCourintine(bool setBoolInitial, float duration)
    {
        isDamaged = true;
        yield return new WaitForSeconds(duration);
        isDamaged = false;

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


        BucketState.SetValues(position);
        // ResetHoldJump();
        // ResetParachute();
        if (isDropping)
        {
            anim.SetBool(BounceBool, true);
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
        // ID.globalEvents.SetCanDashSlash += HandleDashSlash;
        ID.events.OnDashSlash += HandleDashSlash;
        ID.events.OnAttack += HandleNewSlash;


        ID.globalEvents.OnAdjustConstantSpeed += ChangeConstantForce;
        ID.globalEvents.OnOffScreen += OffScreen;
        ID.events.HitBoss += HitBoss;
        ID.globalEvents.OnEnterNextSectionTrigger += HandleNextSectionTrigger;

        // ID.events.OnAttack += HandleSlash;
    }
    private void OnDisable()
    {
        ID.events.OnJump -= HandleJump;
        ID.events.OnAttack -= HandleNewSlash;

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
        // ID.globalEvents.SetCanDashSlash -= HandleDashSlash;
        ID.events.OnDashSlash -= HandleDashSlash;
        ID.globalEvents.OnEnterNextSectionTrigger -= HandleNextSectionTrigger;



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
