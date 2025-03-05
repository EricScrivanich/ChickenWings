using UnityEngine;
using DG.Tweening;

public class CursorTracker : MonoBehaviour
{
    public PlayerID player;

    private Sequence cursorSeq;

    [SerializeField] private Animator anim;
    [SerializeField] private float maxSliceDistance;
    [SerializeField] private float swipeDistanceThreshold = 1.0f; // Minimum swipe distance to trigger calculations

    private SpriteRenderer sprite;
    private TrailRenderer trail;
    private Camera cam;
    private bool showingCursor = false;
    private Sequence camSeq;
    [SerializeField] private float zoomInDuration;
    [SerializeField] private float zoomOutDuration;
    [SerializeField] private Ease easeIn;
    [SerializeField] private Ease easeOut;
    [SerializeField] private float zoomInMultiplier;
    private float startZoom;
    private Vector2 lastPos;


    private Vector2 circleCenter; // Center of the circle (player's position)
    private Vector2 swipeStart;  // Start position of the swipe
    private Vector2 swipeEnd;    // End position of the swipe
    private bool calculationTriggered = false; // Ensure calculations happen only once

    [SerializeField] private ButtonColorsSO colorSO;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        startZoom = cam.orthographicSize;
        sprite = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        // if (sprite != null)
        //     sprite.color = colorSO.WeaponColor;

