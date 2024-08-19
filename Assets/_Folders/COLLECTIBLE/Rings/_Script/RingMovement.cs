using UnityEngine;
using System;
using System.Collections;

public class RingMovement : MonoBehaviour, ICollectible
{
    // public int doesTriggerInt;

    // public float xCordinateTrigger;
    // private bool hasNotTriggered;
    public RingID ID;
    public int index => ID.IDIndex;
    [SerializeField] private SpriteRenderer center;
    // [SerializeField] private SpriteRenderer burst;
    private Vector3 startBurstScale = new Vector3(.5f, 1, 1);
    private Vector3 endBurstScale = new Vector3(1.1f, 1, 1);


    public float speed;
    public bool correctCollision = false;
    public int order;
    private bool isCorrect = false;



    private Transform _transform;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer backRing;




    private bool isFaded = false;
    private Collider2D col;
    // private Rigidbody2D rb;

    private SpriteRenderer sprite;


    // Declare the hash
    // private static readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    // private static readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
      
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

    private IEnumerator FadeCenter()
    {
        // Duration of the fade
        float elapsedTime = 0f;

        Color startColor = center.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Target color with 0 alpha

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            center.color = Color.Lerp(startColor, endColor, elapsedTime / 1);
            // burst.color = center.color;
            yield return null;
        }

        center.color = endColor;
        // burst.color = endColor;

    }

    private IEnumerator BurstCenter()
    {
        // Duration of the fade
        float elapsedTime = 0f;


        ID.GetParticles(transform, speed);
        Color startColor = center.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, .4f); // Target color with 0 alpha

        while (elapsedTime < .22f)
        {
            elapsedTime += Time.deltaTime;
            center.transform.localScale = Vector3.Lerp(startBurstScale, endBurstScale, elapsedTime / .22f);
            center.color = Color.Lerp(startColor, endColor, elapsedTime / .22f);
            // burst.color = center.color;

            yield return null;
        }
        
        elapsedTime = 0;
        startColor = endColor;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        while (elapsedTime < .2f)
        {
            elapsedTime += Time.deltaTime;
            center.transform.localScale = Vector3.Lerp(endBurstScale, startBurstScale, elapsedTime / .2f);
            center.color = Color.Lerp(startColor, endColor, elapsedTime / .2f);
            // burst.color = center.color;
            yield return null;

        }

        center.color = endColor;
        // burst.color = center.color;

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
            StartCoroutine(BurstCenter());
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

            StartCoroutine(FadeCenter());
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
        center.color = ID.CenterColor;
        // burst.color = ID.CenterColor;
        // burst.transform.localScale = startBurstScale;




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


        // if (!isFaded)
        // {
        //     anim.SetBool(BurstBoolHash, false);
        // }
        // else
        // {
        //     anim.SetBool(FadeCenterHash, false);
        //     isFaded = false;

        // }
        // isCorrect = false;
    }
}
