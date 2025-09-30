using UnityEngine;

public class ShotgunParticleNew : MonoBehaviour
{
    [SerializeField] private ShotgunParticleID particleID;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    float timer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // spriteRenderer.sprite = particleID.sprites[0];
        timer = 0f;

    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        float lifeTimePercent = timer / particleID.lifeTime;
        if (lifeTimePercent >= 1f)
        {
            this.gameObject.SetActive(false);
            return;
        }
        //scale
        float scale = Mathf.Lerp(particleID.startScale, particleID.endScale, lifeTimePercent);
        this.transform.localScale = Vector3.one * scale;
        //color
        spriteRenderer.color = Color.Lerp(particleID.startColor, particleID.endColor, lifeTimePercent);

    }
    private bool hasBeenBlocked = false;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Block"))
        {
            hasBeenBlocked = true;
            AudioManager.instance.PlayParrySound();

            Vector2 normal = (transform.position - collider.transform.position).normalized;

            // Reflect current velocity across that normal
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity * .7f, normal);

            return;
        }

        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null && !hasBeenBlocked)
        {
            int type = 1;

            damageableEntity.Damage(1, 1, 0);

        }
    }

    // Update is called once per frame

}
