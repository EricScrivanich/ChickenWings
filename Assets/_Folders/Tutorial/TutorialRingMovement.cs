using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRingMovement : MonoBehaviour
{
    public RingID ID;

    [SerializeField] private Collider2D coll2D;
    

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
        coll2D = GetComponent<Collider2D>();


    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void CheckOrder()
    {
        if (finalOrder == 1)
        {
            ID.ringEvent.tutorialRingPass?.Invoke();
            ID.CorrectRing = 1;
            AudioManager.instance.PlayRingPassSound(order);
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;
        }
        if (order == 1)
        {
            ID.CorrectRing = 2;
            AudioManager.instance.PlayRingPassSound(order);
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;
        }

        else if (order == ID.CorrectRing)
        {

            ID.CorrectRing++;

            coll2D.enabled = true;


            AudioManager.instance.PlayRingPassSound(order);
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;

            if (ID.CorrectRing == finalOrder + 1)
            {
                ID.ringEvent.tutorialRingPass?.Invoke();


            }



        }

        else
        {
            ID.CorrectRing = 1;

        }

        // print("ring" + order);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        CheckOrder();
    }

    private void RingEffect()
    {
        
    }


    private void OnEnable()
    {
        sprite.material = ID.defaultMaterial;
        backRing.material = ID.defaultMaterial;
        centerSprite.color = ID.CenterColor;
        centerCutout.color = ID.CenterColor;
        coll2D.enabled = true;
    }

    private void OnDisable()
    {


    }
}
