using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UiAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] barOpen;
    [SerializeField] private AudioClip[] barClose;
    [SerializeField] private AudioClip[] buttonClick;
    [SerializeField] private AudioClip[] buttonSelect;
    [SerializeField] private AudioClip[] checkBoxClic;


    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayBarOpenSound() => PlayRandomClip(barOpen);
    public void PlayBarCloseSound() => PlayRandomClip(barClose);
    public void PlayButtonClickSound() => PlayRandomClip(buttonClick);
    public void PlayButtonSelectSound() => PlayRandomClip(buttonSelect);

    public void PlayCheckBoxClicSound() => PlayRandomClip(checkBoxClic);


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