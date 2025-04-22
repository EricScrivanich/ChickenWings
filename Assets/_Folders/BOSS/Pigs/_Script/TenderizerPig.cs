using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenderizerPig : MonoBehaviour, ICollectible, IRecordableObject
{
    public float speed;
    public bool hasHammer;
    private CircleCollider2D detection;
    private float amplitude = .3f;
    private float baseFrequency = 1.4f;
    private float baseSpeed = 3.5f;
    private float frequency = 1.4f;


    private bool parried = false;
    private Animator anim;
    [SerializeField] private GameObject Hammer;

    private Vector2 flippedDirection = new Vector2(-1, 1);
    private Transform player; // Reference to the player
    public Transform eye; // Reference to the eye
    public Transform pupil; // Reference to the pupil
    public float eyeRadius = 0.5f; // Radius within which the pupil can move

    private bool flipped;

    private float _sineMagnitude;
    private float _sineFrequency;
    private float magDiff;
    [SerializeField] private float phaseOffset;
    private float startX;

    private float speedDiff;

    [SerializeField] private float magPercent;



    [SerializeField] private PigsScriptableObject pigID;
    Vector2 _position;
    private Rigidbody2D rb;



    private void Awake()
    {
        detection = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {

        if (GameObject.Find("Player") != null)
            player = GameObject.Find("Player").GetComponent<Transform>();

    }


    private void OnEnable()
    {
        Ticker.OnTickAction015 += MoveEyesWithTicker;
        frequency = baseFrequency + ((baseSpeed - Mathf.Abs(speed)) * .27f);

        flipped = false;
        startX = transform.position.x;
        _position = transform.position;

        if (speed < 0)
        {
            flipped = true;

        }
        // if (speed < 0)
        // {
        //     flipped = true;
        //     transform.eulerAngles = new Vector3(0, 180, 0);
        //     speed *= -1;
        // }
        if (detection != null)
            detection.enabled = hasHammer;
        Hammer.SetActive(hasHammer);

    }

    private void OnDisable()
    {
        Ticker.OnTickAction015 -= MoveEyesWithTicker;
    }
    // void Update()
    // {
    //     transform.Translate(Vector2.left * speed * Time.deltaTime);


    //     float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
    //     transform.position = new Vector2(transform.position.x, y);

    // }

    private void FixedUpdate()
    {
        _position += Vector2.left * Time.fixedDeltaTime * speed;

        float period = Mathf.Sin((_position.x - startX) * _sineFrequency);



        rb.MovePosition(_position + Vector2.up * period * _sineMagnitude);
    }
    private void MoveEyesWithTicker()
    {

        if (player != null && hasHammer)
        {
            Vector2 direction = player.position - pupil.position; // Calculate the direction to the player
            // Ensure it's 2D
            direction.Normalize(); // Normalize the direction

            if (flipped)
            {
                direction *= flippedDirection;
            }

            // Move the pupil within the eye's radius
            pupil.localPosition = direction * eyeRadius;
        }

    }

    private IEnumerator AfterSwing()
    {
        yield return new WaitForSeconds(.45f);
        AudioManager.instance.PlayPigHammerSwingSound();
        yield return new WaitForSeconds(1.9f);

        if (!parried)
            detection.enabled = true;

        else
        {
            yield return new WaitForSeconds(4f);
            parried = false;
            detection.enabled = true;

        }



    }

    public void GetParried()
    {
        detection.enabled = false;
        anim.SetTrigger("Parried");
        parried = true;
    }

    public void Collected()
    {
        detection.enabled = false;
        anim.SetTrigger("SwingTrigger");
        StartCoroutine(AfterSwing());
        // Invoke("PlaySound", .5f);

    }

    private void PlaySound()
    {
        AudioManager.instance.PlayPigHammerSwingSound();
    }


    public void ApplyFloatOneData(DataStructFloatOne data)
    {

    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {
        transform.position = data.startPos;
        _position = data.startPos;
        speed = data.float1;
        transform.localScale = Vector3.one * data.float2;
        if (speed < 0) transform.localScale = Vector3.Scale(transform.localScale, BoundariesManager.FlippedXScale);
        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        gameObject.SetActive(true);
    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {

    }
    public void ApplyFloatFourData(DataStructFloatFour data)
    {

        // magPercent = data.float3;
        // phaseOffset = data.float4;

        // pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
        // gameObject.SetActive(true);

    }
    public void ApplyFloatFiveData(DataStructFloatFive data)
    {

    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {

        speed = data.float1;
        transform.localScale = Vector3.one * data.float2;
        if (speed < 0) transform.localScale = Vector3.Scale(transform.localScale, BoundariesManager.FlippedXScale);
        magPercent = data.float3;
        phaseOffset = data.float4;

        pigID.ReturnSineWaveLogic(speed, magPercent, out _sineMagnitude, out _sineFrequency, out magDiff);
    }

    public bool ShowLine()
    {
        return true;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        float xPos = currPos.x + (-speed * time);

        // Apply phase offset correction


        // Compute sine wave period at this x position
        float period = Mathf.Sin((xPos - phaseOffset) * _sineFrequency);
        float yPos = currPos.y + (period * _sineMagnitude);



        return new Vector2(xPos, yPos);
    }

    public float ReturnPhaseOffset(float x)
    {
        return x + (((Mathf.PI) / _sineFrequency));
    }
    // Update is called once per frame

}
