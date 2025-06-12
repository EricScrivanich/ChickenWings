using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotPig : MonoBehaviour
{

    // public enum FlightMode
    // {
    //     GlideUp,
    //     GlideDown,
    //     UpDown,
    //     DownUp,
    //     Straight
    // }

    [Header("0: Glide Up, 1: GlideDown, 2: UpDown, 3: DownUp, 4: Straight")]
    public int flightMode;
    private bool goUp;
    private bool goDown;
    private bool downUp;
    private bool upDown;

    private bool straighten;


    // [SerializeField] private float speed;
    public float initialSpeed;
    public float addForceY;
    public float minY;
    public float maxY;
    private Vector2 addForceVector;
    public float maxYSpeed;
    private float maxYSpeedUp;
    private float maxYSpeedDown;
    private string animType;

    private Rigidbody2D rb;
    private Animator anim;
    public float xTrigger;
    private bool startMotion;


    // Add a public variable to select the flight mode in the inspector
    // public FlightMode currentFlightMode;

    // Start is called before the first frame update

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();


        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void FixedUpdate()
    {

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, maxYSpeedDown, maxYSpeedUp));

        if (startMotion)
        {

            rb.AddForce(addForceVector);

        }
    }

    private void OnDisable()
    {
        anim.SetBool("GlidingUpBool", false);
        anim.SetBool("GlidingDownBool", false);

        anim.SetTrigger("Restart");

        goUp = false;
        goDown = false;
        downUp = false;
        upDown = false;

        startMotion = false;

    }

    private void OnEnable()
    {
        startMotion = false;
        // if (initialSpeed < 0)
        // {
        //     transform.eulerAngles = new Vector3(0, 180, 0);
        // }

        rb.linearVelocity = new Vector2(-initialSpeed, 0);


        if (xTrigger == 0) xTrigger = transform.position.x - ((Mathf.Abs(initialSpeed) / initialSpeed) * .5f);


        switch (flightMode)
        {

            case 0:
                // Implement logic for GlideUp
                anim.SetBool("GlidingUpBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = 0;
                animType = "GoUp";
                addForceVector = new Vector2(0, addForceY);
                break;
            case 1:
                // Implement logic for GlideDown
                anim.SetBool("GlidingDownBool", true);
                maxYSpeedUp = 0;
                maxYSpeedDown = -maxYSpeed;
                animType = "GoDown";

                addForceVector = new Vector2(0, -addForceY);


                break;
            case 2:
                // Implement logic for GlideDown
                anim.SetBool("GlidingUpBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = -maxYSpeed;
                upDown = true;
                animType = "GoUp";


                addForceVector = new Vector2(0, addForceY);


                break;

            case 3:
                // Implement logic for GlideDown
                anim.SetBool("GlidingDownBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = -maxYSpeed;
                downUp = true;
                animType = "GoDown";


                addForceVector = new Vector2(0, -addForceY);


                break;
            case 4:
                // Implement logic for Straight
                addForceVector = new Vector2(0, 0);

                break;
        }




    }

    // Update is called once per frame
    void Update()
    {

        if ((initialSpeed > 0 && transform.position.x < xTrigger && !startMotion) || (initialSpeed < 0 && transform.position.x > xTrigger && !startMotion))
        {
            startMotion = true;
            anim.SetTrigger(animType);
            switch (flightMode)
            {
                case 0:
                    // Implement logic for GlideUp
                    goUp = true;
                    break;
                case 1:
                    // Implement logic for GlideDown
                    goDown = true;


                    break;
                case 4:
                    // Implement logic for Straight


                    break;
            }

        }

        if (((goUp && transform.position.y > maxY) || (goDown && transform.position.y < minY)) && !straighten)
        {
            straighten = true;
            anim.SetBool("GlidingUpBool", false);
            anim.SetBool("GlidingDownBool", false);
            anim.SetTrigger("Revert");
            addForceVector *= -1;
        }

        else if (downUp && transform.position.y < minY || upDown && transform.position.y > maxY)
        {
            if (downUp)
            {
                anim.SetTrigger("GoUp");
            }
            else
            {
                anim.SetTrigger("GoDown");
            }
            downUp = !downUp;
            upDown = !upDown;
            addForceVector *= -1;
        }
        // else if (startMotion)
        // {
        //             switch (currentFlightMode)
        //             {
        //                 case FlightMode.GlideUp:
        //                     // Implement logic for GlideUp
        //                    if (transform.position.y > maxY)
        //                    {

        //                    }
        //                     break;
        //                 case FlightMode.GlideDown:
        //                     // Implement logic for GlideDown



        //                     break;
        //                 case FlightMode.Straight:
        //                     // Implement logic for Straight


        //                     break;
        //             }


        // }
    }
}

