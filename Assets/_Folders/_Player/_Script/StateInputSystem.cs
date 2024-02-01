using UnityEngine;
using UnityEngine.InputSystem;

public class StateInputSystem : MonoBehaviour
{
    private InputController controls;
    private Vector2 touchStartPosition;
    private float touchStartTime;
    private const float swipeThreshold = 0.3f; // Adjust as needed
    private const float tapTimeThreshold = 0.2f; // Adjust as needed
    public PlayerID ID;
    [SerializeField] private RectTransform touchButtonRectTransform; 

    private void Awake()
    {
        controls = new InputController();

        // Bind existing actions to methods
        controls.Movement.Jump.performed += ctx => ID.events.OnJump?.Invoke();
        controls.Movement.JumpRight.performed += ctx => ID.events.OnFlipRight?.Invoke();
        controls.Movement.JumpLeft.performed += ctx => ID.events.OnFlipLeft?.Invoke();
        controls.Movement.Dash.performed += ctx => ID.events.OnDash?.Invoke();
        controls.Movement.Drop.performed += ctx => ID.events.OnDrop?.Invoke();
        controls.Movement.DropEgg.performed += ctx => ID.events.OnEggDrop?.Invoke();
        controls.Movement.JumpHold.performed += ctx => ID.events.OnJumpHeld?.Invoke(true);
        controls.Movement.JumpHold.canceled += ctx => ID.events.OnJumpHeld?.Invoke(false);
        controls.Movement.Fireball.performed += ctx => ID.events.OnAttack?.Invoke(true);
        controls.Movement.Parachute.performed += ctx => ID.events.OnParachute?.Invoke(true);
        controls.Movement.Parachute.canceled += ctx => ID.events.OnParachute?.Invoke(false); 
        // controls.Movement.Parachute.performed += ctx =>
// {
//     // if (Input.touchCount == 2)
//     // {
//     //     ID.events.OnParachute?.Invoke(true);
//     // }
// };

        // Bind touch input actions
        // if (touchButtonRectTransform)
        // {
        //      controls.Movement.TouchPress.started += ctx => StartTouch(ctx);
        // controls.Movement.TouchPress.canceled += ctx => EndTouch(ctx);
        // controls.Movement.TouchPosition.performed += ctx => UpdateTouchPosition(ctx);

        // }

    }
    private void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }
}

    // private void StartTouch(InputAction.CallbackContext context)
    // {
    //     touchStartPosition = controls.Movement.TouchPosition.ReadValue<Vector2>();
    //      Debug.Log($"Tap Detected - Start Position: {touchStartPosition}");
    //     touchStartTime = Time.time;
    // }

    // private void UpdateTouchPosition(InputAction.CallbackContext context)
    // {
    //     // Optional: Logic for continuous touch position updates
    // }

    // private void EndTouch(InputAction.CallbackContext context)
    // {
    //     Vector2 touchEndPosition = controls.Movement.TouchPosition.ReadValue<Vector2>();
    //     float touchDuration = Time.time - touchStartTime;
    //     Vector2 swipeDelta = touchEndPosition - touchStartPosition;

    //      Debug.Log($"End Position: {touchEndPosition} (Before Threshold Check)");

    //     if (touchDuration < tapTimeThreshold && swipeDelta.magnitude < swipeThreshold)

    //     if (IsTouchWithinButton(touchEndPosition))
    //     {
    //         if (touchDuration < tapTimeThreshold && swipeDelta.magnitude < swipeThreshold)
    //         {
    //              Debug.Log($"Tap Detected - Start Position: {touchStartPosition}, End Position: {touchEndPosition}");
    //             // It's a tap within the button bounds
    //             ID.events.OnEggDrop?.Invoke();
    //         }
    //         else if (swipeDelta.y < -swipeThreshold) // Negative Y indicates a swipe down
    //         {
    //             // It's a swipe down within the button bounds
    //             ID.events.OnDrop?.Invoke();
    //         }
    //     }
    // }

    // private bool IsTouchWithinButton(Vector2 touchPosition)
    // {
    //     if (touchButtonRectTransform == null) return false;

    //     Vector2 localPoint;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         touchButtonRectTransform,
    //         touchPosition,
    //         null,
    //         out localPoint
    //     );

    //     return touchButtonRectTransform.rect.Contains(localPoint);
    // }

   

    // Add similar methods for other actions if needed
    // ...

    
