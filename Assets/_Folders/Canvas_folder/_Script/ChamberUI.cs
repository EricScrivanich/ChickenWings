using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ChamberUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    [SerializeField] private Sprite[] eggIMAGES;

    [SerializeField] private Image eggButtomImage;


    [SerializeField] private PlayerID player;

    [SerializeField] private int[] rotationIndexForEggs;
    [SerializeField] private Image[] chamberButtonIMG;
    private RectTransform rect;
    private int currentRot;

    [SerializeField] private RectTransform chamberRect; // Chamber rotation target
    [SerializeField] private Color normalColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private float angularVelStopPoint = 1f; // The velocity threshold below which snapping occurs
    [SerializeField] private float rotationSnapDuration = 0.3f; // Duration of the snapping after release
    private Vector3 rotationClickAmount = new Vector3(0, 0, 60); // The snap amount (60 degrees for each bullet chamber)

    private bool isRotating = false; // Whether the chamber is currently rotating after swipe release
    private bool isButtonPressed = false; // Whether the swipe is happening
    private bool isDraggingUp = false;
    private bool isDraggingDown = false;
    private bool isSnappedUp = false;

    private Vector2 startPointerPosition;
    private Vector2 lastDragPosition;
    private float lastRotationDragPosition;
    private float currentRotation = 0f;
    private float angularVelocity = 0f; // Simulating angular velocity
    private float swipeStartTime;
    private float swipeEndTime;

    // Simulating rotation and damping
    [SerializeField] private float maxSwipeSpeedMultiplier = 3f; // Max multiplier for fast swipe
    [SerializeField] private float rotationMultiplier = 0.3f; // Rotation sensitivity per swipe
    [SerializeField] private float swipeSpeedThreshold = 200f; // Threshold to apply angular velocity after swipe
    [SerializeField] private float damping = 5f; // Damping factor to slow down rotation

    [SerializeField] private float snapUpPosition = -420f; // Position when snapped up
    [SerializeField] private float snapDuration = 0.3f;
    [SerializeField] private Ease snapEase = Ease.OutElastic;

    [SerializeField] private float dragUpThreshold = 140f; // Threshold for dragging up
    [SerializeField] private float dragDownThreshold = 150f; // Threshold for dragging down
    [SerializeField] private float dragDownInitialThreshold = 150f; // Threshold for dragging down
    [SerializeField] private int horizonatalDragThreshold;

    [SerializeField] private float maxDragDistance = 300f; // Maximum allowed drag distance

    private Vector2 startImagePosition;
    [SerializeField] private Vector2 finalPosition;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startImagePosition = rect.anchoredPosition; // Initial position of the UI element
        currentRot = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
        foreach (var img in chamberButtonIMG)
        {
            img.color = pressedColor;
        }

        startPointerPosition = eventData.position;
        lastDragPosition = eventData.position;
        lastRotationDragPosition = eventData.position.x;
        swipeStartTime = Time.unscaledTime; // Track the start time of the swipe
        angularVelocity = 0; // Reset angular velocity on new drag
        Debug.Log("Pointer Down at: " + startPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isButtonPressed)
        {
            if (!isSnappedUp) // Handle vertical drag when the chamber is not snapped up
            {
                HandleVerticalDragUp(eventData);
            }
            else // Handle horizontal rotation only when the chamber is snapped up
            {
                HandleVerticalDragDown(eventData);
                HandleHorizontalRotationDrag(eventData);
            }

            lastDragPosition = eventData.position;
        }
    }
    private void SnapToNearestUnlcokedRotation()
    {



    }
    private void HandleVerticalDragUp(PointerEventData eventData)
    {

        float dragDistanceY = eventData.position.y - startPointerPosition.y;
        dragDistanceY = Mathf.Clamp(dragDistanceY, 0, maxDragDistance);
        rect.anchoredPosition = startImagePosition + new Vector2(0, dragDistanceY); // Move UI element vertically



        if (dragDistanceY > dragUpThreshold)
        {
            Debug.Log("Threshold Passed, Snapping Up.");
            isDraggingUp = false;  // Reset dragging state
            isSnappedUp = true;
            SnapToTopPosition();

        }
        else
        {
            isDraggingUp = true;  // This will keep it dragging upward until the threshold is passed
        }


        // if (rect.anchoredPosition.y < dragDownThreshold && isDraggingUp)
        // {
        //     isSnappedUp = false;
        //     SnapToOriginalPosition(); // Snap back if dragged down beyond threshold
        // }

        // Debug.Log($"Dragging. Current position: {eventData.position}, Drag distance Y: {dragDistanceY}");
    }

    private void HandleVerticalDragDown(PointerEventData eventData)
    {
        // Calculate the vertical drag distance
        float dragDistanceY = startPointerPosition.y - eventData.position.y; // Invert the Y-axis to measure downward drag

        // If we haven't dragged down far enough, do nothing
        if (dragDistanceY < dragDownInitialThreshold)
        {
            return; // Ignore small downward drags
        }

        // If the drag is beyond the initial threshold, allow downward movement
        if (dragDistanceY >= dragDownInitialThreshold)
        {
            isDraggingDown = true; // Mark as dragging down
            Debug.Log($"Dragging Down. Drag distance Y: {dragDistanceY}");

            // Move the UI element downward based on drag distance, but don't let it exceed the start position
            float newPosY = Mathf.Clamp(snapUpPosition - dragDistanceY, startImagePosition.y, snapUpPosition);
            rect.anchoredPosition = new Vector2(startImagePosition.x, newPosY);
        }

        // If the downward drag exceeds the full threshold, snap back to the original position
        if (dragDistanceY > dragDownThreshold)
        {
            Debug.Log("Threshold for dragging down passed, snapping to original position.");
            isDraggingDown = false; // Reset the dragging down state
            isSnappedUp = false; // Mark chamber as no longer snapped up
            SnapToOriginalPosition(); // Snap back to the original position
        }
    }

    private void HandleHorizontalRotationDrag(PointerEventData eventData)
    {
        float dragDistanceX = eventData.position.x - lastRotationDragPosition;

        if (Mathf.Abs(dragDistanceX) > horizonatalDragThreshold) // Only rotate if there is horizontal drag
        {
            float rotationAmount = (-dragDistanceX) * rotationMultiplier; // Negative to handle rotation direction
            currentRotation += rotationAmount; // Update rotation based on swipe direction
            chamberRect.localEulerAngles = new Vector3(0, 0, currentRotation); // Apply rotation immediately during drag

            // Save the current drag speed as angular velocity
            angularVelocity = rotationAmount / Time.unscaledDeltaTime; // This becomes the angular velocity on release
            lastRotationDragPosition = eventData.position.x;

            // Debug.Log("Rotating. Current Rotation: " + currentRotation + ", Angular Velocity: " + angularVelocity);
        }
    }

    // private int SetCurrentAmmoType()
    // {

    //     foreach (var vect in rotationsForEggs)
    //     {
    //         if (vect.x == currentRot)
    //         {
    //             if ()
    //         }
    //     }

    // }

    public void OnPointerUp(PointerEventData eventData)
    {
        swipeEndTime = Time.unscaledTime; // Get the swipe end time

        // When the player releases, the angular velocity remains what it was during the last drag frame
        Debug.Log("Pointer Up. Final Angular Velocity: " + angularVelocity);

        isButtonPressed = false;
        foreach (var img in chamberButtonIMG)
        {
            img.color = normalColor;
        }

        if (isDraggingDown && isSnappedUp)
        {
            SnapToTopPosition();
            isDraggingDown = false;
        }

        if (!isDraggingDown && !isDraggingUp)
        {
            isRotating = true;
        }

        // Enable further rotation after release, using the last known angular velocity
    }

    private void Update()
    {
        if (isRotating && !isDraggingUp && !isDraggingDown)
        {
            // Apply damping to gradually reduce angular velocity over time
            angularVelocity = Mathf.Lerp(angularVelocity, 0, damping * Time.unscaledDeltaTime);

            // Update the chamber's rotation based on current angular velocity
            currentRotation += angularVelocity * Time.unscaledDeltaTime; // Continue rotating the chamber
            chamberRect.localEulerAngles = new Vector3(0, 0, currentRotation); // Apply rotation

            // If the angular velocity is below the threshold, snap to the nearest 60-degree mark
            if (Mathf.Abs(angularVelocity) < angularVelStopPoint)
            {
                isRotating = false;
                HandleSnapping(); // Snap to the nearest 60-degree mark when velocity is low
            }

            // Debug.Log("Rotating with Angular Velocity: " + angularVelocity);
        }

        else if (isRotating && isDraggingDown)
        {
            HandleSnapping();
        }
    }

    private void HandleSnapping()
    {

        int snappedAngle = Mathf.RoundToInt(currentRotation / 60f);
        Vector3 targetRotation = rotationClickAmount * snappedAngle;
        chamberRect.DOLocalRotate(targetRotation, rotationSnapDuration).SetEase(Ease.OutBack).SetUpdate(true);


        if (snappedAngle < eggIMAGES.Length)
        {
            eggButtomImage.sprite = eggIMAGES[snappedAngle];
            player.events.OnSwitchAmmoType?.Invoke(snappedAngle);

        }

        // foreach(var r in rotationIndexForEggs)
        // {
        //     if 
        // }

    }



    private void OnDragUp()
    {
        Debug.Log("Dragged Up! Snapping to position.");
        SnapToTopPosition(); // Snap up after vertical drag
    }

    private void SnapToTopPosition()
    {
        Time.timeScale = 0;
        rect.DOAnchorPosY(snapUpPosition, snapDuration).SetEase(snapEase).SetUpdate(true);
        isSnappedUp = true; // Set snapped up to true
    }

    private void SnapToOriginalPosition()
    {
        rect.DOAnchorPosY(startImagePosition.y, snapDuration).SetEase(snapEase).SetUpdate(true);
        isSnappedUp = false; // Reset snapped state
        isDraggingUp = false; // Reset dragging up state
        Time.timeScale = 1;
    }
}