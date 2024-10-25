using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ButtonColorsSO : ScriptableObject
{
    [Header("Global Button Colors")]
    public Color normalButtonColor;
    public Color damagedOutlineColor;
    public Color frozenOutlineColor;
    public Color frozenFillColor;
    public Color normalButtonColorFull => new Color(normalButtonColor.r, normalButtonColor.g, normalButtonColor.b, 1);
    public Color highlightButtonColor;
    public Color disabledButtonColor;
    public Color disabledButtonColorFull => new Color(disabledButtonColor.r, disabledButtonColor.g, disabledButtonColor.b, 1);
    public Color DashImageManaHighlight;
    public Color DashImageManaHighlight2;
    public Color DashImageManaDisabled;
    public Color DashImageManaDisabledFilling;
    public Color coolDownColorRed;
    public Color coolDownColorBlue;

    [Header("FlashButtonColors")]
    public Color flashButtonColor1;
    public Color flashButtonColor2;



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


    [Header("Text Colors")]
    public Color MainTextColor;
    public Color DisabledEggTextColor;
    public Color DisabledEggColor;

    public Color DisabledScopeFillColor;











}
