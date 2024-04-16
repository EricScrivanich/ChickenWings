using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MountainController : MonoBehaviour
{
    public CameraID cam;

    public Transform player;
    public static Action<Vector2, float> OnMoveCamera;
    public static Action<float, float> OnChangeZoom;
    public static Action<float, float> OnChangeSpeed;


    [SerializeField] private float startSpeed;
    private float speed;

    private float originalCameraSize;




    // Reference to the camera you want to move.
    public Camera targetCamera;

    // Coroutine references
    private Coroutine moveCameraCoroutine;
    private Coroutine changeZoomCoroutine;
    private Coroutine changeSpeedCoroutine;
    private Coroutine adjustCameraXCoroutine;
    private Coroutine adjustCameraYCoroutine;

    private Coroutine moveCameraYCoroutine;
    private Coroutine moveCameraXCoroutine;

    private void Awake()
    {

        speed = startSpeed;
    }

    private void Start()
    {
        Debug.Log(targetCamera.aspect * targetCamera.orthographicSize * 2);
        originalCameraSize = targetCamera.orthographicSize;


        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void Update()
    {

        transform.Translate(Vector2.left * speed * Time.deltaTime);


    }

    public void StartMoving()
    {


    }



    public void AdjustCameraYPosition(float yThreshold, bool setBelow, float duration)
    {
        Vector3 playerViewportPosition = targetCamera.WorldToViewportPoint(player.transform.position);
        bool adjustY = false;

        if ((playerViewportPosition.y > yThreshold && setBelow) || (playerViewportPosition.y < yThreshold && !setBelow))
        {
            adjustY = true;
        }

        if (adjustY)
        {
            if (adjustCameraYCoroutine != null) StopCoroutine(adjustCameraYCoroutine);
            if (moveCameraYCoroutine != null) StopCoroutine(moveCameraYCoroutine);

            adjustCameraYCoroutine = StartCoroutine(AdjustCameraYCoroutine(yThreshold, setBelow, duration));
        }
    }

    private IEnumerator AdjustCameraYCoroutine(float yThreshold, bool setBelow, float duration)
    {
        float playerPosition = targetCamera.WorldToViewportPoint(player.transform.position).y;
        float startingY = targetCamera.transform.position.y;
        float movePercent = MathF.Abs(yThreshold - playerPosition);
        float moveAmount = (targetCamera.orthographicSize * 2) * movePercent;

        if (setBelow)
        {
            moveAmount *= -1;
        }

        float targetY = startingY + moveAmount;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.SmoothStep(startingY, targetY, elapsedTime / duration);

            targetCamera.transform.position = new Vector3(targetCamera.transform.position.x, newY, targetCamera.transform.position.z);
            yield return null;
        }
    }

    public void AdjustCameraXPosition(float xThreshold, bool setBehind, float duration)
    {
        Vector3 playerViewportPosition = targetCamera.WorldToViewportPoint(player.transform.position);
        bool adjustX = false;

        if ((playerViewportPosition.x > xThreshold && setBehind) || (playerViewportPosition.x < xThreshold && !setBehind))
        {
            adjustX = true;
        }



        if (adjustX)
        {
            if (adjustCameraXCoroutine != null) StopCoroutine(adjustCameraXCoroutine);
            if (moveCameraXCoroutine != null) StopCoroutine(moveCameraXCoroutine);
            adjustCameraXCoroutine = StartCoroutine(AdjustCameraXCoroutine(xThreshold, setBehind, duration));
        }
    }

    private IEnumerator AdjustCameraXCoroutine(float xThreshold, bool setBehind, float duration)
    {
        float playerPosition = targetCamera.WorldToViewportPoint(player.transform.position).x;
        float startingX = targetCamera.transform.position.x;
        float movePercent = MathF.Abs(xThreshold - playerPosition);
        float moveAmount = (targetCamera.aspect * targetCamera.orthographicSize * 2) * movePercent;

        if (!setBehind)
        {
            moveAmount *= -1;
        }

        float targetX = startingX + moveAmount;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.SmoothStep(startingX, targetX, elapsedTime / duration);

            targetCamera.transform.position = new Vector3(newX, targetCamera.transform.position.y, targetCamera.transform.position.z);
            yield return null;
        }
    }








    private void MoveCamera(Vector2 targetPosition, float duration)
    {
        if (moveCameraCoroutine != null) StopCoroutine(moveCameraCoroutine);
        moveCameraCoroutine = StartCoroutine(MoveCameraCoroutine(targetPosition, duration));
    }

    private void MoveCameraYPosition(float amount, float time)
    {
        if (moveCameraYCoroutine != null) StopCoroutine(moveCameraYCoroutine);
        if (adjustCameraYCoroutine != null) StopCoroutine(adjustCameraYCoroutine);
        moveCameraYCoroutine = StartCoroutine(MoveCameraYCourintine(amount, time));

    }

    private IEnumerator MoveCameraYCourintine(float targetY, float duration)
    {
        float time = 0;
        float startingY = targetCamera.transform.position.y;
        

        while (time < duration)
        {
            time += Time.deltaTime;
            float newY = Mathf.SmoothStep(startingY, targetY, time / duration);

            targetCamera.transform.position = new Vector3(targetCamera.transform.position.x, newY, targetCamera.transform.position.z);
            yield return null;
        }

    }

    private void MoveCameraXPosition(float amount, float time)
    {
        if (moveCameraXCoroutine != null) StopCoroutine(moveCameraXCoroutine);
        if (adjustCameraXCoroutine != null) StopCoroutine(adjustCameraXCoroutine);
        moveCameraXCoroutine = StartCoroutine(MoveCameraXCourintine(amount, time));

    }

    private IEnumerator MoveCameraXCourintine(float amount, float duration)
    {
        float time = 0;
        float startingX = targetCamera.transform.position.x;
        float targetX = startingX + amount;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newX = Mathf.SmoothStep(startingX, targetX, time / duration);

            targetCamera.transform.position = new Vector3(newX, targetCamera.transform.position.y, targetCamera.transform.position.z);
            yield return null;
        }

    }

    private IEnumerator MoveCameraCoroutine(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = targetCamera.transform.position;


        while (time < duration)
        {
            time += Time.deltaTime;
            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            targetCamera.transform.position = new Vector3(newPosition.x, newPosition.y, targetCamera.transform.position.z);
            yield return null;
        }

        targetCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, targetCamera.transform.position.z);
    }

    // private void MoveCamera(List<GameObject> targetObjects, float duration)
    // {
    //     if (moveCameraCoroutine != null) StopCoroutine(moveCameraCoroutine);
    //     moveCameraCoroutine = StartCoroutine(MoveCameraCoroutine(targetObjects, duration));
    // }

    // private IEnumerator MoveCameraCoroutine(List<GameObject> targetObjects, float duration)
    // {
    //     Debug.Log("ChangingCam");

    //     // Ensure there's at least one object in the list
    //     if (targetObjects == null || targetObjects.Count == 0) yield break;

    //     // Calculate the duration for each segment based on the number of objects
    //     int numberOfSegments = targetObjects.Count;
    //     float segmentDuration = duration / numberOfSegments;

    //     Vector3 startPosition = targetCamera.transform.position;

    //     for (int i = 0; i < numberOfSegments; i++)
    //     {
    //         GameObject currentTarget = targetObjects[i];
    //         float segmentTime = 0;

    //         while (segmentTime < segmentDuration)
    //         {
    //             if (!currentTarget)
    //             {
    //                 Debug.LogWarning("Target GameObject is null, skipping to next target.");
    //                 break; // Skip to the next target if the current one is null (destroyed or not assigned)
    //             }

    //             Vector2 targetPosition = currentTarget.transform.position;
    //             segmentTime += Time.deltaTime;

    //             // Calculate the new position
    //             float t = segmentTime / segmentDuration;
    //             Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, t);
    //             targetCamera.transform.position = new Vector3(newPosition.x, newPosition.y, targetCamera.transform.position.z);

    //             // Check if the camera is close enough to the target position
    //             if (Vector2.Distance(newPosition, targetPosition) < 0.4f)
    //             {
    //                 break; // Move to the next target if within a small distance
    //             }

    //             yield return null;
    //         }

    //         startPosition = targetCamera.transform.position; // Update the start position for the next segment

    //         // Ensure the camera is exactly at the last known position of the current target
    //         if (currentTarget)
    //         {
    //             Vector2 finalPosition = currentTarget.transform.position;
    //             targetCamera.transform.position = new Vector3(finalPosition.x, finalPosition.y, targetCamera.transform.position.z);
    //         }
    //     }
    // }





    private void ChangeSpeed(float targetSpeed, float duration)
    {
        if (changeSpeedCoroutine != null) StopCoroutine(changeSpeedCoroutine);
        changeSpeedCoroutine = StartCoroutine(ChangeSpeedCoroutine(targetSpeed, duration));
    }

    private void ChangeZoom(float targetZoom, float duration)
    {
        if (changeZoomCoroutine != null) StopCoroutine(changeZoomCoroutine);
        changeZoomCoroutine = StartCoroutine(ChangeZoomCoroutine(targetZoom, duration));
    }


    private IEnumerator ChangeZoomCoroutine(float targetZoom, float duration)
    {
        float time = 0;
        float startZoom = targetCamera.orthographicSize;

        while (time < duration)
        {
            time += Time.deltaTime;
            targetCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, time / duration);
            yield return null;
        }

        targetCamera.orthographicSize = targetZoom;
    }

    private IEnumerator ChangeSpeedCoroutine(float targetSpeed, float duration)
    {
        float time = 0;
        float startSpeed = speed;

        while (time < duration)
        {
            time += Time.deltaTime;
            speed = Mathf.Lerp(startSpeed, targetSpeed, time / duration);
            yield return null;
        }

        speed = targetSpeed;
    }

    private void OnEnable()
    {
        // OnMoveCamera += MoveCamera;
        // OnChangeSpeed += ChangeSpeed;
        // OnChangeZoom += ChangeZoom;

        cam.events.AdjustCameraX += AdjustCameraXPosition;
        cam.events.AdjustCameraY += AdjustCameraYPosition;
        cam.events.OnChangeYPosition += MoveCameraYPosition;
        cam.events.OnChangeXPosition += MoveCameraXPosition;

        // cam.events.OnChangePosition += MoveCamera;
        cam.events.OnChangeSpeed += ChangeSpeed;
        cam.events.OnChangeZoom += ChangeZoom;



    }

    private void OnDisable()
    {

        // OnMoveCamera -= MoveCamera;
        // OnChangeSpeed -= ChangeSpeed;
        // OnChangeZoom -= ChangeZoom;

        cam.events.AdjustCameraX -= AdjustCameraXPosition;
        cam.events.AdjustCameraY -= AdjustCameraYPosition;
        cam.events.OnChangeYPosition -= MoveCameraYPosition;
        cam.events.OnChangeXPosition -= MoveCameraXPosition;





        // cam.events.OnChangePosition -= MoveCamera;
        cam.events.OnChangeSpeed -= ChangeSpeed;
        cam.events.OnChangeZoom -= ChangeZoom;
    }
}
