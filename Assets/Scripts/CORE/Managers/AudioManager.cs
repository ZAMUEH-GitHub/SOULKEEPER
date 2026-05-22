using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    protected override bool IsPersistent => false;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Scene Specific Music")]
    [SerializeField] private List<SceneMusicMapping> sceneMusicMappings = new List<SceneMusicMapping>();

    [System.Serializable]
    public struct SceneMusicMapping
    {
        public string sceneName;
        public AudioClip musicClip;
    }

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

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        ApplyVolumes();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnGameStateChanged -= HandleGameStateChange;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            HandleGameStateChange(GameManager.Instance.CurrentState);

        ApplyVolumes();
    }
    #endregion

    #region Volume & Mute
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
    #endregion

    #region Game State Music Handling

    private void HandleGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                CrossFadeMusic(mainMenuMusic, 2f);
                break;
            case GameState.Gameplay:
                PlayGameplayMusicForCurrentScene();
                break;
            case GameState.Cinematic:
                CrossFadeMusic(null, 1f);
                break;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Cinematic"))
        {
            CrossFadeMusic(null, 1f);
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Gameplay)
        {
            PlayGameplayMusicForCurrentScene();
        }
    }

    private void PlayGameplayMusicForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        AudioClip clipToPlay = gameplayMusic;

        foreach (var mapping in sceneMusicMappings)
        {
            if (mapping.sceneName == currentScene)
            {
                clipToPlay = mapping.musicClip;
                break;
            }
        }

        CrossFadeMusic(clipToPlay, 2f);
    }

    private void CrossFadeMusic(AudioClip newClip, float duration)
    {
        if (musicSource == null) return;
        if (musicSource.clip == newClip) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

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

        if (newClip != null)
        {
            musicSource.Play();
        }

        float target = (isMuted ? 0f : masterVolume) * musicVolume;
        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, target, t / half);
            yield return null;
        }

        musicSource.volume = target;
    }
    #endregion

    #region Sound Effects
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
    #endregion
}