
using UnityEngine;
using System;
using System.Collections;
using HellTap.PoolKit;

public class HeliStateManager : MonoBehaviour
{
    [ExposedScriptableObject]
    public HelicopterID ID;
    public bool targetRight;
    public bool isFlipped;
    private bool isShooting;
    private bool isFiringMissiles;
    [SerializeField] private SpriteRenderer backProp;
    [SerializeField] private SpriteRenderer backPropBase;
    private Coroutine ShootingCourintine;

    public bool finishedFlipping;
    private Coroutine rotateShoot;
    private Coroutine homingMissile;
    public PlaneManagerID planeID;

    public Transform player;
    private float startingScale;
    public bool isTest;
    private bool isHit;
    HeliBaseState currentState;
    public HeliNormalState NormalState = new HeliNormalState();
    public HeliFlipState FlipState = new HeliFlipState();
    public HeliDipState DipState = new HeliDipState();
    public HeliFollowState FollowState = new HeliFollowState();
    [SerializeField] private Material mat;
    private Pool pool;





    public Rigidbody2D rb { get; private set; }

    private void Awake()
    {
        isFiringMissiles = false;
        isShooting = false;
        ID.missileCount = 0;

        if (PoolKit.GetPoolContainingPrefab("HomingMissile") != null)
        {
            pool = PoolKit.GetPoolContainingPrefab("HomingMissile");

        }
        
        isHit = false;
        finishedFlipping = false;
        ID.isFlipped = false;
    }

   
    // Start is called before the first frame update
    void Start()
    {


        startingScale = transform.localScale.x;
        // StartCoroutine(Missiles(3));


        if (startingScale > 0)
        {
            backProp.sortingOrder = 6;
            backPropBase.sortingOrder = 5;
            targetRight = false;
            ID.isFlipped = false;
            isFlipped = false;

        }
        else
        {
            backProp.sortingOrder = 3;
            ID.isFlipped = true;

            backPropBase.sortingOrder = 2;
            targetRight = true;
            isFlipped = true;

        }
        startingScale = MathF.Abs(startingScale);





        ID.shootingCooldown = 5.6f;
        ID.Lives = 3;
        ID.bulletAmount = 3;
        ID.shotDuration = 2.5f;
        ID.minSwitchTime = 7f;
        ID.maxSwitchTime = 10f;
        ID.risingYSpeed = 5;
        ID.riseYTarget = 1.1f;
        ID.minSwitchTime = 8f;

        rb = GetComponent<Rigidbody2D>();


        currentState = NormalState;

        currentState.EnterState(this);

        if (isTest)
        {
            ID.shootingCooldown = 4.2f;
            ID.Lives = 3;
            ID.bulletAmount = 5;
            ID.shotDuration = 1.1f;

        }
    }

