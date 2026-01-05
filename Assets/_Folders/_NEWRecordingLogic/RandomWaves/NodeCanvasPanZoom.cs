using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NodeCanvasPanZoom_NewInput : MonoBehaviour
{
    [Header("References")]
    public RectTransform windowRect;
    public Canvas canvas;

    [Header("Pan")]
    public float panSpeed = 1f;

    [Header("Zoom")]
    public float zoomSpeedMobile = 0.01f;
    public float zoomSpeedComputer = 0.01f;
    public float minZoom = 0.4f;
    public float maxZoom = 2.5f;
    public float trackpadZoomThreshold = 0.05f;

    private InputController controls;

    // Pan
    private bool isPanning;
    private Vector2 lastPanPos;

    // Zoom
    private float currentZoom = 1f;

    // Mobile pinch
    private bool finger1Down;
    private bool finger2Down;
    private float lastPinchDistance;

    void Awake()
    {
        controls = new InputController();

        // -------------------------
        // PAN (mouse + single touch)
        // -------------------------
        controls.LevelCreator.CursorClick.performed += ctx =>
        {
            if (CanInteract())
            {
                isPanning = true;
                lastPanPos = Mouse.current.position.ReadValue();
            }
        };

        controls.LevelCreator.CursorClick.canceled += ctx =>
        {
            isPanning = false;
        };

        controls.LevelCreator.Finger1Press.performed += ctx =>
        {
            if (CanInteract())
            {
                finger1Down = true;
                isPanning = true;
                lastPanPos = Pointer.current.position.ReadValue();
            }
        };

        controls.LevelCreator.Finger1Press.canceled += ctx =>
        {
            finger1Down = false;
            isPanning = false;
        };

        controls.LevelCreator.Scroll.performed += ctx =>
       {
           float scroll = ctx.ReadValue<Vector2>().y;
           ApplyZoom(scroll * zoomSpeedComputer);
       };


        controls.LevelCreator.CursorPos.performed += ctx =>
        {
            if (!isPanning || finger2Down) return;
            Pan(ctx.ReadValue<Vector2>());
        };

        controls.LevelCreator.Finger1Pos.performed += ctx =>
        {
            if (!isPanning || finger2Down) return;
            Pan(ctx.ReadValue<Vector2>());
        };

        // -------------------------
        // MOBILE PINCH ZOOM
        // -------------------------
        controls.LevelCreator.Finger2Press.performed += ctx =>
        {
            finger2Down = true;

            // Initialize pinch distance
            lastPinchDistance = GetCurrentPinchDistance();
        };

        controls.LevelCreator.Finger2Press.canceled += ctx =>
        {
            finger2Down = false;
        };

        controls.LevelCreator.Finger2Pos.performed += ctx =>
        {
            if (!finger1Down || !finger2Down) return;

            float currentDistance = GetCurrentPinchDistance();
            float delta = currentDistance - lastPinchDistance;
            lastPinchDistance = currentDistance;

            ApplyZoom(delta * zoomSpeedMobile);
        };

        // -------------------------
        // TRACKPAD PINCH ZOOM
        // -------------------------
        // controls.LevelCreator.PinchDelta.performed += ctx =>
        // {
        //     Vector2 delta = ctx.ReadValue<Vector2>();

        //     if (Mathf.Abs(delta.y) < trackpadZoomThreshold)
        //         return;

        //     ApplyZoom(delta.y * zoomSpeed);
        // };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    // -------------------------
    // Helpers
    // -------------------------
    void Pan(Vector2 currentPos)
    {
        Vector2 delta = currentPos - lastPanPos;
        lastPanPos = currentPos;

        windowRect.anchoredPosition += delta / canvas.scaleFactor * panSpeed;
    }

    float GetCurrentPinchDistance()
    {
        Vector2 p1 = controls.LevelCreator.Finger1Pos.ReadValue<Vector2>();
        Vector2 p2 = controls.LevelCreator.Finger2Pos.ReadValue<Vector2>();
        return Vector2.Distance(p1, p2);
    }

    void ApplyZoom(float delta)
    {
        if (Mathf.Approximately(delta, 0f)) return;

        currentZoom = Mathf.Clamp(currentZoom + delta, minZoom, maxZoom);
        windowRect.localScale = Vector3.one * currentZoom;
    }

    bool CanInteract()
    {
        return !EventSystem.current.IsPointerOverGameObject();
    }
}