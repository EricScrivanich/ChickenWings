using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider vibrationSlider;

    void Start()
    {
        // Initialize sliders with saved PlayerPrefs values or defaults
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        vibrationSlider.value = PlayerPrefs.GetInt("VibrationStrength", 2);

        // Add listeners to handle slider value changes
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        vibrationSlider.onValueChanged.AddListener(value => SetVibration((int)value));
    }

    public void SetDefault()
    {
        // Set slider values to default
        sfxSlider.value = 1;
        musicSlider.value = 1;
        vibrationSlider.value = 2;

        // Save defaults to PlayerPrefs and apply changes
        SetSFXVolume(1.0f);
        SetMusicVolume(1.0f);
        SetVibration(2);
    }

    public void SetSFXVolume(float val)
    {
        PlayerPrefs.SetFloat("SFXVolume", val);
        Debug.LogError("Setting SFX volume: " + val);
        PlayerPrefs.Save();
        AudioManager.instance.LoadVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f), PlayerPrefs.GetFloat("SFXVolume", 1.0f), true);
    }

    public void SetMusicVolume(float val)
    {
        PlayerPrefs.SetFloat("MusicVolume", val);
        PlayerPrefs.Save();
        AudioManager.instance.LoadVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f), PlayerPrefs.GetFloat("SFXVolume", 1.0f), true);
    }

    public void SetVibration(int val)
    {
        PlayerPrefs.SetInt("VibrationStrength", val);
        Debug.LogError("VIB STRENGTH IS: " + val);
        PlayerPrefs.Save();
        HapticFeedbackManager.instance.SetNewStrength(val);
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        sfxSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        vibrationSlider.onValueChanged.RemoveAllListeners();
    }
}