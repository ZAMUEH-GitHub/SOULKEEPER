using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Tooltip("UI control to select when this panel finishes fading in.")]
    public GameObject firstSelected;

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

    private readonly List<PanelFadeSettings> _temporarilyDisabledPanels = new();

    [Header("Toast Settings")]
    [SerializeField] private TMP_Text toastText;

    private PlayerController playerController;

    private readonly Dictionary<PanelType, GameObject> _lastSelectedByPanel = new();

    #region Unity Lifecycle
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
    #endregion

    #region Fade API
    public void FadeIn(PanelType type)
    {
        if (!panelSettings.ContainsKey(type)) return;
        var s = panelSettings[type];
        StartFade(s, s.panel.alpha, 1f, s.fadeDuration, s.interactable, s.blockRaycasts, s.disablesPlayerInput);
    }

    public void FadeOut(PanelType type)
    {
        if (!panelSettings.ContainsKey(type)) return;
        var s = panelSettings[type];
        StartFade(s, s.panel.alpha, 0f, s.fadeDuration, false, false, s.disablesPlayerInput);
    }

    public float GetFadeDuration(PanelType type)
    {
        return panelSettings.ContainsKey(type) ? panelSettings[type].fadeDuration : 0.5f;
    }
    #endregion

    #region Fade Logic
    private void StartFade(PanelFadeSettings s, float startAlpha, float finalAlpha, float duration,
                           bool interactable, bool blockRaycasts, bool disablesInput)
    {
        var panel = s.panel;

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

        if (finalAlpha == 0f && EventSystem.current != null)
        {
            var current = EventSystem.current.currentSelectedGameObject;
            if (current != null && current.transform.IsChildOf(panel.transform))
            {
                _lastSelectedByPanel[s.panelType] = current;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        Coroutine fadeRoutine = StartCoroutine(Fade(s, startAlpha, finalAlpha, duration,
                                                    interactable, blockRaycasts, disablesInput));
        activeFades[panel] = fadeRoutine;
    }

    private IEnumerator Fade(PanelFadeSettings s, float startAlpha, float finalAlpha, float duration,
                             bool interactable, bool blockRaycasts, bool disablesInput)
    {
        var panel = s.panel;

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

        if (finalAlpha > 0f)
            yield return StartCoroutine(ApplyPanelSelectionNextFrame(s));

        if (activeFades.ContainsKey(panel))
            activeFades.Remove(panel);
    }

    private IEnumerator ApplyPanelSelectionNextFrame(PanelFadeSettings s)
    {
        yield return null;
        if (EventSystem.current == null || s == null) yield break;

        GameObject candidate = null;
        if (_lastSelectedByPanel.TryGetValue(s.panelType, out var remembered) && remembered != null)
            candidate = remembered;

        if (candidate == null)
            candidate = s.firstSelected;

        if (candidate == null || !candidate.activeInHierarchy) yield break;

        var selectable = candidate.GetComponent<Selectable>();
        if (selectable == null || !selectable.IsInteractable()) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(candidate);
        selectable.OnSelect(null);
    }
    #endregion

    #region Confirmation System
    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
    {
        currentRequest = new ConfirmationRequest(title, confirmAction, cancelAction);

        if (confirmationTitleText != null) confirmationTitleText.text = title;
        if (confirmationSubtitleText != null) confirmationSubtitleText.text = subtitle;

        _temporarilyDisabledPanels.Clear();
        foreach (var kvp in panelSettings)
        {
            var p = kvp.Value;
            if (p.panelType == PanelType.ConfirmationPanel || p.panelType == PanelType.ToastPanel)
                continue;

            if (p.panel != null && p.panel.alpha > 0.95f && p.panel.interactable)
            {
                p.panel.interactable = false;
                p.panel.blocksRaycasts = false;
                _temporarilyDisabledPanels.Add(p);
            }
        }

        if (EventSystem.current != null)
        {
            var currentSel = EventSystem.current.currentSelectedGameObject;
            if (currentSel != null)
            {
                foreach (var p in _temporarilyDisabledPanels)
                {
                    if (currentSel.transform.IsChildOf(p.panel.transform))
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        break;
                    }
                }
            }
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

        foreach (var p in _temporarilyDisabledPanels)
        {
            if (p.panel == null) continue;
            p.panel.interactable = true;
            p.panel.blocksRaycasts = true;
        }
        _temporarilyDisabledPanels.Clear();

        FadeOut(PanelType.ConfirmationPanel);

        var gm = GameManager.Instance;
        if (gm != null)
        {
            PanelType toFocus = gm.CurrentState == GameState.MainMenu ? PanelType.MainMenu : PanelType.HUD;
            if (panelSettings.TryGetValue(toFocus, out var s))
                StartCoroutine(ApplyPanelSelectionNextFrame(s));
        }

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