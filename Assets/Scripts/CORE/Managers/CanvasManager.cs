using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    HUDPanel,
    PausePanel,
    BlackScreen
}

[System.Serializable]
public class PanelFadeSettings
{
    [Header("Panel Settings")]
    public PanelType panelType;
    public CanvasGroup panel;

    [Header("Fade Properties")]
    public float fadeDuration;
    public bool interactable;
    public bool blockRaycasts;
    public bool disablesPlayerInput;
}

public class CanvasManager : MonoBehaviour
{
    [Header("Panels Configuration")]
    [SerializeField] private List<PanelFadeSettings> panelSettingsList = new List<PanelFadeSettings>();

    private Dictionary<PanelType, PanelFadeSettings> panelSettings = new Dictionary<PanelType, PanelFadeSettings>();
    private Dictionary<CanvasGroup, Coroutine> activeFades = new Dictionary<CanvasGroup, Coroutine>();

    private PlayerController playerController;

    private void Awake()
    {
        foreach (var settings in panelSettingsList)
        {
            if (settings.panel != null && !panelSettings.ContainsKey(settings.panelType))
                panelSettings.Add(settings.panelType, settings);
        }

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void FadeIn(PanelType type)
    {
        if (!panelSettings.ContainsKey(type)) return;
        var s = panelSettings[type];
        StartFade(s.panel, s.panel.alpha, 1f, s.fadeDuration, s.interactable, s.blockRaycasts, s.disablesPlayerInput);
    }

    public void FadeOut(PanelType type)
    {
        if (!panelSettings.ContainsKey(type)) return;
        var s = panelSettings[type];
        StartFade(s.panel, s.panel.alpha, 0f, s.fadeDuration, s.interactable, s.blockRaycasts, s.disablesPlayerInput);
    }

    public float GetFadeDuration(PanelType type)
    {
        if (panelSettings.ContainsKey(type))
            return panelSettings[type].fadeDuration;
        return 0.5f;
    }

    #region Fade Logic

    private void StartFade(CanvasGroup panel, float startAlpha, float finalAlpha, float duration, bool interactable, bool blockRaycasts, bool disablesInput)
    {
        if (activeFades.ContainsKey(panel))
        {
            StopCoroutine(activeFades[panel]);
            activeFades.Remove(panel);
        }

        Coroutine fadeRoutine = StartCoroutine(Fade(panel, startAlpha, finalAlpha, duration, interactable, blockRaycasts, disablesInput));
        activeFades[panel] = fadeRoutine;
    }

    private IEnumerator Fade(CanvasGroup panel, float startAlpha, float finalAlpha, float duration, bool interactable, bool blockRaycasts, bool disablesInput)
    {
        if (finalAlpha > startAlpha)
        {
            panel.interactable = interactable;
            panel.blocksRaycasts = blockRaycasts;

            if (disablesInput && playerController != null)
                playerController.FreezeAllInputs();
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            panel.alpha = Mathf.Lerp(startAlpha, finalAlpha, t / duration);

            if (finalAlpha == 0 && disablesInput && playerController != null && progress >= 0f)
                playerController.UnfreezeAllInputs();

            yield return null;
        }

        panel.alpha = finalAlpha;

        if (finalAlpha == 0)
        {
            panel.interactable = interactable;
            panel.blocksRaycasts = blockRaycasts;
        }

        if (activeFades.ContainsKey(panel))
            activeFades.Remove(panel);
    }
    #endregion
}
