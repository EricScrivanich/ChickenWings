using System.Collections;
using System;
using UnityEngine;


public class MountainController : MonoBehaviour
{
    public CameraID cam;
    private bool isShaking = false;

    [SerializeField] private GameObject[] DeActivate;



    public Transform playerTransform;
    public PlayerID player;
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


    }

    private void Start()
    {
        foreach (GameObject obj in DeActivate)
        {
            obj.SetActive(false);
        }
        speed = player.constantPlayerForce;

        Debug.Log(targetCamera.aspect * targetCamera.orthographicSize * 2);
        originalCameraSize = targetCamera.orthographicSize;


        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void Update()
    {

        targetCamera.transform.Translate(Vector2.right * speed * Time.deltaTime);


    }




    public void AdjustCameraYPosition(float yThreshold, bool setBelow, float duration)
    {
        Vector3 playerTransformViewportPosition = targetCamera.WorldToViewportPoint(playerTransform.transform.position);
        bool adjustY = false;

        if ((playerTransformViewportPosition.y > yThreshold && setBelow) || (playerTransformViewportPosition.y < yThreshold && !setBelow))
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
        float playerTransformPosition = targetCamera.WorldToViewportPoint(playerTransform.transform.position).y;
        float startingY = targetCamera.transform.position.y;
        float movePercent = MathF.Abs(yThreshold - playerTransformPosition);
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

    public void AdjustCameraXPosition(float xThreshold, bool setBehind, float duration)
    {
        Vector3 playerTransformViewportPosition = targetCamera.WorldToViewportPoint(playerTransform.transform.position);
        bool adjustX = false;

        if ((playerTransformViewportPosition.x > xThreshold && setBehind) || (playerTransformViewportPosition.x < xThreshold && !setBehind))
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
        float playerTransformPosition = targetCamera.WorldToViewportPoint(playerTransform.transform.position).x;
        float startingX = targetCamera.transform.position.x;
        float movePercent = MathF.Abs(xThreshold - playerTransformPosition);
        float moveAmount = (targetCamera.aspect * targetCamera.orthographicSize * 2) * movePercent;

        if (!setBehind)
        {
            moveAmount *= -1;
        }

        float targetX = startingX + moveAmount;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            float newX = Mathf.SmoothStep(startingX, targetX, elapsed / duration);
            // Include the constant speed effect by adding the product of speed and elapsed time
            newX += speed * elapsed;
            targetCamera.transform.position = new Vector3(newX, targetCamera.transform.position.y, targetCamera.transform.position.z);
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
        float startTime = Time.time;
        float startingX = targetCamera.transform.position.x;
        float targetX = startingX + amount;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            float newX = Mathf.SmoothStep(startingX, targetX, elapsed / duration);
            // Include the constant speed effect by adding the product of speed and elapsed time
            newX += speed * elapsed;
            targetCamera.transform.position = new Vector3(newX, targetCamera.transform.position.y, targetCamera.transform.position.z);
            yield return null;
        }
    }



    private void ChangeSpeed(float targetSpeed, float duration)
    {
        if (changeSpeedCoroutine != null) StopCoroutine(changeSpeedCoroutine);
        changeSpeedCoroutine = StartCoroutine(ChangeSpeedCoroutine(targetSpeed, duration));
    }

    public void DieChangeSpeed()
    {
        ChangeSpeed(0, 2);
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
            player.globalEvents.OnAdjustConstantSpeed?.Invoke(speed);
            yield return null;
        }

        speed = targetSpeed;
    }


    public void ShakeCamera(float duration, float magnitude)
    {
        // if (!isShaking)
        // {
        //     StartCoroutine(Shake(duration, magnitude));
        //     isShaking = true;
        // }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = targetCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            targetCamera.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, -10);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetCamera.transform.localPosition = originalPosition;
        isShaking = false;
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

        cam.events.OnShakeCamera += ShakeCamera;



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
        cam.events.OnShakeCamera -= ShakeCamera;

    }
}
