using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private CustomButtonOptions options;
    [ExposedScriptableObject]
    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private ButtonColorsSO defaultColors;
    [SerializeField] private GameObject colorMatchWarning;



    private ButtonColorManager saveSystem;


    [SerializeField] private PlayerID player;
    [SerializeField] private Material yer;
    [SerializeField] private Image pauseOutline;
    [SerializeField] private Image pauseFill;
    [SerializeField] private Image[] outlines;
    [SerializeField] private Image[] fills;
    [SerializeField] private Image[] fullFills;

    [SerializeField] private float moveAmountForRecording;


#if UNITY_EDITOR
    private float moveAmountSetVariable = 250;

    [SerializeField] private RectTransform[] moveRects;
    [SerializeField] private RectTransform[] moveRectsLeftSide;

    [SerializeField] private bool moveRectsForRecording;

    [SerializeField] private bool moveLeftRects;
    // Start is called before the first frame update


    void Awake()
    {

        if (moveRectsForRecording)
        {

            if (moveLeftRects)

            {

                foreach (var r in moveRectsLeftSide)
                {
                    // r.anchoredPosition = new Vector2(r.anchoredPosition.x - moveAmountForRecording, r.anchoredPosition.y);
                    r.anchoredPosition = new Vector2(r.anchoredPosition.x + moveAmountSetVariable, r.anchoredPosition.y);
                }

                if (GameObject.Find("CanvasScreen") != null)
                {

                    if (GameObject.Find("ProgressBar") != null)
                    {
                        RectTransform progBar = GameObject.Find("ProgressBar").GetComponent<RectTransform>();
                        if (progBar != null)
                            progBar.anchoredPosition = new Vector2(progBar.anchoredPosition.x + moveAmountForRecording, progBar.anchoredPosition.y);

                    }

                    if (GameObject.Find("ScoreGroup") != null)
                    {
                        RectTransform score = GameObject.Find("ScoreGroup").GetComponent<RectTransform>();
                        if (score != null)
                            score.anchoredPosition = new Vector2(score.anchoredPosition.x + moveAmountForRecording, score.anchoredPosition.y);

                    }





                }



            }

            else

            {
                foreach (var r in moveRects)
                {
                    // r.anchoredPosition = new Vector2(r.anchoredPosition.x - moveAmountForRecording, r.anchoredPosition.y);
                    r.anchoredPosition = new Vector2(r.anchoredPosition.x - moveAmountSetVariable, r.anchoredPosition.y);
                }

                if (GameObject.Find("CanvasScreen") != null)
                {
                    RectTransform lives = GameObject.Find("Lives3").GetComponent<RectTransform>();

                    // if (lives != null)
                    //     lives.anchoredPosition = new Vector2(lives.anchoredPosition.x - moveAmountForRecording, lives.anchoredPosition.y);
                    if (lives != null)
                        lives.anchoredPosition = new Vector2(lives.anchoredPosition.x - moveAmountSetVariable, lives.anchoredPosition.y);
                }

            }

        }
    }

#endif

    public bool CheckColors(int type, int ind)
    {
        bool isAllowed = saveSystem.CheckIfColorsMatch(type, ind);
        if (!isAllowed)
        {
            colorMatchWarning.SetActive(true);
        }
        return isAllowed;
    }
    public void SetNewColors(int iO, int iN, int iD, int type)
    {


        if (type == 0)
        {
            Color o = options.outlineColorOptions[iO];

            foreach (Image img in outlines)
            {
                img.color = o;
                // img.color = Color.white;
                // img.material = yer;
            }
            pauseOutline.color = o;
            saveSystem.ShowNewColors(iO, -1, -1);

        }
        else if (type == 1)
        {
            Color n = options.fillColorOptions[iN];

            foreach (Image img in fills)
            {
                img.color = n;
                // img.material = yer;
            }
            pauseFill.color = new Color(n.r, n.g, n.b, .8f);


            float addedWhiteHighlight = 0;
            float average = (n.r + n.g + n.b) / 3;
            addedWhiteHighlight = (1 - average) * .3f;
            Color h = new Color(n.r + addedWhiteHighlight, n.g + addedWhiteHighlight, n.b + addedWhiteHighlight, 1);
            saveSystem.ShowNewColors(-1, iN, -1);


        }
        else if (type == 2)
        {
            saveSystem.ShowNewColors(-1, -1, iD);

        }




        // saveSystem.SaveButtonColors(iN, iO, iD);

        // saveSystem.LoadButtonColors(colorSO);


    }

    public void RestoreDefaultColors()
    {
        Color o = defaultColors.OutLineColor;
        Color n = defaultColors.normalButtonColor;
        Color h = defaultColors.highlightButtonColor;

        foreach (Image img in outlines)
        {
            img.color = o;
            // img.color = Color.white;
            // img.material = yer;
        }
        pauseOutline.color = o;




        foreach (Image img in fills)
        {
            img.color = n;
            // img.material = yer;
        }
        pauseFill.color = n;


        saveSystem.ShowNewColors(0, 0, 3);

        // saveSystem.LoadButtonColors(colorSO);



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
        // player.globalEvents.OnPlayerDamaged += PlayerDamgedTween;
        player.globalEvents.OnPlayerFrozen += PlayerFrozenTween;
    }
    private void OnDisable()
    {
        // player.globalEvents.OnPlayerDamaged -= PlayerDamgedTween;
        player.globalEvents.OnPlayerFrozen -= PlayerFrozenTween;


    }
    void Start()
    {
        saveSystem = GetComponent<ButtonColorManager>();
        // saveSystem.LoadButtonColors(colorSO);
        pauseFill.color = new Color(colorSO.normalButtonColor.r, colorSO.normalButtonColor.g, colorSO.normalButtonColor.b, .8f);
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
