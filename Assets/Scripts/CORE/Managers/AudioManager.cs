using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Volume Levels")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Mute Settings")]
    [SerializeField] private bool isMuted = false;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void Start()
    {
        HandleGameStateChange(FindFirstObjectByType<GameManager>().CurrentState);
        ApplyVolumes();
    }

    #region Volume Control
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void ToggleMute(bool state)
    {
        isMuted = state;
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        float globalVolume = isMuted ? 0f : masterVolume;

        if (musicSource)
        {
            musicSource.volume = globalVolume * musicVolume;
            musicSource.mute = isMuted;
        }

        if (sfxSource)
        {
            sfxSource.volume = globalVolume * sfxVolume;
            sfxSource.mute = isMuted;
        }
    }

    public bool IsMuted => isMuted;
    #endregion

    #region Music Control
    private void HandleGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                CrossFadeMusic(mainMenuMusic, 2f);
                break;
            case GameState.Gameplay:
                CrossFadeMusic(gameplayMusic, 2f);
                break;
        }
    }

    private void CrossFadeMusic(AudioClip newClip, float duration)
    {
        if (musicSource == null || newClip == null)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(CrossFadeRoutine(newClip, duration));
    }

    private IEnumerator CrossFadeRoutine(AudioClip newClip, float duration)
    {
        float startVol = musicSource.volume;
        for (float t = 0; t < duration / 2; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / (duration / 2));
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < duration / 2; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, masterVolume * musicVolume, t / (duration / 2));
            yield return null;
        }

        musicSource.volume = masterVolume * musicVolume;
    }
    #endregion

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip && !isMuted)
            sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position)
    {
        if (!clip || isMuted) return;
        AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume);
    }
    #endregion
}
