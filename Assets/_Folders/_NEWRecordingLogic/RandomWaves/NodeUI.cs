using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RectTransform Rect { get; private set; }

    // public RectTransform LineConnectPoint;

    private NodeTreeCanvas canvas;
    private Image image;
    private bool isHighlighted;

    private Vector2 dragOffset;

    public void Initialize(NodeTreeCanvas owner)
    {
        canvas = owner;
        Rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetHighlight(bool value)
    {
        isHighlighted = value;
        image.color = value ? canvas.highlightColor : canvas.normalColor;
    }

    // Called by child button
    public void OnConnectPressed()
    {
        canvas.BeginConnection(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHighlighted)
        {
            canvas.TryCompleteConnection(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        canvas.UpdateConnectionsFor(this);
    }

    public NodeTreeCanvas.NodeType NodeType { get; private set; }

    public void SetNodeType(NodeTreeCanvas.NodeType type)
    {
        NodeType = type;
    }
}