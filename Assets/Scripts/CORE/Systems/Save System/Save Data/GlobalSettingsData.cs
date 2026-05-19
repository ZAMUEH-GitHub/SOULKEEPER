using System;

[Serializable]
public class GlobalSettingsData
{
    [UnityEngine.Header("Audio Settings")]
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool isMuted = false;

    [UnityEngine.Header("Video Settings")]
    public bool isFullscreen = true;
    public int resolutionIndex = -1;
    public int qualityIndex = 2;
    public bool particlesEnabled = true;
}