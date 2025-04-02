using UnityEngine;

public class ArtilleryPig : MonoBehaviour, ICollectible
{

    [SerializeField] private Transform arm;


    [SerializeField] private Gun gunScript;

    [SerializeField] private int clipCount;
    [SerializeField] private float fireRate;
    [SerializeField] private float fireDelay;

    private bool isFiring = false;
    private float fireTimer = 0f;
    private int currentClip = 0;
    [SerializeField] private Vector2 clampRotation;
    [SerializeField] private float angleAdjustment;

    [SerializeField] private Vector2[] positions;

    private Transform player;
    private int currentTarget;

    private Rigidbody2D rb;

    private bool flipped;
    [SerializeField] private float flipDistance;
    [SerializeField] private Vector2 flipForce;


    public void Collected()
    {
        // int i = 0;
        // Debug.Log("Collected");
        // if (player != null)
        // {
        //     Vector2 pp = player.position;
        //     if (pp.x > 0)
        //     {
        //         i += 2;

        //     }
        //     if (pp.y > 0)
        //     {
        //         i += 1;
        //     }

        // }
        // currentTarget = i;
    }

    private void FixedUpdate()
    {

        // if (currentTarget != -1)
        // {
        //     Debug.Log("Moving to target: " + currentTarget);
        //     transform.position = Vector2.MoveTowards(transform.position, positions[currentTarget], Time.deltaTime * 9f);

        if (flipped)
        {
            if (transform.position.x > player.position.x + flipDistance)
            {
                flipped = false;
                transform.localScale = new Vector3(1, 1, 1);
                rb.AddForce(flipForce, ForceMode2D.Impulse);
                isFiring = false;
                currentClip = 0;
                fireTimer = 0f;
            }

        }
        else
        {
            if (transform.position.x < player.position.x - flipDistance)
            {
                flipped = true;
                transform.localScale = new Vector3(-1, 1, 1);
                rb.AddForce(flipForce * -1, ForceMode2D.Impulse);
                isFiring = false;
                currentClip = 0;
                fireTimer = 0f;
            }
        }
        //     if (Vector2.Distance(transform.position, positions[currentTarget]) < .1f)
        //     {
        //         currentTarget = -1;
        //     }
        // }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        // transform.position = positions[0];
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (player != null)
        {
            // Convert the player's world position to the parent's local space.
            Vector2 localPlayerPos = arm.parent.InverseTransformPoint(player.position);
            // Use the arm's local position (usually zero, or its configured offset)
            Vector2 localArmPos = arm.localPosition;
            // Calculate the direction in local space.
            Vector2 localDirection = localPlayerPos - localArmPos;

            // Calculate the angle using local coordinates.
            float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
            // Normalize to 0-360.
            angle = (angle + 360f) % 360f;
            // Apply any additional offset and clamp the result.
            angle = Mathf.Clamp(angle + angleAdjustment, clampRotation.x, clampRotation.y);

            // Set the arm's local rotation (so it stays consistent even if the parent's scale is flipped).
            arm.localRotation = Quaternion.Euler(0, 0, angle);
        }

        fireTimer += Time.deltaTime;

        if (isFiring && fireTimer >= fireRate)
        {
            gunScript.Fire(flipped);
            fireTimer = 0f;
            currentClip++;
            if (currentClip >= clipCount)
            {
                isFiring = false;
                currentClip = 0;
            }
        }
        else if (!isFiring && fireTimer >= fireDelay)
        {
            isFiring = true;
            fireTimer = 0f;
        }
    }

    // Update is called once per frame

}
