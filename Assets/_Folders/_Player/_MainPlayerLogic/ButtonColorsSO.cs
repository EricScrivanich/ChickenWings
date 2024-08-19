using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ButtonColorsSO : ScriptableObject
{
    [Header("Global Button Colors")]
    public Color normalButtonColor;
    public Color highlightButtonColor;
    public Color disabledButtonColor;
    public Color DashImageManaHighlight;
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
}
