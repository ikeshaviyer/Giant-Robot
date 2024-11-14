using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer; // Reference to the Audio Mixer

    public Slider masterVolumeSlider; // UI Slider for Master Volume
    public Slider sfxVolumeSlider; // UI Slider for SFX Volume
    public Slider musicVolumeSlider; // UI Slider for Music Volume

    private void Start()
    {
        // Load saved volume settings and set sliders
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        // Add listeners to sliders
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    // Method to set Master Volume
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20); // Convert 0-1 to dB
        PlayerPrefs.SetFloat("MasterVolume", value); // Save setting
    }

    // Method to set SFX Volume
    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); // Convert 0-1 to dB
        PlayerPrefs.SetFloat("SFXVolume", value); // Save setting
    }

    // Method to set Music Volume
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20); // Convert 0-1 to dB
        PlayerPrefs.SetFloat("MusicVolume", value); // Save setting
    }

    public void CloseSettings()
    {
        // Hide the settings panel
        gameObject.SetActive(false);
    }
}
