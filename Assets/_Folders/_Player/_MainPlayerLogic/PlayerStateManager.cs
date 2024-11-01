using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;
public class PlayerStateManager : MonoBehaviour
{

    [SerializeField] private GameObject shotgunObj;
    [SerializeField] private PlayerStartingStatsForLevels stats;
    [SerializeField] private Pool pool;
    [SerializeField] private int yForceMultiplier;
    [SerializeField] private int yForceSubtractMultiplier;
    public bool useChainedAmmo = false;

    private Coroutine aimCouroutine;
    private Coroutine reloadCouroutine;
    private Coroutine MaxSlowTimeCouroutine;




    private Coroutine ShotgunNormalRotationRoutine;
    private Coroutine ShotgunAimingRotationRoutine;

    private float shotgunRotationTarget;
    private float shotgunAimRotationTarget;
    [SerializeField] private float shotgunRotationSpeed;
    [SerializeField] private float shotgunAimRotationSpeed;

    [SerializeField] private Transform blastPoint;
    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private float slowTimeTarget;
    [SerializeField] private float velocityMagnitude;
    [SerializeField] private float slowTimeDuration;
    [SerializeField] private float aimToShootDelay;

    [SerializeField] private float reloadDelay;
    [SerializeField] private float canShootDelay;
    [SerializeField] private float reloadDuration;
    [SerializeField] private float reloadTimeTarget;

    [SerializeField] private float speedTimeDuration;
    [SerializeField] private bool disableButtonsAtStart;
    public bool DisableButtonsAtStart { get { return disableButtonsAtStart; } set { return; } }
    [SerializeField] private Transform airSpawnPos;
    [SerializeField] private GameObject THETHING;

    public bool rotateWithLastState = false;
    private Coroutine shotgunSlow;



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
    public PlayerShotgunState ShotgunState = new PlayerShotgunState();

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
    public readonly int EquipShotgunTrigger = Animator.StringToHash("Equip");
    public readonly int UnEquipShotgunTrigger = Animator.StringToHash("UnEquip");
    public readonly int ShootShotgunTrigger = Animator.StringToHash("Shoot");


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
    [HideInInspector]
    // public bool ignoreParticleCollision = false;

    private bool justDashedSlashed;
    private bool canDash;
    private bool shotgunReleased = false;
    public bool shotgunEquipped { get; private set; }
    private bool startedAim = false;
    private bool movingJoystick = false;
    private bool ignoreChainedShotgunReset;
    private bool canShootShotgun = true;
    private bool inSlowMo = false;
    private float dropCooldownTime = 2f;
    private readonly int rotationLerpSpeed = 20;
    private readonly int jumpRotSpeed = 200;
    private int frozenRotSpeed = 350;
    private readonly int maxRotUp = 15;
    private readonly int maxRotDown = -30;

    private float targetRotateSpeedShotgun;
    [SerializeField] private float baseRotateSpeedShotgun;
    [SerializeField] private float lerpTargetRotateSpeedSpeed;



    private float rotZ;

    private bool ignoreRotation;
    private PlayerAddForceBoundaries playerBoundaries;

    public bool usingConstantForce { get; private set; }


    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    // 

    private void Awake()
    {
        Time.timeScale = FrameRateManager.TargetTimeScale;
        shotgunEquipped = false;
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

        ID.ResetValues(stats);
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
        anim = GetComponent<Animator>();
        if (shotgunObj != null)
            shotgunObj.SetActive(false);

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
        AudioManager.instance.SlowAudioPitch(FrameRateManager.TargetTimeScale);
        if (GetComponent<PlayerAddForceBoundaries>() != null)
        {
            playerBoundaries = GetComponent<PlayerAddForceBoundaries>();
            playerBoundaries.enabled = false;
        }

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

        ID.events.EnableButtons?.Invoke(false);

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

            if (!ignoreRotation)
                currentState.RotateState(this);

            else
                rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, targetRotateSpeedShotgun, lerpTargetRotateSpeedSpeed * Time.fixedDeltaTime);


