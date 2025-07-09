using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HideItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public enum Type
    {
        Lines,
        Speeds,
        Parameters,
        Time,
        Folders,
        MultiTouch
    }

    [SerializeField] private Type type;
    [SerializeField] private Image crossOut;
    [SerializeField] private LevelCreatorColors colorSO;

    private bool isShown;

    private Image fillImage;
    private bool isCrossedOut;
    private void Awake()
    {
        // crossOut.enabled = false;
        fillImage = GetComponent<Image>();
        fillImage.color = colorSO.SelctedUIColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (isCrossedOut) return;
        // crossOut.enabled = true;
        // crossOut.DOFade(1, 0.5f);
        // isCrossedOut = true;
        if (type == Type.Speeds && !LevelRecordManager.ShowLines) return;
        else if (type == Type.MultiTouch)
        {
            LevelRecordManager.instance.SetUsingMultipleSelect(!LevelRecordManager.instance.multipleObjectsSelected);
           
            return;
        }
        LevelRecordManager.instance.SetStaticViewParameter(type, !isShown);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // if (!isCrossedOut) return;
        // crossOut.DOFade(0, 0.5f).OnComplete(() => crossOut.enabled = false);
        // isCrossedOut = false;
    }


    private void CheckSelf()
    {
        switch (type)
        {
            case Type.Lines:
                if (LevelRecordManager.ShowLines)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;

                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;
            case Type.Speeds:
                if (LevelRecordManager.ShowSpeeds && LevelRecordManager.ShowLines)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;
                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;
            case Type.Parameters:
                if (LevelRecordManager.ShowParameters)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;
                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;
            case Type.Time:
                if (LevelRecordManager.ShowTime)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;
                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;
            case Type.Folders:
                if (LevelRecordManager.ShowFolders)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;
                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;

            case Type.MultiTouch:
                if (LevelRecordManager.instance.multipleObjectsSelected)
                {
                    fillImage.color = colorSO.SelctedUIColor;
                    crossOut.enabled = false;
                }
                else
                {
                    fillImage.color = colorSO.MainUIColor;
                    crossOut.enabled = true;
                }
                break;
        }

        isShown = !crossOut.enabled;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        LevelRecordManager.CheckViewParameters += CheckSelf;

    }

    private void OnDisable()
    {
        LevelRecordManager.CheckViewParameters -= CheckSelf;
    }
}
