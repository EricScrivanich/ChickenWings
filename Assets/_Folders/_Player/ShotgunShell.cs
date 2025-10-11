using UnityEngine;

public class ShotgunShell : SpawnedQueuedObject
{
    [SerializeField] private AnimationDataSO AnimData;




    private float bounceThreshold = 0.1f;
    private ushort maxBounces = 3;


    private SpriteRenderer sr;
    private int spriteIndex;
    private float time;
    private float delay;
    private int bounceCount;
    private bool isGrounded;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        spriteIndex = Random.Range(0, 4);


        delay = Random.Range(AnimData.RandomDelaySpriteSwitch.x, AnimData.RandomDelaySpriteSwitch.y);
        sr.sprite = AnimData.sprites[spriteIndex];
        time = 0;
        bounceCount = 0;
        isGrounded = false;
    }

    void Update()
    {
        if (!isGrounded)
        {
            UpdateSprite();
        }
    }

    void FixedUpdate()
    {
        // if (isGrounded)
        // {
        //     // Move with the ground speed when grounded
        //     rb.velocity = new Vector2(groundSpeed, 0);
        //     rb.angularVelocity = 0;
        // }
        if (Mathf.Abs(rb.linearVelocity.y) < bounceThreshold && bounceCount >= maxBounces && !isGrounded)
        {
            // Shell has stopped bouncing, set it to grounded
            isGrounded = true;
            AudioManager.instance.PlayShotgunShell(3);
            rb.linearVelocity = new Vector2(-BoundariesManager.GroundSpeed, .2f);
            rb.angularVelocity = 0;
        }
        else if (isGrounded && transform.position.x < BoundariesManager.leftBoundary)
            gameObject.SetActive(false);



    }
    void OnDisable()
    {
        ReturnToPool();
    }

    private void UpdateSprite()
    {
        time += Time.deltaTime;
        if (time > delay)
        {
            spriteIndex++;
            if (spriteIndex >= AnimData.sprites.Length)
            {
                spriteIndex = 0;
            }
            sr.sprite = AnimData.sprites[spriteIndex];
            time = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGrounded)
        {
            bounceCount++;

            if (bounceCount == 1)
            {
                AudioManager.instance.PlayShotgunShell(0);

            }
            else
            {
                AudioManager.instance.PlayShotgunShell(1);
            }

            foreach (ContactPoint2D contact in collision.contacts)
            {
                rb.AddForceAtPosition(Vector2.left, contact.point, ForceMode2D.Impulse);
            }
        }
    }
}