using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ObjectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private LevelCreatorColors colorSO;
    private Image img;

    [SerializeField] private Image[] imageChangeForTypeOvveride;


    [SerializeField] private string type;

    [SerializeField] private int typeOvveride = -1;
    private void Awake()
    {
        img = GetComponent<Image>();
        img.color = colorSO.MainUIColor;

        if (type == "Ring")
        {
            foreach (var i in imageChangeForTypeOvveride)
            {
                i.color = colorSO.RingColors[typeOvveride];
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        LevelRecordManager.instance.SetObjectToBeAdded(Prefab, typeOvveride);
        img.color = colorSO.SelctedUIColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.color = colorSO.MainUIColor;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
