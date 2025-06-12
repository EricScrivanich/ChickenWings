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


        if (type == "Cage")
        {
            LevelRecordManager.instance.SetCageReady(true);
            return;
        }
        img.color = colorSO.SelctedUIColor;
        LevelRecordManager.instance.SetObjectToBeAdded(Prefab, typeOvveride);

    }

    private void CheckForCage(int i, bool b)
    {
        if (i == 0)
        {
            if (b)
            {
                img.color = colorSO.SelctedUIColor;
            }
            else
            {
                img.color = colorSO.MainUIColor;
            }
        }

    }

    private void OnEnable()
    {
        if (type == "Cage")
        {
            img.color = colorSO.MainUIColor;
            LevelRecordManager.OnSendSpecialDataToActiveObjects += CheckForCage;
        }
    }
    private void OnDisable()
    {
        if (type == "Cage")
        {

            LevelRecordManager.OnSendSpecialDataToActiveObjects -= CheckForCage;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (type == "Cage")
        {
            return;
        }
        img.color = colorSO.MainUIColor;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
