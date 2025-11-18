using System.Collections;
using UnityEngine;
using TMPro;

public class AreaTitlePanelManager : Singleton<AreaTitlePanelManager>
{
    protected override bool IsPersistent => false;

    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Display Settings")]
    [Tooltip("How long the title card remains visible before fading out.")]
    [SerializeField] private float defaultDisplayDuration = 5f;
    [Tooltip("Delay before subtitle fades in after title.")]
    [SerializeField] private float subtitleFadeDelay = 1f;

    private Coroutine currentRoutine;

    public void ShowAreaTitle(string title, string subtitle, float displayDuration)
    {
        if (titleText == null)
        {
            Debug.LogWarning("[AreaTitlePanelManager] Missing title TMP_Text reference!");
            return;
        }

        if (displayDuration <= 0f)
            displayDuration = defaultDisplayDuration;

        titleText.text = title;

        if (subtitleText != null)
        {
            subtitleText.text = subtitle ?? "";
            subtitleText.alpha = 0f;
            subtitleText.gameObject.SetActive(!string.IsNullOrWhiteSpace(subtitle));
        }

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

        if (subtitleText != null && subtitleText.gameObject.activeSelf)
        {
            yield return new WaitForSecondsRealtime(subtitleFadeDelay);
            StartCoroutine(FadeTMPAlpha(subtitleText, 1f, 0.4f));
        }

        yield return new WaitForSecondsRealtime(canvas.GetFadeDuration(PanelType.AreaTitlePanel) + duration);

        canvas.FadeOut(PanelType.AreaTitlePanel);
        currentRoutine = null;
    }

    private IEnumerator FadeTMPAlpha(TMP_Text text, float targetAlpha, float duration)
    {
        float startAlpha = text.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            text.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        text.alpha = targetAlpha;
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
