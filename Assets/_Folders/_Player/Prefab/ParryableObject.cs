using UnityEngine;

public class ParryableObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private bool hasBeenBlocked = false;
    private bool hitEnemy = false;
    public int id;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        hasBeenBlocked = false;
        hitEnemy = false;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasBeenBlocked) return;
        if (collider.gameObject.CompareTag("Block"))
        {
            hasBeenBlocked = true;
            if (!hitEnemy)
                AudioManager.instance.PlayParrySound();

            Vector2 normal = (transform.position - collider.transform.position).normalized;

            // Reflect current velocity across that normal
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity * .7f, normal);

            return;
        }

        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null && !hasBeenBlocked)
        {


            damageableEntity.Damage(1, 1, id);
            hitEnemy = true;
            return;

        }

        IExplodable explodable = collider.gameObject.GetComponent<IExplodable>();
        if (explodable != null && !hasBeenBlocked)
        {

            explodable.Explode(false);
            return;

        }

    }

}
