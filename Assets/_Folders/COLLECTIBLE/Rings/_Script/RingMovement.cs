using UnityEngine;
using System;

public class RingMovement : MonoBehaviour
{
    public int doesTriggerInt;

    public int index = -1;
    public float xCordinateTrigger;
    private bool hasNotTriggered;
    public RingID ID;
    public float speed;
    public bool correctCollision = false;
    public int order;
    private bool isCorrect = false;
    // private bool _isCorrect = false; // Backing field for the property

    // public bool isCorrect
    // {
    //     get => _isCorrect;
    //     set
    //     {
    //         // Check if the new value is different from the current value
    //         if (_isCorrect != value && order ==1)
    //         {
    //             // Log the change with a stack trace
    //             Debug.Log($"isCorrect changed from {_isCorrect} to {value} at frame {Time.frameCount}\nStack Trace: {Environment.StackTrace}");

    //             _isCorrect = value; // Update the value
    //         }
    //     }
    // }
    [SerializeField] private Collider2D coll2D;

    private Transform _transform;
    [SerializeField] private SpriteRenderer backRing;

    [SerializeField] private SpriteRenderer centerSprite;
    [SerializeField] private SpriteRenderer centerCutout;


    private bool isFaded = false;
    private Collider2D[] colliders;

    private SpriteRenderer sprite;
    private Animator anim;

    // Declare the hash
    private static readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    private static readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private void Awake()
    {
        _transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
    }

    private void Update()
    {
        _transform.position += Vector3.left * speed * Time.deltaTime;


        if (doesTriggerInt != 0 && hasNotTriggered)
        {
            if ((speed > 0 && _transform.position.x < xCordinateTrigger) || (speed < 0 && _transform.position.x > xCordinateTrigger))
            {
                ID.ringEvent.OnRingTrigger?.Invoke(doesTriggerInt);
                hasNotTriggered = false;


            }

        }

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

    private void DisableColliders()
    {

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
    private void EnableColliders()
    {

        foreach (var collider in colliders)
        {
            collider.enabled = true;
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
    public void CheckOrder()
    {

        if (order == ID.CorrectRing)
        {
            correctCollision = true;
            isCorrect = false;
            ID.CorrectRing++;

            DisableColliders();
            
            AudioManager.instance.PlayRingPassSound();
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;
            Debug.Log(isCorrect);
            ID.ringEvent.OnCheckOrder?.Invoke();

        }

        else
        {
            ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);
            isCorrect = false;
           
        }

        // print("ring" + order);
    }



    public void NewSetup(bool correctSequence, int index)
    {

        if (correctSequence == false)
        {
            hasNotTriggered = false;

            DisableColliders();
            isFaded = true;
            HandleCorrectRing();

            anim.SetBool(FadeCenterHash, true);
        }


    }



    public int GetOrder()
    {
        return order;
    }

    private void OnEnable()
    {
        correctCollision = false;
        sprite.material = ID.defaultMaterial;
        backRing.material = ID.defaultMaterial;
        centerSprite.color = ID.CenterColor;
        centerCutout.color = ID.CenterColor;
       
        index = ID.IDIndex;

        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;

        EnableColliders();
        SetCorrectRing();
        hasNotTriggered = true;

    }

    void OnDisable()
    {
        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;
        index = -1;
        if (!isFaded)
        {
            anim.SetBool(BurstBoolHash, false);
        }
        else
        {
            anim.SetBool(FadeCenterHash, false);
            isFaded = false;

        }

        if (sprite != null)
        {

            coll2D.enabled = true;
        }
        // isCorrect = false;
    }
}
