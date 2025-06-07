using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HotAirBalloon : MonoBehaviour, IRecordableObject
{

    [SerializeField] private int balloonType;
    [SerializeField] private Gun gun;
    private int currentSpriteIndex = 0;
    private bool inRange;

    private readonly float spriteSwitchTime = .08f;

    [SerializeField] private Transform gunTransform;
    [SerializeField] private float offsetGunRotation;
    private float gunRotationSpeed;
    [SerializeField] private float gunRotationSpeedShooting;
    [SerializeField] private float gunRotationSpeedAiming;
    [SerializeField] private float minShootAngle;



    public int type;
    public float speed;

    public float initialDelay;
    public float yTarget;
    public float delay;

    public readonly int DropTrigger = Animator.StringToHash("DropTrigger");
    public readonly int ResetTrigger = Animator.StringToHash("Reset");
    public readonly int ReverseTrigger = Animator.StringToHash("Reverse");

    private Transform player;
    private float eyeRadius = 0.05f; // Radius within which the pupil can move

    private bool usingGun = false;
    private bool waitingForDelay;

    private Rigidbody2D rb;
    private float time = 0;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private AnimationDataSO animData;
    // [SerializeField] private AnimationDataSO pigAnimData;
    [SerializeField] private ExplosivesPool pool;
    // [SerializeField] private float dropDelay;
    // [SerializeField] private float initialDropDelay;


    [SerializeField] private Transform dropPosition;
    [SerializeField] private Vector2 minMaxYVel;
    [SerializeField] private float velocityLerpSpeed;
    [SerializeField] private float liftForce = 5f;
    private float targetGunRotation = 0;


    private bool addingYForce;
    private float targetYVelocity;

    private Animator anim;
    private bool startedAnim;
    private float delayTimer = 0;

    private float _sineFrequency;
    [SerializeField] private float minSineMagnitude = 2.1f;
    [SerializeField] private float maxSineMagnitude = 2.3f;

    private float _sineMagnitude;
    private float magDiff;
    [SerializeField] private float phaseOffset;
    private float startX;

    [SerializeField] private Transform pupils;
    private bool isReversingGun;

    private float speedDiff;

    [SerializeField] private float magPercent;


    [ExposedScriptableObject]
    [SerializeField] private PigsScriptableObject pigID;
    Vector2 _position;



    // Start is called before the first frame update


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (balloonType > 0)
        {
            player = GameObject.Find("Player").GetComponent<Transform>();

            usingGun = true;
            gunRotationSpeed = gunRotationSpeedAiming;


        }
    }



    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;


        if (time > spriteSwitchTime)
        {
            currentSpriteIndex++;

            if (currentSpriteIndex > animData.sprites.Length - 1)
            {
                currentSpriteIndex = 0;
            }
            sr.sprite = animData.sprites[currentSpriteIndex];
            time = 0;

        }

        if (usingGun)
        {


            // Lerp the rotation angle
            float newRotation = Mathf.LerpAngle(gunTransform.eulerAngles.z, targetGunRotation, Time.deltaTime * gunRotationSpeed);


            // Apply the new rotation (make sure the gun rotates on Z-axis for 2D)
            gunTransform.rotation = Quaternion.Euler(0f, 0f, newRotation);

        }

        if (waitingForDelay)
        {

            delayTimer += Time.deltaTime;

            if (delayTimer >= delay)
            {
                switch (balloonType)
                {
                    case 0:
                        anim.SetTrigger(DropTrigger);
                        break;
                    case 1:
                        if (!inRange) return;
                        Debug.Log("Fire");
                        gun.Fire(false);
                        waitingForDelay = true;
                        // anim.speed = Random.Range(.75f, .9f);
                        break;
                    case 2:
                        // anim.speed = Random.Range(.75f, .9f);
                        break;

                }

                // waitingForDelay = false;
                delayTimer = 0;

            }

        }
        else if (!startedAnim)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= initialDelay)
            {

                // anim.speed = Random.Range(.75f, .9f);
                switch (balloonType)
                {
                    case 0:
                        anim.SetTrigger(DropTrigger);
                        break;
                    case 1:
                    case 2:
                        if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
                            waitingForDelay = true;
                        else
                            return;
                        // anim.speed = Random.Range(.75f, .9f);
                        break;


                    // anim.speed = Random.Range(.75f, .9f);

                    default:
                        break;
                }

                delayTimer = 0;
                startedAnim = true;

            }

        }








    }



    private void OnEnable()
    {
        // anim.SetTrigger(ResetTrigger);
        _position = transform.position;
        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;

        startX = transform.position.x + (((Mathf.PI) / _sineFrequency) * phaseOffset);



        waitingForDelay = false;
        time = 0;
        delayTimer = 0;
        // xTrigger += .2f;
        startedAnim = false;
        currentSpriteIndex = Random.Range(0, animData.sprites.Length - 1);
        sr.sprite = animData.sprites[currentSpriteIndex];
        rb.linearVelocity = new Vector2(speed, 0);

        if (balloonType > 0)
        {
            Ticker.OnTickAction015 += SetGunRotationTarget;
            anim.SetBool(ReverseTrigger, false);
            isReversingGun = false;
        }



    }
    private void OnDisable()
    {
        if (balloonType > 0)
        {
            Ticker.OnTickAction015 -= SetGunRotationTarget;
        }
    }


    private void SetGunRotationTarget()
    {
        if (Mathf.Abs(transform.position.x) > BoundariesManager.rightViewBoundary)
        {
            anim.speed = 0;
            inRange = false;
            return;
        }
        Vector2 direction = player.position - gunTransform.position;
        direction.Normalize();

        // Calculate the angle in degrees
        targetGunRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + offsetGunRotation;
        Debug.Log("Target gun rotation: " + targetGunRotation);
        float angleDiff = Mathf.DeltaAngle(gunTransform.eulerAngles.z, targetGunRotation);
        Debug.Log("Angle diff: " + angleDiff);

        if (!inRange && Mathf.Abs(angleDiff) <= minShootAngle)
        {
            Debug.Log("In range");
            inRange = true;
            delayTimer = .1f;
            waitingForDelay = true;

            gunRotationSpeed = gunRotationSpeedShooting;
        }
        else if (inRange && Mathf.Abs(angleDiff) > minShootAngle)
        {
            Debug.Log("Out of range");
            inRange = false;
            gunRotationSpeed = gunRotationSpeedAiming;
        }



        if (angleDiff < 0 && !isReversingGun) anim.SetBool(ReverseTrigger, true);
        else if (angleDiff >= 0 && isReversingGun) anim.SetBool(ReverseTrigger, false);
        if (Mathf.Abs(angleDiff) < 5) anim.speed = 0;

        else if (inRange)
        {
            anim.speed = .3f;
        }
        else anim.speed = 1;



        Vector2 eyeDirection = player.position - pupils.position; // Calculate the direction to the player
                                                                  // Ensure it's 2D
        eyeDirection.Normalize(); // Normalize the direction



        // Move the pupil within the eye's radius
        pupils.localPosition = eyeDirection * eyeRadius;



    }

    private void FixedUpdate()
    {

        // if (transform.position.y > yTarget + .15f && addingYForce)
        // {
        //     // When above the yTarget, lerp down to minYVelocity

        //     addingYForce = false;
        //     targetYVelocity = minMaxYVel.x;
        // }
        // else if (transform.position.y < yTarget - .2f && !addingYForce)
        // {
        //     // When below the yTarget, lerp up to maxYVelocity
        //     targetYVelocity = minMaxYVel.y;
        //     addingYForce = true;
        // }

        // // Lerp the current Y velocity towards the target Y velocity
        // float newYVelocity = Mathf.Lerp(rb.linearVelocity.y, targetYVelocity, velocityLerpSpeed * Time.fixedDeltaTime);

        // // Set the Rigidbody2D's velocity with the updated Y velocity
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);


        _position += Vector2.left * Time.fixedDeltaTime * speed;

        float period = Mathf.Sin((_position.x - startX) * _sineFrequency);


        // Dynamically adjust sine magnitude using Lerp
        // speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Abs(period));
        float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));


        // Apply movement with dynamic sine magnitude
        rb.MovePosition(_position + Vector2.up * period * dynamicMagnitude);



    }

    private void LerpYVelocity()
    {



    }

    private void ApplyLiftForce()
    {
        // Apply an upward force to simulate lift
        rb.AddForce(Vector2.up * liftForce);
    }




    // private IEnumerator DropBombRoutine()
    // {
    //     yield return new WaitForSeconds(initialDropDelay);

    //     while (true)
    //     {
    //         // pig.DOLocalMoveY(.15f, .2f);

    //         // for[int i = 0; initialDropDelay < ]
    //         // yield return new WaitForSeconds(.05f);
    //         // armSR.sprite = pigAnimData.sprites[0];



    //         yield return new WaitForSeconds(dropDelay);
    //         Debug.Log("Drop bomb");
    //         pool.GetBalloonBomb(dropPosition.position, new Vector2(-speed, 0));
    //     }

    // }

    public void DropBomb()
    {
        if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
        {
            pool.GetBalloonBomb(dropPosition.position, new Vector2(-speed, 0));
        }

        waitingForDelay = true;


    }



    public void ApplyRecordedData(RecordedDataStruct data)
    {

        transform.position = data.startPos;
        _position = data.startPos;
        speed = data.speed;
        magPercent = data.magPercent;
        phaseOffset = data.delayInterval;
        delay = data.timeInterval;
        initialDelay = data.scale * delay;




        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;
        gameObject.SetActive(true);
    }

    public void ApplyFloatOneData(DataStructFloatOne data)
    {

    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {

    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {

    }
    public void ApplyFloatFourData(DataStructFloatFour data)
    {


    }
    public void ApplyFloatFiveData(DataStructFloatFive data)
    {
        transform.position = data.startPos;
        _position = data.startPos;
        speed = data.float1;
        magPercent = data.float2;
        phaseOffset = data.float3;
        delay = data.float4;
        initialDelay = data.float5 * delay;



        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;
        gameObject.SetActive(true);

    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        speed = data.float1;
        magPercent = data.float2;
        phaseOffset = data.float3;
        delay = data.float4;
        initialDelay = data.float5 * delay;



        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;
    }



    public bool ShowLine()
    {
        return true;
    }

    public float TimeAtCreateObject(int index)
    {
        return (index * delay) + initialDelay;
    }

    // private void OnValidate()
    // {
    //     pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
    //     minSineMagnitude = _sineMagnitude - magDiff;
    //     maxSineMagnitude = _sineMagnitude + magDiff;

    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 simulatedPosition = transform.position; // Start from the object's current position
        Vector2 previousPoint = simulatedPosition;
        float startXX = transform.position.x + (((Mathf.PI) / _sineFrequency) * phaseOffset);

        // How far we predict in the future (e.g., 20 units)
        int dir = 1;

        if (speed < 0) dir = -1;
        float predictionDistance = 30f;
        int steps = 300; // More steps = smoother line
        float stepSize = predictionDistance / steps; // Fixed horizontal step size

        for (int i = 0; i < steps; i++)
        {
            // Move left by a fixed step size
            simulatedPosition += Vector2.left * stepSize * dir;
            float period = Mathf.Sin((simulatedPosition.x - startXX) * _sineFrequency);
            float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));

            // Calculate the sine wave offset using the new movement logic






            // Apply vertical offset
            Vector2 pos = simulatedPosition + Vector2.up * period * dynamicMagnitude;

            // Draw the predicted movement
            Gizmos.DrawLine(previousPoint, pos);
            previousPoint = pos;
        }
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        float xPos = currPos.x + (-speed * time);

        // Apply phase offset correction


        // Compute sine wave period at this x position
        float period = Mathf.Sin((xPos - phaseOffset) * _sineFrequency);

        // Dynamically adjust sine magnitude using Lerp
        float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));

        // Compute final vertical position
        float yPos = currPos.y + (period * dynamicMagnitude);

        return new Vector2(xPos, yPos);
    }

    public float ReturnPhaseOffset(float x)
    {
        return x + (((Mathf.PI) / _sineFrequency) * phaseOffset);
    }
}
