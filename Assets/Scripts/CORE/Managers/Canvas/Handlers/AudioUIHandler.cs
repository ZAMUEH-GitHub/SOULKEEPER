using UnityEngine;
using UnityEngine.UI;

public class UIAudioHandler : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("UI Toggles")]
    [SerializeField] private Toggle muteToggle;

    private AudioManager audioManager;
    private SettingsManager settingsManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        settingsManager = SettingsManager.Instance;

        if (audioManager == null)
        {
            Debug.LogWarning("[AudioUIController] AudioManager.Instance is null. UI won't function.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        InitializeUI();
    }

    private void Start()
    {
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(OnMuteToggled);

        InitializeUI();
    }

    private void OnDestroy()
    {
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(OnMuteToggled);
    }

    private void InitializeUI()
    {
        audioManager ??= AudioManager.Instance;
        settingsManager ??= SettingsManager.Instance;

        if (audioManager == null || settingsManager == null || settingsManager.CurrentSettings == null) return;

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(settingsManager.CurrentSettings.masterVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(settingsManager.CurrentSettings.musicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(settingsManager.CurrentSettings.sfxVolume);
        if (muteToggle != null) muteToggle.SetIsOnWithoutNotify(settingsManager.CurrentSettings.isMuted);
    }

    private void OnMasterChanged(float v)
    {
        audioManager?.SetMasterVolume(v);
        if (settingsManager != null) settingsManager.CurrentSettings.masterVolume = v;
    }

    private void OnMusicChanged(float v)
    {
        audioManager?.SetMusicVolume(v);
        if (settingsManager != null) settingsManager.CurrentSettings.musicVolume = v;
    }

    private void OnSfxChanged(float v)
    {
        audioManager?.SetSfxVolume(v);
        if (settingsManager != null) settingsManager.CurrentSettings.sfxVolume = v;
    }

    private void OnMuteToggled(bool on)
    {
        audioManager?.ToggleMute(on);
        if (settingsManager != null) settingsManager.CurrentSettings.isMuted = on;
    }

    public void SaveAudioSettings()
    {
        settingsManager?.SaveSettings();
    }
}