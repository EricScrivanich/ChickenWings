using UnityEngine;

using UnityEngine.EventSystems;

public class ObjectUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject Prefab;

    public void OnPointerDown(PointerEventData eventData)
    {
        LevelRecordManager.AddNewObject?.Invoke(Prefab);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
}
