using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotPig : MonoBehaviour
{

    public enum FlightMode
    {
        GlideUp,
        GlideDown,
        UpDown,
        DownUp,
        Straight
    }
    private bool goUp;
    private bool goDown;
    private bool downUp;
    private bool upDown;
    
    private bool straighten;

    // [SerializeField] private float speed;
    [SerializeField] private Vector2 initialSpeed;
    [SerializeField] private float addForceY;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    private Vector2 addForceVector;
    [SerializeField] private float maxYSpeed;
    private float maxYSpeedUp;
    private float maxYSpeedDown;
    private string animType;
    private string animType2;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private float xTrigger;
    private bool startMotion;


    // Add a public variable to select the flight mode in the inspector
    public FlightMode currentFlightMode;

    // Start is called before the first frame update
    void Start()
    {
        startMotion = false;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        switch (currentFlightMode)
        {
            case FlightMode.GlideUp:
                // Implement logic for GlideUp
                anim.SetBool("GlidingUpBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = 0;
                animType = "GoUp";
                addForceVector = new Vector2(0, addForceY);
                break;
            case FlightMode.GlideDown:
                // Implement logic for GlideDown
                anim.SetBool("GlidingDownBool", true);
                maxYSpeedUp = 0;
                maxYSpeedDown = -maxYSpeed;
                animType = "GoDown";

                addForceVector = new Vector2(0, -addForceY);


                break;
            case FlightMode.UpDown:
                // Implement logic for GlideDown
                anim.SetBool("GlidingUpBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = -maxYSpeed;
                upDown = true;
                animType = "GoUp";
               

                addForceVector = new Vector2(0, addForceY);


                break;

            case FlightMode.DownUp:
                // Implement logic for GlideDown
                anim.SetBool("GlidingDownBool", true);
                maxYSpeedUp = maxYSpeed;
                maxYSpeedDown = -maxYSpeed;
                downUp = true;
                animType = "GoDown";
              

                addForceVector = new Vector2(0, -addForceY);


                break;
            case FlightMode.Straight:
                // Implement logic for Straight
                addForceVector = new Vector2(0, 0);

                break;
        }

        rb.velocity = initialSpeed;


    }

    private void FixedUpdate()
    {

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, maxYSpeedDown, maxYSpeedUp));

        if (startMotion)
        {

            rb.AddForce(addForceVector);

        }



    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < xTrigger && !startMotion)
        {
            startMotion = true;
            anim.SetTrigger(animType);
            switch (currentFlightMode)
            {
                case FlightMode.GlideUp:
                    // Implement logic for GlideUp
                    goUp = true;
                    break;
                case FlightMode.GlideDown:
                    // Implement logic for GlideDown
                    goDown = true;


                    break;
                case FlightMode.Straight:
                    // Implement logic for Straight


                    break;
            }

        }

        if (((goUp && transform.position.y > maxY) || (goDown && transform.position.y < minY)) && !straighten)
        {
            straighten = true;
            addForceVector *= -1;
        }

        else if (downUp && transform.position.y < minY || upDown && transform.position.y > maxY)
        {
            if (downUp)
            {
                anim.SetTrigger("GoUp");
            }
            else{
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

