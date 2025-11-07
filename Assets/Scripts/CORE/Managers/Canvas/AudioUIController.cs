using UnityEngine;
using UnityEngine.UI;

public class AudioUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("UI Toggles")]
    [SerializeField] private Toggle muteToggle;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();

        if (audioManager == null)
        {
            Debug.LogWarning("AudioManager not found in scene!");
            return;
        }

        InitializeUI();

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        if (muteToggle != null)
            muteToggle.onValueChanged.AddListener(OnMuteToggled);
    }

    public void InitializeUI()
    {
        if (audioManager == null) return;

        if (masterSlider != null)
            masterSlider.value = audioManager.masterVolume;
        if (musicSlider != null)
            musicSlider.value = audioManager.musicVolume;
        if (sfxSlider != null)
            sfxSlider.value = audioManager.sfxVolume;

        if (muteToggle != null)
            muteToggle.isOn = audioManager.IsMuted;
    }

    private void OnMasterChanged(float value)
    {
        audioManager?.SetMasterVolume(value);
    }

    private void OnMusicChanged(float value)
    {
        audioManager?.SetMusicVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        audioManager?.SetSFXVolume(value);
    }

    private void OnMuteToggled(bool isOn)
    {
        audioManager?.ToggleMute(isOn);
    }

    private void OnDestroy()
    {
        if (masterSlider != null) masterSlider.onValueChanged.RemoveAllListeners();
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
        if (muteToggle != null) muteToggle.onValueChanged.RemoveAllListeners();
    }
}