            // else


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
        {
            rb.velocity = force;
            ID.globalEvents.OnPlayerVelocityChange?.Invoke(force, rb.gravityScale);


        }


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

        if (ignoreRotation)
            ignoreRotation = false;

        if (useChainedAmmo && newState != ShotgunState)
        {
            ID.globalEvents.OnUseChainedAmmo?.Invoke(false);
        }
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
        SetShotgunRotTarget(5);

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
                SetShotgunRotTarget(-8);

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
            SetShotgunRotTarget(20);

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
            SetShotgunRotTarget(-15);

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

    private void HandleDrop()
    {


        // ResetHoldJump();
        isDropping = true;
        SetShotgunRotTarget(10);

        SwitchState(DropState);
        // ID.events.EnableButtons?.Invoke(false);

        // airResistance.localPosition = airDropPos;
        // airResistance.localEulerAngles = airDropRot;
        anim.SetTrigger(DropTrigger);

        // ID.globalEvents.CanDrop?.Invoke(false);

        // StartCoroutine(DropCooldown());

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
            SetShotgunRotTarget(5);

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
            ID.events.EnableButtons?.Invoke(false);
        DashState.SwitchSlash();

    }

    private void HandleNewSlash(bool holding)
    {
        Debug.LogError("Can Shoot is: " + canShootShotgun + " shotgun release is: " + shotgunReleased + " in slow mo is: " + inSlowMo);
        // if (shotgunReleased) return;
        if (holding && canShootShotgun)
        {

            // if (ID.ShotgunAmmo <= 0 && !useChainedAmmo)
            if (ID.ShotgunAmmo <= 0)
            {
                Debug.Log("ShotgunAmmo is 0 or less and not using chained ammo. Exiting.");
                return;
            }


            if (inSlowMo)
            {
                // StopCoroutine(shotgunSlow);
                StopCoroutine(reloadCouroutine);
            }

            if (MaxSlowTimeCouroutine != null)
                StopCoroutine(MaxSlowTimeCouroutine);

            MaxSlowTimeCouroutine = StartCoroutine(ShootIfOverTime());


            startedAim = true;
            ID.events.EnableButtons?.Invoke(false);

            anim.SetTrigger("Aim");
            aimCouroutine = StartCoroutine(AimShotgunCourintine());
            // shotgunSlow = StartCoroutine(SlowTime(slowTimeDuration, slowTimeTarget));



            // Debug.Log("YDFD");
            // lastState = currentState;
            // rotateWithLastState = true;
            // SwitchState(ParachuteState);

        }
        else if (!holding && startedAim)
        {
            Debug.Log("Realeased");
            ID.events.EnableButtons?.Invoke(true);


            if (ID.ShotgunAmmo > 0)
            {
                Debug.Log($"ShotgunAmmo is greater than 0. Current ShotgunAmmo: {ID.ShotgunAmmo}");
                ID.ShotgunAmmo--;
                Debug.Log($"ShotgunAmmo decremented. New ShotgunAmmo: {ID.ShotgunAmmo}");

                if (ID.ShotgunAmmo <= 0) ID.globalEvents.OnUseChainedAmmo?.Invoke(false); // remove for chained ammo to work

            }

            // if (useChainedAmmo)
            // {
            //     Debug.Log($"Using chained ammo. Current ChainedShotgunAmmo: {ID.ChainedShotgunAmmo}");

            //     ID.ChainedShotgunAmmo--;



            // }

            // else if (ID.ShotgunAmmo == 0 && !useChainedAmmo)
            // {
            //     Debug.Log("ShotgunAmmo is 0 and not using chained ammo. Triggering event to start using chained ammo.");

            //     ID.globalEvents.OnUseChainedAmmo?.Invoke(true);

            // }

            // if (useChainedAmmo && ID.ChainedShotgunAmmo <= 0)
            // {
            //     Debug.Log("ChainedShotgunAmmo is less than 0. Triggering event to stop using chained ammo.");
            //     ignoreChainedShotgunReset = true;
            //     ID.globalEvents.OnUseChainedAmmo?.Invoke(false);

            // }
            shotgunReleased = true;
            canShootShotgun = false;
            startedAim = false;


        }
    }



