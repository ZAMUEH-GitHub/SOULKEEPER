using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PropAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] flameSounds;
    [SerializeField] private AudioClip[] bulbSounds;
    [SerializeField] private AudioClip[] relicSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayFlameSound() => PlayRandomClip(flameSounds);
    public void PlayBulbSound() => PlayRandomClip(bulbSounds);
    public void PlayRelicSound() => PlayRandomClip(relicSounds);

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clipToPlay = clips.Length == 1 ? clips[0] : clips[Random.Range(0, clips.Length)];

        if (AudioManager.Instance != null && !AudioManager.Instance.IsMuted)
        {
            float currentSfxVolume = AudioManager.Instance.MasterVolume * AudioManager.Instance.SfxVolume;
            audioSource.PlayOneShot(clipToPlay, currentSfxVolume);
        }
    }
}