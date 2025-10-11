using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;
public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private bool ignoreStartingState;

    [SerializeField] private bool isLevelTester = false;
    [Header("Scriptable Objects")]

    [ExposedScriptableObject]
    public PlayerID ID;

    [SerializeField] private QPool shotgunShellPool;




    [SerializeField] private bool bounceOffGround;




    [SerializeField] private Material invincibleMat;
    [SerializeField] private TrailRenderer invincibleTrail;

    [SerializeField] private GameObject ScythePrefab;
    [SerializeField] private GameObject parryPrefab;
    private EggShield eggShield;
    [SerializeField] private GameObject cursorPrefab;
    [SerializeField] private ButtonColorsSO colorSO;

    private Queue<Scythe> scythes;



    [SerializeField] private ParryCollisions parry;

    public GameObject feather;

    [ExposedScriptableObject]
    public PlayerMovementData MovementData;
    [ExposedScriptableObject]
    [SerializeField] private PlayerStartingStatsForLevels ammoManager;
    [SerializeField] private GameObject jumpAirPrefab;
    [SerializeField] private bool mutePlayerAudio = false;
    [SerializeField] private GameObject shotgunObj;

    [SerializeField] private Pool pool;
    [SerializeField] private int yForceMultiplier;
    [SerializeField] private int yForceSubtractMultiplier;
    public bool useChainedAmmo { get; private set; }

    // public Coroutine aimCouroutine;
    // public Coroutine reloadCouroutine;
    // public Coroutine MaxSlowTimeCouroutine;
    public Coroutine SlowTimeRoutine;
    public Coroutine SpeedTimeRoutine;
    public Coroutine MaxSlowTimeRoutine;
    public Coroutine ShotgunRotationRoutine;

    private bool checkingMaxTime = false;
    private bool slowingTime = false;
    private bool speedingTime = false;

    private bool ammoButtonPressed;





    public Coroutine ShotgunAimingRotationRoutine;

    public float shotgunRotationTarget;

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
    public PlayerScytheState ScytheState = new PlayerScytheState();

    public PlayerDashSlashState DashSlash = new PlayerDashSlashState();
    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerFrozenState FrozenState = new PlayerFrozenState();
    public PlayerHoldJumpState HoldJumpState = new PlayerHoldJumpState();
    public BucketCollisionState BucketState = new BucketCollisionState();
    public PlayerParachuteState ParachuteState = new PlayerParachuteState();
    public PlayerNextSectionState NextSectionState = new PlayerNextSectionState();
    public PlayerShotgunState ShotgunState = new PlayerShotgunState();
    public PlayerNestState NestState = new PlayerNestState();
    public PlayerParryState ParryState = new PlayerParryState();
    public PlayerNullState NullState = new PlayerNullState();
    public PlayerStuckSyctheState StuckState = new PlayerStuckSyctheState();

    public PigMaterialHandler stuckPig { get; private set; }


    AmmoBaseState currentWeaponState;
    AmmoBaseState prevWeaponState;
    private int currentWeaponIndex;
    [SerializeField] private int numberOfWeapons;


    public AmmoEggState AmmoStateEgg = new AmmoEggState();
    public AmmoShotgunState AmmoStateShotgun = new AmmoShotgunState();
    public AmmoHiddenState AmmoStateHidden = new AmmoHiddenState();
    public AmmoBoomerangState AmmoStateBoomerang = new AmmoBoomerangState();
    public AmmoCageState AmmoStateCage = new AmmoCageState();
    public AmmoScytheState AmmoStateScythe = new AmmoScytheState();

    private int[] availableAmmos;




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
    public readonly int AimShotgunBool = Animator.StringToHash("AimBool");


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
    private bool justSwitchedUsingChainedShotgun;
    private bool canDash;
    private bool shotgunReleased = false;
    public bool shotgunEquipped; //{ get; private set; }
    private bool startedAim = false;
    public bool movingJoystick = false;
    private bool shootChainThenReset;
    private bool canShootShotgun = true;
    private bool inSlowMo = false;
    private float dropCooldownTime = 2f;
    private readonly int rotationLerpSpeed = 20;
    private readonly int jumpRotSpeed = 200;
    private int frozenRotSpeed = 350;
    private readonly int maxRotUp = 15;
    private readonly int maxRotDown = -30;



    private float targetRotateSpeedShotgun;
    private Vector2 targetRotationShotgun;
    // [SerializeField] private float baseRotateSpeedShotgun;
    // [SerializeField] private float lerpTargetRotateSpeedSpeed;



    private float rotZ;

    private bool ignoreRotation;
    private PlayerAddForceBoundaries playerBoundaries;

    public bool usingConstantForce { get; private set; }


    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    // 

    #region Base
    private void Awake()
    {



        if (mutePlayerAudio)
            FrameRateManager.TargetTimeScale = .95f;
        else if (!isLevelTester)
        {
            float newScale = PlayerPrefs.GetFloat("GameSpeed", 1);
            if (newScale < .85f)
            {
                FrameRateManager.under085 = true;
                FrameRateManager.under1 = true;

            }
            else if (newScale < 1)
            {
                FrameRateManager.under085 = false;
                FrameRateManager.under1 = true;
            }
            else
            {
                FrameRateManager.under085 = false;
                FrameRateManager.under1 = false;
            }


            FrameRateManager.TargetTimeScale = FrameRateManager.BaseTimeScale * newScale;
            Time.timeScale = FrameRateManager.TargetTimeScale;

        }



        justSwitchedUsingChainedShotgun = false;
        shotgunEquipped = false;
        useChainedAmmo = false;
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
        if (ammoManager != null)
            ID.SetDataForLevel(ammoManager, null);

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

        ID.SetTransformAndRB(transform, rb);
        if (shotgunObj != null)
            shotgunObj.SetActive(false);

        justDashedSlashed = false;
        originalMaxFallSpeed = MovementData.MaxFallSpeed;
        originalGravityScale = MovementData.GravityScale;

        FlipRightState.CachVariables(MovementData.FlipRightInitialForceVector, MovementData.FlipRightAddForce,
        MovementData.FlipRightDownForce, MovementData.flipRightAddForceTime, MovementData.flipRightDownForceTime);

        FlipLeftState.CachVariables(MovementData.FlipLeftInitialForceVector, MovementData.FlipLeftAddForce,
        MovementData.FlipLeftDownForce, MovementData.flipLeftAddForceTime, MovementData.flipLeftDownForceTime);

        JumpState.CacheVaraibles(MovementData.JumpForce, MovementData.addJumpForce, MovementData.JumpDrag, MovementData.DragLerpSpeed);


        // NextSectionState.CacheVaraibles(MovementData.maxAmpRadRatio, MovementData.outerAmp, MovementData.innerAmp, MovementData.innerCutoff, MovementData.drag);





    }

    public void SetHingeTargetAngle(int target)
    {
        // CustomHingeJoint2D.OnSetRotationTarget?.Invoke(target);
        ID.globalEvents.SetCageAngle?.Invoke(target);
    }

    public void SetShotgunRotationAndActive(bool active, int rot)
    {
        if (!active)
        {
            shotgunObj.SetActive(false);
            return;
        }
        shotgunObj.transform.localEulerAngles = new Vector3(0, 0, rot);
        shotgunObj.SetActive(true);

    }
    // Start is called before the first frame update

    public void StartAfterPreload()
    {
        float newScale = PlayerPrefs.GetFloat("GameSpeed", 1);
        if (newScale < .85f)
        {
            FrameRateManager.under085 = true;
            FrameRateManager.under1 = true;

        }
        else if (newScale < 1)
        {
            FrameRateManager.under085 = false;
            FrameRateManager.under1 = true;
        }
        else
        {
            FrameRateManager.under085 = false;
            FrameRateManager.under1 = false;
        }


        FrameRateManager.TargetTimeScale = FrameRateManager.BaseTimeScale * newScale;
        Time.timeScale = FrameRateManager.TargetTimeScale;
        rb.simulated = true;
        currentState.EnterState(this);

    }

    void EnterStart()
    {
        currentState = StartingState;
        currentState.EnterState(this);

    }
    void Start()
    {


        // if (invTrail != null)
        // {
        //     invTrail.startColor = new Color(colorSO.WeaponColor.r, colorSO.WeaponColor.g, colorSO.WeaponColor.b, 1f);
        //     invTrail.gameObject.SetActive(false);
        // }
        if (invincibleTrail != null) invincibleTrail.gameObject.SetActive(false);
        if (invincibleMat != null) invincibleMat.SetFloat("_Alpha", 0);

        AudioManager.instance.SlowAudioPitch(FrameRateManager.TargetTimeScale);
        AudioManager.instance.LoadVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f), PlayerPrefs.GetFloat("SFXVolume", 1.0f), mutePlayerAudio);

        if (mutePlayerAudio) Time.timeScale = .95f;
        HapticFeedbackManager.instance.LoadSavedData();

        // if (swipeTracker != null)
        //     swipeTracker.SetActive(false);

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

        if (cursorPrefab != null)
        {
            var obj = Instantiate(cursorPrefab).GetComponent<CursorTracker>();
            if (obj != null)
            {
                obj.player = ID;
            }
        }
        if (ScythePrefab != null)
        {
            scythes = new Queue<Scythe>();

            for (int i = 0; i < 3; i++)
            {
                var s = Instantiate(ScythePrefab).GetComponent<Scythe>();
                // s.playerTrans = this.transform;
                s.SetColorsAndPlayer(colorSO.WeaponColor, parry.transform);
                s.gameObject.SetActive(false);

                scythes.Enqueue(s);
            }


        }
        if (parryPrefab != null)
        {
            eggShield = Instantiate(parryPrefab).GetComponent<EggShield>();
            eggShield.followPos = parry.transform;
            eggShield.gameObject.SetActive(false);
        }








        ChangeCollider(0);

        line = GetComponent<LineRenderer>();
        // Feathers.SetActive(false);





        rb.gravityScale = originalGravityScale;



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

        if (ignoreStartingState)
            currentState = IdleState;

        else
            currentState = StartingState;

        // ID.events.EnableButtons(true);

        if (!isLevelTester)
            currentState.EnterState(this);
        else
        {
            rb.simulated = false;
            ChangeCollider(-1);
        }

        // rb.simulated = false;
        // ChangeCollider(-1);
        // currentWeaponState = AmmoStateHidden;
        if (ammoManager != null)
        {
            ammoManager.Initialize();
            int startingAmmo = ammoManager.ReturnStartingEquipedAmmo();

            switch (startingAmmo)
            {
                case -1:
                    currentWeaponState = AmmoStateHidden;
                    break;
                case 0:
                    currentWeaponState = AmmoStateEgg;
                    break;
                case 1:
                    currentWeaponState = AmmoStateShotgun;
                    break;
                case 2:
                    currentWeaponState = AmmoStateBoomerang;
                    break;

            }

            if (startingAmmo != -1)
            {
                currentWeaponIndex = startingAmmo;

                ID.UiEvents.OnSendShownSidebarAmmos?.Invoke(ammoManager.AvailableAmmos, startingAmmo);

            }
            else
            {
                currentWeaponIndex = 0;
                // ID.canPressEggButton = false;
                // ID.UiEvents.OnSendShownSidebarAmmos?.Invoke(ammoManager.AvailableAmmos, 0);


            }

            currentWeaponState.EnterState(this, 0);
            // ID.CheckAmmosOnZero(0);

            // ID.SetAmountOfWeapons(ammoManager.numberOfWeapons);
        }
        else if (ID.playerStartingStats != null)
        {
            ammoManager = ID.playerStartingStats;
            ammoManager.Initialize();
            int startingAmmo = ammoManager.ReturnStartingEquipedAmmo();

            switch (startingAmmo)
            {
                case -1:
                    currentWeaponState = AmmoStateHidden;
                    break;
                case 0:
                    currentWeaponState = AmmoStateEgg;
                    break;
                case 1:
                    currentWeaponState = AmmoStateShotgun;
                    break;
                case 2:
                    currentWeaponState = AmmoStateBoomerang;
                    break;

            }

            if (startingAmmo != -1)
            {
                currentWeaponIndex = startingAmmo;

                ID.UiEvents.OnSendShownSidebarAmmos?.Invoke(ammoManager.AvailableAmmos, startingAmmo);

            }
            else
            {
                currentWeaponIndex = 0;
                // ID.canPressEggButton = false;
                // ID.UiEvents.OnSendShownSidebarAmmos?.Invoke(ammoManager.AvailableAmmos, 0);


            }


            currentWeaponState.EnterState(this, 0);

        }

        else
        {
            GameObject.Find("EggButton")?.SetActive(false);
        }
    }

    public void SetAddForceAtBoundaries(bool isActive)
    {


        if (isActive)
        {
            StartCoroutine(SetAddForceAtBoundariesDelay(.5f));

        }
    }

    public IEnumerator SetAddForceAtBoundariesDelay(float time)
    {
        yield return new WaitForSeconds(time);
        playerBoundaries.enabled = true;
        if (!addedArrow)
        {
            playerBoundaries.CreateArrow(arrowPlayerPosition);
            addedArrow = true;
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
                TargetRotationLogic();

            // else
            //     rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, targetRotateSpeedShotgun, 9 * Time.fixedDeltaTime);

            // else
            // {
            //     float targetAngle = Mathf.Atan2(targetRotationShotgun.y, targetRotationShotgun.x) * Mathf.Rad2Deg;

            //     // Get the current rotation of the Rigidbody2D
            //     float currentAngle = rb.rotation;

            //     // Calculate the shortest angle difference between the current and target angle
            //     float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
            //     if (Mathf.Abs(angleDifference) < 50) angleDifference *= 1.7f;

            //     // Calculate the desired angular velocity to close the angle difference
            //     float desiredAngularVelocity = angleDifference * 3.5f; // Adjust the multiplier (2f) to control rotation speed



            //     // Smoothly adjust the Rigidbody2D's angular velocity
            //     rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, desiredAngularVelocity, 7f * Time.fixedDeltaTime);
            // }


            // else


        }


        currentState.FixedUpdateState(this);


    }

    // public IEnumerator SwitchToIdleTimer(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     SwitchState(IdleState);
    // }

    public void BaseRotationLogic()
    {
        // Check if the Rigidbody is moving upwards and hasn't reached the max rotation limit
        if (rb.linearVelocity.y > 0 && rotZ < maxRotUp)
        {
            rotZ += jumpRotSpeed * Time.fixedDeltaTime;
        }
        // Check if the Rigidbody is moving downwards and hasn't reached the max rotation limit
        else if (rb.linearVelocity.y <= 0 && rotZ > maxRotDown)
        {
            rotZ -= jumpRotSpeed * Time.fixedDeltaTime;
        }

        // Smoothly interpolate the rotation towards the target rotation
        float newRotation = Mathf.LerpAngle(rb.rotation, rotZ, Time.fixedDeltaTime * rotationLerpSpeed);

        // Apply the new rotation
        rb.SetRotation(newRotation);
    }
    private float targetRotation;
    private bool rotatingPositive;
    private float rotateToTargetSpeed;

    public void RotateToTargetFromWeaponState(float targetAngle, float speed)
    {

        rb.angularVelocity = 0;
        targetRotation = targetAngle;
        rotateToTargetSpeed = speed;


        ignoreRotation = true;



    }

    private void ImmediateRotation()
    {
        if (!ignoreRotation) return;
        rb.angularVelocity = 0;

        ignoreRotation = true;


        rb.SetRotation(targetRotation);

        AmmoStateShotgun.HitRotationTarget(this);
    }

    public void TargetRotationLogic()
    {

        float currentRotation = rb.rotation;
        float angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);

        // Determine the correct direction
        // float rotationDirection = rotatingPositive ? 1f : -1f;

        // // Ensure we rotate in the specified direction
        // if (rotatingPositive && angleDifference < 0) rotationDirection = 1f;
        // if (!rotatingPositive && angleDifference > 0) rotationDirection = -1f;

        // Move towards the target at the specified speed
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotateToTargetSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRotation);

        // Stop rotating when we reach the target
        if (Mathf.Approximately(newRotation, targetRotation))
        {
            AmmoStateShotgun.HitRotationTarget(this);

        }




        // Apply the new rotation

    }
    private void ChangeConstantForce(float newSpeed)
    {
        float difference = newSpeed - ID.constantPlayerForce;
        ID.constantPlayerForce = newSpeed;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + difference, rb.linearVelocity.y);
    }

    public void AdjustForce(Vector2 force)
    {

        ID.globalEvents.OnPlayerVelocityChange?.Invoke(force, rb.gravityScale);

        // currentForce = xForce + ID.constantPlayerForce;
        if (!usingConstantForce)
        {
            // float addedForce = 0;
            // if (force.x * rb.linearVelocity.x > 0)
            // {
            //     addedForce = rb.linearVelocity.x * .4f;
            // }
            // force.x += addedForce;
            // force.y += rb.linearVelocity.y * .1f;
            rb.linearVelocity = force;
            ID.globalEvents.OnPlayerVelocityChange?.Invoke(force, rb.gravityScale);


        }


        else
        {
            float f = .4f;
            if (rb.linearVelocityX * force.x < 0)
                f = .1f;
            Debug.Log("Current Force: " + rb.linearVelocity);
            float x = (rb.linearVelocityX * f) + force.x;
            rb.linearVelocity = new Vector2(x, force.y);
            Debug.Log("Adjusted Force: " + rb.linearVelocity);
        }
        // rb.linearVelocity = new Vector2(force.x + ID.constantPlayerForce, force.y);


    }
    private void OnDestroy()
    {

        // DG.Tweening.DOTween.KillAll();
    }
    void Update()
    {

        currentState.UpdateState(this);
        FrozenCheck();



    }

    private void FrozenCheck()
    {
        if (transform.position.y > BoundariesManager.TopPlayerBoundary && !isFrozen && !ID.constantPlayerForceBool)
        {
            // ResetHoldJump();
            anim.SetBool(FrozenBool, true);
            isFrozen = true;
            StopShotgun(true);

            SwitchState(FrozenState);
            ID.events.EnableButtons?.Invoke(false);
        }
    }

    public void MaxFallSpeed()
    {

        // If it does, limit it to the max fall speed
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, maxFallSpeed, 15f));




    }





    private bool canSwitchAmmo = true;
    public void SetCanSwitchAmmo(bool canSwitch)
    {
        canSwitchAmmo = canSwitch;

    }
    public void SwitchWeaponState(int direction, int specificWeapon)
    {
        Debug.Log("Ammo button press is: " + ammoButtonPressed);
        if (!canSwitchAmmo) return;
        if (ammoManager.AvailableAmmos.Length < 2) return;
        if (ammoButtonPressed && specificWeapon == -1) return;
        if (currentWeaponState != null)
        {
            prevWeaponState = currentWeaponState;
            Debug.Log("Prev Weapon State is: " + prevWeaponState);
            currentWeaponState.ExitState(this);
        }

        if (direction == -2)
        {
            bool foundNewAmmo = false;
            int i = 0;
            for (int n = currentWeaponIndex + 1; i < ammoManager.AvailableAmmos.Length; n++)
            {
                if (n >= ammoManager.AvailableAmmos.Length) n = 0;
                if (ID.ReturnNextAvailableAmmo(ammoManager.AvailableAmmos[n]))
                {
                    foundNewAmmo = true;
                    specificWeapon = n;

                    if ((specificWeapon - currentWeaponIndex == 1) || (currentWeaponIndex == ammoManager.AvailableAmmos.Length - 1 && specificWeapon == 0))
                        direction = 1;
                    else if ((specificWeapon - currentWeaponIndex == -1) || (specificWeapon == ammoManager.AvailableAmmos.Length - 1 && currentWeaponIndex == 0))
                        direction = -1;
                    else direction = -2;
                    break;

                }
                else i++;

            }
            if (!foundNewAmmo)
            {
                int val = currentWeaponIndex + 1;
                direction = 1;
                if (val >= ammoManager.AvailableAmmos.Length) val = 0;

                specificWeapon = val;
            }
            HapticFeedbackManager.instance.SwitchAmmo();
        }




        if (specificWeapon >= 0)
        {


            ammoButtonPressed = false;

            currentWeaponIndex = System.Array.IndexOf(ammoManager.AvailableAmmos, specificWeapon);
            switch (specificWeapon)
            {

                case 0:
                    currentWeaponState = AmmoStateEgg;
                    break;
                case 1:
                    currentWeaponState = AmmoStateShotgun;
                    break;
                case 2:
                    currentWeaponState = AmmoStateBoomerang;

                    break;
                case 3:
                    currentWeaponState = AmmoStateScythe;
                    break;
            }

            currentWeaponState.EnterState(this, direction);
            return;


        }

        else if (specificWeapon == -3)
        {

        }
        else if (direction == 0 && currentWeaponState != AmmoStateHidden)
        {


            ammoButtonPressed = false;

            currentWeaponState = AmmoStateHidden;
            currentWeaponState.EnterState(this, 0);
            return;

        }
        else
        {

            HapticFeedbackManager.instance.SwitchAmmo();

            currentWeaponIndex += direction;

            if (currentWeaponIndex >= ammoManager.AvailableAmmos.Length)
                currentWeaponIndex = 0;
            else if (currentWeaponIndex < 0)
                currentWeaponIndex = ammoManager.AvailableAmmos.Length - 1;

        }

        switch (ammoManager.AvailableAmmos[currentWeaponIndex])
        {

            case 0:
                currentWeaponState = AmmoStateEgg;
                break;
            case 1:
                currentWeaponState = AmmoStateShotgun;
                break;
            case 2:
                currentWeaponState = AmmoStateBoomerang;

                break;
            case 3:
                currentWeaponState = AmmoStateScythe;
                break;
        }

        // Debug.LogError("Current Weapon State is: " + currentWeaponState);

        currentWeaponState.EnterState(this, direction);

    }



    public void SwitchState(PlayerBaseState newState)
    {

        if (currentState == NestState) return;

        currentState.ExitState(this);

        if (ignoreRotation)
            ignoreRotation = false;

        if (useChainedAmmo && newState != ShotgunState)
        {
            ID.globalEvents.OnUseChainedAmmo?.Invoke(false);
        }
        // if (ID.trackParrySwipe)

        if (newState != ShotgunState)
        {
            AmmoStateShotgun.StopChainedAmmo(this, true);
        }
        currentState = newState;


        if (currentState == FlipLeftState || currentState == FlipRightState)
        {
            justFlipped = true;
        }
        else
        {
            justFlipped = false;
            justFlippedLeft = false;
            justFlippedRight = false;


        }

        //if (usingScythePower && currentState != ScytheState && currentState != NullState)
        if (usingScythePower)
        {
            SetUseScythePowerSpeed(170, 1.7f);

        }
        currentState.EnterState(this);

        // Debug.Log("Switched state to: " + newState);

    }

    #endregion

    #region handle events

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
        Debug.LogError("HOlding jumo is" + isHolding);
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

    private void HandleEggDrop()
    {
        // if (ammoManager == null) return;

        // if (ID.Ammo > 0)
        // {


        //     AudioManager.instance.PlayEggDrop();
        //     float force = 0;

        //     if (rb.linearVelocity.x < 0)
        //     {
        //         force = rb.linearVelocity.x * .55f;
        //     }
        //     else
        //     {
        //         force = rb.linearVelocity.x * .45f;
        //     }
        //     ammoManager.GetEgg(transform.position, force);

        //     ID.Ammo -= 1;


        // }
    }
    public void GetEgg()
    {
        float x;
        float y = 0;
        float playerY = 0;

        if (rb.linearVelocity.x < 0)
        {
            x = rb.linearVelocity.x * .7f;
        }
        else
        {
            x = rb.linearVelocity.x * .6f;
        }

        if (rb.linearVelocity.y < 0)
        {
            y = rb.linearVelocity.y * .4f;
            playerY = .2f;
        }
        else
        {
            playerY = rb.linearVelocity.y * .7f;
        }
        // else
        // {
        //     y = rb.linearVelocity.y * .25f;
        // }
        Vector2 force = new Vector2(x, y - 1.5f);
        ammoManager.GetEgg(transform.position, force);
        EnterIdleStateWithVel(new Vector2(rb.linearVelocityX * .9f, playerY + 3f));



    }





    private void UsingChainedAmmo(bool use)
    {
        if (use)
        {
            useChainedAmmo = true;
            justSwitchedUsingChainedShotgun = false;
            ID.globalEvents.OnUpdateChainedShotgunAmmo?.Invoke(ID.ChainedShotgunAmmo);

            // Debug.Log("Using chained ammo, event called");
        }


        else
        {
            shootChainThenReset = false;
            useChainedAmmo = false;
            justSwitchedUsingChainedShotgun = false;

            ID.ChainedShotgunAmmo = -1;
            // if (ignoreChainedShotgunReset)
            // {
            //     ignoreChainedShotgunReset = false;
            //     return;
            // }
            // if (!inSlowMo && aimCouroutine != null)
            // {
            //     StopCoroutine(aimCouroutine);
            //     reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());
            // }

            StopShotgun(true);

        }
    }

    private void HandleCollectCage()
    {
        if (currentWeaponState != null)
        {
            prevWeaponState = currentWeaponState;
            currentWeaponState.ExitState(this);

        }
        currentWeaponState = AmmoStateCage;
        currentWeaponState.EnterState(this, 0);

    }

    public Vector2 centerTouchStartPos { get; private set; }

    private void HandlePressWeaponButton(Vector2 pos)
    {

        if (!ammoButtonPressed)
        {

            centerTouchStartPos = pos;
            currentWeaponState.PressButton(this, pos);
            ammoButtonPressed = true;
        }



    }

    private void HandleDragWeaponButton(Vector2 pos)
    {
        currentWeaponState.SwipeButton(this, pos);
    }

    private void HandleReleaseWeaponButton()
    {
        if (ammoButtonPressed)
        {
            ID.events.ReleaseSwipe?.Invoke();
            currentWeaponState.ReleaseButton(this);
            ammoButtonPressed = false;
        }


    }


    private float scytheUseSpeed;
    private float scythePower;
    public bool usingScythePower = false;

    public void SetUseScythePowerSpeed(float s, float pitch)
    {
        scytheUseSpeed = s;
        ID.UiEvents.ChangeScythePowerPitch?.Invoke(pitch);
    }
    public void UseScythePower(int amount)
    {
        scythePower -= amount;
    }
    public void ActivateScythePower()
    {
        scythePower = 100;

        if (!usingScythePower)
        {

            // SetTrailActive(true);
            if (doingDamageFlash)
            {
                StopCoroutine(damagedRoutine);
                ID.PlayerMaterial.SetFloat("_Alpha", 1);
                doingDamageFlash = false;
            }
            if (isUndamagable) StopCoroutine(undamagableRoutine);
            isDamaged = true;
            ScythePowerRoutine = StartCoroutine(ScytheGlideTimer(false));
        }
        else
        {
            StopCoroutine(ScythePowerRoutine);
            ScythePowerRoutine = StartCoroutine(ScytheGlideTimer(!changingMat));

        }
    }
    WaitForSeconds initialScythePowerDelay = new WaitForSeconds(.25f);
    WaitForSeconds finishScythePowerDelay = new WaitForSeconds(.25f);
    private Coroutine ScythePowerRoutine;
    private bool changingMat = false;
    private IEnumerator ScytheGlideTimer(bool isGoing)
    {
        usingScythePower = true;
        bool usingPower = true;
        ID.UiEvents.ShowScythePower?.Invoke(true, .25f);


        if (isGoing)
            yield return initialScythePowerDelay;
        else
        {
            float t = 0;
            invincibleTrail.emitting = true;
            invincibleTrail.gameObject.SetActive(true);

            // invincibleTrail.transform.localScale = Vector3.one * 1.9f;


            while (t < .25f)
            {

                t += Time.deltaTime;

                float p = t / .25f;

                float alpha = Mathf.Lerp(0, 1, p);
                float scale = Mathf.Lerp(1.9f, 1.7f, p);
                invincibleTrail.transform.localScale = Vector3.one * scale;
                invincibleMat.SetFloat("_Alpha", alpha);
            }

            invincibleTrail.transform.localScale = Vector3.one * 1.7f;
            invincibleMat.SetFloat("_Alpha", 1);
            // invincibleTrail.emitting = false;

        }
        while (usingPower)
        {
            scythePower = Mathf.MoveTowards(scythePower, 0, scytheUseSpeed * Time.deltaTime);

            if (scythePower <= 0)
            {
                usingPower = false;
                ID.UiEvents.UseScythePower?.Invoke(0);
            }
            else
            {
                ID.UiEvents.UseScythePower?.Invoke(scythePower * .01f);


            }

            yield return null;
        }
        ID.UiEvents.ShowScythePower?.Invoke(false, .28f);
        ID.UiEvents.StuckPigScythe?.Invoke(false);

        float time = 0;

        invincibleTrail.emitting = false;
        changingMat = true;

        while (time < .35f)
        {

            time += Time.deltaTime;

            float p = time / .35f;

            float alpha = Mathf.Lerp(1, 0, p);
            float scale = Mathf.Lerp(1.7f, 1.9f, p);
            invincibleTrail.transform.localScale = Vector3.one * scale;
            invincibleMat.SetFloat("_Alpha", alpha);
        }
        invincibleTrail.gameObject.SetActive(false);
        usingScythePower = false;


        // yield return finishScythePowerDelay;
        // SetTrailActive(false);
        isDamaged = false;



    }
    private void HandleStuckPig(PigMaterialHandler pig, int t)
    {
        stuckPig = pig;
        StuckState.SetPigType(t);

        SwitchState(StuckState);
    }
    private void HandleSwipeStuckPig(Vector2 dir)
    {
        if (ID.scytheIsStuck)
            StuckState.Swipe(this, dir);
    }
    private void HandleReleaseStick()
    {
        if (ID.scytheIsStuck)
            StuckState.ReleaseStick(this);
        // SwitchState(NullState);

    }

    #endregion


    #region Parries and scythe
    private bool canParry = true;

    public void HandleSuccesfulParry()
    {

        if (currentState == ParryState)
        {
            AudioManager.instance.PlayParrySound();
            ParryState.Parried(this);
            // parry.SuccesfulParry();
            // ID.events.OnTrackParrySwipe?.Invoke(true);
            ID.trackParrySwipe = true;

        }




    }




    public void StopSuccesfulParry()
    {
        // parry.StopAttackMode();
    }

    private void HandleEnterParryState(bool enter)
    {
        if (enter && canParry)
        {
            // ID.trackParrySwipe = true;
            SwitchState(ParryState);
            canParry = false;
            // eggShield.Initialize(parry.transform.position);
            StartCoroutine(ParryCooldownCoroutine(.8f));
        }

        // else
        // {
        //     parry.DoParry(false);
        //     parrying = false;
        //     SwitchState(IdleState);
        // }


    }
    public void DoParry(bool on)
    {
        // parry.gameObject.SetActive(!on);

        // parry.DoParry(on);


    }
    private void GetScytheAttack(byte direction)
    {
        var s = scythes.Dequeue();
        s.Attack(direction);
        scythes.Enqueue(s);
        SwitchState(ScytheState);

        switch (direction)
        {
            case 0:
                rb.SetRotation(70);
                rb.angularVelocity = -400;
                // transform.localScale = scaleTopFront;
                break;
            case 1:
                // transform.localScale = scaleBotFront;
                break;

            case 2:
                // transform.localScale = scaleBotBack;
                break;
            case 3:
                // transform.localScale = scaleTopBack;
                break;
        }
    }

    private Vector3 scaleTopFront = new Vector3(1, 1, 1);
    private Vector3 scaleTopBack = new Vector3(-1, 1, 1);
    private Vector3 scaleBotFront = new Vector3(1, -1, 1);
    private Vector3 scaleBotBack = new Vector3(-1, -1, 1);
    private float maxRotation = 85;

    public void GetScytheSwipeAttack(float angle)
    {
        Vector3 scale;
        float r;
        int dir = 1;
        float targetAngle = 0;


        if (angle < 0)
        {
            if (angle == -135)
            {
                scale = scaleTopBack;
                dir = 1;
                r = 135;
            }
            else if (angle == -225)
            {
                scale = scaleBotBack;
                dir = -1;
                r = 225;
            }
            else if (angle == -300)
            {
                scale = scaleBotFront;
                dir = 1;
                r = 135;
            }
            else if (angle == -350)
            {
                scale = scaleTopFront;
                dir = -1;

                r = 225;

            }
            else
            {
                r = 0;
                dir = 1;
                scale = scaleTopFront;
            }
        }
        else
        {
            if (angle == 0) angle = 360;

            // Debug.Log("Recieved angle of: " + angle);

            if (angle >= 270)
            {
                dir = -1;
                scale = scaleTopFront;
                if (angle == 270) r = 270;
                else
                    r = -ReturnLerpedAngle(360 - angle);

            }
            else if (angle <= 90)
            {
                dir = 1;

                scale = scaleBotFront;
                r = ReturnLerpedAngle(Mathf.Abs(angle));
            }
            else if (angle > 90 && angle < 180)
            {
                dir = -1;

                scale = scaleBotBack;
                r = -ReturnLerpedAngle(180 - angle);
            }
            else
            {
                dir = 1;

                scale = scaleTopBack;
                if (angle == 180) r = 8;
                else
                    r = ReturnLerpedAngle(angle - 180);
            }


        }



        Debug.Log("Targer angle is: " + targetAngle);
        ScytheState.SetTargets(dir);


        var s = scythes.Dequeue();

        SwitchState(ScytheState);
        s.SwipeAttack(parry.transform.position, r, scale);
        scythes.Enqueue(s);

    }

    private float ReturnLerpedAngle(float dif)
    {
        float p = Mathf.Lerp(0, maxRotation, dif / 90);
        // p = Mathf.Round(p / 10) * 10; // Rounds to the nearest 10
        Debug.Log("Difference is: " + dif + " added angle is: " + p);
        return p;
    }

    // private void HandleShowSwipeTracker(bool track)
    // {
    //     swipeTracker.SetActive(track);
    // }


    private IEnumerator ParryCooldownCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        canParry = true;

    }

    #endregion


    #region shotgun
    public void StopShotgun(bool paused)
    {
        if (paused && currentWeaponState == AmmoStateShotgun)
        {
            Debug.LogError("STOPPING SHOTGUN");



            // if (aimCouroutine != null) StopCoroutine(aimCouroutine);
            // if (reloadCouroutine != null) StopCoroutine(reloadCouroutine);
            // if (MaxSlowTimeCouroutine != null) StopCoroutine(MaxSlowTimeCouroutine);
            // if (ShotgunRotationRoutine != null) StopCoroutine(ShotgunRotationRoutine);
            if (slowingTime) StopCoroutine(SlowTimeRoutine);
            if (speedingTime) StopCoroutine(SpeedTimeRoutine);
            if (checkingMaxTime) StopCoroutine(MaxSlowTimeRoutine);
            ammoButtonPressed = false;
            ID.UiEvents.OnUseJoystick?.Invoke(false);

            slowingTime = false;
            speedingTime = false;
            checkingMaxTime = false;
            ID.UiEvents.ReleaseScope?.Invoke(false);
            ID.events.EnableButtons?.Invoke(true);


            Time.timeScale = FrameRateManager.TargetTimeScale;
            AudioManager.instance.SlowAudioPitch(FrameRateManager.TargetTimeScale);

            anim.SetBool(AimShotgunBool, false);
            anim.SetTrigger(EquipShotgunTrigger);
            if (!isFrozen) ID.events.EnableButtons(true);
            shotgunReleased = false;

            startedAim = false;
            canShootShotgun = true;
            inSlowMo = false;

        }

    }




    public void GetShotgunBlast(bool chainShot)
    {
        // ignoreParticleCollision = true;
        // Debug.LogError("Gettting blast");
        // pool.Spawn("ShotgunBlast", blastPoint.position, shotgunObj.transform.rotation);
        bool c = false;

        if (useChainedAmmo || justSwitchedUsingChainedShotgun) c = true;

        ammoManager.GetShotgunBlast(blastPoint.position, shotgunObj.transform.eulerAngles.z, chainShot);
        // sdfsadfs
        // asdfsf
        // asdfdsaf
        AudioManager.instance.PlayShoutgunNoise(0);

        Vector2 force = -shotgunObj.transform.right * velocityMagnitude;

        float yVelRatio = force.y / velocityMagnitude;
        float addedY = 0;
        float subtractY = 0;
        int xVal = 1;

        if (force.x < 0) xVal = -1;

        float yPos = transform.position.y;


        if (yVelRatio > .5f && yPos > 0)
        {
            subtractY = yForceSubtractMultiplier * (yPos / 5);

        }
        else if (yVelRatio < .5f && yPos < 0)
        {
            subtractY = -yForceSubtractMultiplier * (yPos / -5);
            if (chainShot) subtractY *= 1.05f;

        }

        // if (yVelRatio > 0) addedY = yVelRatio * yForceMultiplier;
        // else if (yVelRatio < 0) addedY = yVelRatio * yForceMultiplier * -1;
        addedY = Mathf.Abs(yVelRatio * yForceMultiplier);
        // Vector2 og = new Vector2(force.x, force.y + addedY);
        Vector2 finalForce = new Vector2(force.x + (subtractY * xVal), force.y + addedY - subtractY);

        if (chainShot) finalForce *= 1.14f;

        // Debug.LogError("Subtract y of: " + subtractY + "OriginalForce: " + og + " new force: " + finalForce);

        AdjustForce(finalForce);
        SwitchState(ShotgunState);


        if (useChainedAmmo && !chainShot)
            chainShot = true;

    }

    public void GetShell()
    {
        shotgunShellPool.SpawnWithVelocityAndRotation(shellSpawnPoint.position, (Vector2.up * 2) + (rb.linearVelocity * .4f), shellSpawnPoint.eulerAngles.z, 800);

        AudioManager.instance.PlayShoutgunNoise(1);
    }


    public void RotateShotgun(bool rotate)
    {
        if (rotate)
        {
            if (ShotgunRotationRoutine != null) StopCoroutine(ShotgunRotationRoutine);
            ShotgunRotationRoutine = StartCoroutine(RotateShotgunCoroutine());
        }
        else
        {
            if (ShotgunRotationRoutine != null) StopCoroutine(ShotgunRotationRoutine);
            // shotgunObj.transform.localEulerAngles = new Vector3(0, 0, shotgunRotationTarget);
        }
    }


    public IEnumerator RotateShotgunCoroutine()
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


    private void SetShotgunRotTarget(int rot)
    {
        if (shotgunEquipped)
            shotgunRotationTarget = rot;
    }






    public void CalculateGlobalRotationTarget(Vector2 val)
    {



        movingJoystick = true;


        // if (val == 0) return;
        // targetRotateSpeedShotgun = 400 * val;

        if (val == Vector2.zero)
        {
            ignoreRotation = false;
            return;
        }

        if (!ignoreRotation) ignoreRotation = true;
        targetRotationShotgun = val;




    }


    #endregion





    public void SetTimeScale(bool slowDown, float duration, float targetTime = 0, float maxSlowTime = 0)
    {
        if (slowDown)
        {
            if (speedingTime) StopCoroutine(SpeedTimeRoutine);
            speedingTime = false;
            slowingTime = true;
            if (maxSlowTime > 0)
            {
                MaxSlowTimeRoutine = StartCoroutine(MaxSlowTimeCoroutine(maxSlowTime));
                checkingMaxTime = true;
            }
            SlowTimeRoutine = StartCoroutine(SlowTimeCoroutine(targetTime, duration));
        }
        else
        {
            if (slowingTime) StopCoroutine(SlowTimeRoutine);
            if (checkingMaxTime) StopCoroutine(MaxSlowTimeRoutine);
            slowingTime = false;
            speedingTime = true;
            SpeedTimeRoutine = StartCoroutine(SpeedTimeCoroutine(duration));
        }

    }
    private IEnumerator MaxSlowTimeCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        checkingMaxTime = false;
        currentWeaponState.ReleaseButton(this);


    }
    private IEnumerator SlowTimeCoroutine(float targetTime, float duration)
    {
        float currentTimeScale = Time.timeScale;
        float currentFallSpeed = maxFallSpeed;

        float time = 0;
        AudioManager.instance.PlaySlowMotionSound(true);
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newTime = Mathf.Lerp(currentTimeScale, targetTime, time / duration);
            Time.timeScale = newTime;
            AudioManager.instance.SlowAudioPitch(newTime);


            yield return null;
        }
        slowingTime = false;
    }
    private IEnumerator SpeedTimeCoroutine(float duration)
    {
        float currentTimeScale = Time.timeScale;

        float time = 0;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newTime = Mathf.Lerp(currentTimeScale, FrameRateManager.TargetTimeScale, time / duration);
            AudioManager.instance.SlowAudioPitch(newTime);
            Time.timeScale = newTime;


            yield return null;
        }
        AudioManager.instance.PlaySlowMotionSound(false);
        speedingTime = false;

    }





    private void HandleNextSectionTrigger(float duration, float centerDuration, bool isClockwise, Transform trans, Vector2 centPos, bool doTween)
    {
        NextSectionState.SetValues(duration, centerDuration, isClockwise, trans, centPos, doTween);
        // currentState = NextSectionState;


        // NextSectionState.EnterState(this);

        SwitchState(NextSectionState);


    }

    #region damage/collision
    // private ColliderEnemy enemyCollider;

    // public void SetEnemyCollider(ColliderEnemy e)
    // {
    //     enemyCollider = e;
    // }

    public bool CanPerectParry { get; private set; }
    public bool CanParry { get; private set; }

    private ColliderEnemy enemyCollider;
    private ParryCollisions parryCollider;

    public void SetColliders(ColliderEnemy e, ParryCollisions p)
    {
        if (e != null) enemyCollider = e;
        if (p != null)
        {
            parryCollider = p;
            p.gameObject.SetActive(false);
        }

    }

    public void SetParry(bool startedParry)
    {
        if (startedParry)
        {
            enemyCollider.gameObject.SetActive(false);
            parryCollider.gameObject.SetActive(true);
        }
        else enemyCollider.gameObject.SetActive(true);

    }

    public void SetParryType(bool perfect, bool normal)
    {
        CanPerectParry = perfect;
        CanParry = normal;
    }

    // public byte CheckParry()
    // {
    //     if (ParryState.canPerfectParry)
    //     {

    //     }
    //     else if (ParryState.canParry)
    //     {

    //     }
    //     else
    //         HandleDamaged();
    // }
    public void HandleDamaged()
    {

        if (!isDamaged)
        {
            // if (useChainedAmmo)
            // {
            //     ID.globalEvents.OnUseChainedAmmo?.Invoke(false);
            // }
            AmmoStateShotgun.StopChainedAmmo(this, true);

            isDamaged = true;

            if (!ID.infiniteLives)
            {


                ID.Lives--;
                if (ID.Lives <= 0 && !isLevelTester)
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
    private Coroutine damagedRoutine;
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

        damagedRoutine = StartCoroutine(Flash());

        FeatherParticleQueue.Enqueue(Feather);

    }
    public void EnterIdleStateWithVel(Vector2 vel)
    {
        SwitchState(IdleState);
        AdjustForce(vel);
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


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {

            HandleDamaged();

            isDropping = false;



            anim.SetTrigger(FlipTrigger);
            foreach (ContactPoint2D pos in collision.contacts)
            {


                if (pos.normal.y > .92f)
                {


                    EnterIdleStateWithVel(new Vector2(1, 9));

                }
                else if (pos.normal.x < -.7f)
                {


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
                        SmokeTrailPool.GetPlayerParticleCollider?.Invoke(transform.position, rb.linearVelocity);
                        break;

                    }

                }
            }

        }




    }

    private Vector2 addedForce = new Vector2(0f, 8f);
    private void HandleGroundCollision()
    {


        if (!isDropping)
        {




            if (bounceOffGround)
            {
                AudioManager.instance.PlayBoingSound();
                float x = rb.linearVelocity.x * Random.Range(.8f, 1.2f);
                if (Mathf.Abs(x) < 2f)
                {
                    x = Random.Range(.5f, 3.3f);

                    int d = Random.Range(0, 2);

                    if (d >= 1) x *= -1;
                }
                SwitchState(NullState);

                rb.angularDamping = .3f;
                int r = Random.Range(350, 450);
                if (x < 0)
                    rb.angularVelocity = r;
                else
                    rb.angularVelocity = -r;

                AdjustForce(new Vector2(x, Random.Range(11.5f, 14.3f)));
            }
            else
            {
                HandleDamaged();
                AdjustForce(new Vector2(0, 12.5f));
                SwitchState(IdleState);

            }


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

        // ID.Lives = 0;

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


    private bool doingDamageFlash = false;
    private IEnumerator Flash()
    {
        doingDamageFlash = true;

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
        doingDamageFlash = false;

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
    private Coroutine undamagableRoutine;
    private bool isUndamagable;

    public void ResetDamagable()
    {
        if (isUndamagable)
        {
            StopCoroutine(undamagableRoutine);
            // SetTrailActive(false);
        }
        if (!doingDamageFlash) isDamaged = false;

    }
    public void SetUndamagable(bool fromStuck, float duration)
    {
        if (doingDamageFlash)
        {
            StopCoroutine(damagedRoutine);
            ID.PlayerMaterial.SetFloat("_Alpha", 1);
            doingDamageFlash = false;
        }
        if (isUndamagable) StopCoroutine(undamagableRoutine);
        undamagableRoutine = StartCoroutine(SetUndamagableCourintine(fromStuck, duration));

    }

    private IEnumerator SetUndamagableCourintine(bool fromStuck, float duration)
    {
        isUndamagable = true;

        isDamaged = true;
        yield return new WaitForSeconds(duration);

        // if (fromStuck) SetTrailActive(false);
        isUndamagable = false;

        isDamaged = false;

    }

    #endregion
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
        ID.events.EnableButtons?.Invoke(false);
    }
    private void OnChangeMovementData(PlayerMovementData data)
    {
        originalGravityScale = data.GravityScale;
        originalMaxFallSpeed = data.MaxFallSpeed;
        rb.gravityScale = originalGravityScale;
        maxFallSpeed = originalMaxFallSpeed;
        playerBoundaries.SetMaxFallSpeed(originalMaxFallSpeed);

        FlipRightState.CachVariables(data.FlipRightInitialForceVector, data.FlipRightAddForce,
          data.FlipRightDownForce, data.flipRightAddForceTime, data.flipRightDownForceTime);

        FlipLeftState.CachVariables(data.FlipLeftInitialForceVector, data.FlipLeftAddForce,
        data.FlipLeftDownForce, data.flipLeftAddForceTime, data.flipLeftDownForceTime);

        JumpState.CacheVaraibles(data.JumpForce, data.addJumpForce, data.JumpDrag, data.DragLerpSpeed);
        Debug.Log("Caching variablesssss");

    }

    private void HitWater(bool hit)
    {
        NestState.OnHitWater(hit);
    }


    public void SetIfWeaponButtonPressed(bool isPressed)
    {
        ammoButtonPressed = isPressed;
    }
    public void CollectAmmo(int type)
    {
        if (currentWeaponState == null) return;


        if (currentWeaponState != AmmoStateCage)
            currentWeaponState.CollectAmmo(this, type);
        else
            prevWeaponState.CollectAmmo(this, type);

    }

    private void SetScythePosition(Vector2 pos)
    {
        AmmoStateScythe.SetCenterPos(pos);

    }



    private void OnEnable()
    {
        ID.events.OnSetScythePos += SetScythePosition;
        ID.events.OnReleaseStick += HandleReleaseStick;
        ID.events.OnScytheAttack += GetScytheAttack;
        ID.events.OnSwipeScytheAttack += GetScytheSwipeAttack;
        ID.UiEvents.OnSwitchWeapon += SwitchWeaponState;
        ID.events.OnTouchCenter += HandlePressWeaponButton;
        ID.events.OnJump += HandleJump;
        ID.events.OnWater += HitWater;
        ID.globalEvents.OnSetNewPlayerMovementData += OnChangeMovementData;
        ID.events.OnAimJoystick += CalculateGlobalRotationTarget;
        // ID.events.OnAttack += HandleClocker;
        ID.events.OnFlipRight += HandleRightFlip;
        ID.events.OnFlipLeft += HandleLeftFlip;
        ID.events.OnDash += HandleDash;
        ID.events.OnDrop += HandleDrop;
        ID.events.OnJumpHeld += HandleHoldJump;
        ID.events.OnEggDrop += HandleEggDrop;
        // ID.events.OnParachute += HandleParachute;
        // ID.events.OnJumpReleased += HandleReleaseJump;
        ID.events.OnCompletedRingSequence += BucketCompletion;
        ID.globalEvents.OnBucketExplosion += BucketExplosion;
        ID.events.LoseLife += HandleDamaged;
        ID.events.HitGround += HandleGroundCollision;
        // ID.globalEvents.SetCanDashSlash += HandleDashSlash;
        ID.events.OnDashSlash += HandleDashSlash;


        ID.globalEvents.OnUseChainedAmmo += UsingChainedAmmo;
        ID.globalEvents.OnFinishedLevel += LevelFinished;


        ID.globalEvents.OnAdjustConstantSpeed += ChangeConstantForce;
        ID.globalEvents.OnOffScreen += OffScreen;
        ID.events.HitBoss += HitBoss;
        ID.globalEvents.OnEnterNextSectionTrigger += HandleNextSectionTrigger;

        ID.UiEvents.OnCollectAmmo += CollectAmmo;
        PauseMenuButton.OnPauseGame += StopShotgun;
        ID.events.OnCollectCage += HandleCollectCage;
        ID.events.OnDragCenter += HandleDragWeaponButton;

        ID.events.OnPerformParry += HandleEnterParryState;
        ID.globalEvents.OnScythePig += HandleStuckPig;
        ID.events.OnStuckScytheSwipe += HandleSwipeStuckPig;
        ID.events.OnReleaseCenter += HandleReleaseWeaponButton;
        ID.globalEvents.KillPlayer += Die;
        ID.events.OnCollision += ImmediateRotation;
        ID.events.OnStartPlayer += EnterStart;

        // ID.events.OnTrackParrySwipe += HandleShowSwipeTracker;

        // ID.events.OnAttack += HandleSlash;
    }
    private void OnDisable()
    {
        if (currentWeaponState != null)
            currentWeaponState.ExitState(this);

        if (ID.playerStartingStats != null) ID.playerStartingStats = null;

        // FlipLeftState.StopTweens();
        // FlipRightState.StopTweens();
        ID.events.OnSetScythePos -= SetScythePosition;
        ID.events.OnReleaseStick -= HandleReleaseStick;



        ID.events.OnScytheAttack -= GetScytheAttack;
        ID.events.OnSwipeScytheAttack -= GetScytheSwipeAttack;


        ID.UiEvents.OnSwitchWeapon -= SwitchWeaponState;
        ID.events.OnTouchCenter -= HandlePressWeaponButton;
        ID.events.OnPerformParry -= HandleEnterParryState;


        ID.events.OnStartPlayer -= EnterStart;

        ID.events.OnJump -= HandleJump;
        ID.events.OnWater -= HitWater;


        ID.events.OnAimJoystick -= CalculateGlobalRotationTarget;
        ID.globalEvents.OnUseChainedAmmo -= UsingChainedAmmo;
        ID.globalEvents.OnFinishedLevel -= LevelFinished;
        ID.globalEvents.OnSetNewPlayerMovementData -= OnChangeMovementData;


        ID.events.OnEggDrop -= HandleEggDrop;


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

        ID.UiEvents.OnCollectAmmo -= CollectAmmo;
        ID.events.OnCollectCage -= HandleCollectCage;

        PauseMenuButton.OnPauseGame -= StopShotgun;
        ID.events.OnDragCenter -= HandleDragWeaponButton;
        ID.globalEvents.OnScythePig -= HandleStuckPig;
        ID.events.OnStuckScytheSwipe -= HandleSwipeStuckPig;
        ID.events.OnReleaseCenter -= HandleReleaseWeaponButton;
        ID.globalEvents.KillPlayer -= Die;
        ID.events.OnCollision -= ImmediateRotation;




        // ID.events.OnTrackParrySwipe -= HandleShowSwipeTracker;



    }


}


//OLD SHOTGUN

// public void HandleShotgun(bool holding)
// {
//     if (ammoManager == null) return;
//     // Debug.LogError("Can Shoot is: " + canShootShotgun + " shotgun release is: " + shotgunReleased + " in slow mo is: " + inSlowMo);
//     // if (shotgunReleased) return;
//     if (holding && canShootShotgun)
//     {

//         if (ID.ShotgunAmmo <= 0 && !useChainedAmmo)
//         {
//             // Debug.Log("ShotgunAmmo is 0 or less and not using chained ammo. Exiting.");
//             return;
//         }


//         if (inSlowMo)
//         {
//             // StopCoroutine(shotgunSlow);
//             StopCoroutine(reloadCouroutine);
//         }

//         if (MaxSlowTimeCouroutine != null)
//             StopCoroutine(MaxSlowTimeCouroutine);

//         MaxSlowTimeCouroutine = StartCoroutine(ShootIfOverTime());


//         startedAim = true;
//         ID.events.EnableButtons?.Invoke(false);

//         // anim.SetTrigger("Aim");
//         anim.SetBool(AimShotgunBool, true);
//         aimCouroutine = StartCoroutine(AimShotgunCourintine());
//         // shotgunSlow = StartCoroutine(SlowTime(slowTimeDuration, slowTimeTarget));



//         // Debug.Log("YDFD");
//         // lastState = currentState;
//         // rotateWithLastState = true;
//         // SwitchState(ParachuteState);

//     }
//     else if (!holding && startedAim)
//     {
//         // Debug.Log("Realeased");
//         ID.events.EnableButtons?.Invoke(true);


//         if (ID.ShotgunAmmo > 0)
//         {
//             // Debug.Log($"ShotgunAmmo is greater than 0. Current ShotgunAmmo: {ID.ShotgunAmmo}");
//             // Debug.Log($"ShotgunAmmo decremented. New ShotgunAmmo: {ID.ShotgunAmmo}");

//             ID.ShotgunAmmo--;

//             // if (ID.ShotgunAmmo <= 0) ID.globalEvents.OnUseChainedAmmo?.Invoke(false); // remove for chained ammo to work

//         }

//         if (useChainedAmmo)
//         {
//             // Debug.Log($"Using chained ammo. Current ChainedShotgunAmmo: {ID.ChainedShotgunAmmo}");

//             ID.ChainedShotgunAmmo--;



//         }

//         else if (ID.ShotgunAmmo == 0 && !useChainedAmmo)
//         {
//             // Debug.Log("ShotgunAmmo is 0 and not using chained ammo. Triggering event to start using chained ammo.");

//             ID.globalEvents.OnUseChainedAmmo?.Invoke(true);

//         }

//         if (useChainedAmmo && ID.ChainedShotgunAmmo <= 0)
//         {
//             // Debug.Log("ChainedShotgunAmmo is less than 0. Triggering event to stop using chained ammo.");
//             shootChainThenReset = true;
//             useChainedAmmo = false;
//             // ID.globalEvents.OnUseChainedAmmo?.Invoke(false);

//         }
//         shotgunReleased = true;
//         canShootShotgun = false;
//         startedAim = false;


//     }
// }


// private IEnumerator MaxSlowTime(float duration)
// {

// }
// public IEnumerator AimShotgunCourintine()
// {
//     movingJoystick = false;
//     float currentTimeScale = Time.timeScale;
//     float currentFallSpeed = maxFallSpeed;

//     float time = 0;
//     shotgunRotationTarget *= .7f;
//     AudioManager.instance.PlaySlowMotionSound(true);
//     while (time < slowTimeDuration)
//     {
//         time += Time.unscaledDeltaTime;
//         float newTime = Mathf.Lerp(currentTimeScale, slowTimeTarget, time / slowTimeDuration);
//         // maxFallSpeed = Mathf.Lerp(currentFallSpeed, -8, time / slowTimeDuration);
//         Time.timeScale = newTime;
//         AudioManager.instance.SlowAudioPitch(newTime);


//         yield return null;
//     }
//     StopCoroutine(ShotgunRotationRoutine);

//     // yield return new WaitForSeconds(aimToShootDelay);
//     yield return new WaitUntil(() => shotgunReleased);
//     shotgunReleased = false;
//     GetShotgunBlast(false);

//     // anim.SetTrigger("Shoot");
//     // anim.SetTrigger(ShootShotgunTrigger);
//     anim.SetBool(AimShotgunBool, false);
//     // yield return new WaitForSecondsRealtime(.2f);

//     reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());
// }

// public IEnumerator ShootIfOverTime()
// {
//     yield return new WaitForSeconds(ID.maxShotgunHoldTime);

//     if (startedAim == true)
//     {
//         startedAim = false;
//         StopCoroutine(aimCouroutine);
//         shotgunReleased = false;
//         GetShotgunBlast(false);
//         ID.events.EnableButtons?.Invoke(true);
//         // anim.SetTrigger("Shoot");
//         // anim.SetTrigger(ShootShotgunTrigger);
//         anim.SetBool(AimShotgunBool, false);
//         // yield return new WaitForSecondsRealtime(.2f);

//         reloadCouroutine = StartCoroutine(ReloadShotgunCourintine());

//     }
// }

// public IEnumerator ReloadShotgunCourintine()
// {
//     // StopCoroutine(ShotgunAimingRotationRoutine);
//     movingJoystick = false;

//     shotgunRotationTarget = 45;

//     ShotgunRotationRoutine = StartCoroutine(RotateShotgunCoroutine());
//     inSlowMo = true;
//     float currentTimeScale = Time.timeScale;

//     float time = 0;
//     pool.Spawn("ShotgunShell", shellSpawnPoint.position, shellSpawnPoint.rotation);

//     // yield return new WaitForSeconds(canShootDelay);
//     shotgunRotationTarget = -20;
//     canShootShotgun = true;
//     // Debug.LogError("Rlaoding shotgun, Can Shoot is: " + canShootShotgun);
//     yield return new WaitForSecondsRealtime(reloadDelay);
//     while (time < reloadDuration)
//     {
//         time += Time.unscaledDeltaTime;
//         float newTime = Mathf.Lerp(currentTimeScale, reloadTimeTarget, time / reloadDuration);
//         AudioManager.instance.SlowAudioPitch(newTime);
//         Time.timeScale = newTime;


//         yield return null;
//     }
//     AudioManager.instance.PlaySlowMotionSound(false);
//     AudioManager.instance.PlayShoutgunNoise(1);




//     time = 0;



//     float mfs = maxFallSpeed;

//     while (time < speedTimeDuration)
//     {
//         time += Time.deltaTime;
//         float newTime = Mathf.Lerp(reloadTimeTarget, FrameRateManager.TargetTimeScale, time / speedTimeDuration);
//         maxFallSpeed = Mathf.Lerp(mfs, originalMaxFallSpeed, time / speedTimeDuration);
//         AudioManager.instance.SlowAudioPitch(newTime);
//         Time.timeScale = newTime;


//         yield return null;
//     }
//     inSlowMo = false;
//     Time.timeScale = FrameRateManager.TargetTimeScale;
//     AudioManager.instance.SlowAudioPitch(FrameRateManager.TargetTimeScale);
//     if (shootChainThenReset) ID.globalEvents.OnUseChainedAmmo?.Invoke(false);

// }

// private void SwitchAmmo(int type)
// {
//     if (type == 1)
//     {
//         shotgunEquipped = true;
//         movingJoystick = false;

//         // anim.SetTrigger("Equip");
//         shotgunObj.transform.localEulerAngles = new Vector3(0, 0, -45);
//         shotgunObj.SetActive(true);
//         anim.SetTrigger(EquipShotgunTrigger);



//         shotgunRotationTarget = -20;
//         ShotgunRotationRoutine = StartCoroutine(RotateShotgunCoroutine());

//     }
//     else if (shotgunEquipped)
//     {
//         movingJoystick = false;

//         if (ShotgunRotationRoutine != null)

//             StopCoroutine(ShotgunRotationRoutine);

//         if (ShotgunAimingRotationRoutine != null)
//             StopCoroutine(ShotgunAimingRotationRoutine);

//         // anim.SetTrigger("UnEquip");

//         anim.SetTrigger(UnEquipShotgunTrigger);
//         anim.SetBool(AimShotgunBool, false);

//         shotgunObj.SetActive(false);
//         shotgunEquipped = false;

//     }

// }