    private void UsingChainedAmmo(bool use)
    {
        if (use)
        {
            useChainedAmmo = true;
            ID.globalEvents.OnUpdateChainedShotgunAmmo?.Invoke(ID.ChainedShotgunAmmo);
            Debug.Log("Using chained ammo, event called");
        }


        else
        {
            useChainedAmmo = false;
            ID.ChainedShotgunAmmo = -1;
            if (ignoreChainedShotgunReset)
            {
                ignoreChainedShotgunReset = false;
                return;
            }
            if (!inSlowMo && aimCouroutine != null)
            {
                StopCoroutine(aimCouroutine);
                reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());
            }

        }
    }





    private void GetShotgunBlast()
    {
        // ignoreParticleCollision = true;
        // Debug.LogError("Gettting blast");
        pool.Spawn("ShotgunBlast", blastPoint.position, shotgunObj.transform.rotation);
        AudioManager.instance.PlayShoutgunNoise(0);
        // float rotationInDegrees = transform.eulerAngles.z;

        // // Convert rotation to radians for trigonometric functions
        // float rotationInRadians = rotationInDegrees * Mathf.Deg2Rad;

        // // Calculate the direction vector
        // Vector2 direction = new Vector2(Mathf.Cos(rotationInRadians), Mathf.Sin(rotationInRadians));

        // Apply the velocity (note that we negate the direction because your example implies leftward motion at 0 degrees)
        // rb.velocity = -direction * velocityMagnitude;



        Vector2 force = -shotgunObj.transform.right * velocityMagnitude;


        float yVelRatio = force.y / velocityMagnitude;
        float addedY = 0;
        float subtractY = 0;
        int xVal = 1;

        if (force.x < 0) xVal = -1;

        float yPos = transform.position.y;


        if (yVelRatio > .5f && transform.position.y > 0)
        {
            subtractY = yForceSubtractMultiplier * (yPos / 5);

        }
        else if (yVelRatio < .5f && transform.position.y < 0)
        {
            subtractY = -yForceSubtractMultiplier * (yPos / -5);

        }





        // if (yVelRatio > 0) addedY = yVelRatio * yForceMultiplier;
        // else if (yVelRatio < 0) addedY = yVelRatio * yForceMultiplier * -1;
        addedY = Mathf.Abs(yVelRatio * yForceMultiplier);
        Vector2 og = new Vector2(force.x, force.y + addedY);
        Vector2 finalForce = new Vector2(force.x + (subtractY * xVal), force.y + addedY - subtractY);

        Debug.LogError("Subtract y of: " + subtractY + "OriginalForce: " + og + " new force: " + finalForce);

        AdjustForce(finalForce);
        SwitchState(ShotgunState);

    }

    // private IEnumerator SlowTime(float duration, float target)
    // {
    //     Debug.LogError("Starting slow time with duration of: " + duration + " target is: " + target);
    //     inSlowMo = true;
    //     float currentTimeScale = Time.timeScale;

    //     float time = 0;
    //     if (target < .9f)
    //         AudioManager.instance.PlaySlowMotionSound(true);
    //     else
    //     {
    //         // yield return new WaitForSecondsRealtime(.15f);
    //         while (time < duration * .35f)
    //         {
    //             time += Time.unscaledDeltaTime;
    //             float newTime = Mathf.Lerp(currentTimeScale, 1 * .35f, time / duration);
    //             AudioManager.instance.SlowAudioPitch(newTime);
    //             Time.timeScale = newTime;


    //             yield return null;
    //         }
    //         time = 0;

    //         pool.Spawn("ShotgunShell", shellSpawnPoint.position, shellSpawnPoint.rotation);
    //         AudioManager.instance.PlaySlowMotionSound(false);
    //         canShootShotgun = true;
    //         Debug.LogError("Rlaoding shotgun, Can Shoot is: " + canShootShotgun);
    //         AudioManager.instance.PlayShoutgunNoise(1);
    //     }

    //     while (time < duration * .75f)
    //     {
    //         time += Time.unscaledDeltaTime;
    //         float newTime = Mathf.Lerp(currentTimeScale, target, time / duration);
    //         AudioManager.instance.SlowAudioPitch(newTime);
    //         Time.timeScale = newTime;


    //         yield return null;
    //     }

    //     if (target < .9f)
    //     {
    //         yield return new WaitUntil(() => shotgunReleased);
    //         shotgunReleased = false;
    //         anim.SetTrigger("Shoot");
    //         GetShotgunBlast();
    //         // yield return new WaitForSecondsRealtime(.2f);
    //         shotgunReleased = false;
    //         shotgunSlow = StartCoroutine(SlowTime(speedTimeDuration, 1));

    //         // yield break;

    //     }
    //     else
    //     {



    //         inSlowMo = false;

    //     }
    // }
    private IEnumerator AimShotgunCourintine()
    {
        movingJoystick = false;
        float currentTimeScale = Time.timeScale;
        float currentFallSpeed = maxFallSpeed;

        float time = 0;
        shotgunRotationTarget *= .7f;
        AudioManager.instance.PlaySlowMotionSound(true);
        while (time < slowTimeDuration)
        {
            time += Time.unscaledDeltaTime;
            float newTime = Mathf.Lerp(currentTimeScale, slowTimeTarget, time / slowTimeDuration);
            maxFallSpeed = Mathf.Lerp(currentFallSpeed, -8, time / slowTimeDuration);
            Time.timeScale = newTime;
            AudioManager.instance.SlowAudioPitch(newTime);


            yield return null;
        }
        StopCoroutine(ShotgunNormalRotationRoutine);

        // yield return new WaitForSeconds(aimToShootDelay);
        yield return new WaitUntil(() => shotgunReleased);
        shotgunReleased = false;
        GetShotgunBlast();

        // anim.SetTrigger("Shoot");
        anim.SetTrigger(ShootShotgunTrigger);
        // yield return new WaitForSecondsRealtime(.2f);

        reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());
    }

    private IEnumerator ShootIfOverTime()
    {
        yield return new WaitForSeconds(1.2f);

        if (startedAim == true)
        {
            startedAim = false;
            StopCoroutine(aimCouroutine);
            shotgunReleased = false;
            GetShotgunBlast();
            // anim.SetTrigger("Shoot");
            anim.SetTrigger(ShootShotgunTrigger);
            // yield return new WaitForSecondsRealtime(.2f);

            reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());

        }
    }

    private IEnumerator ReloadShotgunCourintine()
    {
        // StopCoroutine(ShotgunAimingRotationRoutine);
        movingJoystick = false;

        shotgunRotationTarget = 45;

        ShotgunNormalRotationRoutine = StartCoroutine(RotateShotgun());
        inSlowMo = true;
        float currentTimeScale = Time.timeScale;

        float time = 0;
        pool.Spawn("ShotgunShell", shellSpawnPoint.position, shellSpawnPoint.rotation);
        // yield return new WaitForSeconds(canShootDelay);
        shotgunRotationTarget = -20;
        canShootShotgun = true;
        Debug.LogError("Rlaoding shotgun, Can Shoot is: " + canShootShotgun);
        yield return new WaitForSecondsRealtime(reloadDelay);
        while (time < reloadDuration)
        {
            time += Time.unscaledDeltaTime;
            float newTime = Mathf.Lerp(currentTimeScale, reloadTimeTarget, time / reloadDuration);
            AudioManager.instance.SlowAudioPitch(newTime);
            Time.timeScale = newTime;


            yield return null;
        }
        AudioManager.instance.PlaySlowMotionSound(false);
        AudioManager.instance.PlayShoutgunNoise(1);



        time = 0;



        float mfs = maxFallSpeed;

        while (time < speedTimeDuration)
        {
            time += Time.deltaTime;
            float newTime = Mathf.Lerp(reloadTimeTarget, FrameRateManager.TargetTimeScale, time / speedTimeDuration);
            maxFallSpeed = Mathf.Lerp(mfs, originalMaxFallSpeed, time / speedTimeDuration);
            AudioManager.instance.SlowAudioPitch(newTime);
            Time.timeScale = newTime;


            yield return null;
        }
        inSlowMo = false;
        Time.timeScale = FrameRateManager.TargetTimeScale;

    }

    private void SwitchAmmo(int type)
    {
        if (type == 1)
        {
            shotgunEquipped = true;
            movingJoystick = false;

            // anim.SetTrigger("Equip");
            shotgunObj.transform.localEulerAngles = new Vector3(0, 0, -45);
            shotgunObj.SetActive(true);
            anim.SetTrigger(EquipShotgunTrigger);



            shotgunRotationTarget = -20;
            ShotgunNormalRotationRoutine = StartCoroutine(RotateShotgun());

        }
        else if (shotgunEquipped)
        {
            movingJoystick = false;

            if (ShotgunNormalRotationRoutine != null)

                StopCoroutine(ShotgunNormalRotationRoutine);

            if (ShotgunAimingRotationRoutine != null)
                StopCoroutine(ShotgunAimingRotationRoutine);

            // anim.SetTrigger("UnEquip");
            anim.SetTrigger(UnEquipShotgunTrigger);

            shotgunObj.SetActive(false);
            shotgunEquipped = false;

        }

    }
    private void SetShotgunRotTarget(int rot)
    {
        if (shotgunEquipped)
            shotgunRotationTarget = rot;
    }

    public void ReloadShotgun()
    {
        // canShootShotgun = true;
        // Debug.LogError("Rlaoding shotgun, Can Shoot is: " + canShootShotgun);
        // AudioManager.instance.PlayShoutgunNoise(1);


        //get shell here

    }

    private IEnumerator RotateShotgun()
    {
        while (true)
        {
            Quaternion currentRotation = shotgunObj.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, shotgunRotationTarget);

            // Calculate the angular difference between the current and target rotations
            float angleDifference = Quaternion.Angle(currentRotation, targetRotation);
            if (angleDifference < 1f)
            {
                // shotgunObj.transform.localRotation = targetRotation; // Snap to the final rotation to avoid any minor differences
                yield return null;
            }

            // Lerp the rotation towards the target
            shotgunObj.transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, shotgunRotationSpeed * Time.deltaTime);

            // If the angular difference is less than 1 degree, stop yielding (rotation complete)


            yield return null; // Wait for the next frame
        }
    }

    // private IEnumerator RotateWhileAiming()
    // {


    //     while (shotgunEquipped)
    //     {
    //         if (!movingJoystick) yield return null;





    //         // Quaternion currentRotation = shotgunObj.transform.rotation;
    //         // Quaternion targetRotation = Quaternion.Euler(0, 0, shotgunAimRotationTarget);

    //         // // Calculate the angular difference between the current and target rotations
    //         // float angleDifference = Quaternion.Angle(currentRotation, targetRotation);
    //         // if (angleDifference < 1f)
    //         // {
    //         //     // shotgunObj.transform.localRotation = targetRotation; // Snap to the final rotation to avoid any minor differences
    //         //     yield return null;
    //         // }

    //         // Lerp the rotation towards the target
    //         // shotgunObj.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, shotgunAimRotationSpeed * Time.deltaTime);

    //         // If the angular difference is less than 1 degree, stop yielding (rotation complete)


    //         yield return null; // Wait for the next frame
    //     }
    // }



    public void CalculateGlobalRotationTarget(int val)
    {
        // movingJoystick = true;
        // // Convert joystick input to an angle
        // shotgunAimRotationTarget = Mathf.Atan2(joystickInput.y, joystickInput.x) * Mathf.Rad2Deg;

        // if (joystickInput.x < -.4f)
        //     targetRotateSpeedShotgun = baseRotateSpeedShotgun;
        // else if (joystickInput.x > .4f)
        //     targetRotateSpeedShotgun = -baseRotateSpeedShotgun;

        // if (joystickInput == Vector2.zero)
        //     ignoreRotation = false;
        // else
        //     ignoreRotation = true;


        movingJoystick = true;
        // Convert joystick input to an angle

        if (val == 0) return;
        targetRotateSpeedShotgun = baseRotateSpeedShotgun * val;

        if (!ignoreRotation) ignoreRotation = true;


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
            if (useChainedAmmo)
            {
                ID.globalEvents.OnUseChainedAmmo?.Invoke(false);
            }

            isDamaged = true;

            if (!ID.infiniteLives)
            {


                ID.Lives--;
                if (ID.Lives <= 0)
                {
                    Die();
                    // ID.globalEvents.OnPlayerDamaged?.Invoke(true);

                    return;
                }

                // ID.globalEvents.OnPlayerDamaged?.Invoke(false);

            }

            else
            {
                // ID.globalEvents.OnPlayerDamaged?.Invoke(false);
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

    private void OnParticleCollision(GameObject other)
    {
        // if (!isDamaged && !ignoreParticleCollision)
        if (!isDamaged)
        {
            if (ID.particleYPos != null && ID.particleYPos.Count > 0)
            {
                for (int i = 0; i < ID.particleYPos.Count; i++)
                {
                    // if (y == ID.particleYPos[i]) ID.particleYPos.Remove(i);
                    if (Mathf.Abs(transform.position.y - ID.particleYPos[i]) < 1.08f)
                    {
                        HandleDamaged();
                        SmokeTrailPool.GetPlayerParticleCollider?.Invoke(transform.position, rb.velocity);
                        break;

                    }

                }
            }

        }


        // particleCollider.transform.position = 

        // ID.events.LoseLife?.Invoke();

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

            ID.events.EnableButtons?.Invoke(false);
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


    private void LevelFinished()
    {
        StopAllCoroutines();
    }


    private void OnEnable()
    {
        ID.events.OnJump += HandleJump;
        ID.events.OnAimJoystick += CalculateGlobalRotationTarget;
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

        // ID.globalEvents.OnUseChainedAmmo += UsingChainedAmmo;
        ID.globalEvents.OnFinishedLevel += LevelFinished;


        ID.globalEvents.OnAdjustConstantSpeed += ChangeConstantForce;
        ID.globalEvents.OnOffScreen += OffScreen;
        ID.events.HitBoss += HitBoss;
        ID.globalEvents.OnEnterNextSectionTrigger += HandleNextSectionTrigger;
        ID.events.OnSwitchAmmoType += SwitchAmmo;
        // ID.events.OnAttack += HandleSlash;
    }
    private void OnDisable()
    {
        ID.events.OnJump -= HandleJump;
        ID.events.OnAttack -= HandleNewSlash;
        ID.events.OnAimJoystick -= CalculateGlobalRotationTarget;
        // ID.globalEvents.OnUseChainedAmmo -= UsingChainedAmmo;
        ID.globalEvents.OnFinishedLevel -= LevelFinished;




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
        ID.events.OnSwitchAmmoType -= SwitchAmmo;





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
