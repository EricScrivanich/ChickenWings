using UnityEngine;

public class ForceBoundaries : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int arrowType;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform trans;
    private Arrow arrow;
    [SerializeField] private float slowDownFallBoundary = -1.5f;
    [SerializeField] private float slowDownFallMultiplier = 1.5f;
    private float addForceDownBoundary = 4.8f;
    private float originalMaxFallSpeed;

    [SerializeField] private Vector2 addRightForce = new Vector2(20, 10);
    [SerializeField] private Vector2 addDownForce = new Vector2(0, -6.1f);
    [SerializeField] private Vector2 addLeftForce = new Vector2(-20, 10);
    private float topBoundary;
    private float leftBoundary;
    private float rightBoundary;
    [SerializeField] private bool isActive;

    private bool isTop;
    private bool isLeft;
    private bool isRight;

    [Header("MaxSpeeds")]
    public float maxDownSpeed;
    public float maxUpSpeed;
    public float maxLeftSpeed;
    public float maxRightSpeed;

    private float maxDownSpeedVar;





    // Start is called before the first frame update
    void Start()
    {
        maxDownSpeedVar = maxDownSpeed;


        topBoundary = 5.1f;
        leftBoundary = BoundariesManager.leftPlayerBoundary;
        rightBoundary = BoundariesManager.rightPlayerBoundary;
        CreateArrow(arrowPrefab);

        rb = GetComponent<Rigidbody2D>();







    }

    public void Activate(bool active)
    {
        isActive = active;

    }

    public void CreateArrow(GameObject arrowObject)
    {
        arrow = Instantiate(arrowObject).GetComponent<Arrow>();
        arrow.InitializeArrow(trans, arrowType);
        arrow.gameObject.SetActive(false);

    }

    // public void SetStateManager(PlayerStateManager psm, float fallSpeed)
    // {
    //     stateManager = psm;
    //     originalMaxFallSpeed = fallSpeed;
    // }

    private void FixedUpdate()
    {
        if (!isActive) return;



        MaxFallSpeed();

        if (transform.position.x < leftBoundary)
        {
            rb.AddForce(addRightForce);

            if (!isLeft)
            {
                arrow.SetActive(1);
                isLeft = true;
            }

        }
        else if (transform.position.x > rightBoundary)
        {
            rb.AddForce(addLeftForce);

            if (!isRight)
            {
                arrow.SetActive(2);
                isRight = true;
            }
        }

        if (transform.position.y < slowDownFallBoundary)
        {

            maxDownSpeedVar = maxDownSpeed + ((slowDownFallBoundary - transform.position.y) * slowDownFallMultiplier);


        }

        else if (transform.position.y > addForceDownBoundary)
        {
            if (rb.linearVelocity.y > -4.4f && rb.linearVelocity.y < 7.5f)
                rb.AddForce(addDownForce);

            if (!isTop && transform.position.y > topBoundary)
            {
                isLeft = false;
                isRight = false;
                arrow.SetActive(0);
                isTop = true;
            }
        }

    }

    public void MaxFallSpeed()
    {

        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, maxLeftSpeed, maxRightSpeed), Mathf.Clamp(rb.linearVelocity.y, maxDownSpeedVar, maxUpSpeed));
    }
    public void SetMaxFallSpeed(float speed)
    {
        originalMaxFallSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTop && transform.position.y < topBoundary)
        {
            arrow.gameObject.SetActive(false);
            isTop = false;

        }
        else if (isLeft && transform.position.x > leftBoundary && !isTop)
        {
            arrow.gameObject.SetActive(false);
            isLeft = false;
        }
        else if (isRight && transform.position.x < rightBoundary && !isTop)
        {
            arrow.gameObject.SetActive(false);
            isRight = false;

        }


    }
}
