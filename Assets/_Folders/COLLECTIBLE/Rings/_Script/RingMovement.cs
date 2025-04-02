using UnityEngine;
using System;
using System.Collections;

public class RingMovement : MonoBehaviour, ICollectible, IRecordableObject
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



    // private Transform _transform;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer backRing;




    private bool isFaded = false;
    private Collider2D col;
    // private Rigidbody2D rb;

    private SpriteRenderer sprite;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float lerpStartOffset = 5.0f; // Start lerping 5 units before the boundary
    [SerializeField] private float lerpEndBoundary;
    [SerializeField] private float lerpStartBoundary;

    private Vector2 lerpOffsetRangeBasedOnSpeed = new Vector2(3.8f, 7.3f);
    private Vector2 lerpOffsetSpeedBasedOnRange = new Vector2(3, 7.5f);
    private float lerpedSpeedPercentage = .85f;




    // Declare the hash
    // private static readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    // private static readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // _transform = GetComponent<Transform>();

        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<EdgeCollider2D>();


        Debug.Log("lerp start boundary is: " + lerpStartBoundary);




    }


    // private void Update()
    // {
    //     // _transform.position += Vector3.left * speed * Time.deltaTime;


    //     // if (doesTriggerInt != 0 && hasNotTriggered)
    //     // {
    //     //     if ((speed > 0 && _transform.position.x < xCordinateTrigger) || (speed < 0 && _transform.position.x > xCordinateTrigger))
    //     //     {
    //     //         ID.ringEvent.OnRingTrigger?.Invoke(doesTriggerInt);
    //     //         hasNotTriggered = false;


    //     //     }

    //     // }






    // }
    private void FixedUpdate()
    {

        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);

        if (speed > 0)
        {
            if (!correctCollision && transform.position.x <= -lerpStartBoundary)
                LerpSpeedTowardsBoundary();



            if (isCorrect && rb.position.x < BoundariesManager.leftViewBoundary)
            {
                // Debug.Log($"Passed: {isCorrect} from GameObject {gameObject.name} at frame {Time.frameCount}");
                HandleCorrectRing();
                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);
                return;
            }
            else if (rb.position.x < BoundariesManager.leftBoundary)
            {

                gameObject.SetActive(false);
            }

        }

        else
        {
            if (!correctCollision && transform.position.x >= lerpStartBoundary)
                LerpSpeedTowardsBoundary();
            if (isCorrect && rb.position.x > BoundariesManager.rightViewBoundary)
            {
                HandleCorrectRing();

                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);





            }

            else if (rb.position.x > BoundariesManager.rightBoundary)
            {

                gameObject.SetActive(false);
            }


        }


    }

    private void LerpSpeedTowardsBoundary()
    {
        float distanceToBoundary = Mathf.Abs(rb.position.x - lerpEndBoundary);
        float lerpFactor = Mathf.Clamp01(distanceToBoundary / lerpStartOffset);
        speed = Mathf.Lerp(originalSpeed * lerpedSpeedPercentage, originalSpeed, lerpFactor);
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
        Debug.Log("Checking Ring order: " + order + " correct ring: " + ID.CorrectRing);
        if (ID != null && order == ID.CorrectRing)
        {
            Debug.Log("Correct ring: " + order);
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

        if (ID != null)
        {
            sprite.material = ID.defaultMaterial;
            backRing.material = ID.defaultMaterial;
            center.color = ID.CenterColor;
            ID.ringEvent.OnCheckOrder += SetCorrectRing;
            ID.ringEvent.OnCreateNewSequence += NewSetup;
            ID.ringEvent.DisableRings += DisableRings;
            originalSpeed = speed;

            float speedPercentage = Mathf.InverseLerp(lerpOffsetSpeedBasedOnRange.x, lerpOffsetSpeedBasedOnRange.y, MathF.Abs(originalSpeed));
            lerpStartOffset = Mathf.Lerp(lerpOffsetRangeBasedOnSpeed.x, lerpOffsetRangeBasedOnSpeed.y, speedPercentage);

            if (speed > 0)
            {
                lerpEndBoundary = BoundariesManager.leftViewBoundary;
                lerpStartBoundary = lerpEndBoundary + lerpStartOffset;

            }
            else
            {
                lerpEndBoundary = BoundariesManager.rightViewBoundary;
                lerpStartBoundary = lerpEndBoundary - lerpStartOffset;
            }


            // burst.color = ID.CenterColor;
            // burst.transform.localScale = startBurstScale;




            if (col != null)
                col.enabled = true;

            SetCorrectRing();
        }


        // hasNotTriggered = true;

    }

    void OnDisable()
    {
        if (ID != null)
        {
            ID.ringEvent.OnCheckOrder -= SetCorrectRing;
            ID.ringEvent.OnCreateNewSequence -= NewSetup;
            ID.ringEvent.DisableRings -= DisableRings;
        }



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


    public void ApplyRecordedData(RecordedDataStruct data)
    {
        transform.position = data.startPos;
        speed = data.speed;
        int scaleFlip = 1;
        float addedXScale = 0;
        if (data.scale > 1) addedXScale = (data.scale - 1) * .9f;
        if (speed < 0) scaleFlip = -1;
        transform.localScale = new Vector3(1.05f * scaleFlip + addedXScale, data.scale, data.scale);



        transform.eulerAngles = new Vector3(0, 0, data.timeInterval);
        // transform.ro
    }
    public float TimeAtCreateObject(int index)
    {
        return 0;
    }
    public void ApplyFloatOneData(DataStructFloatOne data)
    {
    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {
    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {
        // transform.position = data.startPos;
        transform.SetPositionAndRotation(data.startPos, Quaternion.Euler(0, 0, data.float3));
        speed = data.float1;
        int scaleFlip = 1;
        if (speed < 0) scaleFlip = -1;
        transform.localScale = new Vector3(data.float2 * scaleFlip, data.float2, data.float2);
        // transform.eulerAngles = new Vector3(0, 0, data.float3);
        gameObject.SetActive(true);
    }
    public void ApplyFloatFourData(DataStructFloatFour data)
    {


    }
    public void ApplyFloatFiveData(DataStructFloatFive data)
    {
    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        speed = data.float1;
        int scaleFlip = 1;
        if (speed < 0) scaleFlip = -1;
        transform.localScale = new Vector3(data.float2 * scaleFlip, data.float2, data.float2);
        transform.eulerAngles = new Vector3(0, 0, data.float3);
    }



    public bool ShowLine()
    {
        return false;
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        return new Vector2(currPos.x + (-speed * time), currPos.y);
    }

    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
}
