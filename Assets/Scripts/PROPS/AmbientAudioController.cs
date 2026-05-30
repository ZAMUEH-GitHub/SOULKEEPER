using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private float baseVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;
    }

    private void Start()
    {
        UpdateVolume();
        AudioManager.OnVolumesChanged += UpdateVolume;
    }

    private void OnDestroy()
    {
        AudioManager.OnVolumesChanged -= UpdateVolume;
    }

    private void UpdateVolume()
    {
        if (AudioManager.Instance != null)
        {
            float global = AudioManager.Instance.IsMuted ? 0f : AudioManager.Instance.MasterVolume;
            audioSource.volume = baseVolume * (global * AudioManager.Instance.SfxVolume);
        }
    }
}