using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{

    [SerializeField] private Rigidbody2D fanRb;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public int bladeAmount;
    public float bladeScaleMultiplier;
    public float bladeSpeed;
    public float heightMultiplier;

    

    [SerializeField] private Vector2 minMaxWindSoundDelay;
    [SerializeField] private Vector2 minMaxWindSpeed;
    private float windMillSoundDelay;

    private float windMillSoundTimer;

    private bool hitEdge;



    [SerializeField] private float clockwiseChance;

    [SerializeField] private Vector2 speedRangeClockwise;
    [SerializeField] private Vector2 speedRangeCounterClockwise;

    [SerializeField] private int spinSpeed;

    [SerializeField] private Vector2 heightRange;

    [SerializeField] private float addedBladeY;

    [SerializeField] private float resetTime;

    private float resetTimer;

    private Vector2 startPos;
    // Start is called before the first frame update

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnEnable()
    {
        AdjustHeightToGround();

        fanRb.transform.localScale = BoundariesManager.vectorThree1 * bladeScaleMultiplier;

        Move();
    }

    private void AdjustHeightToGround()
    {
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on windmill.");
            return;
        }

        // Calculate the desired total height from the ground to the windmill's top
        float desiredTotalHeight = Mathf.Abs(transform.position.y - BoundariesManager.GroundPosition + .15f);

        // Adjust sprite size based on the desired height
        sr.size = new Vector2(sr.size.x, desiredTotalHeight);

    }



    void Move()
    {




        // float addedHeight = Random.Range(heightRange.x, heightRange.y);
        // sr.size = new Vector2(2.7f, addedHeight);

        // fanRb.transform.localPosition = new Vector2(0, addedHeight + addedBladeY);

        // float chance = Random.Range(0f, 1f);
        // float spinSpeed;

        // if (chance < clockwiseChance)
        // {
        //     spinSpeed = Random.Range(speedRangeClockwise.x, speedRangeClockwise.y);

        // }
        // else
        // {
        //     spinSpeed = Random.Range(speedRangeCounterClockwise.x, speedRangeCounterClockwise.y);

        // }
        fanRb.angularVelocity = bladeSpeed;

        AudioManager.instance.ChangeWindMillPitch(Mathf.Lerp(minMaxWindSoundDelay.x, minMaxWindSoundDelay.y, (Mathf.Abs(bladeSpeed) - minMaxWindSpeed.x) / (minMaxWindSpeed.y - minMaxWindSpeed.x)));

        windMillSoundDelay = 360 / bladeAmount / Mathf.Abs(bladeSpeed);

        // StartCoroutine(PlayWindmillSound(windMillSoundDelay));


    }

    // private IEnumerator PlayWindmillSound(float delay)
    // {
    //     while (!hitEdge)
    //     {
    //         AudioManager.instance.PlayWindMillSound();
    //         yield return new WaitForSeconds(delay);
    //     }
    // }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.fixedDeltaTime);

        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            hitEdge = true;
            resetTimer += Time.deltaTime;

            if (resetTimer > resetTime)
            {
                hitEdge = false;
                Move();
                resetTimer = 0;

            }
        }

        else
        {
            windMillSoundTimer += Time.fixedDeltaTime;

            if (windMillSoundTimer >= windMillSoundDelay)
            {
                AudioManager.instance.PlayWindMillSound();
                windMillSoundTimer = 0;

            }

        }



    }
}
