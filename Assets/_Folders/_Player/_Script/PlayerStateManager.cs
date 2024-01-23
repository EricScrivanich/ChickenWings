using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [ExposedScriptableObject]
    public PlayerID ID;
    private bool CanUseStamina = true;
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
    public BucketScript bucket;


    public bool hasFlippedRight = false;
    public bool hasFlippedLeft = false;




    private bool canDash;
    public bool canDrop;
    private bool canSlash;
    private float dashCooldownTime = 1.5f;
    private float dropCooldownTime = 3f;
    public bool jumpHeld;

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
        ID.StaminaUsed += staminaDeplete * Time.deltaTime;

        if (ID.StaminaUsed >= ID.MaxStamina)
        {
            ResetHoldJump();
            StartFillStaminaCoroutine();
        }


    }

    private void StartFillStaminaCoroutine()
    {
        // if (fillStaminaCoroutine != null)
        // {
        //     StopCoroutine(fillStaminaCoroutine);
        // }
        fillStaminaCoroutine = StartCoroutine(FillStamina());
    }

    private IEnumerator FillStamina()
    {
        yield return new WaitForSeconds(2.5f);

        while (ID.StaminaUsed > 0 && !jumpHeld)
        {
            Debug.Log("filling");
            ID.StaminaUsed -= staminaFill * Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // Optional: Reset stamina to 0 and invoke UI update when not jumping
        // if (!jumpHeld)
        // {
        //     ID.StaminaUsed = 0;
        //     ID.globalEvents.OnUseStamina?.Invoke(false);
        //     Debug.Log(ID.StaminaUsed + "Is False Amount");
        //     Debug.Log("It is false");
        // }
    }


    // Update is called once per frame
    void Update()
    {

        currentState.UpdateState(this);
        currentState.RotateState(this);
        if (transform.position.y > BoundariesManager.TopPlayerBoundary && !disableButtons)
        {
            SwitchState(FrozenState);
        }
    }
    public void SwitchState(PlayerBaseState newState)
    {
        previousState = currentState;
        currentState = newState;

        if (previousState is PlayerHoldJumpState)
        {
            rb.gravityScale = originalGravityScale;
            maxFallSpeed = ID.MaxFallSpeed;
            newState.EnterState(this);
            StartCoroutine(WaitForAnim());
        }
        // else if (previousState is PlayerFlipLeftState)
        // {

        // }
        // else if (previousState is PlayerFlipRightState)
        // {

        // }

        else
        {
            newState.EnterState(this);
        }

    }





    public void FlipStateChanges()
    {

    }
    public void HoldJumpStateChanges()
    {


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
        if (!disableButtons)
        {
            SwitchState(JumpState);
        }

    }

    void HandleAttack(bool isAttacking)
    {
        if (!disableButtons)
        {
            if (isAttacking)
            {
                SwitchState(SlashState);
            }
            else
            {
                maxFallSpeed = ID.MaxFallSpeed;
                attackObject.SetActive(false);
                anim.SetBool("AttackBool", false);
                SwitchState(IdleState);


            }
        }

    }



    void HandleRightFlip()
    {
        if (!disableButtons)
        {
            SwitchState(FlipRightState);
        }

    }

    void HandleLeftFlip()
    {
        if (!disableButtons)
        {
            SwitchState(FlipLeftState);
        }

    }

    void HandleDash()
    {
        if (canDash && !disableButtons)
        {

            SwitchState(DashState);
            StartCoroutine(DashCooldown());

        }

    }

    void HandleDrop()
    {
        if (!disableButtons && canDrop)
        {
            SwitchState(DropState);
            StartCoroutine(DropCooldown());
        }

    }

    void HandleHoldJump(bool isHolding)
    {
        if (isHolding && (ID.StaminaUsed < ID.MaxStamina))
        {
            ID.globalEvents.OnUseStamina?.Invoke(true);
            jumpHeld = true;

        }
        else
        {
            ResetHoldJump();
            StartFillStaminaCoroutine();

        }

    }

    private void ResetHoldJump()
    {
        jumpHeld = false;
        maxFallSpeed = ID.MaxFallSpeed;
        anim.SetBool("JumpHeld", false);
        SwitchState(IdleState);

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
        print("BUCKKY");
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
        // ID.events.OnJumpReleased -= HandleReleaseJump;
        ID.events.OnCompletedRingSequence -= BucketCompletion;
        ID.globalEvents.OnBucketExplosion -= BucketExplosion;

        // ID.events.OnAttack -= HandleSlash;

    }
}