using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AreaTitlePanelManager : Singleton<AreaTitlePanelManager>
{
    protected override bool IsPersistent => false;

    [Header("Animation Settings")]
    [SerializeField] private Animator titleAnimator;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip catacombsSound;
    [SerializeField] private AudioClip cathedralSound;
    [SerializeField] private AudioClip cavernsSound;
    [SerializeField] private AudioClip firePalaceSound;
    [SerializeField] private AudioClip purePalaceSound;

    [Header("Display Settings")]
    [Tooltip("How long the panel remains active before disabling.")]
    [SerializeField] private float defaultDisplayDuration = 5f;

    private Coroutine currentRoutine;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ShowAreaTitle(string animationTriggerName, float displayDuration)
    {
        if (displayDuration <= 0f)
            displayDuration = defaultDisplayDuration;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAreaTitleRoutine(animationTriggerName, displayDuration));
    }

    private IEnumerator ShowAreaTitleRoutine(string animationTriggerName, float duration)
    {
        var canvas = CanvasManager.Instance;
        if (canvas == null)
        {
            Debug.LogError("[AreaTitlePanelManager] CanvasManager not found!");
            yield break;
        }

        canvas.FadeIn(PanelType.AreaTitlePanel);

        if (titleAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            titleAnimator.SetTrigger(animationTriggerName);
            PlayAreaSound(animationTriggerName);
        }

        yield return new WaitForSecondsRealtime(canvas.GetFadeDuration(PanelType.AreaTitlePanel) + duration);

        canvas.FadeOut(PanelType.AreaTitlePanel);
        currentRoutine = null;
    }

    private void PlayAreaSound(string triggerName)
    {
        AudioClip clipToPlay = null;

        switch (triggerName)
        {
            case "Catacombs":
                clipToPlay = catacombsSound;
                break;
            case "Cathedral":
                clipToPlay = cathedralSound;
                break;
            case "Caverns":
                clipToPlay = cavernsSound;
                break;
            case "FirePalace":
            case "Fire Palace":
                clipToPlay = firePalaceSound;
                break;
            case "PurePalace":
            case "Pure Palace":
                clipToPlay = purePalaceSound;
                break;
        }

        if (clipToPlay != null && AudioManager.Instance != null && !AudioManager.Instance.IsMuted)
        {
            float currentSfxVolume = AudioManager.Instance.MasterVolume * AudioManager.Instance.SfxVolume;
            audioSource.PlayOneShot(clipToPlay, currentSfxVolume);
        }
    }

    public void ForceHide()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        CanvasManager.Instance?.FadeOut(PanelType.AreaTitlePanel);
    }
}