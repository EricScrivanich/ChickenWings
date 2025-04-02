using UnityEngine;

public class PigWaveMovement : MonoBehaviour, IRecordableObject
{
    private Rigidbody2D rb;
    [SerializeField] private bool useDynamicMag;
    [SerializeField] private float speed;
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
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        // _sineMagnitude = pigID.ReturnMag(xSpeed, magPercent);
        // _sineFrequency = pigID.ReturnFreq(xSpeed, magPercent);

        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;

        // _sineFrequency = (2 * Mathf.PI) / desiredOscillationTime;
    }

    private void OnEnable()
    {
        _position = transform.position;
        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;



        float period = Mathf.Sin((_position.x - startX) * _sineFrequency);

        if (anim != null && Mathf.Abs(phaseOffset) > .5f)
        {
            flyingDown = false;
            anim.SetTrigger("FlyUp");
        }
        else
        {
            flyingDown = true;
            anim.SetTrigger("FlyDown");
        }

        // Dynamically adjust sine magnitude using Lerp
        // speed = Mathf.Lerp(minSpeed, maxSpeed, period);
    }

    private void OnDisable()
    {
        if (anim == null) return;
        anim.SetBool("GlidingUpBool", false);
        anim.SetBool("GlidingDownBool", false);

        anim.SetTrigger("Restart");
    }

    private bool flyingDown;
    private bool initilaizedAnim;

    private void FixedUpdate()
    {

        _position += Vector2.left * Time.fixedDeltaTime * speed;

        float period = Mathf.Sin((_position.x - startX) * _sineFrequency);

        if (flyingDown && period < 0)
        {
            flyingDown = false;
            anim.SetTrigger("FlyUp");
        }
        else if (!flyingDown && period > 0)
        {
            flyingDown = true;
            anim.SetTrigger("FlyDown");
        }

        // Dynamically adjust sine magnitude using Lerp
        // speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Abs(period));
        float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));


        // Apply movement with dynamic sine magnitude
        rb.MovePosition(_position + Vector2.up * period * dynamicMagnitude);




    }

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
            Vector2 pos;
            if (useDynamicMag)
            {
                float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));
                pos = simulatedPosition + Vector2.up * period * dynamicMagnitude;
            }
            else pos = simulatedPosition + Vector2.up * period * _sineMagnitude;


            // Calculate the sine wave offset using the new movement logic






            // Apply vertical offset


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
        float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));
        float yPos = currPos.y + (period * dynamicMagnitude);




        // Dynamically adjust sine magnitude using Lerp


        // Compute final vertical position


        return new Vector2(xPos, yPos);
    }

    public float ReturnPhaseOffset(float x)
    {
        return x + (((Mathf.PI) / _sineFrequency) * phaseOffset);
    }

    

    public void ApplyFloatOneData(DataStructFloatOne data)
    {

    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {

    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {
        transform.position = data.startPos;
        _position = data.startPos;
        speed = data.float1;
        magPercent = data.float2;
        phaseOffset = data.float3;
        transform.localScale = Vector3.one * pigID.baseScale;
        if (speed < 0) transform.localScale = Vector3.Scale(transform.localScale, BoundariesManager.FlippedXScale);

        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;

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
        magPercent = data.float2;
        phaseOffset = data.float3;
        transform.localScale = Vector3.one * pigID.baseScale;
        if (speed < 0) transform.localScale = Vector3.Scale(transform.localScale, BoundariesManager.FlippedXScale);

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


}
