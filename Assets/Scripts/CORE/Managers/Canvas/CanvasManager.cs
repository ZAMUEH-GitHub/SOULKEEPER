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
    Settings,
    VideoSettings,
    AudioSettings,
    KeyBindings,
    Credits,

    [Header("Gameplay Panels")]
    HUD,
    PauseMenu,
    PauseSettings,
    PauseAudioSettings,
    PauseKeybindings,

    [Header("Global Panels")]
    BlackScreen,
    ConfirmationPanel,
    ToastPanel
}

[Serializable]
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

public class CanvasManager : Singleton<CanvasManager>
{
    protected override bool IsPersistent => false;

    [Header("Canvas Groups")]
    [SerializeField] private GameObject _MainMenuCanvas;
    [SerializeField] private GameObject _GameplayCanvas;
    [SerializeField] private GameObject _GlobalCanvas;

    [Header("Panels Configuration")]
    [SerializeField] private List<PanelFadeSettings> panelSettingsList = new List<PanelFadeSettings>();
    private readonly Dictionary<PanelType, PanelFadeSettings> panelSettings = new();
    private readonly Dictionary<CanvasGroup, Coroutine> activeFades = new();

    [Header("Confirmation System")]
    [SerializeField] private TMP_Text confirmationTitleText;
    [SerializeField] private TMP_Text confirmationSubtitleText;
    private bool isClosingConfirmation = false;
    private ConfirmationRequest currentRequest;

    [Header("Toast Settings")]
    [SerializeField] private TMP_Text toastText;

    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();

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

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        bool isMainMenu = state == GameState.MainMenu;

        ToggleCanvasInteractivity(_MainMenuCanvas, isMainMenu);
        ToggleCanvasInteractivity(_GameplayCanvas, !isMainMenu);
        ToggleCanvasInteractivity(_GlobalCanvas, true);

        if (isMainMenu)
            FadeIn(PanelType.MainMenu);
        else
            FadeIn(PanelType.HUD);
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
        return panelSettings.ContainsKey(type) ? panelSettings[type].fadeDuration : 0.5f;
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

        panel.interactable = finalAlpha > 0f;
        panel.blocksRaycasts = finalAlpha > 0f;

        if (activeFades.ContainsKey(panel))
            activeFades.Remove(panel);
    }
    #endregion

    #region Confirmation System
    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
    {
        currentRequest = new ConfirmationRequest(title, confirmAction, cancelAction);

        if (confirmationTitleText != null) confirmationTitleText.text = title;
        if (confirmationSubtitleText != null) confirmationSubtitleText.text = subtitle;

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (gm.CurrentState == GameState.MainMenu)
                ToggleCanvasInteractivity(_MainMenuCanvas, false);
            else if (gm.CurrentState == GameState.Gameplay)
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

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (gm.CurrentState == GameState.MainMenu)
                ToggleCanvasInteractivity(_MainMenuCanvas, true);
            else
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

    #region Toast System
    public void ShowToast(string message, float duration = 1.5f)
    {
        if (toastText == null) return;

        toastText.text = message;
        FadeIn(PanelType.ToastPanel);
        StartCoroutine(HideToastAfterDelay(duration));
    }

    private IEnumerator HideToastAfterDelay(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        FadeOut(PanelType.ToastPanel);
    }
    #endregion

    public void ToggleCanvasInteractivity(GameObject canvasObject, bool enable)
    {
        if (canvasObject == null || canvasObject == _GlobalCanvas) return;

        var raycaster = canvasObject.GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = enable;

        foreach (var t in canvasObject.GetComponentsInChildren<UnityEngine.EventSystems.EventTrigger>(true))
            t.enabled = enable;
    }
}
