using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Required for event system interfaces
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using DG.Tweening;

public class JumpButtonInputs : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera cam;

    [SerializeField] private float zoomInDuration;
    [SerializeField] private float zoomOutDuration;
    [SerializeField] private Ease easeIn;
    [SerializeField] private Ease easeOut;
    [SerializeField] private float zoomInMultiplier;

    [SerializeField] private GameObject cursorObject; // GameObject used as the cursor
    [SerializeField] private LayerMask sliceableLayer; // Layer for objects that can be sliced
    [SerializeField] private bool slice = false; // Bool to enable/disable slicing functionality

    private Sequence camSeq;
    private float startZoom;

    private bool isTwoFingerTouchActive = false;
    public PlayerID ID; // Reference to your player ID or event system

    // Detect when the pointer is down on this UI element
    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if there are exactly two touches on the screen
        if (Touch.activeTouches.Count == 2 && !ID.pressingButton)
        {
            // Two-finger touch start detected, invoke parachute open event
            ID.IsTwoTouchPoints = true;
            ID.events.OnPerformParry?.Invoke(true);
        }
    }

    private void Start()
    {
        startZoom = cam.orthographicSize;
    }

    public void SwitchCamera(bool zoomIn)
    {
        if (camSeq != null && camSeq.IsPlaying())
            camSeq.Kill();
        camSeq = DOTween.Sequence();

        if (zoomIn)
        {
            slice = true;
            camSeq.Append(cam.transform.DOMoveX(playerTransform.position.x, zoomInDuration));
            camSeq.Join(cam.transform.DOMoveY(playerTransform.position.y, zoomInDuration));
            camSeq.Join(cam.DOOrthoSize(startZoom * zoomInMultiplier, zoomInDuration));
            camSeq.Play().SetEase(easeIn);
        }
        else
        {
            slice = false;
            camSeq.Append(cam.transform.DOMoveX(0, zoomOutDuration));
            camSeq.Join(cam.transform.DOMoveY(0, zoomOutDuration));
            camSeq.Join(cam.DOOrthoSize(startZoom, zoomOutDuration));
            camSeq.Play().SetEase(easeOut);
        }
    }

    public void HandleSwipeWeaponLogic()
    {
        // Logic for swipe weapon can be handled here
    }

    // Detect when two-finger touch is released
    void Update()
    {
        // Update cursor position to follow the pointer position
        if (slice && cursorObject != null)
        {
            Debug.Log("YUG");
            // Set cursorObject position to follow the pointer position
            Vector2 pointerPosition;

            // Check if Touchscreen is available and primaryTouch is active
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInProgress)
            {
                pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                // Fallback to mouse input for non-touch devices
                pointerPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                Debug.LogWarning("No input device detected (touchscreen or mouse).");
                return;
            }

            // Convert pointer position to world position
            Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, cam.nearClipPlane));
            cursorObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

            // Check for slicing logic
            // Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, cam.nearClipPlane));
            // cursorObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

            // Check if we are slicing objects
            if (Touch.activeTouches.Count > 0)
            {
                CheckSlice(worldPosition);
            }
        }

        // If two-finger touch was active but now there are fewer touches, consider it a release
        if (ID.IsTwoTouchPoints && Touch.activeTouches.Count < 2)
        {
            ID.IsTwoTouchPoints = false;
            ID.events.OnPerformParry?.Invoke(false);
        }
    }

    private void CheckSlice(Vector3 position)
    {
        // Perform a raycast to detect objects under the cursor
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, Mathf.Infinity, sliceableLayer);

        if (hit.collider != null)
        {
            // Check if the hit object implements the IDamagable interface
            IDamageable damagable = hit.collider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                // Apply damage to the object
                damagable.Damage(1); // Assuming damage of 1 unit
                Debug.Log($"Sliced object: {hit.collider.gameObject.name}");
            }
        }
    }

    // Ensure your component is enabled to register the touch events
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ID.IsTwoTouchPoints = false;
        ID.events.OnPerformParry += SwitchCamera;
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        ID.events.OnPerformParry -= SwitchCamera;
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}