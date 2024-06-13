using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRingMovement : MonoBehaviour, ICollectible
{
    public RingID ID;

    private EdgeCollider2D coll2D;

    private RingParent parent;
    private ParticleSystem ps;


    [SerializeField] private int order;


    [SerializeField] private int finalOrder;

    private Transform _transform;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer backRing;

    [SerializeField] private SpriteRenderer centerSprite;
    [SerializeField] private SpriteRenderer centerCutout;

    private readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    private readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private bool isFaded = false;

    // private Rigidbody2D rb;

    private SpriteRenderer sprite;
    private Animator anim;
    // Start is called before the first frame update


    private void Awake()
    {

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll2D = GetComponent<EdgeCollider2D>();


    }
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        parent = GetComponentInParent<RingParent>();
    }

    // Update is called once per frame
    public void CheckForHighlight(int correctRing)
    {
        if (order == correctRing)
        {
            sprite.material = ID.highlightedMaterial;
            backRing.material = ID.highlightedMaterial;
        }
    }

    public void CheckOrder(bool isCorrect)
    {
        if (isCorrect)
        {

            AudioManager.instance.PlayRingPassSound(order);
            ps.Play();
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;

        }


        // print("ring" + order);
    }

    public void Collected()
    {
        CheckOrder(parent.CheckOrder(order));


    }



    // print("ring" + order);




    private void RingEffect()
    {

    }


    private void OnEnable()
    {
        if (order == 1)
        {
            sprite.material = ID.highlightedMaterial;
            backRing.material = ID.highlightedMaterial;

        }
        else
        {
            sprite.material = ID.defaultMaterial;
            backRing.material = ID.defaultMaterial;
        }

        centerSprite.color = ID.CenterColor;
        centerCutout.color = ID.CenterColor;
        coll2D.enabled = true;
        // if (order == parent.correctRing)
        // {
        //     sprite.material = ID.highlightedMaterial;
        //     backRing.material = ID.highlightedMaterial;
        // }



    }

    // private void OnDisable()
    // {


    // }
}
