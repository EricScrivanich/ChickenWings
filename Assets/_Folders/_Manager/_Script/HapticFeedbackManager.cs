using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager instance;

    private int vibrationStrength;
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
        vibrationStrength = PlayerPrefs.GetInt("VibrationStrength", 2);
    }

    public void LoadSavedData()
    {
        vibrationStrength = PlayerPrefs.GetInt("VibrationStrength", 2);

    }
    // Start is called before the first frame update
    public void PlayerButtonPress()
    {
        switch (vibrationStrength)
        {
            case 0:

                break;
            case 1:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

                break;
            case 2:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

                break;
            case 3:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

                break;
        }

    }

    public void PressShotgunButton()
    {
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);

    }

    public void ReleaseShotgunButton()
    {
        switch (vibrationStrength)
        {
            case 0:

                break;
            case 1:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

                break;
            case 2:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

                break;
            case 3:
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

                break;
        }



    }

    public void SwitchAmmo()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);

    }



    public void PlayerButtonFailure()
    {
        AudioManager.instance.PlayErrorSound();

        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }

    public void SetNewStrength(int s)
    {
        vibrationStrength = s;
    }

    public void SoftImpactButton()
    {
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

    }

    public void PressUIButton()
    {

       
        AudioManager.instance.PlayButtonClickSound();
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

    }
    public void LightImpactButton()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

    }
}
