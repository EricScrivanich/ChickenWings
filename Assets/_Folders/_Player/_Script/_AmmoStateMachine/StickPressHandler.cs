using UnityEngine.EventSystems;
using UnityEngine;

public class StickPressHandler : MonoBehaviour, IPointerUpHandler
{
    public PlayerID player;
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.events.OnReleaseStick?.Invoke();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
