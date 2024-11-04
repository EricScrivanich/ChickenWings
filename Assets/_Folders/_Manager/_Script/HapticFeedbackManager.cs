using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    public void PlayerButtonPress()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
    }

    public void PlayerButtonFailure()
    {
        AudioManager.instance.PlayErrorSound();
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }

    public void SoftImpactButton()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

    }

    public void PressUIButton()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        AudioManager.instance.PlayButtonClickSound();

    }
    public void LightImpactButton()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

    }
}