    private IEnumerator ShootingLogic()
    {
        bool isMissile;
        float time = 0;
        while (true)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("Chance: " + chance);
            if (chance > .8f)
            {
                isMissile = true;
                time = ID.shootingCooldown * 1.8f;
                homingMissile = StartCoroutine(Missiles(3));

            }
            else
            {
                isMissile = false;

                rotateShoot = StartCoroutine(RotateAndShootCourintine(0, 0));
                time = ID.shootingCooldown;
            }
            yield return new WaitForSeconds(.5f);

            yield return new WaitUntil(() => isShooting == false);
            yield return new WaitUntil(() => isFiringMissiles == false);

            yield return new WaitForSeconds(time);


        }
    }

    public void Hit(int damageAmount)
    {
        if (isHit)
        {
            return;
        }

        isHit = true;
        StartCoroutine(HitEffect());

        ID.Lives--;

        if (ID.Lives == 2)
        {
            Invoke("TempSwitch", 3);
            ID.bulletAmount = 4;
            ID.shootingCooldown = 4;
            ID.risingYSpeed = 6;
            ID.riseYTarget = 1.3f;
        }
        else if (ID.Lives == 1)
        {
            ID.minSwitchTime = 4.5f;
            ID.minSwitchTime = 6.4f;
            ID.shootingCooldown = 2.6f;
            ID.risingYSpeed = 7;
            ID.riseYTarget = 1.5f;

        }
        else if (ID.Lives == 0)
        {
            planeID.GetExplosion(transform.position, new Vector3(1.3f, 1.3f, 1.3f));
            gameObject.SetActive(false);
        }




    }
    void OnTriggerEnter2D(Collider2D collider)
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(HeliBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        Debug.Log(state);
        state.EnterState(this);

    }


    public void RotateAndShoot(bool start)
    {
        if (isTest)
        {
            return;

        }
        else if (start)
        {
            ShootingCourintine = StartCoroutine(ShootingLogic());
        }
        else if (ShootingCourintine != null)
        {
            StopCoroutine(ShootingCourintine);

        }


    }

    public IEnumerator RandomRotateAndShootCoroutine(float rotationSpeed, float rotationDuration)
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;

        // Choose a random target rotation between two angles
        float minAngle = 4.1f;  // Define the minimum angle
        float maxAngle = 25f;   // Define the maximum angle
        float randomAngle = UnityEngine.Random.Range(minAngle, maxAngle);

        // Create the target rotation
        Quaternion targetRotation = Quaternion.Euler(0, 0, randomAngle);

        // Rotate over time
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // Invoke shooting event or method
        ID.events.shoot?.Invoke(1.2f, 5);

        // Wait before restarting or finishing
        yield return new WaitForSeconds(ID.shootingCooldown);

        // Optionally restart the coroutine
        rotateShoot = StartCoroutine(RandomRotateAndShootCoroutine(rotationSpeed, rotationDuration));
    }

    private IEnumerator Missiles(int count)
    {
        ID.missileCount = 0;
        isFiringMissiles = true;
        for (int i = 0; i < count; i++)
        {

            float initialRot = 90;
            if (isFlipped)
            {
                initialRot = -90;

            }
            ID.missileCount++;
            Vector3 rot = new Vector3(0, 0, transform.eulerAngles.z + initialRot);

            pool.Spawn("HomingMissile", transform.position, rot);
            yield return new WaitForSeconds(.5f);


        }
        isFiringMissiles = false;
        // StartCoroutine(Missiles(3));


    }

    public IEnumerator RotateAndShootCourintine(float rotationSpeed, float rotationDuration)
    {

        isShooting = true;
        float elapsedTime = 0f;
        bool isFlipped = transform.localScale.x < 0;  // Check if the helicopter is flipped along the Y-axis

        // Calculate the initial and target rotation
        Quaternion initialRotation = transform.rotation;
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;

        // If flipped, adjust the angle
        if (isFlipped)
        {
            angle -= 180f;  // Adjust the angle to account for the flip
            if (angle < -30)
            {
                angle = -30;
            }
            else if (angle > 10)
            {
                angle = 10;
            }
        }
        else
        {

            if (angle < -10)
            {
                angle = -10;
            }
            else if (angle > 30)
            {
                angle = 30;
            }

        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Invoke shooting event or method
        ID.events.shoot?.Invoke(1.2f, 5);

        // Rotate over time
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // Call your shoot function here, if not done in the event
        // Shoot();
        isShooting = false;
        // yield return new WaitForSeconds(ID.shootingCooldown);

        // Restart the coroutine to repeat the behavior
        // rotateShoot = StartCoroutine(RotateAndShootCourintine(rotationSpeed, rotationDuration));
    }

    public void TempSwitch()
    {
        SwitchState(DipState);

    }




    public void FlipRotation(bool flip)
    {
        if (flip != isFlipped)
        {
            StartCoroutine(RotateY(.3f, flip));
            isFlipped = !isFlipped;

        }
        else
        {
            finishedFlipping = true;
        }

    }

    private IEnumerator RotateY(float duration, bool flipVar)
    {
        float startScale = startingScale;
        float endScale = -startingScale;
        int backPropOrder = 3;
        float initialZ = transform.rotation.eulerAngles.z;
        bool hasFlipped = false;
        ID.isFlipped = true;
        if (!flipVar)
        {
            backPropOrder = 6;

            ID.isFlipped = false;
            startScale = -startingScale;
            endScale = startingScale;


        }
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float currentScale = Mathf.Lerp(startScale, endScale, timeElapsed / duration);


            transform.localScale = new Vector2(currentScale, startingScale);
            if (Mathf.Abs(currentScale) < 0.05f && !hasFlipped) // Threshold for flipping rotation
            {
                transform.rotation = Quaternion.Euler(0, 0, -initialZ);
                hasFlipped = true;

            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        backProp.sortingOrder = backPropOrder;
        backPropBase.sortingOrder = backPropOrder - 1;
        transform.localScale = new Vector2(endScale, startingScale);
        transform.rotation = Quaternion.Euler(0, 0, -initialZ);

        finishedFlipping = true;

    }
    //     float startAngle = 0;
    //     float endAngle = 40;
    //     float snapAngle = 140;
    //     float finalAngle = 180;
    //     // First rotation from 0 to 20 degrees
    //     if (!flipVar)
    //     {
    //         startAngle = 180;
    //         endAngle = 140;
    //         snapAngle = 40;
    //         finalAngle = 0;
    //     }

    //     float timeElapsed = 0;

    //     while (timeElapsed < duration)
    //     {
    //         float currentAngle = Mathf.Lerp(startAngle, endAngle, timeElapsed / duration);
    //         transform.rotation = Quaternion.Euler(0, currentAngle, transform.rotation.eulerAngles.z);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     // Snap rotation to 160 degrees
    //     transform.rotation = Quaternion.Euler(0, snapAngle, transform.rotation.eulerAngles.z);

    //     // Second rotation from 160 to 180 degrees

    //     timeElapsed = 0;

    //     while (timeElapsed < duration)
    //     {
    //         float currentAngle = Mathf.Lerp(snapAngle, finalAngle, timeElapsed / duration);
    //         transform.rotation = Quaternion.Euler(0, currentAngle, transform.rotation.eulerAngles.z);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.rotation = Quaternion.Euler(0, finalAngle, transform.rotation.eulerAngles.z);
    //     finishedFlipping = true;
    // }
    private void OnEnable()
    {

        if (isTest)
        {
            rotateShoot = StartCoroutine(RandomRotateAndShootCoroutine(35, .7f));
        }
        ID.events.Damaged += Hit;
        mat.SetFloat("_Alpha", 1);
    }
    private void OnDisable()
    {
        ID.events.Damaged -= Hit;


    }

    private IEnumerator HitEffect()
    {

        for (int i = 0; i < 5; i++)
        {
            // spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Set opacity to 0
            mat.SetFloat("_Alpha", 0f);
            yield return new WaitForSeconds(.1f);
            // spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set opacity to 1
            mat.SetFloat("_Alpha", .9f);
            yield return new WaitForSeconds(.1f);
        }
        mat.SetFloat("_Alpha", 1);

        yield return new WaitForSeconds(.3f);

        isHit = false;

    }

    public void Damage(int damageAmount)
    {
        Hit(damageAmount);
    }
}

