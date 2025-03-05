using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovementBasic : MonoBehaviour, IRecordableObject
{
    [SerializeField] private int id;
    [SerializeField] private SpriteRenderer headSprite;

    [ExposedScriptableObject]
    [SerializeField] private PigsScriptableObject pigID;
    [Header("SpriteObjects")]
    [SerializeField] private Transform frontLegs;
    [SerializeField] private Transform backLegs;
    [SerializeField] private Transform wings;
    [SerializeField] private Transform tail;
    [SerializeField] private Transform body;
    [Header("SpritePositions")]
    [SerializeField] private Transform frontLegsPosition;
    [SerializeField] private Transform backLegsPosition;
    [SerializeField] private Transform wingsPosition;
    [SerializeField] private Transform tailPosition;




    [Header("MovementRB")]

    [SerializeField] private float addYForce;
    private bool hasInitialized = false;

    private Vector2 jumpForce;
    [SerializeField] private float belowAmount;
    private bool justJumped;



    [SerializeField] private GameObject testobj;

    [Header("Movement")]


    [SerializeField] private float ySpeed;
    public float xSpeed;
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    private int direction = 1;

    [SerializeField] private float R; //slope_up_slope_down_ratio

    private float initialY;

    [SerializeField] private bool flapMovement;


    // [SerializeField] private float duration;
    // [SerializeField] private float invokeTime;
    private Animator anim;

    private bool downGlide;
    private bool upGlide;
    private float time;
    private bool sinDown;
    private Rigidbody2D rb;

    [SerializeField] private float sinDownSpeed;

    [SerializeField] float _sineFrequency = 5.0f;
    [SerializeField] float _sineMagnitude = 2.5f;
    [SerializeField] private float desiredOscillationTime;
    [SerializeField] private float offsetX;
    [SerializeField] private float minSineMagnitude = 2.1f; // Smallest sine magnitude
    [SerializeField] private float maxSineMagnitude = 2.3f; // Largest sine magnitude
    [SerializeField] private float flapAnimStart = 2.3f;
    [SerializeField] private float testWaitTime;

    [SerializeField] private float phaseOffset;

    private float startTime;




    Vector2 _axis;
    Vector2 _direction;
    Vector2 _position;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        jumpForce = new Vector2(xSpeed, addYForce);
        _position = transform.position;
        _axis = transform.up;
        _direction = -transform.right;

        upGlide = false;
        downGlide = false;

        // rb.velocity = new Vector2(xSpeed, 0);
        justJumped = false;
        if (testobj != null) StartCoroutine(Test());

        // anim.SetTrigger("FlapTrigger");
        // Invoke("GlideDown", invokeTime);
        // Invoke("GlideUp", invokeTime);

    }

    private float timef;

    [SerializeField] private float magPercent;



    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (headSprite != null)
            headSprite.sprite = pigID.ReturnStartingHead();

    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(testWaitTime);
        testobj.SetActive(true);
    }


    [SerializeField] private float magDiff;

    private void OnValidate()
    {
        // _sineMagnitude = pigID.ReturnMag(xSpeed, magPercent);
        // _sineFrequency = pigID.ReturnFreq(xSpeed, magPercent);

        pigID.ReturnSineWaveLogic(xSpeed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        minSineMagnitude = _sineMagnitude - magDiff;
        maxSineMagnitude = _sineMagnitude + magDiff;
        // _sineFrequency = (2 * Mathf.PI) / desiredOscillationTime;
    }

    // public void 



    private float startX;
    private void OnEnable()
    {
        startTime = Time.time;
        pigID.ReturnSineWaveLogic(xSpeed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        startX = transform.position.x + (((Mathf.PI) / _sineFrequency) * phaseOffset);
        if (!hasInitialized)
        {
            initialY = transform.position.y;





            if (body != null)
            {
                body.localScale = new Vector3(((.85f - transform.localScale.x) * .5f) + 1, 1, 1);
                frontLegs.position = frontLegsPosition.position;
                backLegs.position = backLegsPosition.position;
                wings.position = wingsPosition.position;
                tail.position = tailPosition.position;
            }


        }
    }

    private void SetAnimSpeed()
    {
        anim.speed = 1;
    }
    // private void OnEnable()
    // {
    //     // sinDown = false;


    //     if (hasInitialized)
    //     {
    //         Debug.Log("Initilized");
    //         initialY = transform.position.y;


    //         body.localScale = new Vector3(.75f - transform.localScale.x + 1, 1, 1);

    //         backLegs.position = backLegsPosition.position;
    //         wings.position = wingsPosition.position;
    //         tail.position = tailPosition.position;

    //     }

    //     // if (initialY > 3f)
    //     // {
    //     //     float r = Random.Range(0f, 1f);
    //     //     if (r > .4f)
    //     //     {

    //     //         sinDown = true;
    //     //     }
    //     // }
    // }

    private void GlideDown()
    {
        downGlide = true;
        anim.SetBool("DownGlideBool", true);
        anim.SetTrigger("GoDownTrigger");

    }
    private void GlideUp()
    {
        upGlide = true;
        anim.SetBool("UpGlideBool", true);
        anim.SetTrigger("GoUp");


    }
    // Update is called once per frame

    void ChangeJustJumped()
    {
        justJumped = false;
    }



    // void Update()
    // {

    //     transform.Translate(Vector2.left * xSpeed * Time.deltaTime);

    //     float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
    //     transform.position = new Vector2(transform.position.x, y);

    // }
    private bool hasFlapped = false;



    private void FixedUpdate()
    {




        _position += Vector2.left * Time.fixedDeltaTime * xSpeed;
        float period;
        if (flapMovement)

            period = Mathf.Abs(Mathf.Sin((_position.x - startX) * _sineFrequency));
        else
            period = Mathf.Sin((_position.x - startX) * _sineFrequency);

        // Dynamically adjust sine magnitude using Lerp
        float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));

        // Apply movement with dynamic sine magnitude
        rb.MovePosition(_position + Vector2.up * period * dynamicMagnitude);

        // Trigger flap animation at the bottom of the sine wave
        if (!hasFlapped && period < flapAnimStart)
        {
            anim.SetTrigger("Flap");
            hasFlapped = true;
        }
        if (hasFlapped && period > flapAnimStart)
        {
            hasFlapped = false;
        }





    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 simulatedPosition = transform.position; // Start from the object's current position
        Vector2 previousPoint = simulatedPosition;
        float startXX = transform.position.x + (((Mathf.PI) / _sineFrequency) * phaseOffset);

        // How far we predict in the future (e.g., 20 units)
        int dir = 1;

        if (xSpeed < 0) dir = -1;
        float predictionDistance = 30f;
        int steps = 300; // More steps = smoother line
        float stepSize = predictionDistance / steps; // Fixed horizontal step size

        for (int i = 0; i < steps; i++)
        {
            // Move left by a fixed step size
            simulatedPosition += Vector2.left * stepSize * dir;
            float period;

            // Calculate the sine wave offset using the new movement logic
            if (flapMovement)
                period = Mathf.Abs(Mathf.Sin((simulatedPosition.x - startXX) * _sineFrequency));
            else
                period = Mathf.Sin((simulatedPosition.x - startXX) * _sineFrequency);

            float dynamicMagnitude = Mathf.Lerp(maxSineMagnitude, minSineMagnitude, Mathf.Abs(period));


            // Apply vertical offset
            Vector2 pos = simulatedPosition + Vector2.up * period * dynamicMagnitude;

            // Draw the predicted movement
            Gizmos.DrawLine(previousPoint, pos);
            previousPoint = pos;
        }
    }

    public void InitializePig()
    {

        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        initialY = transform.position.y;
        // anim.speed = 0;

        // Invoke("SetAnimSpeed", Random.Range(0f, .5f));


        body.localScale = new Vector3(((.8f - Mathf.Abs(transform.localScale.x)) * .8f) + 1, 1, 1);

        frontLegs.position = frontLegsPosition.position;
        backLegs.position = backLegsPosition.position;
        wings.position = wingsPosition.position;
        tail.position = tailPosition.position;
        hasInitialized = true;


        gameObject.SetActive(true);

    }

    public RecordedDataStruct RecordData()
    {
        return new RecordedDataStruct(
        id,
        transform.position,
        Mathf.Abs(transform.localScale.x),
        xSpeed,
        magPercent,
        0,
        0,
        0);
    }

    public void RetrieveRecordedData(RecordedDataStruct data)
    {
        transform.position = data.startPos;
        transform.localScale = Vector3.one * data.scale;
        if (data.speed < 0) transform.localScale = Vector3.Scale(transform.localScale, BoundariesManager.FlippedXScale);
        xSpeed = data.speed;
        magPercent = data.magPercent;




    }

    public void ShowPath(bool isSelected)
    {
        throw new System.NotImplementedException();
    }
}
