using UnityEngine;
using System;

public class RingMovement : MonoBehaviour, ICollectible
{
    // public int doesTriggerInt;

    // public float xCordinateTrigger;
    // private bool hasNotTriggered;
    public RingID ID;
    public int index => ID.IDIndex;

    public float speed;
    public bool correctCollision = false;
    public int order;
    private bool isCorrect = false;



    private Transform _transform;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer backRing;

    [SerializeField] private SpriteRenderer centerSprite;
    [SerializeField] private SpriteRenderer centerCutout;


    private bool isFaded = false;
    private Collider2D col;
    // private Rigidbody2D rb;

    private SpriteRenderer sprite;
    private Animator anim;

    // Declare the hash
    private static readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    private static readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<EdgeCollider2D>();

    }


    private void Update()
    {
        _transform.position += Vector3.left * speed * Time.deltaTime;


        // if (doesTriggerInt != 0 && hasNotTriggered)
        // {
        //     if ((speed > 0 && _transform.position.x < xCordinateTrigger) || (speed < 0 && _transform.position.x > xCordinateTrigger))
        //     {
        //         ID.ringEvent.OnRingTrigger?.Invoke(doesTriggerInt);
        //         hasNotTriggered = false;


        //     }

        // }

        if (speed > 0)
        {

            if (isCorrect && _transform.position.x < BoundariesManager.leftViewBoundary)
            {
                // Debug.Log($"Passed: {isCorrect} from GameObject {gameObject.name} at frame {Time.frameCount}");
                HandleCorrectRing();
                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);
                return;
            }
            else if (_transform.position.x < BoundariesManager.leftBoundary)
            {

                gameObject.SetActive(false);
            }

        }

        else
        {
            if (isCorrect && _transform.position.x > BoundariesManager.rightViewBoundary)
            {
                HandleCorrectRing();

                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);





            }

            else if (_transform.position.x > BoundariesManager.rightBoundary)
            {

                gameObject.SetActive(false);
            }


        }


    }

    private void HandleCorrectRing()
    {
        if (isCorrect)
        {
            isCorrect = false;

            if (index == 0)
            {
                ID.ringEvent.OnGetBall?.Invoke(transform.position);
                sprite.material = ID.passedMaterial;
                backRing.material = ID.passedMaterial;
            }




        }
    }



    public void SetCorrectRing()
    {
        if (order == ID.CorrectRing)
        {
            sprite.material = ID.highlightedMaterial;
            backRing.material = ID.highlightedMaterial;

            isCorrect = true;
        }

    }
    public void Collected()
    {

        if (order == ID.CorrectRing)
        {
            correctCollision = true;
            isCorrect = false;
            ID.CorrectRing++;

            col.enabled = false;

            AudioManager.instance.PlayRingPassSound(order);
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;
            Debug.Log(isCorrect);
            ID.ringEvent.OnCheckOrder?.Invoke();

        }

        // else
        // {
        //     ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);
        //     isCorrect = false;

        // }

        // print("ring" + order);
    }



    public void NewSetup(bool correctSequence, int index)
    {

        if (correctSequence == false)
        {
            // hasNotTriggered = false;
            col.enabled = false;
            isFaded = true;
            HandleCorrectRing();

            anim.SetBool(FadeCenterHash, true);
        }


    }

    private void DisableRings(int indexVar)
    {
        if (indexVar == index)
        {
            gameObject.SetActive(false);
        }
    }



    public int GetOrder()
    {
        return order;
    }

    private void OnEnable()
    {
        // rb.velocity = Vector2.left * speed;
        correctCollision = false;
        sprite.material = ID.defaultMaterial;
        backRing.material = ID.defaultMaterial;
        centerSprite.color = ID.CenterColor;
        centerCutout.color = ID.CenterColor;
        Debug.Log("RingParent is: " + ID);



        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;
        ID.ringEvent.DisableRings += DisableRings;

        col.enabled = true;

        SetCorrectRing();
        // hasNotTriggered = true;

    }

    void OnDisable()
    {
        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;
        ID.ringEvent.DisableRings -= DisableRings;


        if (!isFaded)
        {
            anim.SetBool(BurstBoolHash, false);
        }
        else
        {
            anim.SetBool(FadeCenterHash, false);
            isFaded = false;

        }
        // isCorrect = false;
    }
}
