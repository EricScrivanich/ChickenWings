using UnityEngine;

public class Fence : SpawnedObject, IRecordableObject
{
    private SpriteRenderer sr;
    private float baseWidth = 1.28f;

    private float widthPerSection = .31f;
    private float extendAmount;
    [SerializeField] private float testExtendAmount;
    private float currentWidth;
    private float extendSpeed;
    private BoxCollider2D col;
    [SerializeField] private SpriteRenderer blurOutline;
    private float startX;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        extendSpeed = BoundariesManager.GroundSpeed / transform.lossyScale.x;

    }

    void Update()
    {
        if (!move)
        {
            currentWidth = Mathf.MoveTowards(
                currentWidth,
                extendAmount,
                extendSpeed * Time.deltaTime
            );
            sr.size = new Vector2(currentWidth, sr.size.y);

            if (Mathf.Abs(currentWidth - extendAmount) < 0.001f || currentWidth > extendAmount)
            {
                currentWidth = extendAmount;
                move = true;
            }
        }
        // else
        // {
        //     // After extension completes, translate as usual
        //     Vector2 pos = transform.position;
        //     pos.x -= BoundariesManager.GroundSpeed * Time.deltaTime;
        //     transform.position = pos;
        // }
    }
    void FixedUpdate()
    {
        if (!move)
        {
            col.size = new Vector2(currentWidth, col.size.y);
            col.offset = new Vector2(((baseWidth - currentWidth) * .5f) - .64f, col.offset.y);
        }
        else
        {
            rb.MovePosition(new Vector2(rb.position.x - (BoundariesManager.GroundSpeed * Time.fixedDeltaTime), transform.position.y));
        }

    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        startX = data.startPos.x;
        currentWidth = baseWidth + (data.float1 * 100 * widthPerSection);

        sr.size = new Vector2(currentWidth, sr.size.y);
        blurOutline.size = new Vector2(blurOutline.size.x, 1.2f + (data.float1 * 100 * widthPerSection));
        blurOutline.transform.localPosition = new Vector3(currentWidth * .5f, .82f, 0);
    }
    private bool move = false;
    // void Update()
    // {
    //     if (!move)
    //     {
    //         currentWidth = Mathf.Lerp(currentWidth, extendAmount, Time.deltaTime * extendSpeed);
    //         sr.size = new Vector2(currentWidth, sr.size.y);
    //     }
    //     else
    //     {
    //         Vector2 pos = transform.position;
    //         pos.x -= BoundariesManager.GroundSpeed * Time.deltaTime;
    //         transform.position = pos;
    //     }


    //     if (!move && Mathf.Abs(currentWidth - extendAmount) < .01f)
    //     {
    //         currentWidth = extendAmount;
    //         move = true;
    //     }
    // }
    private readonly float baseHeight = 1.25f;
    public override void ApplyFloatTwoData(DataStructFloatTwo data)
    {
        extendAmount = baseWidth + (data.float1 * 100 * widthPerSection);
        transform.position = new Vector2(data.startPos.x, transform.position.y);
        sr.size = new Vector2(baseWidth, sr.size.y);


        // Constant-rate width growth so left edge moves at ground speed
        extendSpeed = BoundariesManager.GroundSpeed / transform.lossyScale.x;


        currentWidth = baseWidth;
        move = false;

        gameObject.SetActive(true);

    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        return new Vector2(currPos.x - (BoundariesManager.GroundSpeed * time), transform.position.y);
    }

    public float ReturnPhaseOffset(float x)
    {
        float rightPos = x + (currentWidth * transform.lossyScale.x);

        if (rightPos <= BoundariesManager.leftBoundary)
            return -1;
        else
            return 0f;
    }

    public bool ShowLine()
    {
        return false;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame

}
