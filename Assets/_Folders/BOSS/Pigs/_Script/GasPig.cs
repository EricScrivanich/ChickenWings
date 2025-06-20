using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPig : MonoBehaviour, IRecordableObject
{
    public float speed;
    public float delay = 0;
    public float initialDelay;


    // [HideInInspector]
    public int id;

    private bool hasCrossedBoundary;

    private bool flipped;



    [SerializeField] private Transform cloudSpawn;

    [SerializeField] private SpriteRenderer gasTexture;



    private Animator anim;
    private Rigidbody2D rb;




    [SerializeField] private bool flying;
    private bool initialized;

    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!initialized)
        {
            bool f = speed < BoundariesManager.GroundSpeed;
            Initialize(speed, delay, f, initialDelay);
            if (f) transform.localScale = new Vector3(-1, 1, 1);

        }
    }


    public void Initialize(float s, float d, bool f, float startDelay)
    {
        initialized = true;
        speed = s;
        delay = d;
        flipped = f;
        initialDelay = startDelay;
        hasCrossedBoundary = false;

        gameObject.SetActive(true);

        if (flying)
        {
            transform.localScale = BoundariesManager.vectorThree1 * .8f;
            anim.SetTrigger("FlyFart");

        }
        else
        {
            // transform.localScale = BoundariesManager.vectorThree1;
            anim.SetTrigger("Walk");
            StartCoroutine(CloudRoutine());

        }
    }





    // Update is called once per frame

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);
    }
    void Update()
    {

        // transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (flying && !hasCrossedBoundary)
        {
            if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
            {
                AudioManager.instance.PlayLongFarts();
                SmokeTrailPool.GetGasSmokeTrail?.Invoke(transform.position, speed, delay, id);
                hasCrossedBoundary = true;
            }
        }
        // else if (!flying && !hasCrossedBoundary)
        // {
        //     if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
        //     {
        //         StartCoroutine(CloudRoutine());
        //         hasCrossedBoundary = true;
        //     }

        // }



    }
    WaitForSeconds awaiterConstant = new WaitForSeconds(.1f);

    private IEnumerator CloudRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {

            // if (Mathf.Abs(transform.position.x) > BoundariesManager.rightBoundary)
            // {
            //     yield return new WaitForSeconds(.2f + delay);


            // }
            // else
            // {
            //     anim.SetTrigger("FartTrigger");
            //     yield return new WaitForSeconds(.1f);
            //     AudioManager.instance.PlayFartSound();
            //     yield return new WaitForSeconds(.1f);


            //     // Instantiate(cloud, cloudSpawn.position, Quaternion.identity, transform);
            //     SmokeTrailPool.GetGasCloud?.Invoke(cloudSpawn.position, -speed, flipped);

            //     yield return new WaitForSeconds(delay);

            // }

            anim.SetTrigger("FartTrigger");
            yield return awaiterConstant;
            if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
                AudioManager.instance.PlayFartSound();
            yield return awaiterConstant;



            // Instantiate(cloud, cloudSpawn.position, Quaternion.identity, transform);
            SmokeTrailPool.GetGasCloud?.Invoke(cloudSpawn.position, -speed, flipped);

            yield return new WaitForSeconds(delay);


        }



    }

    private void OnDisable()
    {
        if (flying)
            SmokeTrailPool.OnDisableGasTrail?.Invoke(id);
    }


    public void ApplyFloatOneData(DataStructFloatOne data)
    {
        transform.position = data.startPos;
        speed = 8;
        delay = data.float1;
        gameObject.SetActive(true);

    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {

    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {
        transform.position = data.startPos;
        speed = data.float1;
        delay = data.float2;

        if (speed < BoundariesManager.GroundSpeed)
        {
            transform.localScale = Vector3.Scale(Vector3.one * .9f, BoundariesManager.FlippedXScale);
        }
        else transform.localScale = Vector3.one * .9f;



        initialDelay = data.float3 * delay;
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
        if (!flying)
        {
            speed = data.float1;

            if (speed < BoundariesManager.GroundSpeed)
            {
                transform.localScale = Vector3.Scale(Vector3.one * .9f, BoundariesManager.FlippedXScale);
            }
            else transform.localScale = Vector3.one * .9f;

            delay = data.float2;

            initialDelay = data.float3 * delay;
        }
        else
        {
            speed = 8;
            delay = data.float1;


        }


    }

    public bool ShowLine()
    {
        if (!flying)
            return true;
        else return false;
    }

    public float TimeAtCreateObject(int index)
    {
        return ((index * delay) + initialDelay);
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        if (flying)
        {
            Vector2 p = new Vector2(currPos.x + (-speed * time), currPos.y);
            if (time <= delay)
            {
                if (p.x < BoundariesManager.leftBoundary)
                {
                    p.x = BoundariesManager.leftBoundary;

                }

                SetTiledSpritePosition(gasTexture, currPos.x, p.x, currPos.y);




            }
            else
            {
                float gasPos = ((delay - time) * 2.1f) + currPos.x;

                if (gasPos <= BoundariesManager.leftBoundary + 2)
                {
                    p.x = BoundariesManager.leftBoundary - .1f;
                    gasTexture.gameObject.SetActive(false);
                }
                else
                {
                    if (p.x < BoundariesManager.leftBoundary)
                    {
                        p.x = BoundariesManager.leftBoundary;

                    }
                    SetTiledSpritePosition(gasTexture, gasPos, p.x, currPos.y);
                }



            }
            return p;
        }

        else
            return new Vector2(currPos.x + (-speed * time), currPos.y);
    }

    void SetTiledSpritePosition(SpriteRenderer spriteRenderer, float originX, float targetX, float y)
    {
        // Get the sprite's world width

        if (!spriteRenderer.gameObject.activeInHierarchy) spriteRenderer.gameObject.SetActive(true);

        float range = originX - targetX;
        spriteRenderer.size = new Vector2(range, spriteRenderer.size.y);


        // Calculate the new position: Since the pivot is on the right side, 
        // we place the right edge at targetX


        // Apply the position
        spriteRenderer.transform.position = new Vector2(originX, y);
    }

    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
}
