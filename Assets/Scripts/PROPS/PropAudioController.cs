using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PropAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] flameSounds;
    [SerializeField] private AudioClip[] bulbSounds;
    [SerializeField] private AudioClip[] relicOpenSounds;
    [SerializeField] private AudioClip[] relicCloseSounds;
    [SerializeField] private AudioClip[] flameSmallSounds;
    [SerializeField] private AudioClip[] flameBigSounds;
    [SerializeField] private AudioClip[] scoreSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayFlameSound() => PlayRandomClip(flameSounds);
    public void PlayScoreSound() => PlayRandomClip(scoreSounds);
    public void PlayBulbSound() => PlayRandomClip(bulbSounds);
    public void PlayRelicOpenSound() => PlayRandomClip(relicOpenSounds);

    public void PlayRelicCloseSound() => PlayRandomClip(relicCloseSounds);
    public void PlayFlameSmallSound() => PlayRandomClip(flameSmallSounds);
    public void PlayFlameBigSound() => PlayRandomClip(flameBigSounds);

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