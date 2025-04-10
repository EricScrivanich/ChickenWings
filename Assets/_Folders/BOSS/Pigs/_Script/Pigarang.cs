using UnityEngine;
using PathCreation;

public class Pigarang : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private AnimationCurve speedCurve;

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
    private PathCreator path;
    private float time;
    private float totalTime;
    private float dynmamicSpeed;

    private float distanceTravelled = 0f;
    private bool flipDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    public void ThrowUsingPath(PathCreator p, float totalT, bool flipped)
    {
        path = p;
        flipDirection = flipped;
        if (flipped)
        {
            rb.angularVelocity = -spinSpeed;
            distanceTravelled = 1;
        }
        else rb.angularVelocity = spinSpeed;
        totalTime = totalT;


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
        if (path != null)
        {
            time += Time.fixedDeltaTime * dynmamicSpeed;

            float p = Mathf.InverseLerp(0, totalTime, time);
            dynmamicSpeed = speedCurve.Evaluate(p);

            if (flipDirection) p = 1 - p;

            rb.MovePosition(path.path.GetPointAtTime(p));


            if (p >= 1 && !flipDirection)
            {
                Destroy(gameObject);
            }
            else if (p <= 0 && flipDirection)
            {
                Destroy(gameObject);
            }


        }



        // if (rb.linearVelocity.magnitude < peakedMag && !peaked)
        // {
        //     peaked = true;
        //     returnSpeed = finalReturnSpeed;
        // }
        // if (returning)
        // {
        //     rb.AddForce(returnDirection * returnSpeed);

        //     if (peaked && rb.linearVelocity.magnitude > maxReturnSpeed)
        //     {
        //         returning = false;

        //     }
        // }

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
