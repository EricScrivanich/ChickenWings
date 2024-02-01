using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required for event system interfaces
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class JumpButtonInputs : MonoBehaviour, IPointerDownHandler



{
    private bool isTwoFingerTouchActive = false;
    public PlayerID ID; // Reference to your player ID or event system

    // Detect when the pointer is down on this UI element
    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if there are exactly two touches on the screen
        if (Touch.activeTouches.Count == 2)
        {
            // Two-finger touch start detected, invoke parachute open event
            ID.IsTwoTouchPoints = true;
            ID.events.OnParachute?.Invoke(true);
        }
    }

    // Detect when the pointer is lifted off this UI element
    // public void OnPointerUp(PointerEventData eventData) 
    // {
    //     // Check if there are less than two touches on the screen, indicating at least one finger was lifted
    //     if (Input.touchCount < 2)
    //     {
    //         // Two-finger touch end detected, invoke parachute close event
    //         ID.events.OnParachute?.Invoke(false);
    //     }
    // }

    void Update()
    {
        // If two-finger touch was active but now there are fewer touches, consider it a release
        if (ID.IsTwoTouchPoints && Touch.activeTouches.Count < 2)
        {
            ID.IsTwoTouchPoints = false;
            // Two-finger touch end detected, invoke parachute close event
            ID.events.OnParachute?.Invoke(false);
            
        }
    }

    // Ensure your component is enabled to register the touch events
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ID.IsTwoTouchPoints = false;
        


        // Ensure the UI component (like Button) is enabled and interactable
        // You might need to ensure this component or the gameObject it's attached to is active and enabled
    }
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();


    }
}

