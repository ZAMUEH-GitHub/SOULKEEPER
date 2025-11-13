using UnityEngine;
using UnityEngine.UI;

public class AudioUIManager : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("UI Toggles")]
    [SerializeField] private Toggle muteToggle;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;

        if (audioManager == null)
        {
            Debug.LogWarning("[AudioUIController] AudioManager.Instance is null. UI won't function.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        InitializeUI();

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(OnMuteToggled);
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
        if (audioManager == null) return;

        if (masterSlider != null) masterSlider.value = audioManager.MasterVolume;
        if (musicSlider != null) musicSlider.value = audioManager.MusicVolume;
        if (sfxSlider != null) sfxSlider.value = audioManager.SfxVolume;
        if (muteToggle != null) muteToggle.isOn = audioManager.IsMuted;
    }

    private void OnMasterChanged(float v) => audioManager?.SetMasterVolume(v);
    private void OnMusicChanged(float v) => audioManager?.SetMusicVolume(v);
    private void OnSfxChanged(float v) => audioManager?.SetSfxVolume(v);
    private void OnMuteToggled(bool on) => audioManager?.ToggleMute(on);
}
