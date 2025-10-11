using UnityEngine;

public class ShotgunParticleNew : MonoBehaviour
{
    [SerializeField] private ShotgunParticleID particleID;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    float timer = 0f;
    private bool colliderActive = true;
    private CircleCollider2D col;

    private int currentFrameIndex = 0;
    private TrailRenderer tr;
    private int id;
    [SerializeField] private bool playerBullet;
    private ParryableObject parryable;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
        tr = GetComponent<TrailRenderer>();
        if (playerBullet)
            parryable = GetComponent<ParryableObject>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // spriteRenderer.sprite = particleID.sprites[0];
        timer = 0f;

        colliderActive = true;


        col.enabled = true;

        currentFrameIndex = 0;
        Invoke("EmitTrail", .04f);

    }
    private void EmitTrail()
    {
        tr.emitting = true;
    }
    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
    }
    public void Finish()
    {





    }

    public void SetVelocity(float speed, int id)
    {
        this.id = id;
        if (playerBullet) parryable.id = id;
        rb.linearVelocity = ((Vector2)transform.right * speed) + (Vector2.left * .3f);
    }
    public void SetSprite(Sprite s, int type)
    {
        spriteRenderer.sprite = s;
        if (type == 1)
        {

            tr.emitting = false;
            if (!playerBullet)
                col.enabled = false;


            rb.linearVelocity *= .35f;


            transform.localScale *= 1.1f;
        }
        else if (type == 2)
        {
            if (playerBullet)
                colliderActive = false;

        }
        if (type == -1)
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
        }
        else
            transform.localScale *= 1.1f;
    }

    // void FixedUpdate()
    // {
    //     timer += Time.fixedDeltaTime;
    //     float lifeTimePercent = timer / particleID.lifeTime;
    //     if (colliderActive && lifeTimePercent >= 1f)
    //     {
    //         colliderActive = false;
    //         tr.emitting = false;

    //         timer = 0;
    //         rb.linearVelocity *= .5f;
    //         currentFrameIndex = 1;
    //         spriteRenderer.sprite = animData.sprites[currentFrameIndex];
    //         transform.localScale *= 1.1f;
    //         return;
    //     }
    //     else if (!colliderActive)
    //     {
    //         if (timer >= animData.constantSwitchTime)
    //         {
    //             col.enabled = false;
    //             currentFrameIndex++;
    //             if (currentFrameIndex >= animData.sprites.Length)
    //             {
    //                 this.gameObject.SetActive(false);
    //                 spriteRenderer.sprite = animData.sprites[0];
    //                 return;
    //             }

    //             spriteRenderer.sprite = animData.sprites[currentFrameIndex];
    //             transform.localScale *= 1.1f;


    //             timer = 0f;

    //         }

    //     }
    //     //scale
    //     // float scale = Mathf.Lerp(particleID.startScale, particleID.endScale, lifeTimePercent);
    //     // this.transform.localScale = Vector3.one * scale;
    //     // //color
    //     // spriteRenderer.color = Color.Lerp(particleID.startColor, particleID.endColor, lifeTimePercent);

    // }

    // Update is called once per frame

}
