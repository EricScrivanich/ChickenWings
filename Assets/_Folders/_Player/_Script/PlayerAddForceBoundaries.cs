using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddForceBoundaries : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStateManager stateManager;
    [SerializeField] private GameObject arrowPrefab;
    private Arrow arrow;
    private float slowDownFallBoundary = -1.5f;
    private float addForceDownBoundary = 4.62f;
    private float originalMaxFallSpeed;
    private bool hasChangedMaxFallSpeed = false;
    private Vector2 addRightForce = new Vector2(20, 10);
    private Vector2 addDownForce = new Vector2(0, -6.3f);
    private Vector2 addLeftForce = new Vector2(-20, 10);
    private float topBoundary;
    private float leftBoundary;
    private float rightBoundary;

    private bool isTop;
    private bool isLeft;
    private bool isRight;

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        playerTransform = GetComponent<Transform>();
        topBoundary = 5.1f;
        leftBoundary = BoundariesManager.leftPlayerBoundary;
        rightBoundary = BoundariesManager.rightPlayerBoundary;

        rb = GetComponent<Rigidbody2D>();

        originalMaxFallSpeed = stateManager.originalMaxFallSpeed;





    }

    public void CreateArrow(GameObject arrowObject)
    {
        arrow = Instantiate(arrowObject).GetComponent<Arrow>();
        arrow.gameObject.SetActive(false);

    }

    // public void SetStateManager(PlayerStateManager psm, float fallSpeed)
    // {
    //     stateManager = psm;
    //     originalMaxFallSpeed = fallSpeed;
    // }

    private void FixedUpdate()
    {



        if (playerTransform.position.x < leftBoundary)
        {
            rb.AddForce(addRightForce);

            if (!isLeft)
            {
                arrow.SetActive(1);
                isLeft = true;
            }

        }
        else if (playerTransform.position.x > rightBoundary)
        {
            rb.AddForce(addLeftForce);

            if (!isRight)
            {
                arrow.SetActive(2);
                isRight = true;
            }
        }

        if (playerTransform.position.y < slowDownFallBoundary && !stateManager.isDropping)
        {
            hasChangedMaxFallSpeed = true;
            stateManager.maxFallSpeed = originalMaxFallSpeed + ((slowDownFallBoundary - playerTransform.position.y) * 1.5f);


        }
        else if (hasChangedMaxFallSpeed && !stateManager.isDropping)
        {
            stateManager.maxFallSpeed = originalMaxFallSpeed;
            hasChangedMaxFallSpeed = false;


        }
        else if (playerTransform.position.y > addForceDownBoundary)
        {
            if (rb.velocity.y > -4.4f && rb.velocity.y < 7.5f)
                rb.AddForce(addDownForce);

            if (!isTop && playerTransform.position.y > topBoundary)
            {
                isLeft = false;
                isRight = false;
                arrow.SetActive(0);
                isTop = true;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isTop && playerTransform.position.y < topBoundary)
        {
            arrow.gameObject.SetActive(false);
            isTop = false;

        }
        else if (isLeft && playerTransform.position.x > leftBoundary && !isTop)
        {
            arrow.gameObject.SetActive(false);
            isLeft = false;
        }
        else if (isRight && playerTransform.position.x < rightBoundary && !isTop)
        {
            arrow.gameObject.SetActive(false);
            isRight = false;

        }


    }
}
