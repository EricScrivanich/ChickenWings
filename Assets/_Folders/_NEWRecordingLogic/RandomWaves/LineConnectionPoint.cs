using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A draggable connection point that sits between two nodes.
/// Used to visualize and manage connections, can be moved and deleted.
/// </summary>
public class LineConnectionPoint : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RectTransform Rect { get; private set; }

    [SerializeField] private GameObject highlight;
    private byte selectedScale = 130;
    private byte unSelectedScale = 100;

    private Image img;

    private NodeTreeCanvas canvas;
    private Vector2 dragOffset;
    private bool isDragging;
    private bool isSelected;

    // Connection data
    public NodeUI FromNode { get; private set; }
    public NodeUI ToNode { get; private set; }
    public NodeTreeCanvas.ConnectionPointType FromPointType { get; private set; }
    public NodeTreeCanvas.ConnectionPointType ToPointType { get; private set; }

    // Visual index for branch connections (determines color)
    public int ConnectionIndex { get; private set; }

    public void Initialize(NodeTreeCanvas owner, NodeUI fromNode, NodeUI toNode,
        NodeTreeCanvas.ConnectionPointType fromPointType, NodeTreeCanvas.ConnectionPointType toPointType,
        int connectionIndex = 0)
    {
        canvas = owner;
        Rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();

        FromNode = fromNode;
        ToNode = toNode;
        FromPointType = fromPointType;
        ToPointType = toPointType;
        ConnectionIndex = connectionIndex;

        // Position between the two nodes
        Vector2 fromPos = GetPointPosition(fromNode, fromPointType);
        Vector2 toPos = GetPointPosition(toNode, toPointType);
        Rect.anchoredPosition = (fromPos + toPos) / 2f;

        UpdateColor();
    }

    private Vector2 GetPointPosition(NodeUI node, NodeTreeCanvas.ConnectionPointType pointType)
    {
        return pointType switch
        {
            NodeTreeCanvas.ConnectionPointType.Entrance => node.GetEntrancePoint(),
            NodeTreeCanvas.ConnectionPointType.Exit => node.GetExitPoint(),
            NodeTreeCanvas.ConnectionPointType.LoopBack => node.GetLoopBackPoint(),
            _ => node.GetExitPoint()
        };
    }

    public void UpdateColor()
    {
        if (img != null && canvas != null)
        {
            img.color = canvas.GetConnectionColor(ConnectionIndex);
        }
    }

    public void SetConnectionIndex(int index)
    {
        ConnectionIndex = index;
        UpdateColor();
    }

    public Vector2 GetPosition()
    {
        return Rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

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
        canvas.UpdateLinesForConnectionPoint(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvas.UpdateLinesForConnectionPoint(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Left-click to select
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            canvas.SelectConnectionPoint(this);
        }
    }

    /// <summary>
    /// Called by NodeTreeCanvas when this connection point is selected or deselected.
    /// Implement your own visual feedback logic here.
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selected)
        {
            highlight.SetActive(true);
            img.rectTransform.localScale = Vector3.one * (selectedScale / 100f);
        }
        else
        {
            highlight.SetActive(false);
            img.rectTransform.localScale = Vector3.one * (unSelectedScale / 100f);
        }
        // User will implement visual feedback logic here
    }

    public bool IsSelected => isSelected;
}
