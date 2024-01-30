using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [ExposedScriptableObject]
    public PlayerID ID;
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

    private float jumpForce = 11.3f;
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
        jumpHeld = false;
        canDash = true;
        canDrop = true;
        disableButtons = true;
        ID.StaminaUsed = 0;

    }
    // Start is called before the first frame update
    void Start()
    {


        maxFallSpeed = ID.MaxFallSpeed;
        rb = GetComponent<Rigidbody2D>();

        attackObject.SetActive(false);
        originalGravityScale = rb.gravityScale;

        anim = GetComponent<Animator>();

        currentState = StartingState;

        currentState.EnterState(this);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        currentState.OnCollisionEnter2D(this, collision);

    }
    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);

        MaxFallSpeed();
    }

    public void UseStamina(float amount)
    {
        ID.StaminaUsed += amount * Time.deltaTime;

        if (ID.StaminaUsed >= ID.MaxStamina)
        {
            ResetHoldJump();
            ResetParachute();

            ID.globalEvents.OnZeroStamina?.Invoke();
            SwitchState(IdleState);

        }


    }

    private void StartFillStaminaCoroutine()
    {
        if (fillStaminaCoroutine != null)
        {
            StopCoroutine(fillStaminaCoroutine);
        }
        fillStaminaCoroutine = StartCoroutine(FillStamina());
    }

    private IEnumerator FillStamina()
    {
        yield return new WaitForSeconds(2.8f);

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
            ResetHoldJump();
            SwitchState(FrozenState);
        }
    }
    public void SwitchState(PlayerBaseState newState)
    {
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
            ResetHoldJump();
            SwitchState(JumpState);
        }

    }

    void HandleAttack(bool attacking)
    {

        if (attacking && !disableButtons)
        {
            ResetHoldJump();
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
            ResetHoldJump();
            SwitchState(FlipRightState);
        }

    }

    void HandleLeftFlip()
    {
        if (!disableButtons)
        {
            ResetHoldJump();
            SwitchState(FlipLeftState);
        }

    }

    void HandleDash()
    {
        if (canDash && !disableButtons)
        {
            ResetHoldJump();
            SwitchState(DashState);
            StartCoroutine(DashCooldown());

        }

    }

    void HandleDrop()
    {
        if (!disableButtons && canDrop)
        {
            ResetHoldJump();
            SwitchState(DropState);
            StartCoroutine(DropCooldown());
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
                ResetHoldJump();
            }
        }
    }
    public void HandleParachute(bool isPressing)
    {
        isTryingToParachute = isPressing;

        if (isPressing && !disableButtons)
        {
            if (ID.StaminaUsed < ID.MaxStamina)
            {

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
            ResetParachute();
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



    private void ResetHoldJump()
    {
        if (jumpHeld)
        {
            jumpHeld = false;
            anim.SetBool("JumpHeld", false);
            maxFallSpeed = ID.MaxFallSpeed;
            StartFillStaminaCoroutine();
        }


    }

    private void ResetParachute()
    {
        if (isParachuting)
        {
            isParachuting = false;
            anim.SetBool("ParachuteBool", false);
            StartFillStaminaCoroutine();
            maxFallSpeed = ID.MaxFallSpeed;
            disableButtons = false;


        }

    }




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
        ResetHoldJump();
        ResetParachute();
        SwitchState(BucketState);

    }

    private void BucketExplosion()
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

        // ID.events.OnAttack -= HandleSlash;

    }
}
