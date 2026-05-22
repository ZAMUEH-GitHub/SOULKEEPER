using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoUIHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Scrollbar qualityScrollbar;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle particlesToggle;

    private Resolution[] availableResolutions;

    private void Start()
    {
        InitializeResolutions();
        LoadCurrentSettings();

        if (qualityScrollbar != null) qualityScrollbar.onValueChanged.AddListener(OnQualityChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (particlesToggle != null) particlesToggle.onValueChanged.AddListener(OnParticlesToggled);
    }

    private void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;

        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadCurrentSettings()
    {
        var current = SettingsManager.Instance.CurrentSettings;

        if (particlesToggle != null)
            particlesToggle.SetIsOnWithoutNotify(current.particlesEnabled);

        if (qualityScrollbar != null)
        {
            float maxLevels = QualitySettings.names.Length - 1;
            qualityScrollbar.SetValueWithoutNotify(maxLevels > 0 ? current.qualityIndex / maxLevels : 0);
        }
    }

    private void OnQualityChanged(float value)
    {
        int totalLevels = QualitySettings.names.Length;
        int targetIndex = Mathf.RoundToInt(value * (totalLevels - 1));

        QualitySettings.SetQualityLevel(targetIndex);
        SettingsManager.Instance.CurrentSettings.qualityIndex = targetIndex;
    }

    private void OnResolutionChanged(int index)
    {
        Resolution res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, SettingsManager.Instance.CurrentSettings.isFullscreen);
        SettingsManager.Instance.CurrentSettings.resolutionIndex = index;
    }

    private void OnParticlesToggled(bool isOn)
    {
        SettingsManager.Instance.CurrentSettings.particlesEnabled = isOn;
    }

    private void OnDestroy()
    {
        if (qualityScrollbar != null) qualityScrollbar.onValueChanged.RemoveListener(OnQualityChanged);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        if (particlesToggle != null) particlesToggle.onValueChanged.RemoveListener(OnParticlesToggled);
    }
}