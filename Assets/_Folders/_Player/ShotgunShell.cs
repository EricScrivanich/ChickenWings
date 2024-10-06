using UnityEngine;

public class ShotgunShell : MonoBehaviour
{
    [SerializeField] private AnimationDataSO AnimData;
    [SerializeField] private float angularVel;
    [SerializeField] private float globalForce;
    [SerializeField] private Vector3 relativeForce;
    [SerializeField] private Vector2 contactForce;
    [SerializeField] private float groundSpeed = -4.7f;
    [SerializeField] private float bounceThreshold = 0.1f;
    [SerializeField] private int maxBounces = 3;

    private Rigidbody2D rb;
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
        rb.velocity = (transform.up * globalForce) + relativeForce;
        rb.angularVelocity = angularVel;
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
        if (Mathf.Abs(rb.velocity.y) < bounceThreshold && bounceCount >= maxBounces && !isGrounded)
        {
            // Shell has stopped bouncing, set it to grounded
            isGrounded = true;
            AudioManager.instance.PlayShotgunShell(3);
            rb.velocity = new Vector2(groundSpeed, rb.velocity.y);
            rb.angularVelocity = 0;
        }
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
                rb.AddForceAtPosition(contactForce, contact.point, ForceMode2D.Impulse);
            }
        }
    }
}