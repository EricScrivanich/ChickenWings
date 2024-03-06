using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{

    [ExposedScriptableObject]
    public PlayerID ID;
    public PlayerParts playerParts;
    [SerializeField] private CameraShake cameraShake;
    public Transform parchutePoint;

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

    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerFrozenState FrozenState = new PlayerFrozenState();
    public PlayerHoldJumpState HoldJumpState = new PlayerHoldJumpState();
    public BucketCollisionState BucketState = new BucketCollisionState();
    public PlayerParachuteState ParachuteState = new PlayerParachuteState();
    public BucketScript bucket;
    private bool isAttacking = false;




    public bool hasFlippedRight = false;
    public bool hasFlippedLeft = false;




    private bool canDash;
    public bool canDrop;
    private bool canSlash;
    private float dashCooldownTime = 1.5f;
    private float dropCooldownTime = 3f;
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



    public Rigidbody2D rb;
    public Animator anim { get; private set; }

    private void Awake()
    {
        isDropping = false;
        isDamaged = false;
        jumpHeld = false;
        canDash = true;
        canDrop = true;
        disableButtons = true;
        ID.StaminaUsed = 0;

    }
    // Start is called before the first frame update
    void Start()
    {


        ChangeCollider(0);
        rb = GetComponent<Rigidbody2D>();
        // Feathers.SetActive(false);
        if (!isSlow)
        {
            ID.MaxFallSpeed = -9.7f;
            jumpForce = 11.2f;
            flipLeftForceX = -6.9f;
            flipLeftForceY = 9.4f;
            flipRightForceX = 6.8f;
            flipRightForceY = 10f;
            rb.gravityScale = 2.3f;
            originalGravityScale = rb.gravityScale;
        }
        else
        {
            ID.MaxFallSpeed = ID.slowMaxFallSpeed;
            jumpForce = ID.slowJumpForce;
            flipLeftForceX = ID.slowFlipLeftForceX;
            flipLeftForceY = ID.slowFlipLeftForceY;
            flipRightForceX = ID.slowFlipRightForceX;
            flipRightForceY = ID.slowFlipRightForceY;
            originalGravityScale = rb.gravityScale;

        }

        maxFallSpeed = ID.MaxFallSpeed;

        FeatherParticleQueue = new Queue<ParticleSystem>();
        for (int i = 0; i < 2; i++)
        {
            var feather = Instantiate(playerParts.FeatherParticle);
            FeatherParticleQueue.Enqueue(feather);
        }
        SmokeParticle = Instantiate(playerParts.SmokeParticle);


        attackObject.SetActive(false);
        // originalGravityScale = rb.gravityScale;

        anim = GetComponent<Animator>();

        currentState = StartingState;

        currentState.EnterState(this);

        ID.ResetValues();
    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     currentState.OnCollisionEnter2D(this, collision);

    // }
    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);

        MaxFallSpeed();
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

    public void UseStamina(float amount)
    {
        ID.StaminaUsed += amount * Time.deltaTime;

        if (ID.StaminaUsed >= ID.MaxStamina)
        {
            // ResetHoldJump();
            // ResetParachute();

            ID.globalEvents.OnZeroStamina?.Invoke();
            SwitchState(IdleState);

        }


    }

    public void StartFillStaminaCoroutine()
    {
        if (fillStaminaCoroutine != null)
        {
            StopCoroutine(fillStaminaCoroutine);
        }
        fillStaminaCoroutine = StartCoroutine(FillStamina());
    }

    private IEnumerator FillStamina()
    {
        yield return new WaitForSeconds(2.5f);

        while (ID.StaminaUsed > 0 && !jumpHeld)
        {

            ID.StaminaUsed -= staminaFill * Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        if (!jumpHeld)
        {
            ID.StaminaUsed = 0;
            ID.globalEvents.OnUseStamina?.Invoke(false);

        }
    }


    // Update is called once per frame
    void Update()
    {

        currentState.UpdateState(this);
        currentState.RotateState(this);
        if (transform.position.y > BoundariesManager.TopPlayerBoundary && !disableButtons)
        {
            // ResetHoldJump();
            SwitchState(FrozenState);
        }
    }
    public void SwitchState(PlayerBaseState newState)
    {
        currentState.ExitState(this);
        if (currentState == FlipLeftState || currentState == FlipRightState)
        {
            justFlipped = true;
        }
        else
        {
            justFlipped = false;
        }
        currentState = newState;
        newState.EnterState(this);
        // previousState = currentState;
        // currentState = newState;

        // if (previousState is PlayerHoldJumpState)
        // {
        //     rb.gravityScale = originalGravityScale;
        //     maxFallSpeed = ID.MaxFallSpeed;
        //     newState.EnterState(this);
        //     StartCoroutine(WaitForAnim());
        // }
        // // else if (previousState is PlayerFlipLeftState)
        // // {

        // // }
        // // else if (previousState is PlayerFlipRightState)
        // // {

        // // }

        // else
        // {
        //     newState.EnterState(this);
        // }

    }


    public void MaxFallSpeed()
    {
        if (rb.velocity.y < maxFallSpeed)
        {
            // If it does, limit it to the max fall speed
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    public void BaseRotationLogic()
    {
        if (rb.velocity.y > 0 && rotZ < maxRotUp)
        {
            // Calculate the new rotation
            rotZ += jumpRotSpeed * Time.deltaTime;
        }
        // If the object is moving downwards, rotate it downwards
        else if (rb.velocity.y < 0 && rotZ > maxRotDown)
        {
            // Calculate the new rotation
            rotZ -= jumpRotSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotZ), Time.deltaTime * rotationLerpSpeed);

    }
    void HandleJump()
    {
        if (!disableButtons && !ID.IsTwoTouchPoints)
        {
            // ResetHoldJump();
            SwitchState(JumpState);
        }

    }

    void HandleAttack(bool attacking)
    {

        if (attacking && !disableButtons && ID.numberOfPowersThatCanBeUsed >= 1)
        {
            ID.numberOfPowersThatCanBeUsed--;
            ID.CurrentMana -= ID.ManaNeeded;
            ID.globalEvents.UsePower?.Invoke();
            // ResetHoldJump();
            isAttacking = true;
            SwitchState(SlashState);
        }
        else if (!attacking && isAttacking)
        {
            // invoked false by featherAnimation, called function on AddDamage()

            maxFallSpeed = ID.MaxFallSpeed;
            attackObject.SetActive(false);
            anim.SetBool("AttackBool", false);
            disableButtons = false;
            isAttacking = false;
            CheckIfIsTryingToParachute();


        }


    }



    void HandleRightFlip()
    {
        if (!disableButtons)
        {
            // ResetHoldJump();
            SwitchState(FlipRightState);
        }

    }

    void HandleLeftFlip()
    {
        if (!disableButtons)
        {
            // ResetHoldJump();
            SwitchState(FlipLeftState);
        }

    }

    void HandleDash()
    {
        if (canDash && !disableButtons)
        {
            // ResetHoldJump();
            SwitchState(DashState);
            StartCoroutine(DashCooldown());

        }

    }



    void HandleHoldJump(bool isHolding)
    {


        if (!disableButtons)
        {
            if (isHolding)
            {

                if (ID.StaminaUsed < ID.MaxStamina)
                {
                    ID.globalEvents.OnUseStamina?.Invoke(true);
                    jumpHeld = true;
                }
                else
                {
                    StartFillStaminaCoroutine();
                    ID.globalEvents.OnZeroStamina?.Invoke();
                }
            }
            else
            {

                if (jumpHeld)
                {

                    SwitchState(IdleState);
                }
                // ResetHoldJump();
            }
        }
    }

    private void HandleDamaged()
    {

        if (!isDamaged)
        {
            isDamaged = true;
            ID.Lives--;
            if (ID.Lives <= 0)
            {
                Die();
                return;
            }
            CameraShake.instance.ShakeCamera(.5f, .14f);
            var Feather = FeatherParticleQueue.Dequeue();
            if (Feather.isPlaying)
            {
                Feather.Stop();
            }
            Feather.transform.position = transform.position;
            Feather.Play();
            StartCoroutine(Flash());

            FeatherParticleQueue.Enqueue(Feather);

        }
        else
        {
            return;
        }

    }
    void HandleDrop()
    {
        if (!disableButtons && canDrop)
        {
            // ResetHoldJump();
            isDropping = true;
            SwitchState(DropState);
            StartCoroutine(DropCooldown());
        }

    }

    private void HandleGroundCollision()
    {

        if (!isDropping)
        {
            Die();
        }
        else
        {
            Debug.Log("Started Bounce");
            // CameraShake.instance.ShakeCamera(.2f, .08f);
            SwitchState(BounceState);
        }
    }

    private void Die()
    {
        Debug.Log(isDropping);
        ID.Lives = 0;
        CameraShake.instance.ShakeCamera(.6f, .2f);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Ring") && isDropping)
        // {
        //     foreach (ContactPoint2D contact in collision.contacts)
        //     {
        //         Vector2 normal = contact.normal;

        //         // Determine direction based on the normal
        //         // Assuming a simple scenario where a positive normal.x suggests a hit from the left
        //         if (normal.x > 0)
        //         {
        //             // Move player to the right
        //             rb.AddForce(new Vector2(dodgeDistance, 0), ForceMode2D.Impulse);
        //         }
        //         else
        //         {
        //             // Move player to the left
        //             rb.AddForce(new Vector2(-dodgeDistance, 0), ForceMode2D.Impulse);
        //         }
        //         Debug.Log("HIT RING went: ");

        //         break; // Assuming we only care about the first contact point
        //     }
        // }
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
    // d
    public void HandleParachute(bool isPressing)
    {
        isTryingToParachute = isPressing;

        if (isPressing && !disableButtons)
        {
            if (ID.StaminaUsed < ID.MaxStamina)
            {
                Debug.Log("Holding chute");

                SwitchState(ParachuteState);
                isParachuting = true;
                ID.globalEvents.OnUseStamina?.Invoke(true);
            }
            else
            {
                StartFillStaminaCoroutine();

                ID.globalEvents.OnZeroStamina?.Invoke();

            }

        }
        else if (!isPressing && isParachuting)
        {
            // ResetParachute();
            SwitchState(IdleState);
        }
    }

    public void CheckIfIsTryingToParachute()
    {
        if (isTryingToParachute)
        {
            HandleParachute(true);
        }
        else
        {
            SwitchState(IdleState);
        }

    }



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





    IEnumerator DashCooldown()
    {
        canDash = false;

        yield return new WaitForSeconds(dashCooldownTime);
        canDash = true;
    }
    IEnumerator DropCooldown()
    {
        canDrop = false;

        yield return new WaitForSeconds(dropCooldownTime);
        canDrop = true;
    }

    IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(.3f);
        anim.SetBool("JumpHeld", false);
    }

    private void BucketCompletion(BucketScript bucketScript)
    {
        bucket = bucketScript;
        // ResetHoldJump();
        // ResetParachute();
        SwitchState(BucketState);

    }

    private void BucketExplosion(int index)
    {
        bucketIsExploded = true;

    }





    private void OnEnable()
    {
        ID.events.OnJump += HandleJump;
        ID.events.OnAttack += HandleAttack;
        ID.events.OnFlipRight += HandleRightFlip;
        ID.events.OnFlipLeft += HandleLeftFlip;
        ID.events.OnDash += HandleDash;
        ID.events.OnDrop += HandleDrop;
        ID.events.OnJumpHeld += HandleHoldJump;
        ID.events.OnParachute += HandleParachute;
        // ID.events.OnJumpReleased += HandleReleaseJump;
        ID.events.OnCompletedRingSequence += BucketCompletion;
        ID.globalEvents.OnBucketExplosion += BucketExplosion;
        ID.events.LoseLife += HandleDamaged;
        ID.events.HitGround += HandleGroundCollision;

        // ID.events.OnAttack += HandleSlash;
    }
    private void OnDisable()
    {
        ID.events.OnJump -= HandleJump;
        ID.events.OnAttack -= HandleAttack;
        ID.events.OnFlipRight -= HandleRightFlip;
        ID.events.OnFlipLeft -= HandleLeftFlip;
        ID.events.OnDash -= HandleDash;
        ID.events.OnDrop -= HandleDrop;
        ID.events.OnJumpHeld -= HandleHoldJump;
        ID.events.OnParachute -= HandleParachute;

        // ID.events.OnJumpReleased -= HandleReleaseJump;
        ID.events.OnCompletedRingSequence -= BucketCompletion;
        ID.globalEvents.OnBucketExplosion -= BucketExplosion;
        ID.events.LoseLife -= HandleDamaged;
        ID.events.HitGround -= HandleGroundCollision;



        // ID.events.OnAttack -= HandleSlash;

    }
}
