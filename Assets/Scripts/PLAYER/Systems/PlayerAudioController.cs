using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] dashSounds;
    [SerializeField] private AudioClip[] fallSounds;
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private AudioClip[] footstepSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAttackSound() => PlayRandomClip(attackSounds);
    public void PlayDashSound() => PlayRandomClip(dashSounds);
    public void PlayFallSound() => PlayRandomClip(fallSounds);
    public void PlayJumpSound() => PlayRandomClip(jumpSounds);
    public void PlayFootstepSound() => PlayRandomClip(footstepSounds);

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