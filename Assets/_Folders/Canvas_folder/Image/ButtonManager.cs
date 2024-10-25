using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    [ExposedScriptableObject]
    [SerializeField] private ButtonColorsSO colorSO;


    [SerializeField] private PlayerID player;
    [SerializeField] private Material yer;
    [SerializeField] private Image pauseOutline;
    [SerializeField] private Image pauseFill;
    [SerializeField] private Image[] outlines;
    [SerializeField] private Image[] fills;
    [SerializeField] private Image[] fullFills;

    [SerializeField] private float moveAmountForRecording;

    [SerializeField] private RectTransform[] moveRects;

    [SerializeField] private bool moveRectsForRecording;
    // Start is called before the first frame update


    void Awake()
    {
        if (moveRectsForRecording)
        {
            foreach (var r in moveRects)
            {
                r.anchoredPosition = new Vector2(r.anchoredPosition.x - moveAmountForRecording, r.anchoredPosition.y);
            }

            if (GameObject.Find("CanvasScreen") != null)
            {
                RectTransform lives = GameObject.Find("Lives3").GetComponent<RectTransform>();

                if (lives != null)
                    lives.anchoredPosition = new Vector2(lives.anchoredPosition.x - moveAmountForRecording, lives.anchoredPosition.y);
            }
        }
    }

    public void PlayerDamgedTween(bool isDead)
    {
        if (!isDead)
        {
            StartCoroutine(DoLinesTween());
        }
        else
        {
            foreach (var line in outlines)
            {
                line.DOColor(colorSO.damagedOutlineColor, .25f);
            }

        }
    }

    public void PlayerFrozenTween(bool isFrozen)
    {
        if (isFrozen)
        {
            foreach (var line in outlines)
            {
                line.DOColor(colorSO.frozenOutlineColor, .3f).SetEase(Ease.InSine);
            }
            foreach (var fill in fills)
            {
                fill.DOColor(colorSO.frozenFillColor, .3f).SetEase(Ease.InSine);
            }
        }

        else
        {
            foreach (var line in outlines)
            {
                line.DOColor(colorSO.OutLineColor, .25f).SetEase(Ease.OutSine);
            }
            foreach (var fill in fills)
            {
                fill.DOColor(colorSO.normalButtonColor, .25f).SetEase(Ease.OutSine);
            }
        }

    }



    private IEnumerator DoLinesTween()
    {
        foreach (var line in outlines)
        {
            line.DOColor(colorSO.damagedOutlineColor, .15f);
        }

        yield return new WaitForSeconds(.25f);
        foreach (var line in outlines)
        {
            line.DOColor(colorSO.OutLineColor, .15f);
        }


    }

    private void OnEnable()
    {
        player.globalEvents.OnPlayerDamaged += PlayerDamgedTween;
        player.globalEvents.OnPlayerFrozen += PlayerFrozenTween;
    }
    private void OnDisable()
    {
        player.globalEvents.OnPlayerDamaged -= PlayerDamgedTween;
        player.globalEvents.OnPlayerFrozen -= PlayerFrozenTween;


    }
    void Start()
    {

        pauseFill.color = colorSO.normalButtonColor;
        pauseOutline.color = colorSO.OutLineColor;
        foreach (Image img in outlines)
        {
            img.color = colorSO.OutLineColor;
            // img.color = Color.white;
            // img.material = yer;


        }

        foreach (Image img in fills)
        {
            img.color = colorSO.normalButtonColor;
            // img.material = yer;
        }


        foreach (Image img in fullFills)
        {
            img.color = colorSO.normalButtonColorFull;
            // img.material = yer;
        }

    }

}