        // if (trail != null)
        // {
        //     trail.startColor = new Color(colorSO.WeaponColor.r, colorSO.WeaponColor.g, colorSO.WeaponColor.b, 1f);
        //     // trail.endColor = new Color(colorSO.WeaponColor.r, colorSO.WeaponColor.g, colorSO.WeaponColor.b, .2f);
        // }


    }
    private bool eggButtonWasEnabled;
    private void EnableSwiper(Vector2 pos, float rot)
    {
        if (cursorSeq != null && cursorSeq.IsPlaying())
            cursorSeq.Complete();
        sprite.color = Color.white;
        trail.emitting = true;

        swipeStart = pos;
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cam.nearClipPlane));
        transform.position = worldPosition;
        transform.eulerAngles = Vector3.forward * rot;
        showingCursor = true;
        sprite.enabled = true;
        trail.enabled = true;
        // if (camSeq != null && camSeq.IsPlaying())
        //     camSeq.Kill();

        // gameObject.SetActive(true);
        // SwitchCamera(true, playerPos);
        // if (player.canPressEggButton)
        // {
        //     player.canPressEggButton = false;
        //     eggButtonWasEnabled = true;

        // }

        // // Set the circle center to the player's position
        // circleCenter = playerPos;
    }

    public void Exit()
    {
        // if (eggButtonWasEnabled)
        // {
        //     player.canPressEggButton = true;
        //     eggButtonWasEnabled = false;


        // }

        // SwitchCamera(false, Vector2.zero);
    }

    public void SwitchCamera(bool zoomIn, Vector2 playerPos)
    {
        // if (camSeq != null && camSeq.IsPlaying())
        //     camSeq.Kill();
        // camSeq = DOTween.Sequence();

        // if (zoomIn)
        // {
        //     camSeq.Append(cam.transform.DOMoveX(playerPos.x, zoomInDuration));
        //     camSeq.Join(cam.transform.DOMoveY(playerPos.y, zoomInDuration));
        //     camSeq.Join(cam.DOOrthoSize(startZoom * zoomInMultiplier, zoomInDuration));
        //     camSeq.Play().SetEase(easeIn);
        // }
        // else
        // {
        //     camSeq.Append(cam.transform.DOMoveX(0, zoomOutDuration));
        //     camSeq.Join(cam.transform.DOMoveY(0, zoomOutDuration));
        //     camSeq.Join(cam.DOOrthoSize(startZoom, zoomOutDuration));
        //     camSeq.Play().SetEase(easeOut).OnComplete(() => gameObject.SetActive(false));
        // }
    }

    private void Track(Vector2 pos)
    {
        // Convert screen position to world position
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cam.nearClipPlane));
        // Debug.Log("Tracking" + worldPosition);

        transform.position = worldPosition;
        Vector2 direction = (pos - lastPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * angle;
        lastPos = pos;


        // If not showing the cursor, enable it
        // if (!showingCursor)
        // {
        //     showingCursor = true;
        //     sprite.enabled = true;
        //     trail.enabled = true;

        //     // Record the start of the swipe
        //     swipeStart = worldPosition;
        // }

        // // Record the end of the swipe dynamically
        // swipeEnd = worldPosition;

        // // Check if swipe distance exceeds the threshold and trigger calculations
        // float totalSwipeDistance = Vector2.Distance(swipeStart, swipeEnd);
        // if (!calculationTriggered && totalSwipeDistance >= swipeDistanceThreshold)
        // {
        //     calculationTriggered = true;
        //     CalculateSwipeData();
        // }
    }

    public void HideCursor()
    {
        showingCursor = false;
        trail.emitting = false;
        cursorSeq = DOTween.Sequence();
        // cursorSeq.AppendInterval(.f);
        cursorSeq.Append(sprite.DOFade(0, .2f));
        cursorSeq.Play().OnComplete(() => trail.enabled = false);
        // trail.enabled = false;

        // Reset swipe data
        swipeStart = Vector2.zero;
        swipeEnd = Vector2.zero;
        calculationTriggered = false; // Reset calculation trigger
    }

    private void CalculateSwipeData()
    {
        // Calculate the direction vector of the swipe
        Vector2 swipeDirection = swipeEnd - swipeStart;

        // Calculate the angle of the swipe relative to the circle center
        float angle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;

        // Normalize the angle to range [0, 360]
        if (angle < 0)
            angle += 360;

        // Determine whether the swipe is clockwise or counterclockwise
        bool isClockwise = IsSwipeClockwise();

        // Calculate the perpendicular distance from the circle center to the swipe line
        float perpendicularDistance = CalculatePerpendicularDistance(circleCenter, swipeStart, swipeDirection);
        anim.transform.position = circleCenter;
        int xRot = 0;
        if (!isClockwise) xRot = 180;
        Quaternion rotation = Quaternion.Euler(xRot, 0, angle);
        anim.transform.rotation = rotation;


        if (perpendicularDistance <= maxSliceDistance)
            anim.SetTrigger("Slice");
        else
            anim.SetTrigger("Swoop");

        // Debug output
        Debug.Log($"Swipe Angle: {angle}°");
        Debug.Log($"Perpendicular Distance from Center: {perpendicularDistance}");
        Debug.Log($"Swipe Direction: {(isClockwise ? "Clockwise" : "Counterclockwise")}");
    }

    private bool IsSwipeClockwise()
    {
        // Determine the direction vector of the swipe
        Vector2 swipeDirection = swipeEnd - swipeStart;

        // Calculate the angle of the swipe in degrees
        float angle = Mathf.Atan2(swipeDirection.y, swipeDirection.x) * Mathf.Rad2Deg;
        bool isAboveCenter;// Swipe starts above the circle
        bool isToTheRight;
        // Normalize the angle to the range [0, 360]
        if (angle < 0)
            angle += 360;

        // Threshold to determine if the swipe is vertical or horizontal
        const float verticalThreshold = 45f; // 45 degrees from vertical axis

        // If the swipe is mostly vertical (angle is close to 90° or 270°)
        if ((angle > 90 - verticalThreshold && angle < 90 + verticalThreshold) ||
            (angle > 270 - verticalThreshold && angle < 270 + verticalThreshold))
        {
            // Check vertical swipe direction
            isAboveCenter = swipeStart.x > circleCenter.x;
            isToTheRight = swipeEnd.y < swipeStart.y;
        }
        else
        {
            isAboveCenter = swipeStart.y > circleCenter.y; // Swipe starts above the circle
            isToTheRight = swipeEnd.x > swipeStart.x;
        }

        // For horizontal or diagonal swipes, use the original logic


        // Clockwise if:
        // - Swipe starts above the center and moves right
        // - OR swipe starts below the center and moves left
        return (isAboveCenter && isToTheRight) || (!isAboveCenter && !isToTheRight);
    }
    private float CalculatePerpendicularDistance(Vector2 center, Vector2 linePoint, Vector2 direction)
    {
        // Represent the infinite line as ax + by + c = 0
        float a = -direction.y;
        float b = direction.x;
        float c = direction.y * linePoint.x - direction.x * linePoint.y;

        // Apply the point-to-line distance formula
        return Mathf.Abs(a * center.x + b * center.y + c) / Mathf.Sqrt(a * a + b * b);
    }

    private void OnEnable()
    {
        showingCursor = false;
        sprite.enabled = false;
        trail.enabled = false;

        player.events.SendParrySwipeData += Track;
        player.events.ReleaseSwipe += HideCursor;
        player.events.OnShowCursor += EnableSwiper;
    }

    private void OnDisable()
    {
        player.events.SendParrySwipeData -= Track;
        player.events.ReleaseSwipe -= HideCursor;
        player.events.OnShowCursor -= EnableSwiper;

    }
}