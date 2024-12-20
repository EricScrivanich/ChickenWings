using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ButtonColorsSO : ScriptableObject
{
    [HideInInspector]
    public int currentO;
    [HideInInspector]
    public int currentN;
    [HideInInspector]
    public int currentD;
    public CustomButtonOptions options;
    [Header("Global Button Colors")]
    public Color normalButtonColor;
    public Color damagedOutlineColor;
    public Color frozenOutlineColor;
    public Color frozenFillColor;
    public Color normalButtonColorFull;
    public Color highlightButtonColor;
    public Color disabledButtonColor;
    public Color disabledButtonColorFull;
    public Color DashImageManaHighlight;
    public Color DashImageManaHighlight2;
    public Color DashImageManaDisabled;
    public Color DashImageManaDisabledFilling;
    public Color coolDownColorRed;
    public Color coolDownColorBlue;

    [Header("FlashButtonColors")]
    public Color flashButtonColor1;
    public Color flashButtonColor2;

    [Header("Star Colors")]
    public Color StarNoneColor;
    public Color StarGottenColor;
    public Color StarNormalColor;
    public Color StarCardDisabledFillColor;
    public Color StarCardDisabledOutlineColor;
    public Color StarCardGoldFillColor1;
    public Color StarCardGoldOutlineColor1;
    public Color StarCardGoldFillColor2;
    public Color StarCardGoldOutlineColor2;



    [Header("Mana Colors")]
    public Color fillingManaColor;
    public Color canUseDashSlashImageColor1;
    public Color canUseDashSlashImageColor2;

    [Header("Pause Menu Colors")]
    public Color pmUnPressedColor;
    public Color pmPressedColor;

    [Header("Gameover Colors")]
    public Color goUnPressedColor;
    public Color goPressedColor;

    [Header("Outline Color")]

    public Color OutLineColor;

    [Header("UI COLORs")]
    public Color NormalSignButtonColor;
    public Color disabledSignButtonColor;
    public Color disabledSignTextColor;

    [Header("Text Colors")]
    public Color MainTextColor;
    public Color DisabledEggTextColor;
    public Color DisabledEggColor;

    public Color DisabledScopeFillColor;


    public void LoadOptions(int n, int o, int d)
    {
        Debug.LogError("Loading Options");
        normalButtonColor = options.fillColorOptions[n];
        normalButtonColorFull = new Color(normalButtonColor.r, normalButtonColor.g, normalButtonColor.b, 1);
        OutLineColor = options.outlineColorOptions[o];
        disabledButtonColor = new Color(options.fillColorOptions[d].r, options.fillColorOptions[d].g, options.fillColorOptions[d].b, .7f);
        disabledButtonColorFull = new Color(disabledButtonColor.r, disabledButtonColor.g, disabledButtonColor.b, 1);

        Color c = options.fillColorOptions[n];

        float addedWhiteHighlight = 0;
        float average = (c.r + c.g + c.b) / 3;

        Debug.LogError("Color stuff: rbg: " + c.r + " - " + c.g + " - " + c.b + " - " + "average: " + average);

        addedWhiteHighlight = (1 - average) * .35f;
        Debug.LogError("Added white is: " + addedWhiteHighlight);
        highlightButtonColor = new Color(c.r + addedWhiteHighlight, c.g + addedWhiteHighlight, c.b + addedWhiteHighlight, 1);
        Debug.LogError("Highlight: rbg: " + highlightButtonColor.r + " - " + highlightButtonColor.g + " - " + highlightButtonColor.b);


    }








}
