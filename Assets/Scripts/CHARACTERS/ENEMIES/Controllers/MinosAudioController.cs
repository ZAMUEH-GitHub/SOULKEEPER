using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MinosAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bigFootstep;
    [SerializeField] private AudioClip[] monsterGrunt;
    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private AudioClip[] bigWhoosh;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip[] prepareAttackSounds;
    [SerializeField] private AudioClip[] prepareJumpSounds;
    [SerializeField] private AudioClip[] jumpSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayBigFootstepSound() => PlayRandomClip(bigFootstep);
    public void PlayGruntSound() => PlayRandomClip(monsterGrunt);
    public void PlayBigWhooshSound() => PlayRandomClip(bigWhoosh);
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