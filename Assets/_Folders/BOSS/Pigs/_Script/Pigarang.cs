using UnityEngine;

public class Pigarang : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxDistance = 10f;

    [SerializeField] private float maxArc;
    private Vector2 returnDirection;

    private Vector2 thrownDirection;

    private bool peaked = false;
    private bool returning = false;
    [SerializeField] private float initialReturnSpeed = 10f;
    [SerializeField] private float finalReturnSpeed = 10f;
    [SerializeField] private float peakedMag;
    private float returnSpeed = 10f; // Speed at which the boomerang returns
    [SerializeField] private float maxReturnSpeed;
    [SerializeField] private float spinSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }



    // Update is called once per frame
    public void Throw(Vector2 direction, float force, float arcChange, bool flipped)
    {
        returning = true;
        returnSpeed = initialReturnSpeed;
        thrownDirection = direction;
        if (flipped) transform.localScale = new Vector3(1, -1, 1);
        rb.linearVelocity = direction * force;
        int s = 1;
        if (arcChange > 0) s = -1;
        rb.angularVelocity = spinSpeed * s;
        float angle = Mathf.Atan2(thrownDirection.y, thrownDirection.x) * Mathf.Rad2Deg;
        float arcAngle = Mathf.Abs(Mathf.Sin(2 * angle * Mathf.Deg2Rad)) * arcChange;
        returnDirection = Quaternion.AngleAxis(arcAngle, Vector3.forward) * -thrownDirection;
    }
    private void FixedUpdate()
    {

        if (rb.linearVelocity.magnitude < peakedMag && !peaked)
        {
            peaked = true;
            returnSpeed = finalReturnSpeed;
        }
        if (returning)
        {
            rb.AddForce(returnDirection * returnSpeed);

            if (peaked && rb.linearVelocity.magnitude > maxReturnSpeed)
            {
                returning = false;

            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pig"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
