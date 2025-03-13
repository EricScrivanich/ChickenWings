using UnityEngine;
using UnityEngine.EventSystems;

public class PressTimeBar : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        LevelRecordManager.PressTimerBar?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
