using System;
using System.IO;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    protected override bool IsPersistent => true;

    private string SettingsFilePath => Path.Combine(Application.persistentDataPath, "GlobalSettings.json");

    public GlobalSettingsData CurrentSettings { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this) return;

        LoadSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists(SettingsFilePath))
        {
            try
            {
                string json = File.ReadAllText(SettingsFilePath);
                CurrentSettings = JsonUtility.FromJson<GlobalSettingsData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SettingsManager] Failed to load settings: {ex.Message}");
                CurrentSettings = new GlobalSettingsData();
            }
        }
        else
        {
            CurrentSettings = new GlobalSettingsData();
            SaveSettings();
        }

        ApplySettings();
    }

    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(CurrentSettings, true);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SettingsManager] Failed to save settings: {ex.Message}");
        }
    }

    public void ApplySettings()
    {
        Screen.fullScreen = CurrentSettings.isFullscreen;
        QualitySettings.SetQualityLevel(CurrentSettings.qualityIndex);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(CurrentSettings.masterVolume);
            AudioManager.Instance.SetMusicVolume(CurrentSettings.musicVolume);
            AudioManager.Instance.SetSfxVolume(CurrentSettings.sfxVolume);
            AudioManager.Instance.ToggleMute(CurrentSettings.isMuted);
        }
    }
}