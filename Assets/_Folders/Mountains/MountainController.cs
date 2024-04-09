using System.Collections;
using System;
using UnityEngine;

public class MountainController : MonoBehaviour
{
    public static  Action<Vector2, float> OnMoveCamera;
    public static  Action<float, float> OnChangeZoom;
    public static  Action<float, float> OnChangeSpeed;

    [SerializeField] private float startSpeed;
    private float speed;

    // Reference to the camera you want to move.
    public Camera targetCamera;

    // Coroutine references
    private Coroutine moveCameraCoroutine;
    private Coroutine changeZoomCoroutine;
    private Coroutine changeSpeedCoroutine;

    private void Awake()
    {
        speed = startSpeed;
    }

    private void Start()
    {
        OnMoveCamera += MoveCamera;
        OnChangeSpeed += ChangeSpeed;
        OnChangeZoom += ChangeZoom;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        OnMoveCamera -= MoveCamera;
        OnChangeSpeed -= ChangeSpeed;
        OnChangeZoom -= ChangeZoom;
    }

    private void MoveCamera(Vector2 targetPosition, float duration)
    {
        if (moveCameraCoroutine != null) StopCoroutine(moveCameraCoroutine);
        moveCameraCoroutine = StartCoroutine(MoveCameraCoroutine(targetPosition, duration));
    }

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
}
