using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] smashAttackSounds;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] prepareAttackSounds;
    [SerializeField] private AudioClip[] prepareJumpSounds;
    [SerializeField] private AudioClip[] jumpSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 25f;
    }

    public void PlaySmashAttackSound() => PlayRandomClip(smashAttackSounds);
    public void PlayAttackSound() => PlayRandomClip(attackSounds);
    public void PlayFootstepSound() => PlayRandomClip(footstepSounds);
    public void PlayDamageSound() => PlayRandomClip(damageSounds);
    public void PlayIdleSound() => PlayRandomClip(idleSounds);
    public void PlayPrepareAttackSound() => PlayRandomClip(prepareAttackSounds);
    public void PlayPrepareJumpSound() => PlayRandomClip(prepareJumpSounds);
    public void PlayJumpSound() => PlayRandomClip(jumpSounds);

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