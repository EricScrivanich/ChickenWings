using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HotAirBalloon : MonoBehaviour, IRecordableObject
{


    private int currentSpriteIndex = 0;

    private readonly float spriteSwitchTime = .08f;

    public int type;
    public float speed;

    public float initialDelay;
    public float yTarget;
    public float delay;

    public readonly int DropTrigger = Animator.StringToHash("DropTrigger");
    public readonly int ResetTrigger = Animator.StringToHash("Reset");


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

    // Update is called once per frame
    void Update()
    {
        if (waitingForDelay)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > delay + .8f)
            {
                anim.SetTrigger(DropTrigger);
                waitingForDelay = false;
                delayTimer = 0;

            }

        }
        else if (!startedAnim)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer > initialDelay)
            {
                // anim.speed = Random.Range(.75f, .9f);
                anim.SetTrigger(DropTrigger);
                delayTimer = 0;
                startedAnim = true;
            }

        }



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
            pool.GetBalloonBomb(dropPosition.position, new Vector2(speed, 0));
        }

        waitingForDelay = true;


    }



    public void ApplyRecordedData(RecordedDataStruct data)
    {
        transform.position = data.startPos;
        speed = data.speed;
        magPercent = data.magPercent;
        phaseOffset = data.delayInterval;
        initialDelay = data.scale;
        delay = data.timeInterval;



        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;
    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        speed = data.speed;
        magPercent = data.magPercent;
        phaseOffset = data.delayInterval;
        initialDelay = data.scale;
        delay = data.timeInterval;


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
        return 0;
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
