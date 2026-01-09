using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NodeCanvasPanZoom : MonoBehaviour
{
    [Header("References")]
    public RectTransform windowRect;
    public Canvas canvas;

    [Header("Pan")]
    public float panSpeed = 1f;

    [Header("Zoom")]
    public float startZoom;
    public float zoomSpeedMobile = 0.01f;
    public float zoomSpeedComputer = 0.01f;
    public float minZoom = 0.4f;
    public float maxZoom = 2.5f;

    [Header("Bounds (max movement at minZoom)")]
    public Vector2 maxWindowOffsets = new Vector2(800f, 800f);
    public Vector2 maxWindowOffsetsMaxZoom;
    public Vector2 maxWindowOffsetsMinZoom;


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
    private bool canPan = false;
    private bool initialFingerZoom = false;

    void Awake()
    {
        controls = new InputController();
        currentZoom = startZoom;
        windowRect.localScale = Vector3.one * currentZoom;

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
            finger1Down = true;
            canPan = false;
            StartCoroutine(CheckFingerOverUI());
            lastPanPos = Pointer.current.position.ReadValue();
        };

        controls.LevelCreator.Finger1Press.canceled += ctx =>
        {
            finger1Down = false;
            isPanning = false;
        };

        // -------------------------
        // MOUSE WHEEL ZOOM
        // -------------------------
        controls.LevelCreator.Scroll.performed += ctx =>
        {
            float scroll = ctx.ReadValue<Vector2>().y;
            if (mouseOverGameWindow)
                TryApplyZoom(scroll * zoomSpeedComputer);
        };

        // -------------------------
        // POINTER MOVE
        // -------------------------
        controls.LevelCreator.CursorPos.performed += ctx =>
        {
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
            mouseOverGameWindow = screenRect.Contains(ctx.ReadValue<Vector2>());
            if (!isPanning || finger2Down) return;
            TryPan(ctx.ReadValue<Vector2>());
        };

        controls.LevelCreator.Finger1Pos.performed += ctx =>
        {
            if (!canPan || !finger1Down || finger2Down)
                return;

            if (!isPanning)
                isPanning = true;

            TryPan(ctx.ReadValue<Vector2>());
        };

        // -------------------------
        // MOBILE PINCH ZOOM
        // -------------------------
        controls.LevelCreator.Finger2Press.performed += ctx =>
        {
            finger2Down = true;
            initialFingerZoom = true;

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

            if (initialFingerZoom)
            {
                initialFingerZoom = false;
                return;
            }

            TryApplyZoom(delta * zoomSpeedMobile);
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    // -------------------------
    //   PAN WITH BOUNDS
    // -------------------------

    void TryPan(Vector2 currentPos)
    {
        Vector2 delta = currentPos - lastPanPos;
        lastPanPos = currentPos;

        Vector2 worldDelta = delta / canvas.scaleFactor * panSpeed;

        windowRect.anchoredPosition += worldDelta;

        ClampWindowToBounds();
    }

    // -------------------------
    //   ZOOM WITH BOUNDS
    // -------------------------

    void TryApplyZoom(float delta)
    {
        if (Mathf.Approximately(delta, 0f)) return;

        float proposedZoom = Mathf.Clamp(currentZoom + delta, minZoom, maxZoom);

        currentZoom = proposedZoom;
        windowRect.localScale = Vector3.one * currentZoom;

        ClampWindowToBounds();
    }

    // -------------------------
    // BOUNDS CLAMPING
    // -------------------------

    void ClampWindowToBounds()
    {
        // Define max movement at minZoom, then scale with zoom
        // At zoom == minZoom  -> allowed = maxWindowOffsets
        // At zoom >  minZoom  -> allowed grows proportionally
        // float zoomFactor = currentZoom / minZoom;
        // if (zoomFactor < 1f) zoomFactor = 1f; // just in case

        // float maxX = maxWindowOffsets.x * zoomFactor;
        // float maxY = maxWindowOffsets.y * zoomFactor;

        // Vector2 pos = windowRect.anchoredPosition;
        // pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        // pos.y = Mathf.Clamp(pos.y, -maxY, maxY);

        // windowRect.anchoredPosition = pos;

        float l = Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
        float maxX = Mathf.Lerp(maxWindowOffsetsMinZoom.x, maxWindowOffsetsMaxZoom.x, l);
        float maxY = Mathf.Lerp(maxWindowOffsetsMinZoom.y, maxWindowOffsetsMaxZoom.y, l);
        Vector2 pos = windowRect.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        pos.y = Mathf.Clamp(pos.y, -maxY, maxY);

        windowRect.anchoredPosition = pos;
    }

    // -------------------------
    // HELPERS
    // -------------------------

    private IEnumerator CheckFingerOverUI()
    {
        // small delay so EventSystem can update selection
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        canPan = !IsTouchOverUI();
    }

    bool IsTouchOverUI()
    {
        if (Touchscreen.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject(
            Touchscreen.current.primaryTouch.touchId.ReadValue()
        );
    }

    float GetCurrentPinchDistance()
    {
        Vector2 p1 = controls.LevelCreator.Finger1Pos.ReadValue<Vector2>();
        Vector2 p2 = controls.LevelCreator.Finger2Pos.ReadValue<Vector2>();
        return Vector2.Distance(p1, p2);
    }

    bool CanInteract()
    {
        return !EventSystem.current.IsPointerOverGameObject();
    }

    private bool mouseOverGameWindow = false;

    bool IsCursorOverWindow()
    {
        Vector2 screenPos;

        // Mouse present?
        if (Mouse.current != null)
            screenPos = Mouse.current.position.ReadValue();
        else
        {
            screenPos = Pointer.current.position.ReadValue(); // fallback
            Debug.LogWarning("Mouse not found, using generic pointer for screen position.");

        }

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(windowRect, screenPos, cam);
    }
}