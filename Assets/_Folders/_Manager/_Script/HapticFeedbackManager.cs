using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager instance;
    private int deviceType;

    private int vibrationStrength;
    // 0 touchscren, 1 gamepad 2 keyboard
    public void SetDeviceType(int type)
    {
        deviceType = type;

    }
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
        if (deviceType > 0) return;
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
        if (deviceType > 0) return;
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);

    }

    public void ReleaseShotgunButton()
    {
        if (deviceType > 1) return;
        else if (deviceType == 1 && vibrationStrength > 0)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
            return;

        }
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
        if (deviceType > 0) return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);

    }



    public void PlayerButtonFailure()
    {
        AudioManager.instance.PlayErrorSound();
        if (deviceType > 0) return;

        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
    }

    public void SetNewStrength(int s)
    {
        vibrationStrength = s;
    }

    public void SoftImpactButton()
    {
        if (deviceType > 0) return;
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

    }

    public void PressUIButton()
    {
        if (deviceType > 0) return;

        AudioManager.instance.PlayButtonClickSound();
        if (vibrationStrength > 0)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

    }
    public void LightImpactButton()
    {
        if (deviceType > 0) return;
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

    }
}
