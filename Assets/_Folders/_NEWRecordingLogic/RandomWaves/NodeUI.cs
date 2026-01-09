using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RectTransform Rect { get; private set; }

    public GameObject parameterButton;

    [SerializeField]
    private RectTransform entrancePoint;
    [SerializeField]
    private RectTransform exitPoint;
    [SerializeField]
    private RectTransform loopBackPoint;  // Only used by Loop nodes

    public Image entranceArea;
    public Image exitArea;
    public Image loopBackArea;  // Only used by Loop nodes

    // Node identification
    public ushort NodeId { get; private set; }
    public NodeTreeCanvas.NodeType NodeType { get; private set; }

    // Parameter data for this node
    public NodeParameterData ParameterData { get; private set; }

    // Reference to defaults component on prefab
    private NodeDefaults nodeDefaults;

    private NodeTreeCanvas canvas;
    private Image image;
    private bool isHighlighted;
    private bool isExistingConnectionHighlighted;
    private bool isSelected;
    private bool isDragging;

    private Vector2 dragOffset;

    public void Initialize(NodeTreeCanvas owner)
    {
        canvas = owner;
        Rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Assign unique ID
        NodeId = owner.GetNextNodeId();
        nodeDefaults = GetComponent<NodeDefaults>();

        // Create parameter data from defaults if available
        if (nodeDefaults != null)
        {
            ParameterData = nodeDefaults.CreateParameterData(this, canvas);
        }
        canvas.SelectNode(this);
    }

    public void SetHighlight(bool value)
    {
        isHighlighted = value;
        if (value)
            isExistingConnectionHighlighted = false;
        UpdateNodeColor();
    }

    public void SetExistingConnectionHighlight(bool value)
    {
        isExistingConnectionHighlighted = value;
        if (value)
            isHighlighted = false;
        UpdateNodeColor();
    }

    public void SetSelected(bool value)
    {
        isSelected = value;
        UpdateParameterButtonVisibility();
        UpdateNodeColor();
    }

    private void UpdateNodeColor()
    {
        if (isExistingConnectionHighlighted)
        {
            image.color = canvas.existingConnectionHighlightColor;
        }
        else if (isHighlighted)
        {
            image.color = canvas.highlightColor;
        }
        else if (isDragging)
        {
            image.color = canvas.nodeDraggingColor;
        }
        else if (isSelected)
        {
            image.color = canvas.nodeSelectedColor;
        }
        else
        {
            image.color = canvas.nodeUnselectedColor;
        }
    }

    private void UpdateParameterButtonVisibility()
    {
        if (parameterButton != null)
        {
            parameterButton.SetActive(isSelected && !isDragging);
        }
    }

    // Called by child buttons (entrance, exit, or loop-back)
    public void OnConnectPressed(NodeTreeCanvas.ConnectionPointType pointType)
    {
        canvas.BeginConnection(this, pointType);
        SetConnectionHighlight(pointType);
    }

    // Legacy method for backwards compatibility with existing button setups
    public void OnConnectPressedEntrance()
    {
        OnConnectPressed(NodeTreeCanvas.ConnectionPointType.Entrance);
    }

    public void OnConnectPressedExit()
    {
        OnConnectPressed(NodeTreeCanvas.ConnectionPointType.Exit);
    }

    public void OnConnectPressedLoopBack()
    {
        OnConnectPressed(NodeTreeCanvas.ConnectionPointType.LoopBack);
    }

    public void SetConnectionHighlight(NodeTreeCanvas.ConnectionPointType pointType)
    {
        ClearConnectionHighlights();
        switch (pointType)
        {
            case NodeTreeCanvas.ConnectionPointType.Entrance:
                if (entranceArea != null)
                    entranceArea.color = canvas.highlightColor;
                break;
            case NodeTreeCanvas.ConnectionPointType.Exit:
                if (exitArea != null)
                    exitArea.color = canvas.highlightColor;
                break;
            case NodeTreeCanvas.ConnectionPointType.LoopBack:
                if (loopBackArea != null)
                    loopBackArea.color = canvas.highlightColor;
                break;
        }
    }

    // Legacy method - kept for compatibility
    public void SetEntranceAndExitHighlight(bool highlightEntrance)
    {
        SetConnectionHighlight(highlightEntrance
            ? NodeTreeCanvas.ConnectionPointType.Entrance
            : NodeTreeCanvas.ConnectionPointType.Exit);
    }

    public void SetExitHighlight(bool highlightExit)
    {
        if (exitArea != null)
            exitArea.color = highlightExit ? canvas.highlightColor : canvas.nodeUnselectedColor;
    }

    public void ClearConnectionHighlights()
    {
        if (entranceArea != null)
            entranceArea.color = canvas.nodeUnselectedColor;
        if (exitArea != null)
            exitArea.color = canvas.nodeUnselectedColor;
        if (loopBackArea != null)
            loopBackArea.color = canvas.nodeUnselectedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHighlighted || isExistingConnectionHighlighted)
        {
            canvas.TryCompleteConnection(this);
        }
        else
        {
            // If clicking on the connection source node, reset connection mode
            if (!canvas.TryResetIfConnectionSource(this))
            {
                canvas.SelectNode(this);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        UpdateNodeColor();
        UpdateParameterButtonVisibility();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );

        dragOffset = Rect.anchoredPosition - dragOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos
        );

        Rect.anchoredPosition = localPos + dragOffset;
        canvas.UpdateConnectionsFor(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvas.UpdateConnectionsFor(this);
        canvas.SelectNode(this);
        UpdateNodeColor();
        UpdateParameterButtonVisibility();
    }

    public void SetNodeType(NodeTreeCanvas.NodeType type)
    {
        NodeType = type;

        // Configure connection points based on node type
        ConfigureConnectionPointsForType();
    }

    private void ConfigureConnectionPointsForType()
    {
        switch (NodeType)
        {
            case NodeTreeCanvas.NodeType.Start:
                // Start node only has exit - hide entrance
                if (entranceArea != null)
                    entranceArea.gameObject.SetActive(false);
                if (entrancePoint != null)
                    entrancePoint.gameObject.SetActive(false);
                break;

            case NodeTreeCanvas.NodeType.End:
                // End node only has entrance - hide exit
                if (exitArea != null)
                    exitArea.gameObject.SetActive(false);
                if (exitPoint != null)
                    exitPoint.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Returns true if this node has an entrance point.
    /// </summary>
    public bool HasEntrancePoint => NodeType != NodeTreeCanvas.NodeType.Start && entrancePoint != null;

    /// <summary>
    /// Returns true if this node has an exit point.
    /// </summary>
    public bool HasExitPoint => NodeType != NodeTreeCanvas.NodeType.End && exitPoint != null;

    /// <summary>
    /// Called when this node is being deleted. Cleans up parameter data.
    /// </summary>
    public void OnDelete()
    {
        ParameterData?.OnNodeDeleted();
    }

    /// <summary>
    /// Opens the parameter window for this node.
    /// </summary>
    public void OpenParameters()
    {
        canvas.ShowParametersForNode(this);
    }

    public Vector2 GetEntrancePoint()
    {
        if (entrancePoint == null)
            return Rect.anchoredPosition;

        return entrancePoint.anchoredPosition + Rect.anchoredPosition;
    }

    public Vector2 GetExitPoint()
    {
        if (exitPoint == null)
            return Rect.anchoredPosition;

        return exitPoint.anchoredPosition + Rect.anchoredPosition;
    }

    public Vector2 GetLoopBackPoint()
    {
        if (loopBackPoint == null)
            return Rect.anchoredPosition;

        return loopBackPoint.anchoredPosition + Rect.anchoredPosition;
    }

    public bool HasLoopBackPoint => loopBackPoint != null;
}