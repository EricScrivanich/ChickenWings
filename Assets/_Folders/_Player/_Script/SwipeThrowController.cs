using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeThrowController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Stuck scythe Settings")]
    [SerializeField] private float centerPressRadius;
    [SerializeField] private float dragThresholdForCenter;
    [Header("Swipe Settings")]
    public float dragThreshold; // Minimum drag distance to register a swipe
    public float maxDragTime = 1f; // Maximum time to affect the velocity calculation
    public float distanceMultiplier = 1f; // Multiplier for drag distance
    public float timeMultiplier = 1f; // Multiplier for drag time
    public float deadZoneRadius = 10f; // Deadzone radius for swipe detection
    private bool ignoreAll = false;

    public float angleSwipeMinDiff;

    [SerializeField] private float angleDeadzone;

    public bool usingScytheSwipeArea;

    [SerializeField] private RectTransform scytheSwipeArea;
    private Vector2 scytheSwipeCenterPos;

    public static Action<Vector2, float, float> OnSwipeButton;

    private RectTransform buttonRect;



    [SerializeField] private PlayerID player;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float startTime;
    private bool isDragging;
    private bool trackingStuckScythe = false;

    public Vector2 VelocityVector { get; private set; } // Final velocity vector from swipe
    private bool sentSwipeData;
    private void Awake()
    {

    }

    private void Start()
    {
        buttonRect = GetComponent<RectTransform>();

        if (usingScytheSwipeArea) scytheSwipeCenterPos = scytheSwipeArea.position;
        else scytheSwipeCenterPos = buttonRect.position;

    }
    void ResetSwipeData()
    {
        startPoint = Vector2.zero;
        endPoint = Vector2.zero;
        startTime = 0f;
        isDragging = false;
        VelocityVector = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (isDragging) return;
        Debug.Log("Yuh");
        // Record the initial touch point and time
        startPoint = eventData.position;
        if (player.scytheIsStuck)
        {
            Debug.Log("Press distance is: " + Vector2.Distance(startPoint, scytheSwipeCenterPos));
        }

        if (player.scytheIsStuck && Vector2.Distance(startPoint, scytheSwipeCenterPos) <= centerPressRadius)
        {
            trackingStuckScythe = true;
            Debug.Log("Trakcing the stuck swipe");
            return;
        }
        else
        {
            trackingStuckScythe = false;
        }

        bool left = eventData.position.x < scytheSwipeCenterPos.x;
        bool under = eventData.position.y < scytheSwipeCenterPos.y;
        float rot = 0;
        if (left)
        {
            if (under) rot = 135;
            else rot = 225;
        }
        else
        {
            if (under) rot = 45;
            else rot = 315;

        }
        player.events.OnShowCursor?.Invoke(eventData.position, rot);

        // Debug.Log("start point is: " + startPoint.y + " pos is: " + buttonRect.position.y);
        startTime = Time.time;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ignoreAll) return;
        if (trackingStuckScythe)
        {
            endPoint = eventData.position;
            if (Vector2.Distance(startPoint, endPoint) > dragThresholdForCenter)
            {
                Debug.Log("Sedning Drag data for stuck sythe");
                player.events.OnStuckScytheSwipe?.Invoke((endPoint - startPoint).normalized);
                trackingStuckScythe = false;
                ignoreAll = true;
            }
            return;
        }
        player.events.SendParrySwipeData?.Invoke(eventData.position);
        if (!sentSwipeData)
        {

            endPoint = eventData.position;


            // Calculate drag distance
            float dragDistance = Vector2.Distance(startPoint, endPoint);

            if (dragDistance > dragThreshold)
            {

                bool isLeftSide = startPoint.x < scytheSwipeCenterPos.x;
                bool under = startPoint.y < scytheSwipeCenterPos.y;



                sentSwipeData = true;
                // Calculate the angle of the swipe in degrees
                Vector2 direction = (endPoint - startPoint).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360;

                if (MathF.Abs(angle - 315) < angleSwipeMinDiff)
                {
                    Debug.Log("Checking Angle, x and y diff of start is: " + xDistance(startPoint.x) + ", " + yDistance(startPoint.y));
                    if (xDistance(startPoint.x) < yDistance(startPoint.y))
                    {

                        player.events.OnSwipeScytheAttack?.Invoke(315);
                        return;

                    }
                    else
                    {

                        player.events.OnSwipeScytheAttack?.Invoke(-135);
                        return;

                    }


                }
                else if (MathF.Abs(angle - 45) < angleSwipeMinDiff)
                {
                    if (xDistance(startPoint.x) < yDistance(startPoint.y))
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(45);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(-225);
                        return;
                    }


                }
                else if (MathF.Abs(angle - 135) < angleSwipeMinDiff)
                {
                    if (xDistance(startPoint.x) < yDistance(startPoint.y))
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(135);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(-300);
                        return;
                    }


                }
                else if (MathF.Abs(angle - 225) < angleSwipeMinDiff)
                {
                    if (xDistance(startPoint.x) < yDistance(startPoint.y))
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(225);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(-350);
                        return;
                    }


                }

                Debug.Log("Not using angles");


                if (angle < 45 || angle > 315)
                {
                    if (under)
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(1);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(0);
                        return;
                    }
                }

                else if (angle < 225 && angle > 135)
                {
                    if (under)
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(179);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(180);
                        return;
                    }
                }

                else if (angle > 45 && angle < 135)
                {
                    if (isLeftSide)
                    {
                        Debug.Log("LEft side up");
                        player.events.OnSwipeScytheAttack?.Invoke(91);
                        return;
                    }
                    else
                    {
                        Debug.Log("Right side up");

                        player.events.OnSwipeScytheAttack?.Invoke(90);
                        return;
                    }
                }
                else if (angle > 225 && angle < 315)
                {
                    if (isLeftSide)
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(269);
                        return;
                    }
                    else
                    {
                        player.events.OnSwipeScytheAttack?.Invoke(270);
                        return;
                    }
                }










                // if (isLeftSide)
                // {
                //     if (angle >= 270 || angle == 0)
                //     {
                //         angle = 269;
                //     }
                //     else if (angle <= 90)
                //         angle = 91;
                // }
                // else
                // {
                //     if (angle < 270 && angle >= 180)
                //         angle = 270;
                //     else if (angle > 90 && angle < 180)
                //         angle = 90;
                // }

                // if ((angle > 360 - angleDeadzone || angle == 0) && startPoint.y < buttonRect.position.y)
                // {
                //     angle = 1;

                // }
                // else if (angle < angleDeadzone && startPoint.y > buttonRect.position.y)
                // {
                //     angle = 0;
                // }
                // else if (angle <= 180 + angleDeadzone && angle >= 180 && startPoint.y < buttonRect.position.y)
                // {
                //     angle = 179;
                // }
                // else if (angle >= 180 - angleDeadzone && angle < 180 && startPoint.y > buttonRect.position.y)
                // {
                //     angle = 180;
                // }



                // player.events.OnSwipeScytheAttack?.Invoke(angle);

            }
        }
        // Update the end point as the user drags



    }

    private float xDistance(float x)
    {
        return MathF.Abs(x - scytheSwipeCenterPos.x);
    }
    private float yDistance(float y)
    {
        return MathF.Abs(y - scytheSwipeCenterPos.y);

    }

    private bool IsSwipeFromLeftSide(Vector2 screenPosition)
    {
        if (buttonRect == null)
        {
            Debug.LogWarning("Button RectTransform is not assigned!");
            return false;
        }

        // Convert the screen position to local UI space
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            buttonRect, screenPosition, null, out localPoint);

        // Get button width and determine its center
        float buttonWidth = buttonRect.rect.width;
        float halfWidth = buttonWidth / 2f;

        // If localPoint.x is less than 0, it's on the left half; otherwise, it's on the right half
        return localPoint.x < 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ignoreAll = false;
        if (sentSwipeData) sentSwipeData = false;


        if (!isDragging)
            return;
        isDragging = false;
        player.events.ReleaseSwipe?.Invoke();
        // Calculate the drag distance and duration
        float dragDistance = Vector2.Distance(startPoint, endPoint);
        float dragTime = Time.time - startTime;

        // Check if the drag distance exceeds the threshold
        // if (dragDistance >= dragThreshold)
        // {
        //     // Calculate the direction of the swipe
        //     Vector2 direction = (endPoint - startPoint).normalized;

        //     // Calculate the angle based on the averaged drag
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //     // Calculate the magnitude based on distance and time
        //     float magnitude = (dragDistance * distanceMultiplier) / Mathf.Clamp(dragTime, 0.1f, maxDragTime) * timeMultiplier;

        //     // Apply deadzone logic
        //     if (dragDistance > deadZoneRadius)
        //     {
        //         // Calculate the velocity vector
        //         VelocityVector = direction * magnitude;
        //         Vector4 val = new Vector4(direction.x, direction.y, dragDistance, dragTime);
        //         Debug.Log($"Velocity Vector: {dragTime}, {dragDistance}, Angle: {angle}");
        //         player.events.OnSwipeButton?.Invoke(direction, dragDistance, dragTime);
        //     }
        //     else
        //     {
        //         Debug.Log("Swipe ignored due to deadzone.");
        //         VelocityVector = Vector2.zero;
        //     }
        // }

        // Reset swipe data
        ResetSwipeData();
    }
}