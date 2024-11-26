using UnityEngine;

public class Bombs : MonoBehaviour
{
    public PlaneManagerID ID;
    // private DropBomb dropBombController;
    private Animator anim;

    private Vector2 dropForceAverage = new Vector2(.2f, .2f);

    private Rigidbody2D rb;

    private bool isDropped;

    private float rotationSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }


    private void FixedUpdate()
    {
        // Only rotate if the object is moving
        if (rb.linearVelocity != Vector2.zero && !isDropped)
        {
            // Get the angle in degrees of the current velocity vector
            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, rb.linearVelocity.normalized);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void GetBomb(float force, bool dropped, int side)
    {
        
        gameObject.SetActive(true);
        if (dropped)
        {

            rb.linearVelocity = force * dropForceAverage;
            isDropped = true;
            rb.angularVelocity = 50 * side;

        }

        else
        {
            isDropped = false;
            rb.angularVelocity = 0;


            rotationSpeed = 100;
            rb.linearVelocity = transform.up * force;
        }

    }




}
