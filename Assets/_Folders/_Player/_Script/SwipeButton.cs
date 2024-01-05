using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerID playerID; // Reference to PlayerID

    private Vector2 touchStartPosition;
    private float touchStartTime;
    private const float swipeThreshold = 0.5f; // Adjust as needed
    private const float tapTimeThreshold = 0.2f; // Adjust as needed

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPosition = eventData.position;
        // touchStartTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 touchEndPosition = eventData.position;
        // float touchDuration = Time.time - touchStartTime;
        Vector2 swipeDelta = touchEndPosition - touchStartPosition;

        // if (touchDuration < tapTimeThreshold && swipeDelta.magnitude < swipeThreshold)
        // {
        //     // It's a tap on the button
       
        //     playerID.events.OnEggDrop?.Invoke();
        // } else
        if (swipeDelta.y < -swipeThreshold) // Negative Y indicates a swipe down
        {
            // It's a swipe down on the button
            playerID.events.OnDrop?.Invoke();
         
        }
    }
}

