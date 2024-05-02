using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigBasicMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private PigID pig;
    private float duration;
    private bool jumpedLeft;
    private float time;
    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


    }
    private void OnEnable()
    {
        jumpedLeft = false;
        duration = Random.Range(2, 4);
        time = 0;
        anim.SetTrigger("Reset");

        transform.position = new Vector2(.5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > duration)
        {
            anim.SetTrigger("Bite");
            duration = Random.Range(7, 10);
            time = 0;

        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Jump();

    }

    private void Jump()
    {

        float xVel;
        if (jumpedLeft)
        {
            xVel = .5f;
            jumpedLeft = false;

        }
        else
        {
            xVel = -.5f;
            jumpedLeft = true;

        }
        anim.SetTrigger("Flap");
        rb.velocity = new Vector2(xVel, pig.jumpForce);

    }
}
