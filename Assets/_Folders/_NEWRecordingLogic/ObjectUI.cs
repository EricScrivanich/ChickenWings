using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ObjectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private LevelCreatorColors colorSO;
    private Image img;
    private void Awake()
    {
        img = GetComponent<Image>();
        img.color = colorSO.MainUIColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        LevelRecordManager.AddNewObject?.Invoke(Prefab);
        img.color = colorSO.SelctedUIColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.color = colorSO.MainUIColor;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
