using UnityEngine;

public class Boomerang : MonoBehaviour, ICollectible
{
    public Transform playerPos;
    public Rigidbody2D playerRb;

    [SerializeField] private CircleCollider2D enemyCollider;
    [SerializeField] private CircleCollider2D collectableCollider;

    [SerializeField] private float throwMagnitude;

    [SerializeField] private PlayerID player;

    public Vector2 minMaxThrowSpeed;
    private Vector2 thrownDirection;

    private bool peaked = false;
    public float startAngVel;

    public float maxReturnSpeed;

    private float startRot;
    [SerializeField] private float returnForce;
    [SerializeField] private float playerVelocityRatio;
    private Vector2 returnDirection;
    private float rotationZ;
    public float throwSpeed = 10f; // Speed at which the boomerang is thrown
    public float maxDistance = 5f; // Maximum distance the boomerang travels before returning
    public float returnSpeed = 10f; // Speed at which the boomerang returns
    public KeyCode throwKey = KeyCode.E; // Key to throw the boomerang

    private Vector3 startPosition; // Position where the boomerang was released
    private bool isReturning = false; // Is the boomerang currently returning?
    private Rigidbody2D rb; // Rigidbody2D for the boomerang
    [SerializeField] private bool holdingBoomerang;
    private Vector2 playerVelWhenThown;


    public float maxDownSpeed;
    public float maxUpSpeed;
    public float maxLeftSpeed;
    public float maxRightSpeed;

    private bool justThrown = false;
    private float throwToCatchCooldown = .45f;
    private float cooldownTimer = 0f;


    public float maxDragTime = 1f; // Maximum time to affect the velocity calculation
    public float distanceMultiplier = 1f; // Multiplier for drag distance
    public float timeMultiplier = 1f; // Multiplier for drag time


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRb = playerPos.GetComponent<Rigidbody2D>();
        startRot = rb.rotation;
        if (holdingBoomerang) rb.simulated = false;


        if (rb == null)
        {
            Debug.LogError("Boomerang requires a Rigidbody2D component!");
        }
    }

    void Update()
    {
        if (holdingBoomerang)
        {
            transform.position = playerPos.position;
        }
        // Check for throw input
        // if (Input.GetKeyDown(KeyCode.E) && holdingBoomerang)
        // {
        //     Debug.Log("YUG");
        //     holdingBoomerang = false;

        //     ThrowBoomerang(Vector2.right * throwSpeed);
        // }

        if (justThrown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= throwToCatchCooldown)
            {
                justThrown = false;
                collectableCollider.enabled = true;
                cooldownTimer = 0;
            }

        }

        // Check if the boomerang should start returning
        // if (!isReturning && Vector3.Distance(startPosition, transform.position) >= maxDistance)
        // {
        //     StartReturn();
        // }
    }

    public void MaxFallSpeed()
    {
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, maxLeftSpeed, maxRightSpeed), Mathf.Clamp(rb.linearVelocity.y, maxDownSpeed, maxUpSpeed));


    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {

    //     if (!justThrown && other.gameObject.CompareTag("Player"))
    //     {
    //         Catch();

    //     }

    // }

    public void Catch()
    {
        collectableCollider.enabled = false;
        rb.angularVelocity = 0;
        rb.SetRotation(startRot);
        holdingBoomerang = true;
        isReturning = false;
        rb.simulated = false;
        peaked = false;

    }



    private void ThrowBoomerang(Vector2 direction, float magnitude)
    {
        justThrown = true;

        rb.simulated = true;

        playerVelWhenThown = playerRb.linearVelocity;
        holdingBoomerang = false;
        startPosition = transform.position; // Record the release position
        thrownDirection = direction;
        returnDirection = -direction;

        // rb.linearVelocity = Vector2.right * throwSpeed; // Throw the boomerang to the right
        rb.linearVelocity = direction * throwMagnitude;
        // isReturning = false; // Boomerang is in the throwing phase
        rb.angularVelocity = startAngVel;
        StartReturn();

    }

    private void StartReturn()
    {

        isReturning = true;


        // Calculate the direction back to the release point

        // rb.linearVelocity = returnDirection * returnSpeed; // Set the return velocity
    }


    private void Throw()
    {


    }
    private void Return()
    {
        Debug.Log("Returning");
        rb.AddForce(returnDirection * returnForce);
        // rb.AddForce(playerVelWhenThown * playerVelocityRatio);

    }

    // private void CalculateForce(Vector2 direction, float dragDistance, float dragTime)
    // {
    //     if (!holdingBoomerang) return;

    //     thrownDirection = direction;
    //     returnDirection = -direction;
    //     // Calculate the magnitude based on distance and time
    //     float rawMagnitude = (dragDistance * distanceMultiplier) / Mathf.Clamp(dragTime, 0.3f, maxDragTime) * timeMultiplier;

    //     // Clamp the magnitude to the defined minMaxThrowSpeed range
    //     float magnitude = Mathf.Clamp(rawMagnitude, minMaxThrowSpeed.x, minMaxThrowSpeed.y);




    //     Debug.Log("Magnitude is: " + magnitude);
    //     rb.simulated = true;
    //     justThrown = true;

    //     ThrowBoomerang(direction * magnitude);
    // }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 3 && !peaked && isReturning)
        {
            peaked = true;
        }
        // MaxFallSpeed();
        // If the boomerang is returning and reaches its start position, stop it
        if (isReturning)
        {
            // rb.linearVelocity = Vector2.zero; // Stop the boomerang
            // isReturning = false; // Reset state
            // Debug.Log("Boomerang returned!");
            Return();

            if (peaked)
            {
                if (rb.linearVelocity.magnitude > maxReturnSpeed)
                    isReturning = false;

            }
        }




    }
    private void Equip(int type, bool equip)
    {
        if (type == 2)

        {
            if (equip)
            {
                gameObject.SetActive(true);
                Catch();

            }
            else if (holdingBoomerang)
            {
                gameObject.SetActive(false);
                holdingBoomerang = false;
                rb.simulated = true;
            }
        }
    }

    private void Awake()
    {
        player.globalEvents.EquipItem += Equip;

    }


    private void OnDestroy()
    {
        player.globalEvents.EquipItem -= Equip;

        // SwipeThrowController.OnSwipeButton -= CalculateForce;
    }

    private void OnEnable()
    {
        player.globalEvents.ThrowItem += ThrowBoomerang;
    }

    private void OnDisable()
    {
        player.globalEvents.ThrowItem -= ThrowBoomerang;
    }

    public void Collected()
    {
        Catch();
    }
}