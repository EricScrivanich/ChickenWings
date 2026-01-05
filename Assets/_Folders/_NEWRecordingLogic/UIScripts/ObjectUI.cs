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

    private RandomWaveButtonParent randomWaveButtonParent;





    private int randomWaveIndex = -1;
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

    public void SetAsRandomWaveButton(int waveIndex, bool isGroup = false, RandomWaveButtonParent parent = null)
    {
        randomWaveIndex = waveIndex;
        var text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (isGroup)
        {
            // 1 = A, 2 = B, 3 = C ...
            char letter = (char)('A' + waveIndex - 1);
            text.text = letter.ToString();
            randomWaveButtonParent = parent;
        }
        else
        {
            text.text = waveIndex.ToString();
        }

    }

    public void PressWaveButton()
    {
        WaveCreator.instance.Open(randomWaveIndex);

    }

    public void PressWaveGroupButton()
    {
        randomWaveButtonParent.OpenWaveGroup(randomWaveIndex);

    }
    public void OnPointerDown(PointerEventData eventData)
    {

        if (randomWaveIndex > 0)
        {
            if (randomWaveButtonParent != null)
            {
                PressWaveGroupButton();
                return;
            }
            PressWaveButton();

            return;
        }


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
