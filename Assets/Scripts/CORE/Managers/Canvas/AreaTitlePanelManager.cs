using System.Collections;
using UnityEngine;

public class AreaTitlePanelManager : Singleton<AreaTitlePanelManager>
{
    protected override bool IsPersistent => false;

    [Header("Animation Settings")]
    [SerializeField] private Animator titleAnimator;
    [Tooltip("Type the exact name of the Trigger parameter in your Animator to show the title.")]
    [SerializeField] private string showTriggerName = "ShowAnimation1";

    [Header("Display Settings")]
    [Tooltip("How long the panel remains active before disabling.")]
    [SerializeField] private float defaultDisplayDuration = 5f;

    private Coroutine currentRoutine;

    public void ShowAreaTitle(string title, string subtitle, float displayDuration)
    {
        if (displayDuration <= 0f)
            displayDuration = defaultDisplayDuration;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAreaTitleRoutine(displayDuration));
    }

    private IEnumerator ShowAreaTitleRoutine(float duration)
    {
        var canvas = CanvasManager.Instance;
        if (canvas == null)
        {
            Debug.LogError("[AreaTitlePanelManager] CanvasManager not found!");
            yield break;
        }

        canvas.FadeIn(PanelType.AreaTitlePanel);

        if (titleAnimator != null && !string.IsNullOrEmpty(showTriggerName))
        {
            titleAnimator.SetTrigger(showTriggerName);
        }

        yield return new WaitForSecondsRealtime(canvas.GetFadeDuration(PanelType.AreaTitlePanel) + duration);

        canvas.FadeOut(PanelType.AreaTitlePanel);
        currentRoutine = null;
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