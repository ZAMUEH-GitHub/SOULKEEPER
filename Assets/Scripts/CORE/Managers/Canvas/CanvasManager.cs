using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PanelType
{
    [Header("Main Menu Panels")]
    MainMenu, PlayGame, Settings, VideoSettings, AudioSettings, KeyBindings, Credits,

    [Header("Gameplay Panels")]
    HUD, PauseMenu, PauseSettings, PauseAudioSettings, PauseKeybindings,

    [Header("Global Panels")]
    BlackScreen, ConfirmationPanel, ToastPanel
}

[Serializable]
public class PanelFadeSettings
{
    [Header("Panel Settings")]
    public PanelType panelType;
    public CanvasGroup panel;
    public GameObject firstSelected;

    [Header("Fade Properties")]
    public float fadeDuration = 0.3f;
    public bool interactable = true;
    public bool blockRaycasts = true;
    public bool disablesPlayerInput = false;

    [Header("Selection Behavior")]
    public bool rememberLastSelection = true;
}

public class CanvasManager : Singleton<CanvasManager>
{
    protected override bool IsPersistent => false;

    [Header("Canvas Groups")]
    [SerializeField] private GameObject _MainMenuCanvas;
    [SerializeField] private GameObject _GameplayCanvas;
    [SerializeField] private GameObject _GlobalCanvas;

    [Header("Panels Configuration")]
    [SerializeField] private List<PanelFadeSettings> panelSettingsList = new();
    private readonly Dictionary<PanelType, PanelFadeSettings> panelSettings = new();
    private readonly Dictionary<CanvasGroup, Coroutine> activeFades = new();

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

    private void OnEnable() => GameManager.OnGameStateChanged += HandleGameStateChanged;
    private void OnDisable() => GameManager.OnGameStateChanged -= HandleGameStateChanged;

    private void HandleGameStateChanged(GameState state)
    {
        bool isMainMenu = state == GameState.MainMenu;

        ToggleCanvasInteractivity(_MainMenuCanvas, isMainMenu);
        ToggleCanvasInteractivity(_GameplayCanvas, !isMainMenu);
        ToggleCanvasInteractivity(_GlobalCanvas, true);

        if (isMainMenu) FadeIn(PanelType.MainMenu);
        else FadeIn(PanelType.HUD);
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
        => panelSettings.ContainsKey(type) ? panelSettings[type].fadeDuration : 0.5f;
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

        if (s.rememberLastSelection && finalAlpha == 0f && EventSystem.current != null)
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
            if (disablesInput && playerController != null) playerController.FreezeAllInputs();
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
            panel.alpha = Mathf.Lerp(startAlpha, finalAlpha, t / duration);
            yield return null;
        }

        panel.alpha = finalAlpha;
        panel.interactable = finalAlpha > 0f;
        panel.blocksRaycasts = finalAlpha > 0f;

        if (finalAlpha > 0f)
            yield return StartCoroutine(ApplyPanelSelectionNextFrame(s));

        activeFades.Remove(panel);
        if (disablesInput && playerController != null) playerController.UnfreezeAllInputs();
    }

    private IEnumerator ApplyPanelSelectionNextFrame(PanelFadeSettings s)
    {
        yield return null;
        if (EventSystem.current == null || s == null) yield break;

        GameObject candidate = null;

        if (s.rememberLastSelection &&
            _lastSelectedByPanel.TryGetValue(s.panelType, out var remembered) &&
            remembered != null)
        {
            candidate = remembered;
        }

        if (candidate == null) candidate = s.firstSelected;
        if (candidate == null || !candidate.activeInHierarchy) yield break;

        var selectable = candidate.GetComponent<Selectable>();
        if (selectable == null || !selectable.IsInteractable()) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(candidate);
        selectable.OnSelect(null);
    }
    #endregion

    #region Delegated API
    public void ShowConfirmation(string title, string subtitle, Action confirmAction, Action cancelAction = null)
        => ConfirmationPanelManager.Instance?.ShowConfirmation(title, subtitle, confirmAction, cancelAction);

    public void ShowToast(string message, float duration = 1.5f)
        => ToastPanelManager.Instance?.ShowToast(message, duration);

    public IReadOnlyDictionary<PanelType, PanelFadeSettings> GetPanelSettings() => panelSettings;
    #endregion

    public void ToggleCanvasInteractivity(GameObject canvasObject, bool enable)
    {
        if (canvasObject == null || canvasObject == _GlobalCanvas) return;

        var raycaster = canvasObject.GetComponent<GraphicRaycaster>();
        if (raycaster != null) raycaster.enabled = enable;

        foreach (var t in canvasObject.GetComponentsInChildren<EventTrigger>(true))
            t.enabled = enable;
    }
}
