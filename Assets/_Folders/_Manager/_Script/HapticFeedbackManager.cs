using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;
using UnityEngine.InputSystem;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager instance;
    private int deviceType;

    private int vibrationStrength;
 
    // 0 touchscren, 1 gamepad 2 keyboard
    public void SetDeviceType(int type)
    {
        Debug.Log("Setting device type to: " + type);
        deviceType = type;



    }

    private void SetDeviceType()
    {
        int type = 2;

        foreach (var device in InputSystem.devices)
        {
            Debug.Log("Device detected: " + device.displayName + " - Type: " + device.GetType().Name);

            if (device is Gamepad gamepad)
            {
                // Skip virtual devices with no product/manufacturer info
                if (string.IsNullOrEmpty(device.description.product) &&
                    string.IsNullOrEmpty(device.description.manufacturer))
                {
                    Debug.Log("Skipping virtual gamepad: " + device.displayName);
                    continue;
                }

                Debug.Log("Physical controller detected: " + device.displayName +
                          " (" + device.description.manufacturer + " - " + device.description.product + ")");
                SetDeviceType(1);
                return;
            }
            else if (device is Touchscreen)
            {
                Debug.Log("Touchscreen detected: " + device.displayName);
                type = 0;

            }
            else if (device is Keyboard)
            {
                Debug.Log("Keyboard detected: " + device.displayName);


            }

        }

        if (HapticFeedbackManager.instance != null)
            SetDeviceType(type);

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
        SetDeviceType();
        vibrationStrength = PlayerPrefs.GetInt("VibrationStrength", 2);

     
    }

 
    public void LoadSavedData()
    {
        vibrationStrength = PlayerPrefs.GetInt("VibrationStrength", 2);

    }
    // Start is called before the first frame update
    public void PlayerButtonPress()
    {
        if (deviceType > 1) return;

        else if (deviceType == 1)
        {
            HapticPatterns.PlayEmphasis(.05f, .3f);
            Debug.Log("Soft impact played");
            return;

        }
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
