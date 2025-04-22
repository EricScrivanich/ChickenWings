using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAddForceBoundaries : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStateManager stateManager;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private bool dontSlowFall;
    private Arrow arrow;
    private float slowDownFallBoundary = -1.5f;
    private float addForceDownBoundary = 4.8f;
    private float originalMaxFallSpeed;
    private bool hasChangedMaxFallSpeed = false;
    private Vector2 addRightForce = new Vector2(20, 10);
    private Vector2 addDownForce = new Vector2(0, -6.1f);
    private Vector2 addLeftForce = new Vector2(-20, 10);
    private float topBoundary;
    private float leftBoundary;
    private float rightBoundary;

    private bool isTop;
    private bool isLeft;
    private bool isRight;

    [SerializeField] private float moveCameraPlayerYStart;
    [SerializeField] private float moveCameraPlayerYEnd;

    [SerializeField] private float cameraMaxZoom;
    [SerializeField] private float cameraMaxY;
    [SerializeField] private float cameraMoveSmoothSpeed = 2f;
    [SerializeField] private float cameraZoomSmoothSpeed = 2f;
    private float cameraMinZoom = 5.5f;
    private float cameraOriginalZoom;
    private float cameraOriginalY;
    private Camera mainCam;


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
        mainCam = Camera.main;

        originalMaxFallSpeed = stateManager.originalMaxFallSpeed;
        cameraOriginalY = mainCam.transform.position.y;
        cameraOriginalZoom = mainCam.orthographicSize;





    }

    public void CreateArrow(GameObject arrowObject)
    {
        arrow = Instantiate(arrowObject).GetComponent<Arrow>();
        arrow.InitializeArrow(transform, 0);
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

        if (playerTransform.position.y < slowDownFallBoundary && !stateManager.isDropping && !dontSlowFall)
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
            if (rb.linearVelocity.y > -4.4f && rb.linearVelocity.y < 7.5f)
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

    private void HandleCameraFollowZoom()
    {
        float playerY = playerTransform.position.y;

        if (playerY > moveCameraPlayerYStart)
        {
            float t = Mathf.InverseLerp(moveCameraPlayerYStart, moveCameraPlayerYEnd, playerY);
            float targetY = Mathf.Lerp(cameraOriginalY, cameraMaxY, t);
            float targetZoom = Mathf.Lerp(cameraOriginalZoom, cameraMaxZoom, t);

            Vector3 camPos = mainCam.transform.position;
            camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * cameraMoveSmoothSpeed);
            mainCam.transform.position = camPos;

            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targetZoom, Time.deltaTime * cameraZoomSmoothSpeed);
        }
        else
        {
            // Smoothly return to original values if player goes back down
            Vector3 camPos = mainCam.transform.position;
            camPos.y = Mathf.Lerp(camPos.y, cameraOriginalY, Time.deltaTime * cameraMoveSmoothSpeed);
            mainCam.transform.position = camPos;

            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, cameraOriginalZoom, Time.deltaTime * cameraZoomSmoothSpeed);
        }
    }
    public void SetMaxFallSpeed(float speed)
    {
        originalMaxFallSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // HandleCameraFollowZoom();
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
