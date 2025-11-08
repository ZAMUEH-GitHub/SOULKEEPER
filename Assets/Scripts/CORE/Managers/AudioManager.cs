using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Volume Levels")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

    [Header("Mute Settings")]
    [SerializeField] private bool isMuted = false;

    private Coroutine fadeRoutine;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;
    public bool IsMuted => isMuted;

    private void Start()
    {
        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null) HandleGameStateChange(gm.CurrentState);

        ApplyVolumes();
    }

    private void OnEnable() => ApplyVolumes();

    private void OnValidate()
    {
        if (musicSource == null)
            Debug.LogWarning("[AudioManager] Music Source is not assigned.");
        if (sfxSource == null)
            Debug.LogWarning("[AudioManager] SFX Source is not assigned (SFX volume will only scale one-shots if you provide a source).");
        if (musicSource != null && sfxSource != null && musicSource == sfxSource)
            Debug.LogWarning("[AudioManager] Music and SFX Source reference the SAME AudioSource. Use two separate components.");
    }

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

    public void SetSfxVolume(float value)
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
        float global = isMuted ? 0f : masterVolume;

        if (musicSource != null)
        {
            musicSource.volume = global * musicVolume;
            musicSource.mute = isMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = global * sfxVolume;
            sfxSource.mute = isMuted;
        }
    }

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
        if (musicSource == null || newClip == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossFadeRoutine(newClip, duration));
    }

    private IEnumerator CrossFadeRoutine(AudioClip newClip, float duration)
    {
        float half = Mathf.Max(0.01f, duration * 0.5f);
        float startVol = musicSource.volume;

        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / half);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        float target = (isMuted ? 0f : masterVolume) * musicVolume;
        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, target, t / half);
            yield return null;
        }

        musicSource.volume = target;
    }

    public void PlaySfx(AudioClip clip)
    {
        if (!clip || isMuted) return;

        var src = sfxSource != null ? sfxSource : musicSource;
        if (src != null)
            src.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void PlaySfxAtPoint(AudioClip clip, Vector3 position)
    {
        if (!clip || isMuted) return;
        AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume);
    }
}
