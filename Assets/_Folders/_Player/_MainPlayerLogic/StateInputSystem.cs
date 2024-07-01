using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class StateInputSystem : MonoBehaviour
{
    private InputController controls;
    private Sequence dropButtonSeq;
    private Coroutine dropButtonCor;
    private Sequence dashButtonSeq;
    [SerializeField] private Color normalButtonColor;
    [SerializeField] private Color highlightButtonColor;
    [SerializeField] private Color disabledButtonColor;
    private Image DropButton;
    private Image DashButton;
    private bool canDrop;
    private bool canDash;

    private bool ButtonsEnabled;

    private Vector2 touchStartPosition;
    private float touchStartTime;
    private const float swipeThreshold = 0.3f; // Adjust as needed
    private const float tapTimeThreshold = 0.2f; // Adjust as needed
    public PlayerID ID;
    [SerializeField] private RectTransform touchButtonRectTransform;

    private void Awake()
    {
        ID.UsingClocker = false;



        controls = new InputController();

        // Bind existing actions to methods


        controls.Movement.Jump.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJump?.Invoke();
        };

        controls.Movement.JumpRight.started += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipRight?.Invoke(true);
        };
        controls.Movement.JumpRight.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipRight?.Invoke(false);
        };

        controls.Movement.JumpLeft.started += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipLeft?.Invoke(true);
        };
        controls.Movement.JumpLeft.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipLeft?.Invoke(false);
        };

        controls.Movement.Dash.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnDash?.Invoke(true);
        };
        controls.Movement.Dash.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnDash?.Invoke(false);
        };

        controls.Movement.Drop.performed += ctx =>
        {
            if (ButtonsEnabled && canDrop)
            {
                ID.events.OnDrop?.Invoke();
                DropCooldown();

            }
        };
        controls.Movement.DropEgg.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnEggDrop?.Invoke();
        };
        controls.Movement.Fireball.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnAttack?.Invoke(true);
        };
        controls.Movement.Fireball.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnAttack?.Invoke(false);
        };

        controls.Movement.JumpHold.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJumpHeld?.Invoke(true);
        };

        controls.Movement.JumpHold.canceled += ctx =>
        {
            ID.events.OnJumpHeld?.Invoke(false);

        };

        controls.Movement.Fireball.performed += ctx => ID.events.OnAttack?.Invoke(!ID.UsingClocker);
        controls.Movement.Parachute.performed += ctx => ID.events.OnParachute?.Invoke(true);
        controls.Movement.Parachute.canceled += ctx => ID.events.OnParachute?.Invoke(false);







        // if (TimeManager.DebogLogEnabled)
        // {
        //     controls.Movement.Dash.performed += ctx => ID.globalEvents.HighlightDash?.Invoke(true);
        //     controls.Movement.Drop.performed += ctx => ID.globalEvents.HighlightDrop?.Invoke(true);
        //     controls.Movement.DropEgg.performed += ctx => ID.globalEvents.HighlightEgg?.Invoke(true);

        //     controls.Movement.Dash.canceled += ctx => ID.globalEvents.HighlightDash?.Invoke(false);
        //     controls.Movement.Drop.canceled += ctx => ID.globalEvents.HighlightDrop?.Invoke(false);
        //     controls.Movement.DropEgg.canceled += ctx => ID.globalEvents.HighlightEgg?.Invoke(false);


        // }
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
        // controls.Movement.DashSlash.performed += ctx => ID.events.OnDashSlash?.Invoke();

        //     controls.Movement.HoldJumpRight.performed += ctx =>
        //    {
        //        isHoldingFlip = true;
        //        ID.events.OnHoldFlip?.Invoke(true);
        //    };

        //     controls.Movement.HoldJumpLeft.performed += ctx =>

        //     {
        //         isHoldingFlip = true;
        //         ID.events.OnHoldFlip?.Invoke(true);
        //     };

        //     controls.Movement.HoldJumpRight.canceled += ctx =>
        //   {

        //       ID.events.OnHoldFlip?.Invoke(false);




        //   };

        //     controls.Movement.HoldJumpLeft.canceled += ctx =>

        //     {

        //         ID.events.OnHoldFlip?.Invoke(false);


        //     };

        // controls.Movement.HoldJumpRight.performed += ctx =>
        // {
        //     ID.events.OnHoldFlip?.Invoke(!ID.UsingClocker);
        // };

        // controls.Movement.HoldJumpLeft.performed += ctx =>

        //     {
        //         ID.events.OnClocker?.Invoke(!ID.UsingClocker);
        //     };

    }

    void Start()
    {
        if (GameObject.Find("DropButton") != null)
            DropButton = GameObject.Find("DropButton").GetComponent<Image>();
        canDrop = true;
        canDash = true;
    }


    private void DropCooldown()
    {
        if (dropButtonSeq != null && dropButtonSeq.IsPlaying())
            dropButtonSeq.Kill();

        canDrop = false;
        dropButtonSeq = DOTween.Sequence();
        dropButtonSeq.Append(DropButton.DOColor(highlightButtonColor, .15f));
        dropButtonSeq.AppendInterval(.15f);

        dropButtonSeq.Append(DropButton.DOColor(disabledButtonColor, .15f));
        dropButtonSeq.AppendInterval(1.8f);
        dropButtonSeq.Append(DropButton.DOColor(normalButtonColor, .1f));
        dropButtonSeq.OnComplete(() => canDrop = true);



        dropButtonSeq.Play();


    }
    public void ActivateButtons(bool IsActive)
    {
        ButtonsEnabled = IsActive;


    }

    private void OnEnable()
    {
        controls.Movement.Enable();
        ID.events.EnableButtons += ActivateButtons;
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
        ID.events.EnableButtons -= ActivateButtons;

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


