using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPigMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float initialY;


    private Vector2 addForce;
    private bool addedForce = false;
    public float speed;
    public float yForce;
    public float distanceToFlap;


    private static readonly int FlapTriggerHash = Animator.StringToHash("FlapTrigger");



    [Header("Positions")]
    [SerializeField] private Transform wingPosition;
    [SerializeField] private Transform legPosition;
    [SerializeField] private Transform tailPosition;

    [Header("Objects")]
    [SerializeField] private Transform Body;
    [SerializeField] private Transform wingObject;
    [SerializeField] private Transform legObject;
    [SerializeField] private Transform tailObject;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public void InitializeObject()
    {

    }

    private void OnEnable()
    {
        Body.localScale = new Vector3(1 + ((.9f - transform.localScale.x) * .7f), 1, 1);
        wingObject.position = wingPosition.position;
        legObject.position = legPosition.position;
        tailObject.position = tailPosition.position;
        initialY = transform.position.y;
        rb.velocity = new Vector2(-speed, 0);
        addForce = new Vector2(-speed, yForce);

    }


    // Update is called once per frame


    void Update()
    {
        if (speed > 0)
        {
            if (transform.position.x < BoundariesManager.leftBoundary)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (transform.position.x > BoundariesManager.rightBoundary)
            {
                gameObject.SetActive(false);
            }
        }


        if (transform.position.y < (initialY - distanceToFlap))
        {
            rb.velocity = addForce;


            if (!addedForce)
            {
                anim.SetTrigger(FlapTriggerHash);
                addedForce = true;
            }
        }
        else if (transform.position.y > (initialY - distanceToFlap) && addedForce)
        {
            addedForce = false;
        }



    }
}
