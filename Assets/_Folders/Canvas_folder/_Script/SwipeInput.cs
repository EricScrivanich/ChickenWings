
using UnityEngine;
using UnityEngine.EventSystems;
public class SwipeInput : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private GameObject player;
    private PowerUps powerUps;
    private Vector2 startDrag;
    private const float DRAG_THRESHOLD = 6f;

    void Start()
    {
        powerUps = player.GetComponent<PowerUps>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDrag = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - startDrag;

        // Use a threshold to avoid tiny movements causing a dash or drop
        if (direction.magnitude < DRAG_THRESHOLD) return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                powerUps.StartDash();
            }
        }
        else
        {
            if (direction.y < 0)
            {
                powerUps.StartDrop();
            }
        }

        // Reset start position for next significant movement
        startDrag = eventData.position;
    }
}
