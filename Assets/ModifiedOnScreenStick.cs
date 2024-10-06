using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ModifiedOnScreenStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private const string kDynamicOriginClickable = "DynamicOriginClickable";


    

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    [Min(0)]
    private float m_MovementRange = 50;

    [SerializeField]
    [Tooltip("Defines the circular region where the onscreen control may have its origin placed.")]
    [Min(0)]
    private float m_DynamicOriginRange = 100;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    [SerializeField]
    [Tooltip("Choose how the onscreen stick will move relative to its origin and the press position.")]
    private Behaviour m_Behaviour;

    [SerializeField]
    [Tooltip("Set this to true to prevent cancellation of pointer events due to device switching.")]
    private bool m_UseIsolatedInputActions;

    [SerializeField]
    [Tooltip("The action that will be used to detect pointer down events on the stick control.")]
    private InputAction m_PointerDownAction;

    [SerializeField]
    [Tooltip("The action that will be used to detect pointer movement on the stick control.")]
    private InputAction m_PointerMoveAction;

    private Vector2 m_StartPos;
    private Vector2 m_PointerDownPos;

    private readonly float m_DragThreshold = 40f;

    private Vector2 m_LastPosition;
    private bool m_IsDragging = false;


    [NonSerialized]
    private List<RaycastResult> m_RaycastResults;
    [NonSerialized]
    private PointerEventData m_PointerEventData;

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    public void SetNewRect(RectTransform target)
    {
        // m_MovableRect = target;
    }

    public float dynamicOriginRange
    {
        get => m_DynamicOriginRange;
        set
        {
            if (m_DynamicOriginRange != value)
            {
                m_DynamicOriginRange = value;
                UpdateDynamicOriginClickableArea();
            }
        }
    }

    public bool useIsolatedInputActions
    {
        get => m_UseIsolatedInputActions;
        set => m_UseIsolatedInputActions = value;
    }

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public Behaviour behaviour
    {
        get => m_Behaviour;
        set => m_Behaviour = value;
    }

    private void Start()
    {
        // if (m_MovableRect == null)
        // {
        //     Debug.LogError("Movable Rect is not assigned in the ModifiedOnScreenStick component.");
        //     return;
        // }

        // m_StartPos = m_MovableRect.anchoredPosition;
        m_StartPos = gameObject.GetComponent<Image>().rectTransform.anchoredPosition;

        if (m_UseIsolatedInputActions)
        {
            m_RaycastResults = new List<RaycastResult>();
            m_PointerEventData = new PointerEventData(EventSystem.current);

            if (m_PointerDownAction == null || m_PointerDownAction.bindings.Count == 0)
            {
                if (m_PointerDownAction == null)
                    m_PointerDownAction = new InputAction();

                m_PointerDownAction.AddBinding("<Mouse>/leftButton");
                m_PointerDownAction.AddBinding("<Pen>/tip");
                m_PointerDownAction.AddBinding("<Touchscreen>/touch*/press");
                m_PointerDownAction.AddBinding("<XRController>/trigger");
            }

            if (m_PointerMoveAction == null || m_PointerMoveAction.bindings.Count == 0)
            {
                if (m_PointerMoveAction == null)
                    m_PointerMoveAction = new InputAction();

                m_PointerMoveAction.AddBinding("<Mouse>/position");
                m_PointerMoveAction.AddBinding("<Pen>/position");
                m_PointerMoveAction.AddBinding("<Touchscreen>/touch*/position");
            }

            m_PointerDownAction.started += OnPointerDown;
            m_PointerDownAction.canceled += OnPointerUp;
            m_PointerDownAction.Enable();
            m_PointerMoveAction.Enable();
        }

        if (m_Behaviour == Behaviour.ExactPositionWithDynamicOrigin)
        {
            m_PointerDownPos = m_StartPos;
            CreateDynamicOriginClickable();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_UseIsolatedInputActions)
            return;

        if (eventData == null)
            throw new ArgumentNullException(nameof(eventData));

        BeginInteraction(eventData.position, eventData.pressEventCamera);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_UseIsolatedInputActions)
            return;

        if (eventData == null)
            throw new ArgumentNullException(nameof(eventData));

        MoveStick(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_UseIsolatedInputActions)
            return;

        EndInteraction();
    }

    // ... (other existing fields and properties)

    private void BeginInteraction(Vector2 pointerPosition, Camera uiCamera)
    {
        var canvasRect = transform.parent?.GetComponentInParent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("ModifiedOnScreenStick needs to be attached as a child to a UI Canvas to function properly.");
            return;
        }

        switch (m_Behaviour)
        {
            case Behaviour.RelativePositionWithStaticOrigin:
            case Behaviour.ExactPositionWithStaticOrigin:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out m_PointerDownPos);
                break;
            case Behaviour.ExactPositionWithDynamicOrigin:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out var pointerDown);
                // m_PointerDownPos = m_MovableRect.anchoredPosition = pointerDown;
                break;
        }

        m_LastPosition = m_PointerDownPos;
        m_IsDragging = false;

        if (m_Behaviour == Behaviour.ExactPositionWithStaticOrigin)
        {
            MoveStick(pointerPosition, uiCamera);
        }
    }

    private void MoveStick(Vector2 pointerPosition, Camera uiCamera)
    {
        var canvasRect = transform.parent?.GetComponentInParent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("ModifiedOnScreenStick needs to be attached as a child to a UI Canvas to function properly.");
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerPosition, uiCamera, out var position);
        var delta = position - m_PointerDownPos;

        if (!m_IsDragging)
        {
            if (delta.magnitude >= m_DragThreshold)
            {
                m_IsDragging = true;
                m_LastPosition = position;
            }
            else
            {
                return; // Exit early if we haven't exceeded the drag threshold
            }
        }

        switch (m_Behaviour)
        {
            case Behaviour.RelativePositionWithStaticOrigin:
                delta = Vector2.ClampMagnitude(delta, movementRange);
                // m_MovableRect.anchoredPosition = m_StartPos + delta;
                break;

            case Behaviour.ExactPositionWithStaticOrigin:
                delta = position - m_StartPos;
                delta = Vector2.ClampMagnitude(delta, movementRange);
                // m_MovableRect.anchoredPosition = m_StartPos + delta;
                break;

            case Behaviour.ExactPositionWithDynamicOrigin:
                delta = Vector2.ClampMagnitude(delta, movementRange);
                // m_MovableRect.anchoredPosition = m_PointerDownPos + delta;
                break;
        }

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);

        m_LastPosition = position;
    }

    private void EndInteraction()
    {
        // m_MovableRect.anchoredPosition = m_PointerDownPos = m_StartPos;
        SendValueToControl(Vector2.zero);
        m_IsDragging = false;
    }

    private void OnPointerDown(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current == null) return;

        var screenPosition = Vector2.zero;
        if (ctx.control?.device is Pointer pointer)
            screenPosition = pointer.position.ReadValue();

        m_PointerEventData.position = screenPosition;
        EventSystem.current.RaycastAll(m_PointerEventData, m_RaycastResults);
        if (m_RaycastResults.Count == 0)
            return;

        var stickSelected = false;
        foreach (var result in m_RaycastResults)
        {
            if (result.gameObject != gameObject) continue;

            stickSelected = true;
            break;
        }

        if (!stickSelected)
            return;

        BeginInteraction(screenPosition, GetCameraFromCanvas());
        m_PointerMoveAction.performed += OnPointerMove;
    }

    private void OnPointerMove(InputAction.CallbackContext ctx)
    {
        if (!(ctx.control?.device is Pointer)) return;

        var screenPosition = ((Pointer)ctx.control.device).position.ReadValue();
        MoveStick(screenPosition, GetCameraFromCanvas());
    }

    private void OnPointerUp(InputAction.CallbackContext ctx)
    {
        EndInteraction();
        m_PointerMoveAction.performed -= OnPointerMove;
    }

    private Camera GetCameraFromCanvas()
    {
        var canvas = GetComponentInParent<Canvas>();
        var renderMode = canvas?.renderMode;
        if (renderMode == RenderMode.ScreenSpaceOverlay
            || (renderMode == RenderMode.ScreenSpaceCamera && canvas?.worldCamera == null))
            return null;

        return canvas?.worldCamera ?? Camera.main;
    }

    private void CreateDynamicOriginClickable()
    {
        var dynamicOrigin = new GameObject(kDynamicOriginClickable, typeof(Image));
        dynamicOrigin.transform.SetParent(transform);
        var image = dynamicOrigin.GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0);
        var rectTransform = (RectTransform)dynamicOrigin.transform;
        rectTransform.sizeDelta = new Vector2(m_DynamicOriginRange * 2, m_DynamicOriginRange * 2);
        rectTransform.localScale = new Vector3(1, 1, 0);
        rectTransform.anchoredPosition3D = Vector3.zero;

        image.sprite = CreateCircleSprite(16, new Color32(255, 255, 255, 255));
        image.alphaHitTestMinimumThreshold = 0.5f;
    }

    private void UpdateDynamicOriginClickableArea()
    {
        var dynamicOriginTransform = transform.Find(kDynamicOriginClickable);
        if (dynamicOriginTransform)
        {
            var rectTransform = (RectTransform)dynamicOriginTransform;
            rectTransform.sizeDelta = new Vector2(m_DynamicOriginRange * 2, m_DynamicOriginRange * 2);
        }
    }

    private static Sprite CreateCircleSprite(int resolution, Color32 color)
    {
        var texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        var center = new Vector2(resolution / 2f, resolution / 2f);
        var radius = resolution / 2f;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }

    public enum Behaviour
    {
        RelativePositionWithStaticOrigin,
        ExactPositionWithStaticOrigin,
        ExactPositionWithDynamicOrigin
    }
}