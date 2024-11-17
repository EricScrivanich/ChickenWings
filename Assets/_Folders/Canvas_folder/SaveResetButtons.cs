using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SaveResetButtons : MonoBehaviour
{
    private CanvasGroup group;
    [SerializeField] private int type;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image outlineImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image blockButtonsImage;
    private RectTransform rect;

    [SerializeField] private MainMenuScript mainMenu;

    [SerializeField] private GameObject CheckToSaveDropDown;

    private bool needsCheck;

    private Button button;


    [Header("Button Colors")]

    [SerializeField] private Color normalTextOutlineColor;
    [SerializeField] private Color normalFillColor;
    [SerializeField] private Color disabledTextOutlineColor;
    [SerializeField] private Color disabledFillColor;
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();


    }
    // Start is called before the first frame update
    void Start()
    {
        if (type == 0)
        {
            CheckToSaveDropDown.SetActive(false);
            blockButtonsImage.gameObject.SetActive(false);
        }
        else if (type == -1)
        {
            fillImage.color = normalFillColor;
            text.color = normalTextOutlineColor;
            outlineImage.color = normalTextOutlineColor;
        }




    }

    private void Hide()
    {
        if (type == -1) return;
        bool enabled = button.enabled;
        button.enabled = false;

        var seq = DOTween.Sequence();
        seq.Append(group.DOFade(0, .1f));
        seq.AppendInterval(1.7f);
        seq.Append(group.DOFade(1, .3f));
        seq.Play().OnComplete(() => button.enabled = enabled);
    }

    private void OnEnable()
    {
        ButtonColorManager.OnSetSaveResetButtonPressable += SetPressable;
        ButtonColorManager.OnHideSaveDefaultButtons += Hide;

    }
    private void OnDisable()
    {
        ButtonColorManager.OnSetSaveResetButtonPressable -= SetPressable;
        ButtonColorManager.OnHideSaveDefaultButtons -= Hide;

    }

    public void OnPress()
    {
        needsCheck = false;
        var seq = DOTween.Sequence();
        seq.Append(rect.DOScale(1.2f, .25f));
        seq.Join(fillImage.DOFade(.9f, .25f));
        seq.Append(rect.DOScale(1f, .25f));

        HapticFeedbackManager.instance.PressUIButton();
        if (type != -1)
        {
            seq.Join(fillImage.DOColor(disabledFillColor, .1f));
            seq.Join(text.DOColor(disabledTextOutlineColor, .1f));
            seq.Join(outlineImage.DOColor(disabledTextOutlineColor, 1f));
        }
        else
        {
            seq.Join(fillImage.DOColor(normalFillColor, .1f));

        }

        seq.Play();

    }


    public void OnSaveWithCheck()
    {

        StartCoroutine(SaveWithCheckDelay());


    }

    private IEnumerator SaveWithCheckDelay()
    {
        yield return new WaitForSecondsRealtime(.4f);
        CheckToSaveDropDown.GetComponent<SignMovement>().SpecialRetract();
        blockButtonsImage.DOFade(0f, .3f);
        yield return new WaitForSecondsRealtime(.3f);
        blockButtonsImage.gameObject.SetActive(false);
        mainMenu.SwitchMenu(-2);


    }




    public void SetPressable(int t, bool pressable)
    {
        if (type != t) return;

        needsCheck = pressable;
        button.enabled = pressable;
        if (pressable)
        {
            outlineImage.color = normalTextOutlineColor;
            text.color = normalTextOutlineColor;
            fillImage.color = normalFillColor;

        }
        else
        {
            outlineImage.color = disabledTextOutlineColor;
            text.color = disabledTextOutlineColor;
            fillImage.color = disabledFillColor;

        }

    }

    public void CheckIfNeedsSave()
    {
        if (type != 0) return;

        HapticFeedbackManager.instance.PressUIButton();
        if (!needsCheck)
        {
            mainMenu.SwitchMenu(0);
        }
        else
        {
            CheckToSaveDropDown.SetActive(true);
            blockButtonsImage.gameObject.SetActive(true);
            blockButtonsImage.DOFade(.65f, .5f).From(0);

        }
    }



    // Update is called once per frame

}
