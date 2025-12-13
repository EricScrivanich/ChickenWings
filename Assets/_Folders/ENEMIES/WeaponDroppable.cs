using UnityEngine;

public class WeaponDroppable : MonoBehaviour
{

    [SerializeField] private bool activateCollider;
    private bool isDuplicated = false;
    private Rigidbody2D rb;
    [SerializeField] private float contactForce;
    [SerializeField] private bool destroyOnFinish = true;
    public void DuplicateObject(Vector2 vel)
    {

        GameObject duplicate = Instantiate(gameObject, transform.position, transform.rotation);
        duplicate.GetComponent<WeaponDroppable>().ActivateObject(vel);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;


    }
    void OnDisable()
    {
        if (!isDuplicated)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;

        }
    }

    public void ActivateOnly()
    {
        rb = GetComponent<Rigidbody2D>();
        this.enabled = true;
    }

    void FixedUpdate()
    {
        if (onGround && rb.linearVelocity.x > -BoundariesManager.GroundSpeed)
        {
            rb.AddForce(Vector2.left * BoundariesManager.GroundSpeed);
            // if (rb.linearVelocityX < -BoundariesManager.GroundSpeed)
            // {

            //     rb.linearVelocityX = -BoundariesManager.GroundSpeed;
            // }
        }
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            if (destroyOnFinish)
                Destroy(gameObject);
            else
            {
                this.enabled = false;
                gameObject.SetActive(false);

            }

        }

    }
    private bool onGround = false;
    private bool maxSpeed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //add force at contact point
        if (rb.linearVelocity.x > -BoundariesManager.GroundSpeed)
        {
            rb.AddForce(Vector2.right * contactForce * (-BoundariesManager.GroundSpeed - rb.linearVelocity.x), ForceMode2D.Impulse);
            rb.angularVelocity += Random.Range(-90f, 90f);
        }


        // if (!onGround)
        //     rb.AddForceAtPosition(Vector2.left), collision.contacts[0].point, ForceMode2D.Impulse);
        onGround = true;

    }
    private void OnCollisionExit2D(Collision2D other)
    {
        onGround = false;
    }


    public void ActivateObject(Vector2 vel)
    {
        this.enabled = true;
        isDuplicated = true;
        if (activateCollider)
        {
            GetComponent<Collider2D>().enabled = true;
        }
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(-200f, 200f);
        rb.linearVelocity = vel * .8f;

        rb.simulated = true;
    }
}
