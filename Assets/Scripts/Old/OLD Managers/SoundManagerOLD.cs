/*using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManagerOLD : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public struct SceneMusic
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    [Header("Assign BGMs per scene here")]
    public SceneMusic[] sceneMusics;

    private AudioSource audioSource;
    private float savedVolume = 1f; // Default volume

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Load saved volume from PlayerPrefs
        savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;
    }

    public void SetMute(bool mute)
    {
        AudioListener.volume = mute ? 0f : savedVolume;
    }

    public void SetVolume(float volume)
    {
        savedVolume = volume;
        if (AudioListener.volume > 0f)  // Only update if not muted
            AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", savedVolume);
        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        AudioClip clip = null;
        foreach (var sm in sceneMusics)
        {
            if (sm.sceneName == sceneName)
            {
                clip = sm.bgmClip;
                break;
            }
        }

        if (clip != null)
        {
            if (audioSource.clip == clip && audioSource.isPlaying) return; // Already playing this BGM

            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[SoundManager] No BGM assigned for scene: {sceneName}");
            audioSource.Stop();
        }
    }
}
*/