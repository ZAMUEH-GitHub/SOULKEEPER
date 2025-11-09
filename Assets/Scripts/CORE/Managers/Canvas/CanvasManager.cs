using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PanelType
{
    [Header("Main Menu Panels")]
    MainMenu,
    PlayGame,
    LoadGame,
    Settings,
    VideoSettings,
    AudioSettings,
    KeyBindings,
    Credits,

    [Header("Gameplay Panels")]
    HUD,
    BlackScreen,
    PauseMenu,
    PauseSettings,
    PauseAudioSettings,
    PauseKeybindings,
    ConfirmationPanel
}

[System.Serializable]
public class PanelFadeSettings
{
    [Header("Panel Settings")]
    public PanelType panelType;
    public CanvasGroup panel;

    [Header("Fade Properties")]
    public float fadeDuration = 0.3f;
    public bool interactable = true;
    public bool blockRaycasts = true;
    public bool disablesPlayerInput = false;
}

public class ConfirmationRequest
{
    public string message;
    public Action onConfirm;
    public Action onCancel;

    public ConfirmationRequest(string message, Action confirmAction, Action cancelAction = null)
    {
        this.message = message;
        this.onConfirm = confirmAction;
        this.onCancel = cancelAction;
    }
}

public class CanvasManager : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private GameObject _MainMenuCanvas;
    [SerializeField] private GameObject _GameplayCanvas;
    [SerializeField] private GameObject _GlobalCanvas;

    [Header("Panels Configuration")]
    [SerializeField] private List<PanelFadeSettings> panelSettingsList = new List<PanelFadeSettings>();

    private Dictionary<PanelType, PanelFadeSettings> panelSettings = new Dictionary<PanelType, PanelFadeSettings>();
    private Dictionary<CanvasGroup, Coroutine> activeFades = new Dictionary<CanvasGroup, Coroutine>();

    private PlayerController playerController;

    [Header("Confirmation System")]
    private ConfirmationRequest currentRequest;
    [SerializeField] private TMP_Text confirmationTitleText;
    [SerializeField] private TMP_Text confirmationSubtitleText;
    private bool isClosingConfirmation = false;


    private void Awake()
    {
        foreach (var settings in panelSettingsList)
        {
            if (settings.panel != null && !panelSettings.ContainsKey(settings.panelType))
                panelSettings.Add(settings.panelType, settings);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerController = playerObj.GetComponent<PlayerController>();

        if (_GlobalCanvas != null && panelSettings.ContainsKey(PanelType.BlackScreen))
            FadeIn(PanelType.BlackScreen);
    }

    #region Fade API
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
        StartFade(s.panel, s.panel.alpha, 0f, s.fadeDuration, false, false, s.disablesPlayerInput);
    }

    public float GetFadeDuration(PanelType type)
    {
        if (panelSettings.ContainsKey(type))
            return panelSettings[type].fadeDuration;
        return 0.5f;
    }
    #endregion

    #region Fade Logic
    private void StartFade(CanvasGroup panel, float startAlpha, float finalAlpha, float duration, bool interactable, bool blockRaycasts, bool disablesInput)
    {
        if (activeFades.ContainsKey(panel))
        {
            StopCoroutine(activeFades[panel]);
            activeFades.Remove(panel);
        }

        if (panelSettings.TryGetValue(PanelType.BlackScreen, out var black))
        {
            if (panel == black.panel)
            {
                panel.interactable = false;
                panel.blocksRaycasts = false;
            }
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
        else if (finalAlpha == 0)
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / duration;
            panel.alpha = Mathf.Lerp(startAlpha, finalAlpha, progress);

            if (finalAlpha == 0 && disablesInput && playerController != null && progress >= 0f)
                playerController.UnfreezeAllInputs();

            yield return null;
        }

        panel.alpha = finalAlpha;

        if (finalAlpha == 0f)
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
        else
        {
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }

        if (activeFades.ContainsKey(panel))
            activeFades.Remove(panel);
    }
    #endregion

    #region Confirmation System

    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
    {
        currentRequest = new ConfirmationRequest(title, confirmAction, cancelAction);

        if (confirmationTitleText != null)
            confirmationTitleText.text = title;

        if (confirmationSubtitleText != null)
            confirmationSubtitleText.text = subtitle;

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null && gm.CurrentState == GameState.MainMenu)
        {
            ToggleCanvasInteractivity(_MainMenuCanvas, false);
        }
        else if (gm != null && gm.CurrentState == GameState.Gameplay)
        {
            ToggleCanvasInteractivity(_GameplayCanvas, false);
        }


        if (_MainMenuCanvas != null)
        {
            var raycaster = _MainMenuCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster != null) raycaster.enabled = false;
        }

        FadeIn(PanelType.ConfirmationPanel);
    }

    public void OnConfirmPressed()
    {
        currentRequest?.onConfirm?.Invoke();
        CloseConfirmation();
    }

    public void OnCancelPressed()
    {
        currentRequest?.onCancel?.Invoke();
        CloseConfirmation();
    }

    private void CloseConfirmation()
    {
        if (isClosingConfirmation) return;
        isClosingConfirmation = true;

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null && gm.CurrentState == GameState.MainMenu)
        {
            ToggleCanvasInteractivity(_MainMenuCanvas, true);
        }
        else
        {
            ToggleCanvasInteractivity(_GameplayCanvas, true);
        }


        if (_MainMenuCanvas != null)
        {
            var raycaster = _MainMenuCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster != null) raycaster.enabled = true;
        }

        FadeOut(PanelType.ConfirmationPanel);
        currentRequest = null;
        isClosingConfirmation = false;
    }
    #endregion

    public void ToggleCanvasInteractivity(GameObject canvasObject, bool enable)
    {
        if (canvasObject == null || canvasObject == _GlobalCanvas) return;

        var raycaster = canvasObject.GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster != null)
            raycaster.enabled = enable;

        var triggers = canvasObject.GetComponentsInChildren<UnityEngine.EventSystems.EventTrigger>(true);
        foreach (var t in triggers)
            t.enabled = enable;
    }
}
