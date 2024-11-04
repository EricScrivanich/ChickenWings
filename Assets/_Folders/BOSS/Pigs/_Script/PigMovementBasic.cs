using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovementBasic : MonoBehaviour
{
    [Header("SpriteObjects")]
    [SerializeField] private Transform frontLegs;
    [SerializeField] private Transform backLegs;
    [SerializeField] private Transform wings;
    [SerializeField] private Transform tail;
    [SerializeField] private Transform body;
    [Header("SpritePositions")]
    [SerializeField] private Transform frontLegsPosition;
    [SerializeField] private Transform backLegsPosition;
    [SerializeField] private Transform wingsPosition;
    [SerializeField] private Transform tailPosition;




    [Header("MovementRB")]

    [SerializeField] private float addYForce;
    private bool hasInitialized = false;

    private Vector2 jumpForce;
    [SerializeField] private float belowAmount;
    private bool justJumped;





    [Header("Movement")]


    [SerializeField] private float ySpeed;
    public float xSpeed;
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    private int direction = 1;

    [SerializeField] private float R; //slope_up_slope_down_ratio

    private float initialY;


    // [SerializeField] private float duration;
    // [SerializeField] private float invokeTime;
    private Animator anim;

    private bool downGlide;
    private bool upGlide;
    private float time;
    private bool sinDown;

    [SerializeField] private float sinDownSpeed;


    // Start is called before the first frame update
    void Start()
    {


        jumpForce = new Vector2(xSpeed, addYForce);


        upGlide = false;
        downGlide = false;

        // rb.velocity = new Vector2(xSpeed, 0);
        justJumped = false;

        // anim.SetTrigger("FlapTrigger");
        // Invoke("GlideDown", invokeTime);
        // Invoke("GlideUp", invokeTime);

    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }

    public void InitializePig()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        initialY = transform.position.y;
        // anim.speed = 0;

        // Invoke("SetAnimSpeed", Random.Range(0f, .5f));


        body.localScale = new Vector3(((.8f - Mathf.Abs(transform.localScale.x)) * .8f) + 1, 1, 1);

        frontLegs.position = frontLegsPosition.position;
        backLegs.position = backLegsPosition.position;
        wings.position = wingsPosition.position;
        tail.position = tailPosition.position;
        hasInitialized = true;


        gameObject.SetActive(true);

    }

    private void OnEnable()
    {
        if (!hasInitialized)
        {
            initialY = transform.position.y;




            if (body != null)
            {
                body.localScale = new Vector3(((.85f - transform.localScale.x) * .5f) + 1, 1, 1);
                frontLegs.position = frontLegsPosition.position;
                backLegs.position = backLegsPosition.position;
                wings.position = wingsPosition.position;
                tail.position = tailPosition.position;
            }


        }
    }

    private void SetAnimSpeed()
    {
        anim.speed = 1;
    }
    // private void OnEnable()
    // {
    //     // sinDown = false;


    //     if (hasInitialized)
    //     {
    //         Debug.Log("Initilized");
    //         initialY = transform.position.y;


    //         body.localScale = new Vector3(.75f - transform.localScale.x + 1, 1, 1);

    //         backLegs.position = backLegsPosition.position;
    //         wings.position = wingsPosition.position;
    //         tail.position = tailPosition.position;

    //     }

    //     // if (initialY > 3f)
    //     // {
    //     //     float r = Random.Range(0f, 1f);
    //     //     if (r > .4f)
    //     //     {

    //     //         sinDown = true;
    //     //     }
    //     // }
    // }

    private void GlideDown()
    {
        downGlide = true;
        anim.SetBool("DownGlideBool", true);
        anim.SetTrigger("GoDownTrigger");

    }
    private void GlideUp()
    {
        upGlide = true;
        anim.SetBool("UpGlideBool", true);
        anim.SetTrigger("GoUp");


    }
    // Update is called once per frame

    void ChangeJustJumped()
    {
        justJumped = false;
    }



    void Update()
    {



        // if (initialY - transform.position.y > belowAmount && !justJumped)
        // {
        //     // rb.AddForce(new Vector2(0, addYForce), ForceMode2D.Impulse);
        //     rb.velocity = jumpForce;
        //     anim.SetTrigger("FlapTrigger");
        //     justJumped = true;
        //     Invoke("ChangeJustJumped", .2f);

        // }

        // float B = 0;
        // if (R == -1)
        //     Debug.Log("R cannot be -1");
        // else
        //     B = (1 - R) / (1 + R);
        // transform.Translate(Vector2.left * xSpeed * Time.deltaTime);
        // float omega_t = 2 * Mathf.PI * (-transform.position.x) / xSpeed * frequency;
        // float y = Mathf.Sin(omega_t - B * Mathf.Sin(omega_t)) * amplitude + initialY;
        // transform.position = new Vector2(transform.position.x, y);
        // if (y > initialY && direction == -1)
        // {
        //     anim.SetTrigger("FlapTrigger");
        //     direction = 1;
        // }
        // else if (y < initialY && direction == 1)
        // {
        //     direction = -1;
        // }




        transform.Translate(Vector2.left * xSpeed * Time.deltaTime);


        float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
        transform.position = new Vector2(transform.position.x, y);



        if (downGlide)
        {

            time += Time.deltaTime;
            transform.Translate(Vector2.down * ySpeed * Time.deltaTime);


            // if (time > duration)
            // {
            //     anim.SetBool("UpGlideBool", true);
            //     anim.SetBool("DownGlideBool", false);
            //     downGlide = false;
            //     upGlide = true;
            //     time = 0;


            // }
        }
        else if (upGlide)
        {
            time += Time.deltaTime;
            transform.Translate(Vector2.up * ySpeed * Time.deltaTime);
        }

        // else if (upGlide)
        // {
        //     time += Time.deltaTime;
        //     transform.Translate(Vector2.up * ySpeed * Time.deltaTime);

        //     if (time > duration)
        //     {
        //         anim.SetBool("UpGlideBool", false);
        //         time = 0;
        //         upGlide = false;
        //         Invoke("GlideDown", invokeTime);


        //     }

        // }

        // if (upGlide)
        // {

        //     time += Time.deltaTime;
        //     transform.Translate(Vector2.up * ySpeed * Time.deltaTime);


        //     if (time > duration)
        //     {
        //         anim.SetBool("DownGlideBool", true);
        //         anim.SetBool("UpGlideBool", false);
        //         upGlide = false;
        //         downGlide = true;

        //         time = 0;


        //     }
        // }

        // else if (downGlide)
        // {
        //     time += Time.deltaTime;
        //     transform.Translate(Vector2.down * ySpeed * Time.deltaTime);

        //     if (time > duration)
        //     {
        //         anim.SetBool("DownGlideBool", false);
        //         time = 0;
        //         downGlide = false;
        //         Invoke("GlideUp", invokeTime);


        //     }

        // }

    }
}
